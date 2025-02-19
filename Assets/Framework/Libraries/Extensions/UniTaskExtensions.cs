/*
* 文件名：UniTaskExtensions.cs
* 作者：依旧
* 版本：1.0
* Unity版本：2021.3.26f1
* 创建日期：2024/09/03 00:20:23
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// 类：UniTaskExtensions
/// 描述：此类的功能和用途...
/// </summary>


public static class UniTaskExtensions
{
    /// <summary>
    /// 延迟指定的时间，单位为秒。
    /// </summary>
    /// <param name="seconds">延迟时间，单位为秒。</param>
    /// <returns>返回一个 UniTask，用于等待指定的时间。</returns>
    public static async UniTask DelaySeconds(int seconds)
    {
        if (seconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds), "延迟时间不能为负数");
        }

        int delayInMilliseconds = seconds * 1000;
        await UniTask.Delay(delayInMilliseconds);
    }

    /// <summary>
    /// 延迟指定的时间，单位为秒，并返回指定的结果。
    /// </summary>
    /// <typeparam name="T">返回的结果类型。</typeparam>
    /// <param name="seconds">延迟时间，单位为秒。</param>
    /// <param name="result">延迟后返回的结果。</param>
    /// <returns>返回一个 UniTask，用于等待指定的时间并返回结果。</returns>
    public static async UniTask<T> DelaySeconds<T>(int seconds, T result)
    {
        if (seconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds), "延迟时间不能为负数");
        }

        int delayInMilliseconds = seconds * 1000;
        await UniTask.Delay(delayInMilliseconds);
        return result;
    }
}

