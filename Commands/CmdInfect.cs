using System;
using System.IO;


namespace MCForge.Commands
{
    /// <summary>
    /// This is the command /infect
    /// use /help infect in-game for more info
    /// </summary>
    public class CmdInfect : Command
    {
        public override string name { get { return "infect"; } }
        public override string shortcut { get { return "i"; } }
        public override string type { get { return "headop"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdInfect() { }
        public override void Use(Player p, string message)
        {
            Player who = null;
            who = Player.Find(message);
            if (who == null) { Player.SendMessage(p,"Player not found"); return; }
            if (who.infected)
            {
                Player.SendMessage(p,"Player is already infected");
            }
            else
            {
                if (!who.referee)
                {
                    if (Server.zombie.GameInProgess())
                    {
                        Server.zombie.InfectPlayer(who);
                        Player.GlobalMessage(who.color + who.name + Server.DefaultColor + " just got Infected!");
                    }
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/infect [name] - infects [name]");
        }
    }
}