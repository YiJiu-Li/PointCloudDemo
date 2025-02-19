/*
* 文件名：AnimatorExtensions.cs
* 作者：依旧
* 版本：#VERSION#
* Unity版本：2021.3.26f1
* 创建日期：2024/11/29 11:56:44
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HQG
{
    /// <summary>
    /// 类：AnimatorExtensions
    /// 描述：此类的功能和用途...
    /// </summary>
    public static class AnimatorExtensions
    {
        /// <summary>
        /// 异步播放指定的动画，并在动画播放完成后调用回调函数。
        /// </summary>
        /// <param name="animator">Animator 组件，控制动画播放的对象。</param>
        /// <param name="animatorName">动画的名称，确保动画在 Animator 中存在。</param>
        /// <param name="callback">动画播放完成后的回调函数（可选）。</param>
        /// <param name="cancellationTokenSource">取消操作的 Token 源（可选）。可以在需要时取消等待。</param>
        /// <returns>一个表示异步操作的 UniTask。</returns>
        public static void PlayAnimationCompleted(this Animator animator, string animatorName, Action callback = null, CancellationTokenSource cancellationTokenSource = null)
        {
            // 获取指定名称的动画剪辑
            var animationClip = GetAnimationClipByName(animator, animatorName);

            // 如果动画剪辑不存在，记录错误并返回
            if (animationClip == null)
            {
                Debug.LogError($"动画 {animatorName} 不存在！");
                return;
            }

            // 如果提供了回调函数，则等待动画的播放完成
            if (callback != null)
            {
                // 使用 UniTask.Delay 等待动画的长度，确保回调在动画结束后被调用
                // await UniTask.Delay(TimeSpan.FromSeconds(animationClip.length), cancellationToken: cancellationTokenSource?.Token);

                // 动画播放完成后调用回调函数
                callback?.Invoke();
            }
        }


        /// <summary>
        /// 获取动画片段
        /// </summary>
        /// <param name="animationName">动画片段的名称</param>
        /// <returns>找到的动画片段</returns>
        private static AnimationClip GetAnimationClipByName(this Animator animator, string animationName)
        {
            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == animationName)
                    return clip;
            }
            return null;
        }
    }
}