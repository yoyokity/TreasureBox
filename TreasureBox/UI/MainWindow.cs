using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using TreasureBox.Helper;
using TreasureBox.Helper.Viod;

namespace TreasureBox.UI;

public class MainWindow : Window, IDisposable
{
    private string _selectedMenu = "Main";
    private Dictionary<string, IPlugin> _plugins;
    
    public MainWindow(TreasureBox plugin) : base($"{plugin.PluginName}###{plugin.PluginName}")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        RespectCloseHotkey = false;
        _plugins = plugin.Plugins;
    }

    public override void Draw()
    {
        ImGui.BeginChild("###Toolbox插件", new Vector2(100, 0), false);

        var path = _selectedMenu == "Main" ? "" : _plugins[_selectedMenu].ImgPath;
        path = path == "" ? @"Resources\img\yoship.png" : path;

        var image = Helper.ImageHelper.Import(path);
        if (image != null) ImGui.Image((IntPtr)image, new Vector2(100, 100));

        ImGui.Indent(5);
        ImGui.Separator();
        ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(0f, 0.5f));
        if (ImGui.Selectable("Main", _selectedMenu == "Main"))
            _selectedMenu = "Main";
        ImGui.PopStyleVar();

        ImGui.Separator();

        foreach (var v in _plugins.Keys)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(0f, 0.5f));
            if (ImGui.Selectable(v, _selectedMenu == v))
                _selectedMenu = v;
            ImGui.PopStyleVar();
        }

        ImGui.Unindent();
        ImGui.Separator();
        ImGui.EndChild();
        ImGui.SameLine();

        ImGui.BeginChild("###ToolboxMain", new Vector2(0, 0), true);
        if (string.IsNullOrEmpty(_selectedMenu))
            _selectedMenu = _plugins.FirstOrDefault().Key;
        if (_selectedMenu == "Main")
        {
            Main.MainDraw();
        }
        else
        {
            var plugin = _plugins[_selectedMenu];
            plugin.Draw();
        }

        ImGui.EndChild();
    }

    public void Dispose()
    {
    }
}

public class Main
{
    private static int _devColor;
    private static int _itemId;
    private static string[] itemFlag = Enum.GetNames(typeof(ItemFlag));
    private static int _itemFlag = 0;

    public static void MainDraw()
    {
        if (ImGui.CollapsingHeader("虚空交互"))
        {
            ImGuiHelper.TextColor(Color.Orange, "请在当前地图有需要交互的对象时使用，选中同类型单位作为目标。");

            ImGui.Bullet();
            ImGui.SameLine();
            ImGui.Text("NPC: 可选雇员、水晶、主城NPC、部队箱、房区门牌");
            if (ImGui.Button("雇员"))
                VoidAddonHelper.Start(721440);
            ImGui.SameLine();
            if (ImGui.Button("市场板"))
                VoidAddonHelper.Start(720935);
            ImGui.SameLine();
            if (ImGui.Button("魔晶石"))
                VoidAddonHelper.Start(721253);
            ImGuiHelper.SetHoverTooltip("不能离魔晶石师傅太远");
            ImGui.SameLine();
            if (ImGui.Button("大水晶"))
            {
                switch (Svc.ClientState.TerritoryType)
                {
                    case 132:
                        VoidAddonHelper.Start(327682);
                        break;
                    case 130:
                        VoidAddonHelper.Start(327689);
                        break;
                    case 129:
                        VoidAddonHelper.Start(327688);
                        break;
                }
            }
            ImGuiHelper.SetHoverTooltip("仅限三大主城");
            ImGui.NewLine();

            ImGui.Bullet();
            ImGui.SameLine();
            ImGui.Text("军队: 可选雇员、水晶、主城NPC、部队箱");
            if (ImGui.Button("交换材料"))
            {
                switch (Svc.ClientState.TerritoryType)
                {
                    case 132:
                        VoidAddonHelper.Start(1441794);//森
                        break;
                    case 130:
                        VoidAddonHelper.Start(1441795);//沙
                        break;
                    case 129:
                        VoidAddonHelper.Start(1441793);//海
                        break;
                }
            }
            ImGui.NewLine();
            //if (ImGui.Button("测试"))
            //{
            //    Core.Get<IMemApiAddon>().SetAddonClicked("MYCWeaponAdjust2", [11, -162, 210, 210, 210, 0, 0, 0, 0]);
            //}
            //if (ImGui.Button("测试1"))
            //{
            //    KeyHelper.Send(Keys.U);
            //    Core.Get<IMemApiAddon>().SetAddonClicked("ContentsFinderMenu", [0]);
            //    Core.Get<IMemApiAddon>().SetAddonClicked("SelectYesno", [0]);
            //}

        }

        if (ImGui.CollapsingHeader("dev"))
        {
            ImGui.Checkbox("显示交互网络日志", ref VoidCore._networkLog);
            ImGui.Checkbox("显示其它网络日志", ref VoidCore._networkLogOther);

            if (ImGui.Button("打印彩色文本"))
            {
                ChatHelper.Print.ColorText("[百宝箱] 打印测试", _devColor);
            }
            ImGui.SameLine(0, 20);
            ImGui.SetNextItemWidth(100);
            ImGui.InputInt("Color", ref _devColor);
            if (_devColor < 0)
                _devColor = 0;

            if (ImGui.Button("查询物品数量"))
            {
                LogHelper.Tips($"{ItemHelper.FindItem((uint)_itemId, (ItemFlag)_itemFlag)}");
            }
            ImGui.SameLine(0, 20);
            ImGui.SetNextItemWidth(100);
            ImGui.InputInt("ItemId", ref _itemId);
            if (_itemId < 0)
                _itemId = 0;
            ImGui.SameLine();
            ImGui.SetNextItemWidth(100);
            ImGui.Combo("Type", ref _itemFlag, itemFlag, itemFlag.Length);

            状态树();
        }
    }

    private static void 状态树()
    {
        if (ImGui.TreeNode("玩家状态"))
        {
            unsafe
            {
                ImGui.Text($"注销: {Svc.Condition[ConditionFlag.LoggingOut]}");
                ImGui.Text($"过图: {Svc.Condition[ConditionFlag.BetweenAreas]}");
                ImGui.Text($"观看动画：{Svc.Condition[ConditionFlag.WatchingCutscene]}");
                ImGui.Text($"战斗状态: {Svc.Condition[ConditionFlag.InCombat]}");
                ImGui.Text($"正在施法: {Svc.Condition[ConditionFlag.Casting]}");
                ImGui.Text($"正在移动: {AgentMap.Instance()->IsPlayerMoving == 1}");
                ImGui.Text($"在坐骑上: {Svc.Condition[ConditionFlag.Mounted]}");
                ImGui.Text($"飞行中: {Svc.Condition[ConditionFlag.InFlight]}");
                ImGui.Text($"跳跃: {Svc.Condition[ConditionFlag.Jumping]}");
                ImGui.Text($"正在使用传唤铃: {Svc.Condition[ConditionFlag.OccupiedSummoningBell]}");
                ImGui.Text($"任务中: {Svc.Condition[ConditionFlag.OccupiedInQuestEvent]}");

                //ProperOnLogin.PlayerPresent
                ImGui.TreePop();
            }
        }
    }
}