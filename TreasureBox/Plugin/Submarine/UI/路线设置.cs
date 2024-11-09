using System;
using System.Numerics;
using ImGuiNET;
using TreasureBox.Helper;

namespace TreasureBox.Plugin.Submarine.UI;

public class 路线设置
{
    private static string[] 路线 = new[] { "无", "OJ", "MROJZ", "JORZ", "练级" };
    private static int _潜艇1 = 0;
    private static int _潜艇2 = 0;
    private static int _潜艇3 = 0;
    private static int _潜艇4 = 0;

    public static void Draw()
    {
        ImGui.BeginChild("###潜水艇路线选择子窗口", new Vector2(300, 150), false);
        if (Settings.Instance.自动分配路线)
        {
            自动分配路线ui.Draw();
        }
        else
        {
            潜艇设置();
        }
        ImGui.EndChild();

        ImGuiHelper.Separator();

        {
            if (ImGui.Checkbox("自动分配路线", ref Settings.Instance.自动分配路线))
            {
                Settings.Instance.Save();
            }

            if (ImGui.Checkbox("练级时开主线图", ref Settings.Instance.开主线))
            {
                Settings.Instance.Save();
            }

            ImGuiHelper.SetHoverTooltip("练级不会帮你开支线，后期捞金要跑MR这两个地方的自己开图（老老实实OJ行不行？）");
            ImGui.SameLine();
            if (ImGui.Checkbox("练级只跑一天路线", ref Settings.Instance.练级1天))
            {
                Settings.Instance.Save();
            }
            ImGuiHelper.SetHoverTooltip("不会跑超过24小时的路线");
        }
    }

    private static void 潜艇设置()
    {
        ImGui.Text("手动路线选择：");
        _潜艇1 = Array.IndexOf(路线, Settings.Instance.潜艇1路线);
        if (ImGui.Combo("1号潜艇", ref _潜艇1, 路线, 路线.Length))
        {
            Settings.Instance.潜艇1路线 = 路线[_潜艇1];
            Settings.Instance.Save();
        }

        _潜艇2 = Array.IndexOf(路线, Settings.Instance.潜艇2路线);
        if (ImGui.Combo("2号潜艇", ref _潜艇2, 路线, 路线.Length))
        {
            Settings.Instance.潜艇2路线 = 路线[_潜艇2];
            Settings.Instance.Save();
        }

        _潜艇3 = Array.IndexOf(路线, Settings.Instance.潜艇3路线);
        if (ImGui.Combo("3号潜艇", ref _潜艇3, 路线, 路线.Length))
        {
            Settings.Instance.潜艇3路线 = 路线[_潜艇3];
            Settings.Instance.Save();
        }

        _潜艇4 = Array.IndexOf(路线, Settings.Instance.潜艇4路线);
        if (ImGui.Combo("4号潜艇", ref _潜艇4, 路线, 路线.Length))
        {
            Settings.Instance.潜艇4路线 = 路线[_潜艇4];
            Settings.Instance.Save();
        }
    }
}