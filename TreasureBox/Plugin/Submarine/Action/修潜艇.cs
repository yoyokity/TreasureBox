using System;
using System.Threading.Tasks;
using TreasureBox.Helper;

namespace TreasureBox.Plugin.Submarine.Action;

public class 修潜艇
{
    public static async Task Run(int index)
    {
        AddonHelper.SetAddonClicked(Addon.选择界面, 3);

        await Task.Delay(1000);
        if (AddonHelper.CheckAddon(Addon.对话框))
            AddonHelper.SetAddonClicked(Addon.对话框);

        if (!await AddonHelper.WaitAddonUntil(Addon.修理界面) || Option.ClickedStop)
            return;

        LogHelper.PrintInfo($"正在登记潜水艇信息 {Main.潜艇[index].Name}");
        登记配件(Addon.修理界面, index);

        //修理
        await 修理潜艇(3);
        await 修理潜艇(11);
        await 修理潜艇(19);
        await 修理潜艇(27);

        AddonHelper.SetAddonClicked(Addon.修理界面, 5);
    }

    private static async Task 修理潜艇(int index)
    {
        if (Option.ClickedStop)
            return;
        var v = AddonHelper.GetAddonValue(Addon.修理界面, (uint)index).UInt;
        LogHelper.PrintInfo($"检查配件1 耗损度{v}");

        if (v < 1)
        {
            var 修理索引 = (index + 5) / 8 - 1;
            AddonHelper.SetAddonClicked(Addon.修理界面, 3, 0, 修理索引, 0, 0, 0);
            await Task.Delay(500);
            if (Option.ClickedStop)
                return;
            if (!AddonHelper.CheckAddon(Addon.确认框))
            {
                AddonHelper.SetAddonClicked(Addon.修理界面, 3, 0, 修理索引, 0, 0, 0);
                await Task.Delay(500);
            }

            if (!await AddonHelper.WaitAddonUntil(Addon.确认框))
                return;
            AddonHelper.SetAddonClicked(Addon.确认框, 0);
        }
    }

    static void 登记配件(string addon, int index)
    {
        var 潜艇 = Main.潜艇[index];
        var v = AddonHelper.GetAddonValue(addon, 2).String;
        潜艇.船体配件 = (潜艇配件.船体)Enum.Parse(typeof(潜艇配件.船体), v);
        v = AddonHelper.GetAddonValue(addon, 10).String;
        潜艇.船尾配件 = (潜艇配件.船尾)Enum.Parse(typeof(潜艇配件.船尾), v);
        v = AddonHelper.GetAddonValue(addon, 18).String;
        潜艇.船首配件 = (潜艇配件.船首)Enum.Parse(typeof(潜艇配件.船首), v);
        v = AddonHelper.GetAddonValue(addon, 26).String;
        潜艇.舰桥配件 = (潜艇配件.舰桥)Enum.Parse(typeof(潜艇配件.舰桥), v);

        潜艇.Speed = AddonHelper.GetAddonValue(addon, 53).Int;
        潜艇.Level = AddonHelper.GetAddonValue(addon, 32).UInt;

        //判断是否提示升级配件
        if (潜艇 is { Level: >= 15, 船体配件: 潜艇配件.船体.鲨鱼级船体, 船首配件: 潜艇配件.船首.鲨鱼级船首, 船尾配件: 潜艇配件.船尾.鲨鱼级船尾, 舰桥配件: 潜艇配件.舰桥.鲨鱼级舰桥 })
        {
            LogHelper.PrintInfo($"潜艇【{潜艇.Name}】已超过15级，建议升级配件为 1121");
            收艇.Tips.Add($"潜艇【{潜艇.Name}】已超过15级，建议升级配件为 1121");
        }

        if (潜艇.Level >= Settings.Instance.自动等级)
        {
            if (潜艇配件.转俗语(潜艇.船体配件) != Settings.Instance.自动配件船体 && 潜艇配件.转俗语(潜艇.船首配件) != Settings.Instance.自动配件船首 &&
                潜艇配件.转俗语(潜艇.船尾配件) != Settings.Instance.自动配件船尾 && 潜艇配件.转俗语(潜艇.舰桥配件) != Settings.Instance.自动配件舰桥)
            {
                LogHelper.PrintInfo(
                    $"潜艇【{潜艇.Name}】已到达{Settings.Instance.自动路线}捞金等级，建议升级配件为 {Settings.Instance.自动配件船体}{Settings.Instance.自动配件船尾}{Settings.Instance.自动配件船首}{Settings.Instance.自动配件舰桥}");
                收艇.Tips.Add(
                    $"潜艇【{潜艇.Name}】已到达{Settings.Instance.自动路线}捞金等级，建议升级配件为 {Settings.Instance.自动配件船体}{Settings.Instance.自动配件船尾}{Settings.Instance.自动配件船首}{Settings.Instance.自动配件舰桥}");
            }
        }
    }
}