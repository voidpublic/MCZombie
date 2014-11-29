
using System;


namespace MCForge.Commands
{
    public class CmdLevelCreator : Command
    {
        public override string name { get { return "levelcreator"; } }
        public override string shortcut { get { return "lc"; } }
        public override string type { get { return "control"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdLevelCreator() { }

        public override void Use(Player p, string message)
        { 
            if (message == "") { Help(p); return; }
            int number = message.Split(' ').Length;
            if (number > 2 || number < 1) { Help(p); return; }
            if (number == 1)
            {
                Player.SendMessage(p,"%aLevel creator successfully edited");
                p.level.creator = message;
                Level.SaveSettings(p.level);
            }
            else
            {
                int pos = message.IndexOf(' ');
                string t = message.Substring(0, pos).ToLower();
                string creator = message.Substring(pos + 1);
                Level level = Level.Find(t);
                if (level != null)
                {
                    Player.SendMessage(p,"%aLevel creator successfully edited");
                    level.creator = creator;
                    Level.SaveSettings(level);
                }
                else
                {
                        Player.SendMessage(p, "Can only set the creators name of loaded levels :(");
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/LevelCreator <playername> - sets the creator of an map");
        }
    }
}