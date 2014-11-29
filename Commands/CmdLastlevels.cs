using System;
using System.Collections.Generic;
using System.Text;

namespace MCForge.Commands
{
    class CmdLastLevels : Command
    {
        public override string name { get { return "lastlevels"; } }
        public override string shortcut { get { return "ll"; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public static string keywords { get { return ""; } }
        public CmdLastLevels() { }

        public override void Use(Player p, string message)
        {
            Player.SendMessage(p, c.aqua + "Last levels that have been played (latest are last):");
            string end = "";
            foreach (string level in ZombieGame.levelsplayed)
            {
                end = end + level + ",";
            }
            Player.SendMessage(p, end);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/lastlevels - show the last levels that have been played");
        }
    }
}