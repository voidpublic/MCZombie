using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MCForge.Commands
{
    public class CmdEditstats : Command
    {
        public override string name { get { return "editstats"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "nobody"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        public CmdEditstats() { }
        public override void Use(Player p, string message)
        {
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p,"/editstats <player> <type> <value> - edits a players stats");
        }
    }
}