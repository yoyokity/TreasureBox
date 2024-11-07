using System.Collections.Generic;
using System.Linq;

namespace TreasureBox.Plugin.Submarine.练级;

public class 海图
{
    private List<string> 航线;

    public List<string> 未解锁航线 = new();
    public List<string> 已解锁航线 = new();
    public Dictionary<string, bool> 主线;

    public 海图(string name)
    {
        if (name == "溺没海")
        {
            主线 = new Dictionary<string, bool>()
            {
                { "B", true },
                { "E", false },
                { "J", false },
                { "N", false },
                { "O", false },
                { "S", false },
                { "T", false },
                { "Y", false },
                { "Z", false },
                { "AA", false },
                { "AB", false },
                { "AD", false },
            };
            航线 = new List<string>()
            {
                "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
                "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD"
            };
        }
        else
        {
            主线 = new Dictionary<string, bool>()
            {
                { "A", true },
                { "B", false },
                { "C", false },
                { "F", false },
                { "G", false },
                { "H", false },
                { "K", false }
            };
            航线 = new List<string>()
                { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T" };
        }
    }

    public void 解锁航线(List<string> 航线列表)
    {
        //主线解锁
        foreach (var i in 主线.Keys)
        {
            if (航线列表.Contains(i))
            {
                主线[i] = true;
            }
        }

        已解锁航线 = 航线列表;
        //未解锁航线
        未解锁航线 = 航线.Except(航线列表).ToList();
    }

    private string 获取最新主线航线()
    {
        //倒序遍历
        var reversedDictionary = 主线.OrderByDescending(kv => kv.Key).ToList();
        var re = "";
        foreach (var i in reversedDictionary)
        {
            if (i.Value)
            {
                re = i.Key;
            }
        }

        return re == reversedDictionary.Last().Key ? "" : re;
    }

    public MogApi GetMogApi(string map, int rank, string range, int speed, bool 开主线 = false)
    {
        var api = new MogApi();
        api.Rank = rank;
        api.Range = range;
        api.Speed = speed;

        if (map == "溺没海")
            api.IncludeMaps.Add(1);
        else
            api.IncludeMaps.Add(2);

        if (未解锁航线.Count > 0)
        {
            foreach (var i in 未解锁航线)
            {
                api.ExcludeSectors.Add(获取航线编号(map, i));
            }
        }

        if (开主线)
        {
            var 最新主线 = 获取最新主线航线();
            if (最新主线 != "")
            {
                api.IncludeSectors.Add(获取航线编号(map, 最新主线));
            }
        }

        return api;
    }

    public List<string> 解析route(CollectionItem? route)
    {
        if (route == null)
            return new List<string>();
        var _route = route.MapID == 1
            ? route.Route.Select(x => x - 1).ToList()
            : route.Route.Select(x => x - 32).ToList();

        var re = new List<string>();
        foreach (var i in _route)
        {
            re.Add(航线[i]);
        }

        return re;
    }

    private int 获取航线编号(string map, string name)
    {
        if (map == "溺没海")
        {
            return 航线.IndexOf(name) + 1;
        }

        return 航线.IndexOf(name) + 32;
    }
}