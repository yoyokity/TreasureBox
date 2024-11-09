using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Utility.Numerics;
using ECommons;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using TreasureBox.Helper;

namespace TreasureBox.Plugin.Submarine.UI;

public class 交互
{
    private static int _青磷水 = 99;

    public static void Draw()
    {
        ImGui.Bullet();
        ImGui.SameLine();
        ImGui.Text("传送：");
        if (ImGui.Button("传送到部队"))
        {
            传送到部队();
        }

        ImGui.SameLine();
        if (ImGui.Button("进部队工坊"))
        {
            进部队工坊();
        }

        ImGui.NewLine();
        ImGuiHelper.TextColor(Color.Orange, "请站在部队工坊需要交互的对象面前");
        ImGui.Bullet();
        ImGui.SameLine();
        ImGui.Text("面板：");
        if (ImGui.Button("部队合建面板"))
            AddonHelper.InteractWithUnit(多语言文本.部队合建设备);

        ImGui.SameLine();
        if (ImGui.Button("潜水艇设计图"))
        {
            潜水艇设计图();
        }

        if (ImGui.Button("部队箱"))
            AddonHelper.InteractWithUnit(多语言文本.部队储物柜);

        ImGui.SameLine();
        if (ImGui.Button("雇员"))
            AddonHelper.InteractWithUnit(多语言文本.传唤铃);

        ImGui.NewLine();
        if (ImGui.Button("一键提交潜艇合建物品"))
        {
            提交潜艇合建物品();
        }

        if (!Svc.PluginInterface.InstalledPlugins.Any(p => p.InternalName == "PandorasBox" && p.IsLoaded))
        {
            ImGui.SameLine();
            ImGuiHelper.TextColor(Color.Red, "请先安装潘多拉魔盒PandorasBox插件！并开启自动提交物品功能。");
        }
    }

    private static async Task 传送到部队()
    {
        unsafe
        {
            if (ActionManager.Instance()->GetActionStatus(ActionType.GeneralAction, 7) != 0)
            {
                LogHelper.Error("当前无法使用传送", "部队房屋");
                return;
            }

            //打开传送面板
            ActionManager.Instance()->UseAction(ActionType.GeneralAction, 7);
        }

        await AddonHelper.WaitAddonUntil("Teleport");
        if (AddonHelper.GetAddonValue("Teleport", 10).String == "部队房屋")
        {
            AddonHelper.SetAddonClicked("Teleport", 0, 0, 1);
        }
        else
        {
            if (AddonHelper.GetAddonValue("Teleport", 16).String == "部队房屋")
            {
                AddonHelper.SetAddonClicked("Teleport", 0, 1, 2);
            }
        }

        //传送券
        await Task.Delay(500);
        if (AddonHelper.CheckAddon("SelectYesno"))
        {
            AddonHelper.SetAddonClicked("SelectYesno", 1);
        }
    }

    private static async Task 进部队工坊()
    {
        Svc.Objects.Where(x => "进入房屋" == x.Name.TextValue)
            .OrderBy(x => Vector3.Distance(Svc.ClientState.LocalPlayer.Position, x.Position)).TryGetFirst(out var obj);
        if (obj != null)
        {
            PosHelper.TPpos(obj.Position.WithY(obj.Position.Y - 5));
            AddonHelper.InteractWithUnit("进入房屋");
            await AddonHelper.WaitAddonUntil("SelectYesno");
            AddonHelper.SetAddonClicked("SelectYesno", 0);

            //过图
            var startTime = DateTime.Now;
            var timeoutSpan = TimeSpan.FromMilliseconds(5000);

            await Task.Delay(1500);
            while (Svc.Condition[ConditionFlag.BetweenAreas])
            {
                if (DateTime.Now - startTime >= timeoutSpan)
                    return;
                await Task.Delay(200);
            }

            await Task.Delay(1000);
        }

        Svc.Objects.Where(x => "移动到其他房间" == x.Name.TextValue)
            .OrderBy(x => Vector3.Distance(Svc.ClientState.LocalPlayer.Position, x.Position)).TryGetFirst(out var obj2);
        if (obj2 != null)
        {
            PosHelper.TPpos(obj2.Position);
            AddonHelper.InteractWithUnit("移动到其他房间");
            await AddonHelper.WaitAddonUntil("SelectString");
            AddonHelper.SetAddonClicked("SelectString", 0);
        }
    }

    private static async Task 潜水艇设计图()
    {
        AddonHelper.InteractWithUnit(多语言文本.部队制图板);
        if (await AddonHelper.WaitAddonUntil("SelectString"))
        {
            AddonHelper.SetAddonClicked("SelectString", 0);
        }
    }

    private static async Task 提交潜艇合建物品()
    {
        if (!Svc.PluginInterface.InternalName.Contains("PandorasBox"))
            return;

        //打开面板
        if (!Svc.Condition[ConditionFlag.OccupiedInQuestEvent])
            AddonHelper.InteractWithUnit(多语言文本.部队合建设备);

        //流程
        await Task.Delay(500);
        var v = AddonHelper.GetAddonValue("SelectString", 7).String;
        if (v.StartsWith("交纳素材"))
        {
            AddonHelper.SetAddonClicked("SelectString", 0);
        }
        else
        {
            if (v.StartsWith("推进") || v.StartsWith("领取"))
            {
                AddonHelper.SetAddonClicked("SelectString", 0);
                if (await AddonHelper.WaitAddonUntil("SelectYesno"))
                {
                    AddonHelper.SetAddonClicked("SelectYesno", 0);
                    await Task.Delay(1000);
                    await 提交潜艇合建物品();
                }
            }
            else
            {
                if (v.StartsWith("完成"))
                {
                    AddonHelper.SetAddonClicked("SelectString", 0);
                    await Task.Delay(5000);
                    await 提交潜艇合建物品();
                }
                else
                {
                    return;
                }
            }
        }

        //主面板
        if (!await AddonHelper.WaitAddonUntil("SubmarinePartsMenu") || Option.ClickedStop)
            return;
        var count = AddonHelper.GetAddonValue("SubmarinePartsMenu", 11).UInt;

        //循环提交
        for (uint i = 0; i < count; i++)
        {
            var name = AddonHelper.GetAddonValue("SubmarinePartsMenu", 36 + i).String;
            var 需要 = AddonHelper.GetAddonValue("SubmarinePartsMenu", 60 + i).UInt;
            var 含有 = AddonHelper.GetAddonValue("SubmarinePartsMenu", 72 + i).UInt;
            var 还需提交 = AddonHelper.GetAddonValue("SubmarinePartsMenu", 120 + i).UInt -
                       AddonHelper.GetAddonValue("SubmarinePartsMenu", 108 + i).UInt;

            if (还需提交 == 0)
                continue;
            if (需要 > 含有)
            {
                LogHelper.Error($"{name}不足！合建提交结束。", "潜艇");
                return;
            }

            //多次提交
            var 已提交 = AddonHelper.GetAddonValue("SubmarinePartsMenu", 108 + i).UInt;
            var 需要提交 = AddonHelper.GetAddonValue("SubmarinePartsMenu", 120 + i).UInt;
            for (var j = 已提交; j < 需要提交; j++)
            {
                LogHelper.Normal($"正在提交{name} {j}/{需要提交}");
                //点击
                AddonHelper.SetAddonClicked("SubmarinePartsMenu", 0, i, 需要, 0);
                LogHelper.Normal($"1");
                await Task.Delay(500);
                LogHelper.Normal($"2");
                AddonHelper.SetAddonClicked("Request", 0, 0, 0, 0);
                LogHelper.Normal($"3");

                await Task.Delay(200);

                if (AddonHelper.CheckAddon("SelectYesno"))
                {
                    if (AddonHelper.GetAddonValue("SelectYesno", 0).String.StartsWith("确定要交易优质"))
                    {
                        AddonHelper.SetAddonClicked("SelectYesno", 0);
                        await Task.Delay(200);
                    }

                    AddonHelper.SetAddonClicked("SelectYesno", 0);
                }
                else
                {
                    LogHelper.Error($"请先安装潘多拉魔盒PandorasBox插件！并开启自动提交物品功能。", "潜艇");
                    return;
                }

                await Task.Delay(1500);
            }
        }

        LogHelper.Normal("当前提交进展结束");
        await 提交潜艇合建物品();
    }
}