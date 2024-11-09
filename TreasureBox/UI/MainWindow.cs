using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Windowing;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;
using TreasureBox.Helper;

namespace TreasureBox.UI;

public class MainWindow : Window, IDisposable
{
    private string _selectedMenu = "Main";
    private Dictionary<string, IPlugin> _plugins;

    public MainWindow(TreasureBox plugin) : base($"{plugin.PluginName}###{plugin.PluginName}")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        RespectCloseHotkey = false;
        _plugins = plugin.Plugins;
    }

    public override void Draw()
    {
        ImGui.BeginChild("###Toolbox插件", new Vector2(100, 0), false);

        var path = _selectedMenu == "Main" ? "" : _plugins[_selectedMenu].ImgPath;
        path = path == "" ? @"Resources\img\yoship.png" : path;

        var image = FileHelper.ImportImage(path);
        if (image != null) ImGui.Image((IntPtr)image, new Vector2(100, 100));

        ImGui.Indent(5);
        ImGui.Separator();
        ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(0f, 0.5f));
        if (ImGui.Selectable("Main", _selectedMenu == "Main"))
            _selectedMenu = "Main";
        ImGui.PopStyleVar();

        ImGui.Separator();

        foreach (var v in _plugins.Keys)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Vector2(0f, 0.5f));
            if (ImGui.Selectable(v, _selectedMenu == v))
                _selectedMenu = v;
            ImGui.PopStyleVar();
        }

        ImGui.Unindent();
        ImGui.Separator();
        ImGui.EndChild();
        ImGui.SameLine();

        ImGui.BeginChild("###ToolboxMain", new Vector2(0, 0), true);
        if (string.IsNullOrEmpty(_selectedMenu))
            _selectedMenu = _plugins.FirstOrDefault().Key;
        if (_selectedMenu == "Main")
        {
            Main.MainDraw();
        }
        else
        {
            var plugin = _plugins[_selectedMenu];
            plugin.Draw();
        }

        ImGui.EndChild();
    }

    public void Dispose()
    {
    }
}

public class Main
{
    private static int _devColor;
    private static int _itemId;
    private static string[] itemFlag = Enum.GetNames(typeof(ItemFlag));
    private static int _itemFlag = 0;

    public static void MainDraw()
    {
    }
}