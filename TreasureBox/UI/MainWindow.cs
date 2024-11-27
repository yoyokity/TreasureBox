using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Utility.Numerics;
using ImGuiNET;
using TreasureBox.Helper;
using ECommons.Automation.LegacyTaskManager;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using TreasureBox.Plugin.Submarine;

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
        path = path == "" ? @"Resources\yoship.png" : path;

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
        if (ImGui.CollapsingHeader("快捷传送", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.Dummy(new Vector2(0, 3));
            if (ImGui.Button("自己服务器", new Vector2(100, 40)))
            {
                ChatHelper.SendMessage("/li");
            }
            
            ImGui.Dummy(new Vector2(0, 3));
            if (ImGui.Button("军队", new Vector2(100, 40)))
            {
                ChatHelper.SendMessage("/li gc");
            }
            
            ImGui.Dummy(new Vector2(0, 3));
            if (ImGui.Button("部队房", new Vector2(100, 40)))
            {
                ChatHelper.SendMessage("/li fc");
            }

            ImGui.SameLine();
            if (ImGui.Button("部队工坊", new Vector2(100, 40)))
            {
                if (!PosHelper.NavIsEnabled)
                {
                    LogHelper.PrintError("请先安装vnavmesh自动导航插件");
                    return;
                }
                var obj = ObjectHelper.FindObject(多语言文本.进入房屋);
                if (obj == null) return;
                
                ObjectHelper.SelectTarget(obj);
                P.TaskManager.Abort();
                P.TaskManager.Enqueue(() => PosHelper.MoveTo(ObjectHelper.Target.Position));
                P.TaskManager.Enqueue(() => AddonHelper.InteractWithUnit(obj.EntityId));
                P.TaskManager.Enqueue(() => AddonHelper.CheckAddon("SelectYesno"));
                P.TaskManager.Enqueue(() => AddonHelper.SetAddonClicked("SelectYesno", 0));
                P.TaskManager.Enqueue(PosHelper.WaitBetweenAreas);
                P.TaskManager.Enqueue(() =>
                {
                    obj = ObjectHelper.FindObject(多语言文本.移动到其他房间);
                    PosHelper.MoveTo(obj.Position);
                });
                P.TaskManager.DelayNext(1000);
                P.TaskManager.Enqueue(() => ObjectHelper.SelectTarget(obj));
                P.TaskManager.Enqueue(() => AddonHelper.InteractWithUnit(obj.EntityId));
            }
        }
    }
}