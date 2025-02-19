/*
* 文件名：TaskExtensions.cs
* 作者：依旧
* 版本：1.0
* Unity版本：2021.3.26f1
* 创建日期：2024/09/03 00:17:29
* 版权：© 2024 杭州西雨动画有限公司
* All rights reserved.
*/

using System.Threading.Tasks;
using System;
/// <summary>
/// 类：TaskExtensions
/// 描述：此类的功能和用途...
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// 延迟指定的时间，单位为秒。
    /// </summary>
    /// <param name="task">当前任务。</param>
    /// <param name="seconds">延迟时间，单位为秒。</param>
    /// <returns>返回一个任务，用于等待指定的时间。</returns>
    public static async Task DelaySeconds(this Task task, int seconds)
    {
        if (seconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds), "延迟时间不能为负数");
        }
        int delayInMilliseconds = seconds * 1000;
        await Task.Delay(delayInMilliseconds);
        await task;
    }
}

