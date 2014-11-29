using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Commands
{
    public class CmdMyreferences : Command
    {
        public override string name { get { return "myreferences"; } }
        public override string shortcut { get { return "myrefs"; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdMyreferences() { }

        public override void Use(Player p, string message)
        {
            if (p == null)
            {
                Player.SendMessage(p, "Console has no references derp...");
                return;
            }
            p.ignorePermission = true;
            Command.all.Find("references").Use(p, p.name);
            p.ignorePermission = false;
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/myrefernces - shows your own references");
        }
    }
}