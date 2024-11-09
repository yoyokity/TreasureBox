using System.Drawing;
using System.Numerics;
using ECommons.DalamudServices;
using ImGuiNET;
using TreasureBox.Helper;
using TreasureBox.Plugin.Submarine.Action;
using TreasureBox.Plugin.Submarine.UI;

namespace TreasureBox.Plugin.Submarine;

public class Main : IPlugin
{
    public string Name => "潜水艇";
    public string Version => "v1.0";
    public string ImgPath => @"Resources\img\Penguin01.jpg";
    public string Tips => "yoyo得意力作，遥遥领先！";

    public static 潜艇[] 潜艇 = [new(), new(), new(), new()];

    public void Init()
    {
        多语言文本.Lan = Svc.ClientState.ClientLanguage;
    }

    public void Draw()
    {
        //头部
        ImGui.Text($"{Version}");

        ImGui.SameLine(0, 30);

        ImGuiHelper.TextColor(Color.Orange, Tips);
        Strategy();

        ImGui.Dummy(new Vector2(0, 0));
        {
            if (ImGui.Button("一键收艇", new Vector2(70, 30)))
            {
                Option.ClickedStop = false;
                收艇.Run();
            }

            ImGui.SameLine(0, 10);
            if (ImGui.Button("停止", new Vector2(40, 30)))
            {
                Option.ClickedStop = true;
            }
        }

        //主体
        ImGui.Dummy(new Vector2(0, 0));

        if (ImGui.BeginTabBar("###潜水艇TabBar"))
        {
            if (ImGui.BeginTabItem("路线"))
            {
                ImGui.Dummy(new Vector2(0, 0));
                ImGui.SameLine(0, 5);
                ImGui.BeginChild("###潜水艇tab路线", new Vector2(-10, 0), false);
                ImGui.Dummy(new Vector2(0, 10));

                路线设置.Draw();

                ImGui.EndChild();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("收益"))
            {
                ImGui.Dummy(new Vector2(0, 0));
                ImGui.SameLine(0, 5);
                ImGui.BeginChild("###潜水艇tab收益", new Vector2(-10, 0), false);
                ImGui.Dummy(new Vector2(0, 5));

                收益.Draw();

                ImGui.EndChild();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("交互"))
            {
                ImGui.Dummy(new Vector2(0, 0));
                ImGui.SameLine(0, 5);
                ImGui.BeginChild("###潜水艇tab交互", new Vector2(-10, 0), false);
                ImGui.Dummy(new Vector2(0, 5));

                交互.Draw();

                ImGui.EndChild();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    // 攻略
    private static void Strategy()
    {
        if (!ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
            return;
        ImGui.BeginTooltip();

        ImGui.Text("如果遇到bug，反馈给 yoyokity");
        ImGui.Text("潜水艇插件仅支持中文客户端");

        ImGui.Dummy(new Vector2(0, 5));
        {
            ImGuiHelper.TextColor(Color.LimeGreen, "练级指导：");
            ImGuiHelper.TextColor(Color.LimeGreen, "1级：1111");
            ImGuiHelper.TextColor(Color.LimeGreen, "15级：1121");
            ImGuiHelper.TextColor(Color.LimeGreen, "85级：3124");
        }
        ImGui.Dummy(new Vector2(0, 5));
        {
            ImGuiHelper.TextColor(Color.Orange, "捞金指导：");
            ImGuiHelper.TextColor(Color.Orange, "85 级 3124 艇OJ荣光！");
        }
        ImGui.EndTooltip();
    }
}