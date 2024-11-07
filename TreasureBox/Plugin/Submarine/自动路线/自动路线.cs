namespace TreasureBox.Plugin.Submarine;

public class 自动路线
{
    private static int 等级条件 => Settings.Instance.自动等级;
    private static string 船体条件 => Settings.Instance.自动配件船体;
    private static string 船尾条件 => Settings.Instance.自动配件船尾;
    private static string 船首条件 => Settings.Instance.自动配件船首;
    private static string 舰桥条件 => Settings.Instance.自动配件舰桥;

    public static string 选择(潜艇 潜艇)
    {
        //等级判断
        if (潜艇.Level < 等级条件)
            return "练级";

        //配件判断
        var 船体 = 潜艇配件.转俗语(潜艇.船体配件);
        var 船尾 = 潜艇配件.转俗语(潜艇.船尾配件);
        var 船首 = 潜艇配件.转俗语(潜艇.船首配件);
        var 舰桥 = 潜艇配件.转俗语(潜艇.舰桥配件);
        if (船体 == 船体条件 && 船尾 == 船尾条件 && 船首 == 船首条件 && 舰桥 == 舰桥条件)
            return Settings.Instance.自动路线;

        return "练级";
    }
}