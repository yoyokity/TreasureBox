using System;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Utility.Numerics;
using ECommons;
using ECommons.DalamudServices;
using ECommons.DalamudServices.Legacy;

namespace TreasureBox.Helper;
#nullable enable

public static class ObjectHelper
{
    /// <summary>
    /// 通过名字获取单位，重名则取距离最近
    /// </summary>
    public static IGameObject? FindObject(string name)
    {
        try
        {
            Svc.Objects.Where(x => name == x.Name.TextValue)
                .OrderBy(x => Vector3.Distance(Svc.ClientState.LocalPlayer.Position, x.Position))
                .TryGetFirst(out var obj);
            return obj;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public static IGameObject? Player => Svc.ClientState.LocalPlayer;
    public static IGameObject? Target => Player?.TargetObject;
    public static void SelectTarget(IGameObject obj) => Svc.Targets.SetTarget(obj);
}