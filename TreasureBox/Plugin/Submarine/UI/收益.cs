using ImGuiNET;
using TreasureBox.Helper;

namespace TreasureBox.Plugin.Submarine.UI;

public class 收益
{
    private static int _背包 = -1;
    public static int 本次收益 = 0;

    public static void Draw()
    {
        ImGui.Text($"单次收益：{本次收益:N1}");

        if (ImGui.Button("统计背包"))
        {
            _背包 = 统计背包();
        }

        if (_背包 >= 0)
        {
            ImGui.SameLine(0, 10);
            ImGui.Text($"当前所含金币：{_背包:N1}");
        }
    }

    public static int 统计背包()
    {
        var 背包 = 0;
        背包 += ItemHelper.FindItem(22500) * 8000;
        背包 += ItemHelper.FindItem(22501) * 9000;
        背包 += ItemHelper.FindItem(22502) * 10000;
        背包 += ItemHelper.FindItem(22503) * 13000;
        背包 += ItemHelper.FindItem(22504) * 27000;
        背包 += ItemHelper.FindItem(22505) * 28500;
        背包 += ItemHelper.FindItem(22506) * 30000;
        背包 += ItemHelper.FindItem(22507) * 34500;
        return 背包;
    }
}