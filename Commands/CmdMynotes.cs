using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Commands
{
    public class CmdMynotes : Command
    {
        public override string name { get { return "mynotes"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdMynotes() { }

        public override void Use(Player p, string message)
        {
            if (p == null)
            {
                Player.SendMessage(p, c.red + "Console has no notes derp...");
                return;
            }
            p.ignorePermission = true;
            if (message == "all")
                Command.all.Find("notes").Use(p, p.name + " all");
            else
                Command.all.Find("notes").Use(p, p.name);
            p.ignorePermission = false;
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p,"/mynotes - shows your latest notes");
            Player.SendMessage(p,"/mynotes all - shows all of your notes");
        }
    }
}