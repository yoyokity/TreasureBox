using System;
using System.Runtime.InteropServices;
using ECommons.DalamudServices;

namespace TreasureBox.Helper.Viod;

public static class VoidAddonHelper
{
    public static void Start(uint evtId)
    {
        try
        {
            var targetId = GetTargetId();
            if (targetId == 0)
                return;
            VoidCore.SendStart(targetId, evtId);
        }
        catch (Exception e)
        {
            LogHelper.Error("交互失败");
        }
    }

    public static void Finish(uint param1, uint param2 = 0)
    {
        try
        {
            var targetId = GetTargetId();
            if (targetId == 0)
                return;
            VoidCore.SendFinish(targetId, param1, param2);
        }
        catch (Exception e)
        {
            LogHelper.Error("交互失败");
        }
    }

    private static uint GetTargetId()
    {
        var target = Svc.Targets.Target;
        if (target == null)
        {
            LogHelper.Error("请先选中一个目标！", "虚空交互");
            return 0;
        }
        var address = target.Address + 0x78;
        var npcId = (uint)Marshal.ReadInt32(address);
        return npcId;
    }
}