using System.Linq;
using ECommons.DalamudServices;

namespace TreasureBox.Helper;

public static class PluginHelper
{
    public static bool IsPluginEnabled(string internalName)
    {
        var re = Svc.PluginInterface.InstalledPlugins.Any(x => x.InternalName == internalName && x.IsLoaded);
        if (!re)
        {
            LogHelper.PrintError($"未安装插件{internalName}!");
        }

        return re;
    }
}