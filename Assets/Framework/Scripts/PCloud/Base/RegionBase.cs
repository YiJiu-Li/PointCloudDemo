/*
* 文件名：RegionBase.cs
* 作者：依旧
* 版本：#VERSION#
* Unity版本：2021.3.26f1
* 创建日期：2024/11/18 11:07:54
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 区域基类
/// 负责管理场景中特定区域的行为和状态
/// 包含：触发器管理、节点管理、高亮效果、音频播放等功能
/// </summary>
public abstract class RegionBase : MonoBehaviour
{
    #region 序列化字段
    [Header("基础信息")]
    [SerializeField, Tooltip("区域名称")]
    public string regionName;

    [SerializeField, Tooltip("区域ID")]
    public string regionID;

    [Header("节点管理")]
    [SerializeField, Tooltip("区域内的交互点位列表")]
    private List<NodeBase> nodes = new List<NodeBase>();

    [Header("状态控制")]
    [SerializeField, Tooltip("区域是否激活")]
    public bool isActive;

    [Header("触发器引用")]
    [SerializeField, Tooltip("进入区域触发器")]
    private Trigger enterTrigger;

    [SerializeField, Tooltip("退出区域触发器")]
    private Trigger exitTrigger;

    [Header("特效控制")]
    [SerializeField, Tooltip("区域高亮效果")]
    private GameObject[] highlightEffect;

    [Header("音频控制")]
    [SerializeField, Tooltip("是否播放区域音效")]
    private bool isPlayRegionAudio;

    [ConditionalField("isPlayRegionAudio")]
    [SerializeField, Tooltip("音频ID")]
    private string audioID;
    #endregion

    #region 私有字段
    private Transform nodeRoot;
    #endregion

    #region 属性
    public string AudioID => audioID;
    public IReadOnlyList<NodeBase> Nodes => nodes;
    #endregion

    #region Unity生命周期
    protected virtual void Start()
    {
        InitializeComponents();
    }

    private void OnDestroy()
    {
        UnbindEvents();
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 激活区域，显示所有节点并开启高亮效果
    /// </summary>
    public virtual void ActivateRegion()
    {
        if (isActive)
        {
            Log("区域已经处于激活状态", LogColor.Yellow);
            return;
        }

        isActive = true;
        PlayTriggerAudioAsync().Forget();
        ShowNodes();
        SetHighlight(true);
        Log("区域已激活", LogColor.Green);
    }

    /// <summary>
    /// 退出区域，关闭所有节点和效果
    /// </summary>
    public virtual async UniTask ExitRegion()
    {
        if (!isActive) return;

        SetHighlight(false);
        AudioMgr.Instance.StopAudio(AudioMgr.GUIDE_TYPE);
        await ResetRegionAsync();
        Log("区域已退出并重置", LogColor.Green);
    }

    /// <summary>
    /// 设置区域高亮状态
    /// </summary>
    public void SetHighlight(bool enable)
    {
        if (highlightEffect == null || highlightEffect.Length == 0)
        {
            Log("高亮效果未配置", LogColor.Yellow);
            return;
        }

        foreach (var effect in highlightEffect)
        {
            effect.SetActive(enable);
        }
    }

    /// <summary>
    /// 获取音频路径
    /// </summary>
    public string GetAudioPath() => $"{regionID}/{audioID}";
    #endregion

    #region 私有方法
    private void InitializeComponents()
    {
        InitializeTriggers();
        InitializeNodes();
        InitializeHighlight();
    }

    private void InitializeTriggers()
    {
        enterTrigger = transform.Find("EnterTrigger")?.GetComponent<Trigger>();
        exitTrigger = transform.Find("ExitTrigger")?.GetComponent<Trigger>();

        if (enterTrigger == null || exitTrigger == null)
        {
            Log("未找到触发器组件", LogColor.Red);
            return;
        }

        BindEvents();
    }

    private void InitializeNodes()
    {
        if (nodes.Count > 0) return;

        nodeRoot = transform.Find("Nodes");
        if (nodeRoot == null)
        {
            Log("未找到节点根物体", LogColor.Red);
            return;
        }

        foreach (Transform nodeTransform in nodeRoot)
        {
            if (nodeTransform == nodeRoot) continue;

            if (nodeTransform.TryGetComponent(out NodeBase node))
            {
                nodes.Add(node);
                node.gameObject.Hide();
            }
            else
            {
                Log($"节点 {nodeTransform.name} 缺少NodeBase组件", LogColor.Yellow);
            }
        }
    }

    private void InitializeHighlight()
    {
        if (highlightEffect == null || highlightEffect.Length == 0)
        {
            Log("高亮效果未配置", LogColor.Yellow);
            return;
        }

        foreach (var effect in highlightEffect)
        {
            effect.Hide();
        }
    }

    private void BindEvents()
    {
        enterTrigger.OnPlayerEnter += HandlePlayerEnter;
        exitTrigger.OnPlayerExit += HandlePlayerExit;
    }

    private void UnbindEvents()
    {
        if (enterTrigger != null)
        {
            enterTrigger.OnPlayerEnter -= HandlePlayerEnter;
        }
        if (exitTrigger != null)
        {
            exitTrigger.OnPlayerExit -= HandlePlayerExit;
        }
    }

    private void HandlePlayerEnter(Collider other)
    {
        if (!isActive)
        {
            Log("玩家进入区域");
            ActivateRegion();
        }
    }

    private void HandlePlayerExit(Collider other)
    {
        if (isActive)
        {
            Log("玩家离开区域");
            ExitRegion().Forget();
        }
    }

    private void ShowNodes()
    {
        foreach (var node in nodes)
        {
            node.gameObject.Show();
        }
    }

    private async UniTask PlayTriggerAudioAsync()
    {
        if (!isPlayRegionAudio) return;

        string audioPath = GetAudioPath();
        if (string.IsNullOrEmpty(audioPath))
        {
            Log("音频路径无效", LogColor.Red);
            return;
        }

        try
        {
            await AudioMgr.Instance.PlayDelayGuideFromResourcesAsync(audioPath);
        }
        catch (System.Exception ex)
        {
            Log($"音频播放失败: {ex.Message}", LogColor.Red);
        }
    }

    private async UniTask ResetRegionAsync()
    {
        // RegionMgr.Instance.oldNodeBase = null;
        isActive = false;

        foreach (var node in nodes)
        {
            await node.CloseNodeAsync();
            node.gameObject.Hide();
        }

        var npcEntity = RegionMgr.Instance.NpcEntity;
        if (npcEntity != null)
        {
            npcEntity.transform.position = Vector3.one * 1000;
        }
        else
        {
            Log("NPC实体未配置", LogColor.Yellow);
        }
    }

    private void Log(string message, LogColor color = LogColor.None)
    {
        Util.Log($"区域 {regionName}: {message}", color);
    }
    #endregion
}