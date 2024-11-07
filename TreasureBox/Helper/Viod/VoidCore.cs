using System;
using System.Runtime.InteropServices;
using Dalamud.Game.Network;
using ECommons.DalamudServices;

namespace TreasureBox.Helper.Viod;

public class VoidCore
{
    public static bool _networkLog = false;
    public static bool _networkLogOther = false;

    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    public struct StructEventStart
    {
        public ulong targetId;
        public uint eventId;
        public byte param1;
        public byte param2;
        public byte param3;

        public override readonly string ToString()
        {
            return $"targetId:{targetId:X}|eventId:{eventId}|{param1}|{param2}|{param3}";
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    public struct StructEventFinish
    {
        public uint eventId;
        public uint param1; // event
        public uint param2; // result
        public uint param3; // eventArg

        public override readonly string ToString()
        {
            return $"eventId:{eventId}|{param1}|{param2}|{param3}";
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    public struct StructEventAction
    {
        public uint eventId;
        public uint param1;
        public uint param2;
        public uint param3;

        public override readonly string ToString()
        {
            return $"eventId:{eventId}|{param1}|{param2}|{param3}";
        }
    }

    private static IntPtr eventStart;
    private static IntPtr eventAction;
    private static IntPtr eventFinish;
    private static IntPtr sendPacketAddress;
    public static IntPtr framework = IntPtr.Zero;
    public static int eventStartOpcode = -1;
    public static int eventActionOpcode = -1;
    public static int eventFinishOpcode = -1;

    public delegate void SendPacketDelegate(IntPtr networkModule, IntPtr packetPtr, ulong unk1, ulong unk2,
        ushort unk3);

    public static SendPacketDelegate SendPacketMethod;

    private static void OnNetworkEvent(IntPtr dataPtr, ushort opcode, uint sourceActorId, uint targetActorId,
        NetworkMessageDirection direction)
    {
        if (direction != NetworkMessageDirection.ZoneUp)
            return;
        if (_networkLog)
        {
            if (opcode == eventStartOpcode || opcode == eventActionOpcode || opcode == eventFinishOpcode)
            {
                if (opcode == eventStartOpcode)
                {
                    LogHelper.Normal($"client evtStart  -> {Marshal.PtrToStructure<StructEventStart>(dataPtr)}");
                }
                else if (opcode == eventActionOpcode)
                {
                    LogHelper.Normal($"client evtAction -> {Marshal.PtrToStructure<StructEventAction>(dataPtr)}");
                }
                else
                {
                    LogHelper.Normal($"client evtFinish -> {Marshal.PtrToStructure<StructEventFinish>(dataPtr)}");
                }
            }
        }

        if (_networkLogOther)
        {
            if (opcode != eventStartOpcode && opcode != eventActionOpcode && opcode != eventFinishOpcode)
            {
                LogHelper.Normal($"client data opcode:{opcode} -> {Marshal.PtrToStructure<StructEventStart>(dataPtr)}");
            }
        }
    }

    public static void SendStart(uint npcId, uint evtId)
    {
        StructEventStart evtStart = new();
        var npcIdBytes = new byte[8];
        Array.Copy(BitConverter.GetBytes(npcId), 0, npcIdBytes, 0, 4);
        Array.Copy(BitConverter.GetBytes(1), 0, npcIdBytes, 4, 4);
        evtStart.targetId = BitConverter.ToUInt64(npcIdBytes);
        evtStart.eventId = evtId;
        LogHelper.Normal($"npcId -> {evtStart.targetId:X}");
        LogHelper.Normal($"evtId -> {evtStart.eventId}");
        SendPacket(eventStartOpcode, -1, StructToByteArray(evtStart));
    }

    public static void SendAction(uint evtId, uint param1, uint param2, uint param3)
    {
        StructEventAction evtAction = new();
        evtAction.eventId = evtId;
        evtAction.param1 = param1;
        evtAction.param2 = param2;
        evtAction.param3 = param3;
        LogHelper.Normal($"evtId -> {evtAction.eventId}");
        SendPacket(eventActionOpcode, -1, StructToByteArray(evtAction));
    }

    public static void SendFinish(uint evtId, uint param1, uint param2 = 0)
    {
        StructEventFinish evtFinish = new();
        evtFinish.eventId = evtId;
        evtFinish.param1 = param1;
        evtFinish.param2 = param2;
        LogHelper.Normal($"evtId -> {evtFinish.eventId}");
        SendPacket(eventFinishOpcode, -1, StructToByteArray(evtFinish));
    }

    public static unsafe byte[] StructToByteArray<T>(T _struct) where T : unmanaged
    {
        var pointer = (byte*)&_struct;

        var bytes = new byte[sizeof(T)];
        for (var i = 0; i < sizeof(T); i++)
        {
            bytes[i] = pointer[i];
        }

        return bytes;
    }

    public static unsafe void SendPacket(int opcode, int packetLen, byte[] data)
    {
        if (packetLen == -1)
        {
            packetLen = data.Length;
        }

        var packet = new byte[0x20 + packetLen];
        Array.Copy(BitConverter.GetBytes(opcode), 0, packet, 0, 4);
        Array.Copy(BitConverter.GetBytes(packetLen + 0x10), 0, packet, 8, 4);
        Array.Copy(data, 0, packet, 0x20, data.Length);
        SendPacket(packet);
    }

    public static unsafe void SendPacket(byte[] packet)
    {
        if (framework == IntPtr.Zero)
        {
            framework = (IntPtr)FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance();
        }

        if (((FFXIVClientStructs.FFXIV.Client.System.Framework.Framework*)framework)->IsNetworkModuleInitialized)
        {
            var networkModule = framework + 0x1670;
            var unmanagedPointer = Marshal.AllocHGlobal(packet.Length);
            Marshal.Copy(packet, 0, unmanagedPointer, packet.Length);
            SendPacketMethod(Marshal.ReadIntPtr(Marshal.ReadIntPtr(Marshal.ReadIntPtr(networkModule) + 0x8) + 0x930),
                unmanagedPointer, 0, 0, 0);
            Marshal.FreeHGlobal(unmanagedPointer);
        }
        else
        {
            LogHelper.Error("Network module not initialized!");
        }
    }

    public static void Enable()
    {
        eventStart = Svc.SigScanner.ScanText("C7 44 24 ?? ?? ?? ?? ?? 48 C7 44 24 ?? ?? ?? ?? ?? 89 5C 24 ?? 0F 85") +
                     0x4;
        Svc.Log.Debug($"eventstart -> {eventStartOpcode = Marshal.ReadInt32(eventStart):X}");
        eventAction = Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? 40 08 AB ?? ?? ?? ??") + 0x6E;
        Svc.Log.Debug($"eventaction -> {eventActionOpcode = Marshal.ReadInt32(eventAction):X}");
        eventFinish = Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? EB 10 48 8B 0D ?? ?? ?? ??") + 0xCE;
        Svc.Log.Debug($"eventfinish -> {eventFinishOpcode = Marshal.ReadInt32(eventFinish):X}");
        sendPacketAddress = Svc.SigScanner.ScanText("48 83 EC ?? 48 8B 89 98 00 00 00");
        Svc.Log.Debug($"SendPacket -> {sendPacketAddress:X}");
        SendPacketMethod = Marshal.GetDelegateForFunctionPointer<SendPacketDelegate>(sendPacketAddress);

        Svc.GameNetwork.NetworkMessage += OnNetworkEvent;
    }

    public static void Disable()
    {
        Svc.GameNetwork.NetworkMessage -= OnNetworkEvent;
    }
}