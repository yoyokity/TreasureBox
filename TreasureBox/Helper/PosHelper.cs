using ECommons.DalamudServices;
using ECommons.EzIpcManager;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Utility.Numerics;
using ECommons.GameHelpers;

namespace TreasureBox.Helper;

public static class PosHelper
{
    public unsafe static void TPpos(Vector3 pos)
    {
        Player.Character->SetPosition(pos.X, pos.Y, pos.Z);
    }

    /// <summary>
    /// 玩家当前坐标
    /// </summary>
    public static Vector3? GetPos => Svc.ClientState.LocalPlayer?.Position;


    public static float Distance2D(Vector3 pos1, Vector3 pos2)
    {
        var v2 = pos2.WithY(pos1.Y);
        return Vector3.Distance(pos1, v2);
    }

    public static bool NavIsEnabled => PluginHelper.IsPluginEnabled("vnavmesh");

    /// <summary>
    /// 自动导航到坐标，放在task中
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="fly"></param>
    /// <param name="nearStop">2D距离靠近到x米时停止</param>
    public static void MoveTo(Vector3 pos, bool fly = false, float nearStop = 0)
    {
        P.TaskManager.EnqueueImmediate(() => VNavmeshIPC.Nav_IsReady());
        var pos2 = VNavmeshIPC.Query_Mesh_NearestPoint(pos, 4, 4); //寻找最近的点
        VNavmeshIPC.Nav_PathfindCancelAll();
        VNavmeshIPC.Path_Stop();
        VNavmeshIPC.SimpleMove_PathfindAndMoveTo(pos2, fly);
        P.TaskManager.DelayNextImmediate(50);
        if (nearStop <= 0)
        {
            P.TaskManager.EnqueueImmediate(() => !VNavmeshIPC.Path_IsRunning());
        }
        else
        {
            P.TaskManager.EnqueueImmediate(() =>
            {
                if (!(Distance2D(GetPos.Value, pos) < nearStop)) return false;
                VNavmeshIPC.Path_Stop();
                return true;
            });
        }
    }

    /// <summary>
    /// 等待切换地图完成，放在task中
    /// </summary>
    public static void WaitBetweenAreas()
    {
        P.TaskManager.DelayNextImmediate(2000);
        P.TaskManager.EnqueueImmediate(() =>
            !Svc.Condition[Dalamud.Game.ClientState.Conditions.ConditionFlag.BetweenAreas]);
        P.TaskManager.DelayNextImmediate(1000);
    }
}

internal class IPCSubscriber_Common
{
    internal static bool IsReady(string pluginName)
        => DalamudReflector.TryGetDalamudPlugin(pluginName, out _, false, true);

    internal static void DisposeAll(EzIPCDisposalToken[] _disposalTokens)
    {
        foreach (var token in _disposalTokens)
        {
            try
            {
                token.Dispose();
            }
            catch (Exception ex)
            {
                Svc.Log.Error($"Error while unregistering IPC: {ex}");
            }
        }
    }
}

internal static class VNavmeshIPC
{
    private static EzIPCDisposalToken[] _disposalTokens = EzIPC.Init(typeof(VNavmeshIPC), "vnavmesh");

    internal static bool IsEnabled
        => IPCSubscriber_Common.IsReady("vnavmesh");

    [EzIPC("vnavmesh.Nav.IsReady", applyPrefix: false)]
    internal static readonly Func<bool> Nav_IsReady;

    [EzIPC("vnavmesh.Nav.BuildProgress", applyPrefix: false)]
    internal static readonly Func<float> Nav_BuildProgress;

    [EzIPC("vnavmesh.Nav.Reload", applyPrefix: false)]
    internal static readonly Func<bool> Nav_Reload;

    [EzIPC("vnavmesh.Nav.Rebuild", applyPrefix: false)]
    internal static readonly Func<bool> Nav_Rebuild;

    [EzIPC("vnavmesh.Nav.Pathfind", applyPrefix: false)]
    internal static readonly Func<Vector3, Vector3, bool, Task<List<Vector3>>> Nav_Pathfind;

    [EzIPC("vnavmesh.Nav.PathfindCancelable", applyPrefix: false)]
    internal static readonly Func<Vector3, Vector3, bool, CancellationToken, Task<List<Vector3>>>
        Nav_PathfindCancelable;

    [EzIPC("vnavmesh.Nav.PathfindCancelAll", applyPrefix: false)]
    internal static readonly Action Nav_PathfindCancelAll;

    [EzIPC("vnavmesh.Nav.PathfindInProgress", applyPrefix: false)]
    internal static readonly Func<bool> Nav_PathfindInProgress;

    [EzIPC("vnavmesh.Nav.PathfindNumQueued", applyPrefix: false)]
    internal static readonly Func<int> Nav_PathfindNumQueued;

    [EzIPC("vnavmesh.Nav.IsAutoLoad", applyPrefix: false)]
    internal static readonly Func<bool> Nav_IsAutoLoad;

    [EzIPC("vnavmesh.Nav.SetAutoLoad", applyPrefix: false)]
    internal static readonly Action<bool> Nav_SetAutoLoad;

    [EzIPC("vnavmesh.Query.Mesh.NearestPoint", applyPrefix: false)]
    internal static readonly Func<Vector3, float, float, Vector3> Query_Mesh_NearestPoint;

    [EzIPC("vnavmesh.Query.Mesh.PointOnFloor", applyPrefix: false)]
    internal static readonly Func<Vector3, bool, float, Vector3> Query_Mesh_PointOnFloor;

    [EzIPC("vnavmesh.Path.MoveTo", applyPrefix: false)]
    internal static readonly Action<List<Vector3>, bool> Path_MoveTo;

    [EzIPC("vnavmesh.Path.Stop", applyPrefix: false)]
    internal static readonly Action Path_Stop;

    [EzIPC("vnavmesh.Path.IsRunning", applyPrefix: false)]
    internal static readonly Func<bool> Path_IsRunning;

    [EzIPC("vnavmesh.Path.NumWaypoints", applyPrefix: false)]
    internal static readonly Func<int> Path_NumWaypoints;

    [EzIPC("vnavmesh.Path.GetMovementAllowed", applyPrefix: false)]
    internal static readonly Func<bool> Path_GetMovementAllowed;

    [EzIPC("vnavmesh.Path.SetMovementAllowed", applyPrefix: false)]
    internal static readonly Action<bool> Path_SetMovementAllowed;

    [EzIPC("vnavmesh.Path.GetAlignCamera", applyPrefix: false)]
    internal static readonly Func<bool> Path_GetAlignCamera;

    [EzIPC("vnavmesh.Path.SetAlignCamera", applyPrefix: false)]
    internal static readonly Action<bool> Path_SetAlignCamera;

    [EzIPC("vnavmesh.Path.GetTolerance", applyPrefix: false)]
    internal static readonly Func<float> Path_GetTolerance;

    [EzIPC("vnavmesh.Path.SetTolerance", applyPrefix: false)]
    internal static readonly Action<float> Path_SetTolerance;

    [EzIPC("vnavmesh.SimpleMove.PathfindAndMoveTo", applyPrefix: false)]
    internal static readonly Func<Vector3, bool, bool> SimpleMove_PathfindAndMoveTo;

    [EzIPC("vnavmesh.SimpleMove.PathfindInProgress", applyPrefix: false)]
    internal static readonly Func<bool> SimpleMove_PathfindInProgress;

    [EzIPC("vnavmesh.Window.IsOpen", applyPrefix: false)]
    internal static readonly Func<bool> Window_IsOpen;

    [EzIPC("vnavmesh.Window.SetOpen", applyPrefix: false)]
    internal static readonly Action<bool> Window_SetOpen;

    [EzIPC("vnavmesh.DTR.IsShown", applyPrefix: false)]
    internal static readonly Func<bool> DTR_IsShown;

    [EzIPC("vnavmesh.DTR.SetShown", applyPrefix: false)]
    internal static readonly Action<bool> DTR_SetShown;

    internal static void Dispose()
        => IPCSubscriber_Common.DisposeAll(_disposalTokens);
}