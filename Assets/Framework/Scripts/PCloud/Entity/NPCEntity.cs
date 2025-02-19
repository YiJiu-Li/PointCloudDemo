/*
* 文件名：NPCEntity.cs
* 作者：依旧
* 版本：#VERSION#
* Unity版本：2021.3.26f1
* 创建日期：2024/11/19 11:17:56
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

// 定义动画类型枚举
public enum AnimationType : byte
{
    Appear1,
    Appear2,
    Disappear,
    Call
}
/// <summary>
/// 类：NPCEntity
/// 描述：此类的功能和用途...
/// </summary>
public class NPCEntity : MonoBehaviour
{
    private Animator animator;
    // 动画参数名称
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int Appear1Hash = Animator.StringToHash("Appear1");
    private static readonly int Appear2Hash = Animator.StringToHash("Appear2");
    private static readonly int DisappearHash = Animator.StringToHash("Disappear");
    private static readonly int CallHash = Animator.StringToHash("Call");
    private static readonly int IdleSpeak01Hash = Animator.StringToHash("IdleSpeak1");
    private static readonly int IdleSpeak02Hash = Animator.StringToHash("IdleSpeak2");
    private static readonly int Speak1Hash = Animator.StringToHash("Speak1");
    private static readonly int Speak2Hash = Animator.StringToHash("Speak2");
    private static readonly int Speak3Hash = Animator.StringToHash("Speak3");
    private static readonly int Speak4Hash = Animator.StringToHash("Speak4");
    private static readonly int Speak5Hash = Animator.StringToHash("Speak5");
    private static readonly int IdleFlyHash = Animator.StringToHash("IdleFly");
    private static readonly int FlyLHash = Animator.StringToHash("FlyL");
    private static readonly int FlyRHash = Animator.StringToHash("FlyR");
    /// <summary>
    /// 出现1
    /// </summary>
    private static readonly string Appear1Name = "TY_appear01";
    /// <summary>
    /// 出现2
    /// </summary>
    private static readonly string Appear2Name = "TY_appear02";
    /// <summary>
    /// 消失
    /// </summary>
    private static readonly string DisappearName = "TY_disappear";
    /// <summary>
    /// 打招呼
    /// </summary>
    private static readonly string CallName = "TY_call";
    /// <summary>
    /// 左飞
    /// </summary>
    private static readonly string flyLName = "TY_flyL";
    /// <summary>
    /// 右飞
    /// </summary>
    private static readonly string flyRName = "TY_flyR";
    private static readonly string IdleSpeak01Name = "TY_idlespeak01";
    private static readonly string IdleSpeak02Name = "TY_idlespeak02";
    private static readonly string Speak1Name = "TY_speak01";
    private static readonly string Speak2Name = "TY_speak02";
    private static readonly string Speak3Name = "TY_speak03";
    private static readonly string Speak4Name = "TY_speak04";
    private static readonly string Speak5Name = "TY_speak05";
    /// <summary>
    /// 名牌
    /// </summary>
    private SpriteRenderer Titel;

    private CancellationToken cancellationToken;
    Transform player;
    void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        Titel = transform.GetChild(1).GetComponent<SpriteRenderer>();
        player = RegionMgr.Instance.Player.transform;
    }

    public void Update()
    {
        transform.LookAtTarget(player, Vector3.up);
    }
    /// <summary>
    /// NPC 左飞动画
    /// </summary>
    public void PlayFlyLAnimation()
    {
        animator?.SetTrigger(IdleFlyHash);
        animator?.SetTrigger(FlyLHash);
    }
    /// <summary>
    /// NPC 右飞动画
    /// </summary>
    public void PlayFlyRAnimation()
    {
        animator?.SetTrigger(IdleFlyHash);
        animator?.SetTrigger(FlyRHash);
    }
    /// <summary>
    /// NPC 出现动画
    /// </summary>
    /// <param name="type">1是落下 2是钻出</param>
    /// <param name="callback"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTask PlayAppearAnimation(int type = 1, Action callback = null, CancellationToken token = default)
    {
        animator.transform.localScale = Vector3.one;
        cancellationToken = token;
        if (type == 1)
        {
            await PlayAnimation(AnimationType.Appear1, callback, token);
        }
        else if (type == 2)
        {
            await PlayAnimation(AnimationType.Appear2, callback, token);
        }
        else
        {
            Debug.LogError("无效的出现动画类型！");
        }
        await ShowTitle(token);
    }

    /// <summary>
    /// NPC 消失动画
    /// </summary>
    public async UniTask PlayDisappearAnimation(Action callback = null, CancellationToken token = default)
    {
        cancellationToken = token;
        await PlayAnimation(AnimationType.Disappear, callback, token);
        Titel.DOFade(0, 1);
    }

    /// <summary>
    /// NPC 打招呼动画
    /// </summary>
    public async UniTask PlayCallAnimation(Action callback = null, CancellationToken token = default)
    {
        cancellationToken = token;
        await PlayAnimation(AnimationType.Call, callback, token);
    }

    /// <summary>
    /// 通用动画播放方法
    /// </summary>
    /// <param name="animationType">动画类型</param>
    /// <param name="callback">动画完成回调</param>
    /// <param name="token">取消令牌</param>
    /// <returns></returns>
    public async UniTask PlayAnimation(AnimationType animationType, Action callback = null, CancellationToken token = default)
    {
        string animationName;
        int triggerHash;
        cancellationToken = token;
        // 根据动画类型设置触发器和动画名称
        switch (animationType)
        {
            case AnimationType.Appear1:
                triggerHash = Appear1Hash;
                animationName = Appear1Name;
                break;
            case AnimationType.Appear2:
                triggerHash = Appear2Hash;
                animationName = Appear2Name;
                break;
            case AnimationType.Disappear:
                triggerHash = DisappearHash;
                animationName = DisappearName;
                break;
            case AnimationType.Call:
                triggerHash = CallHash;
                animationName = CallName;
                break;
            default:
                Debug.LogError("未知的动画类型！");
                return;
        }

        // 触发动画
        animator?.SetTrigger(triggerHash);
        var animationClip = GetAnimationClipByName(animationName);
        if (animationClip == null)
        {
            Debug.LogError($"动画 {animationName} 不存在！");
            return;
        }

        if (callback != null)
        {
            try
            {
                if (animationClip.length > 0)
                {
                    if (token.IsCancellationRequested)
                    {
                        Debug.Log("任务已被取消.");
                        return; // 直接返回，避免重复取消
                    }
                    // 等待动画的长度
                    await this.DelaySeconds(animationClip.length, token);

                    // 动画播放完成，调用回调函数
                    callback?.Invoke();
                }
                else
                {
                    Debug.LogError("动画时长为0，无法播放动画！");
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("动画播放被取消");
            }
        }
    }
    /// <summary>
    /// NPC 打招呼动画
    /// </summary>
    public void PlayCallAnimation()
    {
        animator?.SetTrigger(CallHash);
    }
    /// <summary>
    /// NPC 待机飞动画
    /// </summary>
    public void PlayIdleFlyAnimation()
    {
        animator?.SetTrigger(IdleFlyHash);
    }
    /// <summary>
    /// NPC 待机动画
    /// </summary>
    public void PlayIdleAnimation()
    {
        animator?.SetTrigger(IdleHash);
    }

    /// <summary>
    /// NPC 讲解动画
    /// </summary>
    public void PlayIdleSpeakAnimation(bool play)
    {
        animator?.SetTrigger(IdleSpeak01Hash);
    }
    /// <summary>
    /// 随机播放 NPC 讲话动画
    /// </summary>
    public async UniTask PlayRandomSpeakAnimation(int adminIdleIndex, CancellationToken token = default)
    {
        int randomIndex = Util.Random(1, 8); // 生成1到4之间的随机数
        int speakHash = 0;
        float adminLength = 0;
        string animationName = string.Empty;
        cancellationToken = token;
        switch (randomIndex)
        {
            case 1:
                speakHash = IdleSpeak01Hash;
                animationName = IdleSpeak01Name;
                adminLength = 2;
                break;
            case 2:
                speakHash = IdleSpeak02Hash;
                animationName = IdleSpeak02Name;
                adminLength = 4;
                break;
            case 3:
                speakHash = Speak1Hash;
                animationName = Speak1Name;
                adminLength = 4;
                break;
            case 4:
                speakHash = Speak2Hash;
                animationName = Speak2Name;
                adminLength = 5.33f;
                break;
            case 5:
                speakHash = Speak3Hash;
                animationName = Speak3Name;
                adminLength = 6.267f;
                break;
            case 6:
                speakHash = Speak4Hash;
                animationName = Speak4Name;
                adminLength = 3.867f;
                break;
            case 7:
                speakHash = Speak5Hash;
                animationName = Speak5Name;
                adminLength = 4;
                break;
            default:
                break;
        }
        animator?.SetTrigger(speakHash);
        var animationClip = GetAnimationClipByName(animationName);
        if (animationClip == null)
        {
            Debug.LogError($"动画 {animationName} 不存在！");
            return;
        }
        if (token.IsCancellationRequested)
        {
            Debug.Log("任务已被取消.");
            return; // 直接返回，避免重复取消
        }
        Debug.Log(adminLength);
        // 等待动画的长度
        await this.DelaySeconds(animationClip.length, token);
        animator?.SetTrigger(adminIdleIndex == 0 ? "Idle" : "IdleFly");
        if (token.IsCancellationRequested)
        {
            Debug.Log("任务已被取消.");
            return; // 直接返回，避免重复取消
        }
        await PlayRandomSpeakAnimation(adminIdleIndex, token);

    }
    public void StopPlaySpeakAnimation()
    {
        // if (animator != null)
        // {
        //     animator.SetBool(IdleSpeak01Hash, false);
        //     animator.SetBool(IdleSpeak02Hash, false);
        //     animator.SetBool(Speak1Hash, false);
        //     animator.SetBool(Speak2Hash, false);
        //     animator.SetBool(Speak3Hash, false);
        //     animator.SetBool(Speak4Hash, false);
        //     animator.SetBool(Speak5Hash, false);
        // }
    }
    /// <summary>
    /// 获取动画片段
    /// </summary>
    /// <param name="animationName">动画片段的名称</param>
    /// <returns>找到的动画片段</returns>
    private AnimationClip GetAnimationClipByName(string name)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator 或 RuntimeAnimatorController 未设置！");
            return null;
        }
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return clip;
            }
        }
        return null;
    }
    /// <summary>
    /// 显示 标签
    /// </summary>
    public async UniTask ShowTitle(CancellationToken token = default)
    {
        cancellationToken = token;
        Titel.DOFade(1, 1);
        await this.DelaySeconds(3, token);
        Titel.DOFade(0, 1);
    }

    private void OnDisable()
    {

    }
}