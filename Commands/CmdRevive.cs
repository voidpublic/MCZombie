using System;

namespace MCForge.Commands
{
    public class CmdRevive : Command
    {
        public override string name { get { return "revive"; } }
        public override string shortcut { get { return "y"; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdRevive() { }
        public override void Use(Player p, string message)
        {
            Command.all.Find("buy").Use(p, "revive");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "Use /shop revive for help");
        }
    }
}