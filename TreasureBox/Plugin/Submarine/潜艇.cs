namespace TreasureBox.Plugin.Submarine;

public class 潜艇
{
    public bool Enabled = false;
    public string Name = "";
    public uint Level = 0;
    public 探索状态 State = 探索状态.停靠;
    public 潜艇配件.船首 船首配件;
    public 潜艇配件.船体 船体配件;
    public 潜艇配件.舰桥 舰桥配件;
    public 潜艇配件.船尾 船尾配件;
    public int Speed;
    public string Range;
}

public enum 探索状态
{
    停靠,
    正在探索,
    探索完成
}

public static class 潜艇配件
{
    public static string 转俗语(船体 配件)
    {
        var value = (int)配件 + 1;
        return value > 5 ? $"{value - 5}改" : value.ToString();
    }

    public static string 转俗语(船尾 配件)
    {
        var value = (int)配件 + 1;
        return value > 5 ? $"{value - 5}改" : value.ToString();
    }

    public static string 转俗语(船首 配件)
    {
        var value = (int)配件 + 1;
        return value > 5 ? $"{value - 5}改" : value.ToString();
    }

    public static string 转俗语(舰桥 配件)
    {
        var value = (int)配件 + 1;
        return value > 5 ? $"{value - 5}改" : value.ToString();
    }

    public enum 船体
    {
        鲨鱼级船体,
        甲鲎级船体,
        须鲸级船体,
        腔棘鱼级船体,
        希尔德拉级船体,
        鲨鱼改级船体,
        甲鲎改级船体,
        须鲸改级船体,
        腔棘鱼改级船体,
        希尔德拉改级船体
    }

    public enum 船尾
    {
        鲨鱼级船尾,
        甲鲎级船尾,
        须鲸级船尾,
        腔棘鱼级船尾,
        希尔德拉级船尾,
        鲨鱼改级船尾,
        甲鲎改级船尾,
        须鲸改级船尾,
        腔棘鱼改级船尾,
        希尔德拉改级船尾
    }

    public enum 船首
    {
        鲨鱼级船首,
        甲鲎级船首,
        须鲸级船首,
        腔棘鱼级船首,
        希尔德拉级船首,
        鲨鱼改级船首,
        甲鲎改级船首,
        须鲸改级船首,
        腔棘鱼改级船首,
        希尔德拉改级船首
    }

    public enum 舰桥
    {
        鲨鱼级舰桥,
        甲鲎级舰桥,
        须鲸级舰桥,
        腔棘鱼级舰桥,
        希尔德拉级舰桥,
        鲨鱼改级舰桥,
        甲鲎改级舰桥,
        须鲸改级舰桥,
        腔棘鱼改级舰桥,
        希尔德拉改级舰桥
    }
}