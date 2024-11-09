using System;
using System.IO;
using ECommons.DalamudServices;

namespace TreasureBox.Helper;

public static class FileHelper
{
    /// <summary>
    /// 插件的根目录
    /// </summary>
    public static string RootDirectory => Svc.PluginInterface.AssemblyLocation.Directory?.FullName!;

    /// <summary>
    /// 合并相对路径为绝对路径
    /// </summary>
    /// <param name="path">相对路径</param>
    /// <returns></returns>
    public static string JoinPath(string path) => Path.Combine(RootDirectory, path);

    public static IntPtr? ImportImage(string path)
    {
        path = JoinPath(path);
        var goatImage = Svc.Texture.GetFromFile(path).GetWrapOrDefault();
        return goatImage?.ImGuiHandle;
    }
}