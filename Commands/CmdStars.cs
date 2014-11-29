using System;


namespace MCForge.Commands
{
    public class CmdStars : Command
    {
        public override string name { get { return "stars"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "headop"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdStars() { }

        public override void Use(Player p, string message)
        {
            if (message.Split(' ').Length != 2)
            {
                Help(p);
                return;
            }
            Player who = null;
            who = Player.Find(message.Split(' ')[0]);
            if (who == null)
            {
                Player.SendMessage(p, c.red + "Player " + message + " not found");
                return;
            }
            if (who.infected)
            {
                Player.SendMessage(p, c.red + "Player is infected");
            }
            int pos = message.IndexOf(' ');
            string number = message.Substring(pos + 1);
            int i = 0;
            try
            {
                i = Convert.ToInt32(number);
            }
            catch
            {
                Player.SendMessage(p, c.red + "Value must be a number");
                return;
            }
            who.winstreakcount = i;
            who.SetPrefix();
            Player.SendMessage(p, c.lime + "Sucessfully set stars for " + who.group.color + who.name + c.lime + " to " + c.aqua + i.ToString());
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/stars <player> <number> - Gives a person <number> stars back");
        }
    }
}


