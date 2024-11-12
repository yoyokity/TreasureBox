using ECommons.Logging;

namespace TreasureBox.Helper;

//格式更加统一的chat输出
public static class LogHelper
{
    public static void Log(string text) => PluginLog.Log(text);
    public static void Error(string text) => PluginLog.Error(text);
    
    public static void PrintInfo(string text, string tittle = "百宝箱")
    {
        ChatHelper.Print.ColorText($"[{tittle}] {text}", 561);
        PluginLog.Log($"[{tittle}] {text}");
    }

    public static void PrintTips(string text, string tittle = "百宝箱")
    {
        ChatHelper.Print.ColorText($"[{tittle}] ", 561, $"{text}", 26);
        PluginLog.Information($"[{tittle}] {text}");
    }
    
    public static void PrintSuccess(string text, string tittle = "百宝箱")
    {
        ChatHelper.Print.ColorText($"[{tittle}] ", 561, $"{text}", 570);
        PluginLog.Log($"[{tittle}] {text}");
    }

    public static void PrintError(string text, string tittle = "百宝箱")
    {
        ChatHelper.Print.ColorText($"[{tittle}] ", 561, $"{text}", 518);
        PluginLog.Error($"[{tittle}] {text}");
    }
}