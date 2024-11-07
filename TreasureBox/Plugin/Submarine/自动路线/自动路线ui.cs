using System;
using ImGuiNET;

namespace TreasureBox.Plugin.Submarine;

public class 自动分配路线ui
{
    private static string[] 自动捞金路线 = new[] { "OJ", "MROJZ", "JORZ" };
    private static int _自动捞金路线index = 0;

    private static string[] 配件list = new[] { "1", "2", "3", "4", "5", "1改", "2改", "3改", "4改", "5改" };
    private static int _配件船首index = 0;
    private static int _配件船体index = 0;
    private static int _配件舰桥index = 0;
    private static int _配件船尾index = 0;

    public static void Draw()
    {
        ImGui.Text("满足以下条件则捞金，不满足则练级：");

        _自动捞金路线index = Array.IndexOf(自动捞金路线, Settings.Instance.自动路线);
        if (ImGui.Combo("捞金路线", ref _自动捞金路线index, 自动捞金路线, 自动捞金路线.Length))
        {
            Settings.Instance.自动路线 = 自动捞金路线[_自动捞金路线index];
            Settings.Instance.Save();
        }


        if (ImGui.InputInt("等级", ref Settings.Instance.自动等级))
        {
            Settings.Instance.Save();
        }

        const int width = 79;

        ImGui.SetNextItemWidth(width);
        _配件船体index = Array.IndexOf(配件list, Settings.Instance.自动配件船体);
        if (ImGui.Combo("船体", ref _配件船体index, 配件list, 配件list.Length))
        {
            Settings.Instance.自动配件船体 = 配件list[_配件船体index];
            Settings.Instance.Save();
        }
        ImGui.SameLine();
        ImGui.SetNextItemWidth(width);
        _配件船尾index = Array.IndexOf(配件list, Settings.Instance.自动配件船尾);
        if (ImGui.Combo("船尾", ref _配件船尾index, 配件list, 配件list.Length))
        {
            Settings.Instance.自动配件船尾 = 配件list[_配件船尾index];
            Settings.Instance.Save();
        }

        ImGui.SetNextItemWidth(width);
        _配件船首index = Array.IndexOf(配件list, Settings.Instance.自动配件船首);
        if (ImGui.Combo("船首", ref _配件船首index, 配件list, 配件list.Length))
        {
            Settings.Instance.自动配件船首 = 配件list[_配件船首index];
            Settings.Instance.Save();
        }
        ImGui.SameLine();
        ImGui.SetNextItemWidth(width);
        _配件舰桥index = Array.IndexOf(配件list, Settings.Instance.自动配件舰桥);
        if (ImGui.Combo("舰桥", ref _配件舰桥index, 配件list, 配件list.Length))
        {
            Settings.Instance.自动配件舰桥 = 配件list[_配件舰桥index];
            Settings.Instance.Save();
        }
    }
}