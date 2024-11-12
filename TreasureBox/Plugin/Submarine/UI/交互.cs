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

        ImGui.SameLine();
        ImGuiHelper.TextColor(Color.Red, $"请先安装潘多拉魔盒{多语言文本.潘多拉魔盒}插件！并开启自动提交物品功能。");

        if (ImGui.Button("停止提交"))
        {
            P.TaskManager.Abort();
        }
    }

    private static async Task 传送到部队()
    {
        unsafe
        {
            if (ActionManager.Instance()->GetActionStatus(ActionType.GeneralAction, 7) != 0)
            {
                LogHelper.PrintError("当前无法使用传送", "部队房屋");
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

    private static void 提交潜艇合建物品()
    {
        P.TaskManager.Abort();
        // 物品提交面板
        if (AddonHelper.CheckAddon("SubmarinePartsMenu"))
        {
            var count = AddonHelper.GetAddonValue("SubmarinePartsMenu", 11).UInt;

            //循环提交
            uint indexI = 0;
            for (uint i = 0; i < count; i++)
            {
                var name = AddonHelper.GetAddonValue("SubmarinePartsMenu", 36 + i).String;
                var 需要 = AddonHelper.GetAddonValue("SubmarinePartsMenu", 60 + i).UInt;
                var 含有 = AddonHelper.GetAddonValue("SubmarinePartsMenu", 72 + i).UInt;
                var 还需提交 = AddonHelper.GetAddonValue("SubmarinePartsMenu", 120 + i).UInt -
                           AddonHelper.GetAddonValue("SubmarinePartsMenu", 108 + i).UInt;

                LogHelper.Log($"name:{name}, 需要:{需要}, 含有:{含有}, 还需提交:{还需提交}");
                if (还需提交 == 0)
                {
                    P.TaskManager.Enqueue(() => indexI++);
                    continue;
                }

                if (需要 > 含有)
                {
                    //物品不足时跳过这个
                    P.TaskManager.Enqueue(() => LogHelper.PrintInfo($"{name}不足！"));
                    P.TaskManager.Enqueue(() => indexI++);
                    continue;
                }

                //多次提交
                var 已提交 = AddonHelper.GetAddonValue("SubmarinePartsMenu", 108 + i).UInt;
                var 需要提交 = AddonHelper.GetAddonValue("SubmarinePartsMenu", 120 + i).UInt;
                uint 能提交次数 = 含有 / 需要;
                if (能提交次数 + 已提交 < 需要提交)
                {
                    P.TaskManager.Enqueue(() => LogHelper.PrintInfo($"{name}不足！"));
                    需要提交 = 能提交次数 + 已提交;
                }

                var indexJ = 已提交;
                for (var j = 已提交; j < 需要提交; j++)
                {
                    P.TaskManager.Enqueue(() => LogHelper.Log($"正在提交 {indexI} {name} {indexJ}/{需要提交}"));
                    //点击
                    P.TaskManager.Enqueue(() => AddonHelper.SetAddonClicked("SubmarinePartsMenu", 0, indexI, 需要, 0));
                    P.TaskManager.DelayNext(500);
                    P.TaskManager.Enqueue(() => AddonHelper.CheckAddon("Request"));
                    P.TaskManager.Enqueue(() => AddonHelper.SetAddonClicked("Request", 0));

                    //可能有两次确认也可能一次，两次的话有一次优质物品的确认
                    P.TaskManager.DelayNext(300);
                    P.TaskManager.Enqueue(() => AddonHelper.SetAddonClicked("SelectYesno", 0));
                    P.TaskManager.DelayNext(300);
                    P.TaskManager.Enqueue(() =>
                    {
                        if (AddonHelper.CheckAddon("SelectYesno"))
                        {
                            AddonHelper.SetAddonClicked("SelectYesno", 0);
                        }
                    });
                    P.TaskManager.DelayNext(1500);
                    P.TaskManager.Enqueue(() => indexJ++);
                }

                P.TaskManager.Enqueue(() => indexI++);
            }

            P.TaskManager.Enqueue(End);
            return;
        }

        //二级菜单
        if (AddonHelper.CheckAddon("SelectString"))
        {
            var v = AddonHelper.GetAddonValue("SelectString", 7).String;
            if (v.StartsWith(多语言文本.交纳素材))
            {
                P.TaskManager.Enqueue(() => AddonHelper.SetAddonClicked("SelectString", 0));
                P.TaskManager.DelayNext(200);
                P.TaskManager.Enqueue(() => AddonHelper.CheckAddon("SubmarinePartsMenu"));
                P.TaskManager.Enqueue(提交潜艇合建物品);
                return;
            }

            if (v.StartsWith(多语言文本.推进工程进展) || v.StartsWith(多语言文本.领取道具) || v.StartsWith("完成"))
            {
                P.TaskManager.Enqueue(() => AddonHelper.SetAddonClicked("SelectString", 0));
                P.TaskManager.Enqueue(() => AddonHelper.CheckAddon("SelectYesno"));
                P.TaskManager.Enqueue(() => AddonHelper.SetAddonClicked("SelectYesno", 0));
                P.TaskManager.Enqueue(End);
                return;
            }

            P.TaskManager.Enqueue(End);
            return;
        }

        //打开面板
        if (!Svc.Condition[ConditionFlag.OccupiedInQuestEvent])
        {
            var obj = ObjectHelper.FindObject(多语言文本.部队合建设备);
            if (obj == null)
            {
                LogHelper.PrintError("请先进入部队工坊！");
                return;
            }

            if (!PosHelper.NavIsEnabled)
            {
                if (PosHelper.Distance2D(ObjectHelper.Player.Position, obj.Position) >= 4.5)
                {
                    LogHelper.PrintError("请先靠近部队合建设备！");
                    return;
                }
            }
            else
            {
                if (PosHelper.Distance2D(ObjectHelper.Player.Position, obj.Position) >= 4.5)
                {
                    P.TaskManager.Enqueue(() => PosHelper.MoveTo(obj.Position, nearStop: 4.5f));
                }
            }

            P.TaskManager.Enqueue(() => ObjectHelper.SelectTarget(obj));
            P.TaskManager.Enqueue(() => AddonHelper.InteractWithUnit(多语言文本.部队合建设备));
            P.TaskManager.Enqueue(() => AddonHelper.CheckAddon("SelectString"));
            P.TaskManager.Enqueue(提交潜艇合建物品);
        }
        else
        {
            LogHelper.PrintError("请先关闭其余对话框!");
        }

        return;

        void End()
        {
            LogHelper.PrintSuccess("提交潜艇合建物品结束");
            P.TaskManager.Abort();
        }
    }
}