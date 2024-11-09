using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using TreasureBox.UI;

namespace TreasureBox;

public class TreasureBox : IDalamudPlugin
{
    public string PluginName => "TreasureBox";
    private const string CommandName = "/tb";

    public readonly WindowSystem WindowSystem;
    public Configuration Configuration { get; init; }
    private MainWindow MainWindow { get; init; }

    private void DrawUI() => WindowSystem.Draw();
    public void ToggleMainUI() => MainWindow.Toggle();

    public Dictionary<string, IPlugin> Plugins = new();

    public TreasureBox(IDalamudPluginInterface pluginInterface)
    {
        Svc.Init(pluginInterface);
        Configuration = Svc.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Settings.Instance = Configuration;

        WindowSystem = new WindowSystem(PluginName);
        MainWindow = new MainWindow(this);

        //添加指令
        Svc.Commands.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "打开/关闭百宝箱操作界面"
        });

        Init();
        

        WindowSystem.AddWindow(MainWindow);

        Svc.PluginInterface.UiBuilder.Draw += DrawUI;
        Svc.PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    private void Init()
    {
        //加载插件
        var assembly = Assembly.GetExecutingAssembly();
        var toolBoxTypes = assembly.GetTypes()
            .Where(type => typeof(IPlugin).IsAssignableFrom(type) && type != typeof(IPlugin));
        foreach (var type in toolBoxTypes)
        {
            if (Activator.CreateInstance(type) is not IPlugin tool) continue;
            tool.Init();
            Plugins.TryAdd(tool.Name, tool);
        }

        //图片预读取，防止第一次切换时卡顿一下
        // foreach (var p in Plugins.Values)
        // {
        //     var path = p.ImgPath == "" ? @"Resources\img\yoship.png" : p.ImgPath;
        //     Helper.ImageHelper.Import(path);
        // }

        //将插件名拼音排序
        var plugin = Plugins.OrderBy(kvp => kvp.Key, StringComparer.Create(new CultureInfo("zh-CN"), false));
        Plugins = plugin.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        // ConfigWindow.Dispose();
        MainWindow.Dispose();

        Svc.Commands.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        ToggleMainUI();
    }
}