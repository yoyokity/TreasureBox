using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TreasureBox.Helper;
using TreasureBox.Plugin.Submarine.UI;
using TreasureBox.Plugin.Submarine.练级;

namespace TreasureBox.Plugin.Submarine.Action;

public static class Addon
{
    public const string 选择界面 = "SelectString";
    public const string 对话框 = "Talk";
    public const string 选择航海图 = "SubmarineExplorationMapSelect";
    public const string 选择航线 = "AirShipExploration";
    public const string 修理界面 = "CompanyCraftSupply";
    public const string 报告界面 = "AirShipExplorationResult";
    public const string 出发对话框 = "AirShipExplorationDetail";
    public const string 确认框 = "SelectYesno";
}

public static class 收艇
{
    public static List<string> Tips = new();
    private static void Print(string text) => LogHelper.Normal(text, "潜艇");
    private static void PrintTips(string text) => LogHelper.Tips(text, "潜艇");
    private static void PrintError(string text) => LogHelper.Error(text, "潜艇");


    private static 海图 _海图;

    public static async Task Run()
    {
        Main.潜艇 = [new(), new(), new(), new()];
        Tips.Clear();
        收益.本次收益 = 0;

        //确保所有对话框关闭
        if (AddonHelper.CheckAddon(Addon.选择航海图) || AddonHelper.CheckAddon(Addon.选择界面) ||
            AddonHelper.CheckAddon(Addon.对话框) || AddonHelper.CheckAddon(Addon.修理界面) ||
            AddonHelper.CheckAddon(Addon.选择航线) || AddonHelper.CheckAddon(Addon.报告界面) ||
            AddonHelper.CheckAddon(Addon.确认框) || AddonHelper.CheckAddon(Addon.出发对话框))
        {
            PrintError("请先关闭所有对话框。");
            return;
        }

        Print("一键收艇开始。。。");
        await 打开潜水艇面板();

        await 一键收艇();

        Print("收艇完成");
        ChatHelper.Print.Echo("");
        foreach (var line in Tips)
        {
            PrintTips(line);
        }

        Option.ClickedStop = false;
    }

    static async Task 一键收艇()
    {
        if (!await AddonHelper.WaitAddonUntil(Addon.选择界面) || Option.ClickedStop)
            return;
        AddonHelper.SetAddonClicked(Addon.选择界面, 1);
        //潜水艇基础面板
        if (!await AddonHelper.WaitAddonUntil(Addon.选择界面) || Option.ClickedStop)
            return;

        Regex r = new(@"(?<name>.*)  \[(?<level>\d+)级\]   ?\[?(?<state>探索完成|正在探索|探索完了|探索中)?");

        //登记潜艇
        uint index = 7;
        foreach (var 潜艇 in Main.潜艇)
        {
            var v = AddonHelper.GetAddonValue(Addon.选择界面, index).String;
            if (r.IsMatch(v))
            {
                潜艇.Enabled = true;
                潜艇.Name = r.Match(v).Groups["name"].Value;
                潜艇.Level = uint.Parse(r.Match(v).Groups["level"].Value);
                var state = r.Match(v).Groups["state"].Value;
                if (state == 多语言文本.正在探索)
                    潜艇.State = 探索状态.正在探索;
                if (state == 多语言文本.探索完成)
                    潜艇.State = 探索状态.探索完成;
            }

            index++;
        }

        //收发潜艇
        var count = -1;
        foreach (var 潜艇 in Main.潜艇)
        {
            count++;
            if (!潜艇.Enabled)
                continue;
            if (潜艇.State == 探索状态.正在探索)
                continue;
            if (!await 等待潜艇选择窗口() || Option.ClickedStop)
                return;

            //统计收益0
            var 单艘收益0 = 收益.统计背包();

            //收潜艇
            if (潜艇.State == 探索状态.探索完成)
            {
                LogHelper.Normal($"点击潜艇 {潜艇.Name} {潜艇.State}");
                AddonHelper.SetAddonClicked(Addon.选择界面, count);
                if (!await AddonHelper.WaitAddonUntil(Addon.报告界面) || Option.ClickedStop)
                    return;

                //确认
                AddonHelper.SetAddonClicked(Addon.报告界面, 0);

                if (!await AddonHelper.WaitAddonUntil(Addon.选择界面) || Option.ClickedStop)
                    return;

                //统计收益1
                var 单艘收益1 = 收益.统计背包();
                var 单艘收益 = 单艘收益1 - 单艘收益0;
                收益.本次收益 += 单艘收益;

                await 修潜艇.Run(count);
                if (!await AddonHelper.WaitAddonUntil(Addon.选择界面) || Option.ClickedStop)
                    return;
                await 发潜艇(count);
            }

            if (潜艇.State == 探索状态.停靠)
            {
                LogHelper.Normal($"点击潜艇 {潜艇.Name} {潜艇.State}");
                AddonHelper.SetAddonClicked(Addon.选择界面, count);
                if (!await AddonHelper.WaitAddonUntil(Addon.选择界面) || Option.ClickedStop)
                    return;
                await 修潜艇.Run(count);
                if (!await AddonHelper.WaitAddonUntil(Addon.选择界面) || Option.ClickedStop)
                    return;
                await 发潜艇(count);
            }

            LogHelper.Normal($"{潜艇.Name}结束");
        }

        //关闭潜艇面板
        if (!await 等待潜艇选择窗口() || Option.ClickedStop)
            return;
        AddonHelper.SetAddonClicked(Addon.选择界面, -1);
        if (!await AddonHelper.WaitAddonUntil(Addon.选择界面) || Option.ClickedStop)
            return;
        AddonHelper.SetAddonClicked(Addon.选择界面, -1);
    }

    static async Task 发潜艇(int index)
    {
        //选择发艇线路
        var 路线 = "";
        if (Settings.Instance.自动分配路线)
        {
            路线 = 自动路线.选择(Main.潜艇[index]);
            LogHelper.Normal($"{Main.潜艇[index].Name} 自动选择方案为 {路线}");
        }
        else
        {
            switch (index)
            {
                case 0:
                    路线 = Settings.Instance.潜艇1路线;
                    break;
                case 1:
                    路线 = Settings.Instance.潜艇2路线;
                    break;
                case 2:
                    路线 = Settings.Instance.潜艇3路线;
                    break;
                case 3:
                    路线 = Settings.Instance.潜艇4路线;
                    break;
                default:
                    return;
            }
        }

        if (路线 == "无")
            return;

        AddonHelper.SetAddonClicked(Addon.选择界面, 0); //出发

        await Task.Delay(1000);

        //青磷水不够的情况
        if (AddonHelper.CheckAddon(Addon.对话框))
        {
            ChatHelper.Print.Urgent($"[潜艇] 青磷水不够，收艇结束");
            return;
        }

        //多张海图的情况
        var 当前海图 = "溺没海";

        if (AddonHelper.CheckAddon(Addon.选择航海图))
        {
            if (路线 != "练级")
            {
                AddonHelper.SetAddonClicked(Addon.选择航海图, 2, 0, 1);
            }
            else
            {
                if (Main.潜艇[index].Level >= 50)
                {
                    AddonHelper.SetAddonClicked(Addon.选择航海图, 2, 0, 2);
                    当前海图 = "灰海";
                }

                AddonHelper.SetAddonClicked(Addon.选择航海图, 2, 0, 1);
            }
        }

        if (!await AddonHelper.WaitAddonUntil(Addon.选择航线) || Option.ClickedStop)
            return;

        //选择航线
        if (路线 == "OJ")
        {
            选中指定航线("溺没海", "O", "J");
            await 发艇信息Print(Main.潜艇[index].Name, "OJ");
        }

        if (路线 == "MROJZ")
        {
            选中指定航线("溺没海", "M", "R", "O", "J", "Z");
            AddonHelper.SetAddonClicked(Addon.选择航线, 0, 5, 12, 17, 14, 9, 25, 25);
            await 发艇信息Print(Main.潜艇[index].Name, "MROJZ");
        }

        if (路线 == "JORZ")
        {
            选中指定航线("溺没海", "J", "O", "R", "Z");
            AddonHelper.SetAddonClicked(Addon.选择航线, 0, 4, 9, 14, 17, 25, 0, 25);
            await 发艇信息Print(Main.潜艇[index].Name, "JORZ");
        }

        if (路线 == "练级")
        {
            //记录航行距离
            var _range = AddonHelper.GetAddonValue(Addon.选择航线, 7).String;
            Regex regex = new Regex(@"\/(?<range>\d*)");
            var range = regex.Match(_range).Groups["range"].Value;
            Main.潜艇[index].Range = range;

            //记录当前海图航线情况
            _海图 = new 海图(当前海图);

            List<string> 已解锁航线 = new();
            var num = AddonHelper.GetAddonValue(Addon.选择航线, 12).UInt;
            for (int i = 0; i < num; i++)
            {
                var _index = 15 + i * 7;
                var v = AddonHelper.GetAddonValue(Addon.选择航线, (uint)_index).String;
                已解锁航线.Add(v);
            }

            _海图.解锁航线(已解锁航线);

            //
            var 潜艇 = Main.潜艇[index];
            var api = _海图.GetMogApi(当前海图, (int)潜艇.Level, 潜艇.Range, 潜艇.Speed, Settings.Instance.开主线);

            LogHelper.Normal($"海图:{当前海图} level:{潜艇.Level} range:{潜艇.Range} speed:{潜艇.Speed}");

            //MogShip求解
            求解并发艇(api, 潜艇.Name);
        }

        if (!await AddonHelper.WaitAddonUntil(Addon.出发对话框, timeout: 10000) || Option.ClickedStop)
            return;
        AddonHelper.SetAddonClicked(Addon.出发对话框, 0);

        static void 选中指定航线(string 指定海图, params string[] 航线)
        {
            var 当前海图 = new 海图(指定海图);
            List<string> 已解锁航线 = new();
            var num = AddonHelper.GetAddonValue(Addon.选择航线, 12).UInt;
            for (int i = 0; i < num; i++)
            {
                var _index = 15 + i * 7;
                var v = AddonHelper.GetAddonValue(Addon.选择航线, (uint)_index).String;
                已解锁航线.Add(v);
            }

            当前海图.解锁航线(已解锁航线);
            var route = new List<int>();
            foreach (var i in 航线)
                route.Add(当前海图.已解锁航线.IndexOf(i));

            var p1 = route.Count >= 1 ? route[0] : 0;
            var p2 = route.Count >= 2 ? route[1] : 0;
            var p3 = route.Count >= 3 ? route[2] : 0;
            var p4 = route.Count >= 4 ? route[3] : 0;
            var p5 = route.Count >= 5 ? route[4] : 0;
            AddonHelper.SetAddonClicked(Addon.选择航线, 0, route.Count, p1, p2, p3, p4, p5, route.Last());
        }
    }

    private static async Task 求解并发艇(MogApi api, string 潜艇name)
    {
        try
        {
            var re = 求解器.GetMogShip(api).Result;
            if (re != null)
            {
                var _re = 求解器.GetRoute(re, Settings.Instance.练级1天);
                if (_re != null)
                {
                    //选择路线
                    var route = new List<int>();
                    foreach (var name in _海图.解析route(_re))
                        route.Add(_海图.已解锁航线.IndexOf(name));

                    LogHelper.Normal($"{string.Join(" → ", _海图.解析route(_re))}");

                    var p1 = route.Count >= 1 ? route[0] : 0;
                    var p2 = route.Count >= 2 ? route[1] : 0;
                    var p3 = route.Count >= 3 ? route[2] : 0;
                    var p4 = route.Count >= 4 ? route[3] : 0;
                    var p5 = route.Count >= 5 ? route[4] : 0;

                    AddonHelper.SetAddonClicked(Addon.选择航线, 0, route.Count, p1, p2, p3, p4, p5, route.Last());
                    await 发艇信息Print(潜艇name, _re);
                }
                else
                {
                    LogHelper.Normal($"route信息有误或无求解方案");
                }
            }
            else
            {
                LogHelper.Normal($"获取不到MogShip信息");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return;
        }
    }

    private static async Task 发艇信息Print(string 潜艇名称, CollectionItem route)
    {
        if (!await AddonHelper.WaitAddonUntil(Addon.出发对话框) || Option.ClickedStop)
            return;

        var 航线 = _海图.解析route(route);
        var 当前海图 = route.MapID == 1 ? "溺没海" : "灰海";

        var time = AddonHelper.GetAddonValue(Addon.出发对话框, 4).String;
        double hoursDifference = 0;
        if (DateTime.TryParse(time, out DateTime targetTime))
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeDifference = targetTime - currentTime;
            hoursDifference = timeDifference.TotalHours;
        }

        Print("===============================");
        Print($"已派出 {潜艇名称} 【练级】");
        Print($"-- 航线：{当前海图} {string.Join(" → ", 航线)}");
        Print($"-- 总经验：{route.Exp}");
        Print($"-- 每分钟经验：{route.ExpPerMinute}");
        Print($"-- 总时间：{hoursDifference:F1} h");
        Print($"-- 返航时间：{time}");
    }

    private static async Task 发艇信息Print(string 潜艇名称, string 路线)
    {
        if (!await AddonHelper.WaitAddonUntil(Addon.出发对话框) || Option.ClickedStop)
            return;

        var time = AddonHelper.GetAddonValue(Addon.出发对话框, 4).String;
        double hoursDifference = 0;
        if (DateTime.TryParse(time, out DateTime targetTime))
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan timeDifference = targetTime - currentTime;
            hoursDifference = timeDifference.TotalHours;
        }

        Print("===============================");
        Print($"已派出 {潜艇名称} 【捞金】");
        Print($"-- 航线：溺没海 {string.Join(" → ", 路线.Select(c => c.ToString()))}");
        Print($"-- 总时间：{hoursDifference:F1} h");
        Print($"-- 返航时间：{time}");
    }


    static async Task 打开潜水艇面板()
    {
        if (AddonHelper.CheckAddon(Addon.选择界面))
        {
            AddonHelper.InteractWithUnit(多语言文本.航行管制面板);
        }
    }

    private static async Task<bool> 等待潜艇选择窗口(int timeout = 60000, int delay = 300)
    {
        await Task.Delay(delay / 2);

        const string addonName = Addon.选择界面;
        var startTime = DateTime.Now;
        var timeoutSpan = TimeSpan.FromMilliseconds(timeout);

        while (DateTime.Now - startTime <= timeoutSpan)
        {
            if (AddonHelper.CheckAddon(addonName))
            {
                var v = AddonHelper.GetAddonValue(addonName, 2).String;
                if (v.StartsWith(多语言文本.请选择潜水艇))
                {
                    await Task.Delay(delay / 2);
                    return true;
                }
            }

            await Task.Delay(500); //500毫秒判断一次
        }

        return false;
    }
}