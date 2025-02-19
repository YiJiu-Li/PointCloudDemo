/*
* 文件名：NodeBase.cs
* 作者：依旧
* 版本：#VERSION#
* Unity版本：2021.3.26f1
* 创建日期：2024/11/18 11:01:42
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 节点基类
/// 提供场景中交互点位的基础功能实现，包括：
/// 1. 触发器管理
/// 2. 音频播放
/// 3. 状态控制
/// 4. 事件系统
/// </summary>
public abstract class NodeBase : MonoBehaviour
{
    #region 序列化字段
    [Header("基础设置")]
    [SerializeField, Tooltip("节点触发类型")]
    private TriggerType nodeType = TriggerType.Unknown;

    [SerializeField, Tooltip("节点唯一标识")]
    private string nodeId;

    [SerializeField, Tooltip("节点显示名称")]
    private string nodeName;

    [Header("状态控制")]
    [SerializeField, Tooltip("是否激活")]
    private bool isActive;

    [SerializeField, Tooltip("是否已完成")]
    private bool isCompleted;

    [Header("触发器引用")]
    [SerializeField, Tooltip("进入区域触发器")]
    private Trigger enterTrigger;

    [SerializeField, Tooltip("退出区域触发器")]
    private Trigger exitTrigger;

    [Header("组件引用")]
    [SerializeField, Tooltip("所属区域节点")]
    private RegionBase regionBase;

    [SerializeField, Tooltip("NPC实体")]
    private NPCEntity npcEntity;

    [SerializeField, Tooltip("NPC位置点")]
    private Transform npcPoint;
    #endregion

    #region 私有字段
    private AudioSource _audioSource;
    private bool _isClosing;
    private CancellationTokenSource _cancellationTokenSource;
    #endregion

    #region 属性
    public TriggerType NodeType => nodeType;
    public string NodeId => nodeId;
    public string NodeName => nodeName;
    public bool IsActive
    {
        get => isActive;
        protected set => isActive = value;
    }
    public bool IsCompleted
    {
        get => isCompleted;
        protected set => isCompleted = value;
    }
    public RegionBase RegionBase => regionBase;
    public NPCEntity NpcEntity => npcEntity;
    public Transform NpcPoint => npcPoint;
    #endregion

    #region 事件
    public event Action OnActivate;
    public event Action OnComplete;
    public event Action<Collider> OnEnter;
    public event Action<Collider> OnStay;
    public event Action<Collider> OnExit;
    public event Func<UniTask> OnCloseAsync;
    #endregion

    #region Unity生命周期
    protected virtual void Awake()
    {
        InitializeComponents();
        BindEvents();
    }

    protected virtual void OnDestroy()
    {
        UnbindEvents();
    }
    #endregion

    #region 公共方法
    /// <summary>
    /// 初始化节点
    /// </summary>
    public virtual void Initialize()
    {
        // AudioMgr.Instance.StopAudio(AudioMgr.GUIDE_TYPE);
        _cancellationTokenSource ??= new CancellationTokenSource();
    }

    /// <summary>
    /// 取消当前任务
    /// </summary>
    public void CancelToken()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    /// <summary>
    /// 获取音频路径
    /// </summary>
    public string GetAudioPath() => $"{regionBase.regionName}/{nodeId}";

    /// <summary>
    /// 关闭节点
    /// </summary>
    public virtual async UniTask CloseNodeAsync()
    {
        if (_isClosing)
        {
            Log("节点正在关闭中...", LogColor.Yellow);
            return;
        }

        _isClosing = true;
        IsActive = false;
        IsCompleted = false;

        StopAudio();

        if (OnCloseAsync != null)
        {
            await OnCloseAsync.Invoke();
        }

        _isClosing = false;
    }

    /// <summary>
    /// 播放音频
    /// </summary>
    public async UniTask PlayAudio(AudioClip clip, Action onAudioFinished = null, CancellationToken token = default)
    {
        if (!ValidateAudioParameters(clip)) return;

        StopAudio();
        _audioSource.clip = clip;
        _audioSource.Play();

        if (onAudioFinished != null)
        {
            await PlayAudioWithCallback(clip, onAudioFinished, token);
        }
    }

    /// <summary>
    /// 停止音频播放
    /// </summary>
    public void StopAudio()
    {
        if (_audioSource != null && _audioSource.isPlaying)
        {
            _audioSource.Stop();
            _audioSource.clip = null;
        }
    }

    /// <summary>
    /// 执行节点行为（需要子类实现）
    /// </summary>
    public abstract void PerformAction();
    #endregion

    #region 私有方法
    private void InitializeComponents()
    {
        enterTrigger = transform.Find("EnterTrigger")?.GetComponent<Trigger>();
        exitTrigger = transform.Find("ExitTrigger")?.GetComponent<Trigger>();
        _audioSource = transform.Find("AudioSource")?.GetComponent<AudioSource>();

        if (regionBase == null)
        {
            regionBase = transform.parent?.parent?.GetComponent<RegionBase>();
        }
    }

    private void BindEvents()
    {
        if (enterTrigger != null)
        {
            enterTrigger.OnPlayerEnter += HandlePlayerEnter;
        }
        if (exitTrigger != null)
        {
            exitTrigger.OnPlayerExit += HandlePlayerExit;
        }
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

    private async void HandlePlayerEnter(Collider collider)
    {
        Initialize();
        await HandleNodeTransition();
        OnEnter?.Invoke(collider);
    }

    private void HandlePlayerExit(Collider collider)
    {
        OnExit?.Invoke(collider);
    }

    private async UniTask HandleNodeTransition()
    {
        var regionMgr = RegionMgr.Instance;
        if (regionMgr.PreviousNode == null && nodeType == TriggerType.Zone)
        {
            await regionMgr.SwitchToNode(this);
            return;
        }

        if (regionMgr.PreviousNode != this && nodeType == TriggerType.Zone)
        {
            regionMgr.PreviousNode.OnExit?.Invoke(null);
            await regionMgr.PreviousNode.CloseNodeAsync();
            await regionMgr.SwitchToNode(this);
        }
    }

    private bool ValidateAudioParameters(AudioClip clip)
    {
        if (_audioSource == null)
        {
            Log("音频组件未找到", LogColor.Red);
            return false;
        }
        if (clip == null)
        {
            Log("音频片段为空", LogColor.Red);
            return false;
        }
        if (clip.length <= 0)
        {
            Log("音频长度无效", LogColor.Red);
            return false;
        }
        return true;
    }

    private async UniTask PlayAudioWithCallback(AudioClip clip, Action onAudioFinished, CancellationToken token)
    {
        try
        {
            if (token.IsCancellationRequested)
            {
                Log("音频播放已取消");
                return;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(clip.length), cancellationToken: token);
            onAudioFinished?.Invoke();
        }
        catch (OperationCanceledException)
        {
            Log($"{NodeName}: 音频播放已取消", LogColor.Yellow);
        }
    }

    protected void Log(string message, LogColor color = LogColor.None)
    {
        Util.Log($"节点 {nodeId}: {message}", color);
    }
    #endregion
}