namespace TreasureBox.Helper;

//格式更加统一的chat输出
public static class LogHelper
{
    public static void Normal(string text, string tittle = "百宝箱")
    {
        ChatHelper.Print.ColorText($"[{tittle}] {text}", 561);
    }

    public static void Tips(string text, string tittle = "百宝箱")
    {
        ChatHelper.Print.ColorText($"[{tittle}] ", 561, $"{text}", 26);
    }

    public static void Error(string text, string tittle = "百宝箱")
    {
        ChatHelper.Print.ColorText($"[{tittle}] ", 561, $"{text}", 518);
    }
}