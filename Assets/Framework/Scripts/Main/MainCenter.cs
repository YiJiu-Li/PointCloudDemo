/*
* 文件名：MainCenter.cs
* 作者：依旧
* 版本：#VERSION#
* Unity版本：2021.3.26f1
* 创建日期：2025/01/13 16:53:20
* 版权：© 2025 杭州西雨动画有限公司
* All rights reserved.
*/

using UnityEngine;

/// <summary>
/// 类：MainCenter
/// 描述：此类的功能和用途...
/// </summary>
public class MainCenter : MonoBehaviour
{
    void Awake()
    {
        InitSettings();
        InitDebug();
        // 初始化各个管理器
        RegionMgr.Instance.Initialize();
    }
    /// <summary>
    /// 初始化设置
    /// </summary>
    private void InitSettings()
    {
        GameSettings gameSettings = Util.GetGameSettings();
        AppConst.LogMode = gameSettings.showDebugInfo;
        AppConst.ShowFps = gameSettings.showFps;
        AppConst.GameFrameRate = gameSettings.targetFrameRate;
    }
    /// <summary>
    /// 添加调试
    /// </summary>
    private void InitDebug()
    {
        if (AppConst.ShowFps)
        {
            gameObject.AddComponent<AppFPS>();
        }
        if (AppConst.LogMode)
        {
            gameObject.AddComponent<GameCMD>();
        }
        QualitySettings.vSyncCount = 2;
        Application.targetFrameRate = AppConst.GameFrameRate;

        // 设置应用不休眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    private void OnDestroy()
    {

    }
}