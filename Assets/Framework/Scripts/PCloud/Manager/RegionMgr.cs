/*
* 文件名：RegionManager.cs
* 作者：依旧
* 版本：#VERSION#
* Unity版本：2021.3.26f1
* 创建日期：2024/11/18 17:44:19
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using HQG;
using UnityEngine;
using YZJ;

/// <summary>
/// 区域管理器
/// 负责管理场景中所有区域的初始化、激活和完成状态
/// 提供区域查找、激活和完成的功能接口
/// </summary>
public class RegionMgr : UnitySingleton<RegionMgr>
{
    #region 序列化字段
    [SerializeField, Header("玩家Transform引用")]
    private Transform player;

    [SerializeField, Header("NPC实体引用")]
    private NPCEntity npcEntity;
    #endregion

    #region 私有字段
    /// <summary>
    /// 节点历史记录堆栈
    /// </summary>
    private Stack<NodeBase> _nodeHistory;

    /// <summary>
    /// 存储所有区域的字典,key为区域名称,value为区域实例
    /// </summary>
    private Dictionary<string, RegionBase> _regionsDictionary;

    /// <summary>
    /// 当前活动节点
    /// </summary>
    private NodeBase _currentNode;

    /// <summary>
    /// 对话系统的取消令牌
    /// </summary>
    private CancellationTokenSource _speakTokenSource;
    #endregion

    #region 属性
    /// <summary>
    /// 获取玩家Transform
    /// </summary>
    public Transform Player => player;

    /// <summary>
    /// 获取NPC实体
    /// </summary>
    public NPCEntity NpcEntity => npcEntity;

    /// <summary>
    /// 获取当前节点
    /// </summary>
    public NodeBase CurrentNode
    {
        get => _currentNode;
        private set
        {
            if (_currentNode != value)
            {
                if (_currentNode != null)
                {
                    _nodeHistory.Push(_currentNode);
                    Log($"节点切换: {_currentNode.NodeName} -> {value?.NodeName ?? "null"}");
                }
                _currentNode = value;
            }
        }
    }

    /// <summary>
    /// 获取上一个节点
    /// </summary>
    public NodeBase PreviousNode => _nodeHistory.Count > 0 ? _nodeHistory.Peek() : null;
    #endregion

    #region 事件
    /// <summary>
    /// 区域激活时触发的事件
    /// </summary>
    public event Action<string> OnRegionActivated;

    /// <summary>
    /// 区域完成时触发的事件
    /// </summary>
    public event Action<string> OnRegionCompleted;
    #endregion

    #region 公共方法
    /// <summary>
    /// 初始化区域管理器
    /// </summary>
    public void Initialize()
    {
        ValidateReferences();
        InitializeRegions();
    }

    /// <summary>
    /// 激活指定名称的区域
    /// </summary>
    /// <param name="regionName">要激活的区域名称</param>
    public void ActivateRegion(string regionName)
    {
        if (TryGetRegion(regionName, out RegionBase region))
        {
            region.ActivateRegion();
            OnRegionActivated?.Invoke(regionName);
        }
    }

    /// <summary>
    /// 完成指定名称的区域
    /// </summary>
    /// <param name="regionName">要完成的区域名称</param>
    public void CompleteRegion(string regionName)
    {
        if (TryGetRegion(regionName, out RegionBase region))
        {
            // region.CompleteRegion();
            OnRegionCompleted?.Invoke(regionName);
        }
    }

    /// <summary>
    /// 切换到指定节点
    /// </summary>
    public async UniTask SwitchToNode(NodeBase newNode)
    {
        if (newNode == null)
        {
            Log("错误：试图切换到空节点", LogColor.Red);
            return;
        }

        if (CurrentNode == newNode)
        {
            Log($"警告：当前已经是节点 {newNode.NodeName}", LogColor.Yellow);
            return;
        }

        // 关闭当前节点
        if (CurrentNode != null)
        {
            await CurrentNode.CloseNodeAsync();
        }

        // 切换到新节点
        CurrentNode = newNode;
    }

    /// <summary>
    /// 返回上一个节点
    /// </summary>
    public async UniTask BackToPreviousNode()
    {
        if (_nodeHistory.Count == 0)
        {
            Log("警告：没有上一个节点可返回", LogColor.Yellow);
            return;
        }

        NodeBase previousNode = _nodeHistory.Pop();
        await SwitchToNode(previousNode);
    }

    /// <summary>
    /// 清空节点历史记录
    /// </summary>
    public void ClearNodeHistory()
    {
        _nodeHistory.Clear();
        Log("节点历史记录已清空");
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 验证必要的引用是否设置
    /// </summary>
    private void ValidateReferences()
    {
        if (player == null)
        {
            throw new NullReferenceException("错误：玩家引用未设置");
        }

        if (npcEntity == null)
        {
            Debug.LogWarning("警告：NPC实体引用未设置");
        }

        _nodeHistory = new Stack<NodeBase>();
        _speakTokenSource = new CancellationTokenSource();
        _regionsDictionary = new Dictionary<string, RegionBase>();
    }

    /// <summary>
    /// 初始化所有区域
    /// </summary>
    private void InitializeRegions()
    {
        foreach (Transform regionTransform in transform)
        {
            if (regionTransform == transform) continue;

            if (!TryInitializeRegion(regionTransform))
            {
                Debug.LogError($"错误：区域 {regionTransform.name} 初始化失败");
            }
        }
    }

    /// <summary>
    /// 尝试初始化单个区域
    /// </summary>
    private bool TryInitializeRegion(Transform regionTransform)
    {
        var region = regionTransform.GetComponent<RegionBase>();
        if (region == null)
        {
            Debug.LogError($"错误：{regionTransform.name} 缺少 RegionBase 组件");
            return false;
        }

        if (_regionsDictionary.ContainsKey(region.regionName))
        {
            Debug.LogError($"错误：检测到重复的区域名称 {region.regionName}");
            return false;
        }

        _regionsDictionary.Add(region.regionName, region);
        return true;
    }

    /// <summary>
    /// 尝试获取指定名称的区域
    /// </summary>
    private bool TryGetRegion(string regionName, out RegionBase region)
    {
        if (_regionsDictionary.TryGetValue(regionName, out region))
        {
            return true;
        }

        Debug.LogWarning($"警告：未找到名为 {regionName} 的区域");
        return false;
    }

    // private void Log(string message, LogColor color = LogColor.None)
    // {
    //     Util.Log($"区域管理器: {message}", color);
    // }
    #endregion

    #region Unity生命周期
    protected override void OnDestroy()
    {
        base.OnDestroy();
        _speakTokenSource?.Cancel();
        _speakTokenSource?.Dispose();
        Debug.Log("区域管理器已销毁");
    }
    #endregion
}