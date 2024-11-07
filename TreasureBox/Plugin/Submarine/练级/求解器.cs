using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TreasureBox.Helper;

namespace TreasureBox.Plugin.Submarine.练级;

public class MogApi
{
    public int Rank = 1;
    public string Range;
    public int Speed = 110;
    public string Sort = "ExpPerMinute,desc";
    public List<int> IncludeSectors = new();
    public List<int> ExcludeSectors = new();
    public List<int> IncludeMaps = new();
    public List<int> ExcludeMaps = new();
}

public class 求解器
{
    private const string BaseUrl = "https://api.mogship.com/submarine/exp-calculator";

    public static async Task<Route> GetMogShip(MogApi api)
    {
        var client = new HttpClient();
        try
        {
            var text = $"{BaseUrl}?";
            text += $"rank={api.Rank}&";
            text += $"range={api.Range}&";
            text += $"speed={api.Speed}&";
            text += $"sort={api.Sort}&";
            if (api.IncludeSectors.Count > 0)
                text += $"includeSectors={string.Join(",", api.IncludeSectors)}&";
            if (api.ExcludeSectors.Count > 0)
                text += $"excludeSectors={string.Join(",", api.ExcludeSectors)}&";
            if (api.IncludeMaps.Count > 0)
                text += $"includeMaps={string.Join(",", api.IncludeMaps)}&";
            if (api.ExcludeMaps.Count > 0)
                text += $"excludeMaps={string.Join(",", api.ExcludeMaps)}";

            if (text.EndsWith("&"))
            {
                text = text[..^1];
            }

            var response = await client.GetAsync(text);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            //如果含IncludeSectors，且返回为空，则尝试去掉IncludeSectors再次请求
            if (responseBody == "{\"collection\":[]}" && api.IncludeSectors.Count > 0)
            {
                text = $"{BaseUrl}?";
                text += $"rank={api.Rank}&";
                text += $"range={api.Range}&";
                text += $"speed={api.Speed}&";
                text += $"sort={api.Sort}&";
                if (api.ExcludeSectors.Count > 0)
                    text += $"excludeSectors={string.Join(",", api.ExcludeSectors)}&";
                if (api.IncludeMaps.Count > 0)
                    text += $"includeMaps={string.Join(",", api.IncludeMaps)}&";
                if (api.ExcludeMaps.Count > 0)
                    text += $"excludeMaps={string.Join(",", api.ExcludeMaps)}";

                if (text.EndsWith("&"))
                {
                    text = text[..^1];
                }

                LogHelper.Normal($"再次发送请求 {text}");

                response = await client.GetAsync(text);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
            }

            var newRoute = JsonConvert.DeserializeObject<Route>(responseBody);

            return newRoute;
        }
        catch (HttpRequestException ex)
        {
            LogHelper.Error($"MogShip请求失败：{ex.Message}");
            return null;
        }
    }

    public static CollectionItem? GetRoute(Route? route, bool oneDay = false)
    {
        if (route.Collection.Count == 0)
            return null;

        if (!oneDay)
        {
            return route.Collection[0];
        }

        //一天的方案
        foreach (var 方案 in route.Collection)
        {
            var time = 方案.Exp / (float)方案.ExpPerMinute / 60;
            if (time <= 24)
            {
                return 方案;
            }
        }

        LogHelper.Tips("由于找不到满足一天的航线，因此该潜艇将使用经验比最高航线。");
        return route.Collection[0];
    }
}