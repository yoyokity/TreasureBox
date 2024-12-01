using System;
using System.Drawing;
using ImGuiNET;
using TreasureBox.Helper;

namespace TreasureBox.Plugin.Submarine.UI;

public class 返航时间
{
    public static void Draw()
    {
        foreach (var (player, value) in Settings.Instance.返航时间)
        {
            if (!ImGui.CollapsingHeader($"{player}###返航时间")) continue;

            foreach (var (shipName, time) in value)
            {
                if (!DateTime.TryParse(time, out var targetTime)) continue;

                var timeSpan = targetTime - DateTime.Now;
                if (timeSpan.Ticks < 0)
                {
                    ImGuiHelper.TextColor(Color.Orange, $"  {shipName}：已返航，等待收艇中！");
                }
                else
                {
                    var formatted = $"{timeSpan.Days}天{timeSpan.Hours}小时{timeSpan.Minutes + 1}分钟";
                    ImGui.Text($"  {shipName}：{formatted}");
                }
            }
        }
    }
}