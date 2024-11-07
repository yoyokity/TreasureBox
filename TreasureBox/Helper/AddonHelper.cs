using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Memory;
using ECommons;
using ECommons.Automation;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.ValueType;

namespace TreasureBox.Helper;

public static class AddonHelper
{
    /// <summary>
    /// 检测窗口是否存在
    /// </summary>
    /// <param name="addonName"></param>
    /// <returns></returns>
    public static unsafe bool CheckAddon(string addonName)
    {
        var addon = GetUnitBase(addonName);
        return addon != null && addon->IsVisible;
    }

    /// <summary>
    /// 一直等到指定窗口存在或消失，才继续往下执行，超时后返回false
    /// （使用该函数前加上 await 关键字）
    /// </summary>
    /// <param name="addonName">窗口名称</param>
    /// <param name="visible">是否可见</param>
    /// <param name="timeout">超时限制 ms</param>
    /// <param name="delay">固定延时，防止自动交互太快</param>
    public static async Task<bool> WaitAddonUntil(string addonName, bool visible = true, int timeout = 5000, int delay = 300)
    {
        await Task.Delay(delay / 2);

        var startTime = DateTime.Now;
        var timeoutSpan = TimeSpan.FromMilliseconds(timeout);

        while (CheckAddon(addonName) != visible)
        {
            if (DateTime.Now - startTime >= timeoutSpan)
                return false;
            await Task.Delay(100); //100毫秒判断一次
        }

        await Task.Delay(delay / 2);
        return true;
    }

    /// <summary>
    /// 获取窗口上的信息
    /// </summary>
    /// <param name="addonName"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static unsafe AddonValue GetAddonValue(string addonName, uint index)
    {
        var addon = GetUnitBase(addonName);
        var re = addon->AtkValues[index];
        var value = new AddonValue
        {
            Byte = re.Byte
        };
        if (re.Type != ValueType.String && re.Type != ValueType.WideString && re.Type != ValueType.String8)
        {
            value.Int = re.Int;
            value.UInt = re.UInt;
            value.Float = re.Float;
        }
        else
        {
            value.String = MemoryHelper.ReadSeStringNullTerminated((IntPtr)re.String).TextValue;
        }

        return value;
    }

    /// <summary>
    /// 设置控件文本
    /// </summary>
    /// <param name="addonName"></param>
    /// <param name="nodeIndex"></param>
    /// <param name="values"></param>
    public static unsafe void SetAddonValue(string addonName, uint nodeIndex, Int32 values)
    {
        var addon = GetUnitBase(addonName);
        if (addon == null)
            return;
        var priceComponentNumericInput =
            (AtkComponentNumericInput*)addon->UldManager.NodeList[nodeIndex]->GetComponent();
        priceComponentNumericInput->SetValue(values);
    }

    /// <summary>
    /// 点击窗口上的按钮
    /// </summary>
    /// <param name="addonName"></param>
    /// <param name="values"></param>
    public static unsafe void SetAddonClicked(string addonName, params object[] values)
    {
        var addon = GetUnitBase(addonName);
        Callback.Fire(addon, true, values);
    }

    /// <summary>
    /// 与游戏内单位交互
    /// </summary>
    public static unsafe bool InteractWithUnit(uint objectId, bool checklineOnSight = true)
    {
        foreach (var obj in Svc.Objects)
        {
            if (obj.EntityId != objectId)
                continue;
            try
            {
                TargetSystem.Instance()->InteractWithObject(
                    (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)obj.Address, checklineOnSight);
                return true;
            }
            catch (Exception)
            {
                LogHelper.Error($"找不到交互对象 {objectId} ");
                return false;
            }
        }

        LogHelper.Error($"找不到交互对象 {objectId} ");
        return false;
    }

    /// <summary>
    /// 与游戏内单位交互
    /// </summary>
    /// <param name="objectName">单位名字，如有重复找最近单位</param>
    /// <returns></returns>
    public static unsafe bool InteractWithUnit(string objectName, bool checklineOnSight = true)
    {
        Svc.Objects.Where(x => objectName == x.Name.TextValue)
            .OrderBy(x => Vector3.Distance(Svc.ClientState.LocalPlayer.Position, x.Position)).TryGetFirst(out var obj);

        try
        {
            TargetSystem.Instance()->InteractWithObject(
                (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)obj.Address, checklineOnSight);
            return true;
        }
        catch (Exception)
        {
            LogHelper.Error($"找不到交互对象 {objectName} ");
        }

        return false;
    }

    /// <summary>
    /// 与游戏内单位交互
    /// </summary>
    /// <param name="objectName">符合数组包含的单位名字，如有重复找最近单位</param>
    /// <returns></returns>
    public static unsafe bool InteractWithUnit(string[] objectName, bool checklineOnSight = true)
    {
        Svc.Objects.Where(x => objectName.Contains(x.Name.TextValue))
            .OrderBy(x => Vector3.Distance(Svc.ClientState.LocalPlayer.Position, x.Position)).TryGetFirst(out var obj);

        try
        {
            TargetSystem.Instance()->InteractWithObject(
                (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)obj.Address, checklineOnSight);
            return true;
        }
        catch (Exception)
        {
            LogHelper.Error($"找不到交互对象 {objectName} ");
        }

        return false;
    }

    private static unsafe AtkUnitBase* GetUnitBase(string name, int index = 1)
    {
        return (AtkUnitBase*)Svc.GameGui.GetAddonByName(name, index).ToPointer();
    }
}

public class AddonValue
{
    public byte Byte;
    public int Int;
    public uint UInt;
    public float Float;
    public string String = "";
}