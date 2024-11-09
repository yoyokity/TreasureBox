using System;
using Dalamud.Game;

namespace TreasureBox.Plugin.Submarine;

public static class 多语言文本
{
    public static ClientLanguage Lan = ClientLanguage.ChineseSimplified;

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