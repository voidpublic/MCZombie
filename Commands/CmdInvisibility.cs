
using System;

namespace MCForge.Commands
{
    public class CmdInvisibility : Command
    {
        public override string name { get { return "invisibility"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return ""; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdInvisibility() { }
        public override void Use(Player p, string message)
        {
            Command.all.Find("buy").Use(p, "invisibility");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "Use /shop invisibility for help");
            Player.SendMessage(p, "No longer in use");
        }
    }
}
