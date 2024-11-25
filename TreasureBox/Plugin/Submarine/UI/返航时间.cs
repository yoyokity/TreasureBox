using System;
using ImGuiNET;

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
                var formatted = $"{timeSpan.Days}天{timeSpan.Hours}小时{timeSpan.Minutes + 1}分钟";
                ImGui.Text($"  {shipName}：{formatted}");
            }
        }
    }
}