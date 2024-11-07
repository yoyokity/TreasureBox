using System;
using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace TreasureBox.Helper;

public static class ImageHelper
{
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;

    public static IntPtr? Import(string path)
    {
        var goatImage = TextureProvider.GetFromFile(path).GetWrapOrDefault();
        return goatImage?.ImGuiHandle;
    }
}