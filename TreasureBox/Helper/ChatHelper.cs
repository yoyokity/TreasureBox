using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using ECommons.DalamudServices;

namespace TreasureBox.Helper;

public static class ChatHelper
{
    public static uint LastLinkedItemId() => Svc.Chat.LastLinkedItemId;
    public static byte LastLinkedItemFlags() => Svc.Chat.LastLinkedItemFlags;

    /// 发送宏指令
    public static void SendMessage(string message) => ECommons.Automation.Chat.Instance.SendMessage(message);

    /// <summary>
    /// 本地打印纯文本，别人看不到，也没有声音
    /// </summary>
    public static class Print
    {
        /// 说话频道 /s
        public static void Say(string message) => PrintChat(new XivChatEntry()
        {
            Message = (SeString)message,
            Type = XivChatType.Say
        });

        /// 喊话频道 /sh
        public static void Shout(string message) => PrintChat(new XivChatEntry()
        {
            Message = (SeString)message,
            Type = XivChatType.Shout
        });

        /// 呼喊频道 /y
        public static void Yell(string message) => PrintChat(new XivChatEntry()
        {
            Message = (SeString)message,
            Type = XivChatType.Yell
        });

        /// 战队频道 /pt
        public static void PvPTeam(string message) => PrintChat(new XivChatEntry()
        {
            Message = (SeString)message,
            Type = XivChatType.PvPTeam
        });

        /// 默语频道 /e
        public static void Echo(string message) => PrintChat(new XivChatEntry()
        {
            Message = (SeString)message,
            Type = XivChatType.Echo
        });

        /// 小队频道 /p
        public static void Party(string message) => PrintChat(new XivChatEntry()
        {
            Message = (SeString)message,
            Type = XivChatType.Party
        });

        /// 团队频道 /a
        public static void Alliance(string message) => PrintChat(new XivChatEntry()
        {
            Message = (SeString)message,
            Type = XivChatType.Alliance
        });

        /// 部队频道 /fc
        public static void FreeCompany(string message) => PrintChat(new XivChatEntry()
        {
            Message = (SeString)message,
            Type = XivChatType.FreeCompany
        });

        /// 自定义表情
        public static void CustomEmote(string message) => PrintChat(new XivChatEntry()
        {
            Message = (SeString)message,
            Type = XivChatType.CustomEmote
        });

        /// 标准表情
        public static void StandardEmote(string message) => PrintChat(new XivChatEntry()
        {
            Message = (SeString)message,
            Type = XivChatType.StandardEmote
        });

        /// 红字警告(淡红色)
        public static void Urgent(string message) => Svc.Chat.PrintError(message);

        /// 红字报错(正红色)
        public static void ErrorMessage(string message) => PrintChat(new XivChatEntry()
        {
            Message = (SeString)message,
            Type = XivChatType.ErrorMessage
        });

        /// NPC说话频道
        public static void NpcDialogue(string message) => PrintChat(new XivChatEntry()
        {
            Message = (SeString)message,
            Type = XivChatType.NPCDialogue
        });

        /// <summary>
        /// 打印自定义彩色文本，如：ColorText("哈哈哈",69,"你好",22,"!")
        /// </summary>
        /// <param name="args">每个参数一段文本，文本参数后面跟int的颜色值参数，不带颜色默认白色</param>
        public static void ColorText(params object[] args)
        {
            var _message = new SeStringBuilder();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is string)
                {
                    var text = args[i].ToString();
                    var color = 567; // 默认白色
                    if (i != args.Length - 1)
                        if (args[i + 1] is int c)
                            color = c;
                    _message.AddUiForeground(text, (ushort)color);
                }
            }

            var message = new XivChatEntry()
            {
                Message = _message.Build()
            };
            PrintChat(message);
        }
    }

    private static void PrintChat(XivChatEntry message)
    {
        Svc.Chat.Print(message);
    }
}