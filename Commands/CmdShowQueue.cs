
using System;
using System.Collections.Generic;
using System.Text;


namespace MCForge.Commands
{
    class CmdShowQueue : Command
    {
        public override string name { get { return "showqueue"; } }
        public override string shortcut { get { return "sq"; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public static string keywords { get { return ""; } }
        public CmdShowQueue() { }

        public override void Use(Player p, string message)
        {
            if (Server.queLevel == false)
                Player.SendMessage(p, c.red + "There is no level queued");
            else
                Player.SendMessage(p, c.lime + "The level " + c.aqua + Server.nextLevel + c.lime + " is queued");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/showqueue - shows wheater or not and which level is queued");
        }
    }
}