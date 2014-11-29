using System;
using System.IO;


namespace MCForge.Commands
{
    /// <summary>
    /// This is the command /infected
    /// use /help infected in-game for more info
    /// </summary>
    public class CmdInfected : Command
    {
        public override string name { get { return "infected"; } }
        public override string shortcut { get { return "zombies"; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdInfected() { }
        public override void Use(Player p, string message)
        {
            if (p == null)
            {
                Player.SendMessage(null, c.red + "Players who are infected are: " + ZombieGame.infectd.Count);
                string playerstring = "";
                ZombieGame.infectd.ForEach(delegate(Player player)
                {
                    playerstring = playerstring + c.red + player.name + Server.DefaultColor + ", ";
                });
                Player.SendMessage(null, playerstring);
                return;
            }
            Player who = null;
            if (message == "") { who = p; message = p.name; } else { who = Player.Find(message); }
            if (ZombieGame.infectd.Count == 0)
            {
                Player.SendMessage(p, "No one is infected");
            }
            else
            {
                Player.SendMessage(p, c.red +"Players who are infected are: "+ZombieGame.infectd.Count);
                string playerstring = "";
                ZombieGame.infectd.ForEach(delegate(Player player)
                {
                    playerstring = playerstring + c.red + player.name + Server.DefaultColor + ", ";
                });
                Player.SendMessage(p, playerstring);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/infected /zombies - shows who is infected");
        }
    }
}