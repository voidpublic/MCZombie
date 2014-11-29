using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MCForge.Commands
{
    public class CmdReferences : Command
    {
        public override string name { get { return "references"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "operator"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdReferences() { }
        public override void Use(Player p, string message)
        {
            Player References = Player.Find(message);
            string playername = "";
            if (References == null)
            {
                Player.SendMessage(p,"Player not online.. searching in Database");
                playername = message;
            }
            else playername = References.name;
            string path = "text/references/" + playername + ".txt";
            if (!File.Exists(path)) { Player.SendMessage(p, "No references found for " + playername); return; }
            string[] lines = File.ReadAllLines(path);
            Player.SendMessage(p, "Player " + c.aqua + playername + Server.DefaultColor + " has %b" + lines.Count() + Server.DefaultColor + " references:");
            for (int a = 0; a < lines.Count(); a++)
            {
                Player.SendMessage(p,c.lime + lines[a]);
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/references <playername> - shows what references the player got");
        }
    }
}