using System;
using Dalamud.Game;

namespace TreasureBox.Plugin.Submarine;

public static class 多语言文本
{
    public static ClientLanguage Lan = ClientLanguage.ChineseSimplified;

    public static string 潘多拉魔盒 => "Pandora's Box";
    public static string 进入房屋 => Lan switch
    {
        ClientLanguage.ChineseSimplified => "进入房屋",
        ClientLanguage.English => "entrance",
        ClientLanguage.Japanese => "ハウスへ入る",
        _ => "进入房屋"
    };

    public static string 移动到其他房间 => Lan switch
    {
        ClientLanguage.ChineseSimplified => "移动到其他房间",
        ClientLanguage.English => "entrance to additional chambers",
        ClientLanguage.Japanese => "別室へ移動する",
        _ => "移动到其他房间"
    };
    
    public static string 交纳素材 => Lan switch
    {
        ClientLanguage.ChineseSimplified => "交纳素材",
        ClientLanguage.English => "Project Material Delivery",
        ClientLanguage.Japanese => "素材の納品",
        _ => "交纳素材"
    };
    
    public static string 推进工程进展 => Lan switch
    {
        ClientLanguage.ChineseSimplified => "推进工程进展",
        ClientLanguage.English => "Project Progression",
        ClientLanguage.Japanese => "工程進捗の実行",
        _ => "推进工程进展"
    };
    
    public static string 领取道具 => Lan switch
    {
        ClientLanguage.ChineseSimplified => "领取道具",
        ClientLanguage.English => "Collect finished product",
        ClientLanguage.Japanese => "アイテムを受け取る",
        _ => "领取道具"
    };
    
    public static string 航行管制面板 => Lan switch
    {
        ClientLanguage.ChineseSimplified => "航行管制面板",
        ClientLanguage.English => "voyage control panel",
        ClientLanguage.Japanese => "管制卓",
        _ => "航行管制面板"
    };
    
    public static string 部队合建设备 => Lan switch
    {
        ClientLanguage.ChineseSimplified => "部队合建设备",
        ClientLanguage.English => "fabrication station",
        ClientLanguage.Japanese => "カンパニー製作設備",
        _ => "部队合建设备"
    };
    
    public static string 冒险人偶014号 => Lan switch
    {
        ClientLanguage.ChineseSimplified => "冒险人偶014号",
        ClientLanguage.English => "mammet voyager #004A",
        ClientLanguage.Japanese => "マメット014・アドベンチャラー",
        _ => "冒险人偶014号"
    };
    
    public static string 部队制图板 => Lan switch
    {
        ClientLanguage.ChineseSimplified => "部队制图板",
        ClientLanguage.English => "schematic board",
        ClientLanguage.Japanese => "カンパニー製図板",
        _ => "部队制图板"
    };
    
    public static string 传唤铃 => Lan switch
    {
        ClientLanguage.ChineseSimplified => "传唤铃",
        ClientLanguage.English => "summoning bell",
        ClientLanguage.Japanese => "呼び鈴",
        _ => "传唤铃"
    };
    
    public static string 部队储物柜=> Lan switch
    {
        ClientLanguage.ChineseSimplified => "部队储物柜",
        ClientLanguage.English => "Company Chest",
        ClientLanguage.Japanese => "カンパニーチェスト",
        _ => "部队储物柜"
    };
    
    public static string 请选择潜水艇 => Lan switch
    {
        ClientLanguage.ChineseSimplified => "请选择潜水艇",
        ClientLanguage.Japanese => "潜水艦を選択",
        _ => "请选择潜水艇"
    };

    public static string 探索完成 => Lan switch
    {

        ClientLanguage.ChineseSimplified => "探索完成",
        ClientLanguage.Japanese => "探索完了",
        _ => "探索完成"

    };
    
    public static string 正在探索 => Lan switch
    {

        ClientLanguage.ChineseSimplified => "正在探索",
        ClientLanguage.Japanese => "探索中",
        _ => "正在探索"

    };
}