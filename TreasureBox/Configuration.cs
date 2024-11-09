using Dalamud.Configuration;
using System;
using ECommons.DalamudServices;

namespace TreasureBox;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;
    
    //
    public bool 开主线 = true;
    public bool 练级1天 = false;
    public bool 自动分配路线 = true;
    public int 自动等级 = 85;
    public string 自动路线 = "OJ";
    public string 自动配件船首 = "2";
    public string 自动配件船体 = "3";
    public string 自动配件舰桥 = "4";
    public string 自动配件船尾 = "1";
    public string 潜艇1路线 = "无";
    public string 潜艇2路线 = "无";
    public string 潜艇3路线 = "无";
    public string 潜艇4路线 = "无";

    
    public void Save()
    {
        Svc.PluginInterface.SavePluginConfig(this);
    }
}