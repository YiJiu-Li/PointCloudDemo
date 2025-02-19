/*
* 文件名：TriggerType.cs
* 作者：依旧
* Unity版本：2021.3.26f1
* 创建日期：2025/02/14 15:12:33
* 版权：© 2025 DefaultCompany
* All rights reserved.
*/

using UnityEngine;


[UnityEngine.SerializeField]
public enum TriggerType : byte
{
    [Tooltip("未知触发方式")]
    Unknown,            // 默认值，防止未定义的情况

    [Tooltip("音频触发")]
    Audio,              // 播放音频（如靠近或点击时播放）

    [Tooltip("触发区触发")]
    Zone,               // 进入某个触发区域时激活

    [Tooltip("注释触发")]
    Annotation,         // 用户点击或查看时弹出注释信息

    [Tooltip("时间触发")]
    Timed,              // 经过一定时间自动触发

    [Tooltip("脚本触发")]
    Script,             // 通过脚本逻辑触发

    [Tooltip("交互触发")]
    Interaction,        // 需要用户交互（如点击、滑动）

    [Tooltip("视觉特效触发")]
    VisualEffect        // 仅用于触发特效（如粒子、动画）
}