using UnityEngine;

public class AppConst
{
    public static bool LogMode = false;                                 // 日志模式
    public static bool ShowFps = false;                                 // 显示帧频
    public static int GameFrameRate = 30;                               // 帧频数
    public static PlatformType platformType = PlatformType.Android;     // 当前平台

    public const string FrameworkName = "GameSettings";                 // 游戏设置
    public static string AppPrefix = Application.productName + "_";     // 应用程序前缀
}