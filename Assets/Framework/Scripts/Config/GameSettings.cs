using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Framework/GameSettings")]
[UnityEngine.SerializeField]
public class GameSettings : ScriptableObject
{
    [Header("调试设置")]
    [Tooltip("是否显示帧率")]
    public bool showFps = false;
    [Tooltip("是否显示调试信息")]
    public bool showDebugInfo = false;

    [Header("性能设置")]
    [Tooltip("目标帧率")]
    [Range(25,120)]
    public int targetFrameRate = 30;
   [Header("平台设置")]
    public PlatformType PlatformType = PlatformType.Windows;
}