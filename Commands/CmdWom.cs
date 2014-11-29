
using System;
using System.Threading;

namespace MCForge.Commands
{
    public class CmdWom : Command
    {
        public override string name { get { return "wom"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "operator"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdWom() { }

        public override void Use(Player p, string message)
        {
            Player who = Player.Find(message);
            if (who == null) Player.SendMessage(p,"Player not found");
            else if (!who.womwarning)
            {
                who.womwarning = true;
                if(!who.frozen) Command.all.Find("freeze").Use(p, who.name);
                who.SendMessage("%c--------------WARNING---------------");
                who.SendMessage("%cYou are on wom which is forbitten");
                who.SendMessage("%cPlease type: /client hacks off");
                who.SendMessage("%cand rejoin the server or get banned");
                who.SendMessage("%c--------------WARNING--------------");
                Player.GlobalMessageOps(p.color + p.name + Server.DefaultColor + " wom warned " +who.color +  who.name);
                Player.SendMessage(p,"Wom warning sucessfully sent");
                who.OffenseNote(DateTime.Now, "%dwarned%c", "wom", p.name);
                Thread.Sleep(30000);
                if (who.womwarning == true)
                {
                    who.SendMessage("%c--------------WARNING---------------");
                    who.SendMessage("%cYou are on wom which is forbitten");
                    who.SendMessage("%cPlease type: /client hacks off");
                    who.SendMessage("%cand rejoin the server or get banned");
                    who.SendMessage("%c--------------WARNING--------------");
                    Thread.Sleep(30000);
                }
                if (who.womwarning == true)
                {
                    who.Kick("Wom is not allowed, please rejoin with normal client");
                }
            }
            else Player.SendMessage(p,p.name + " is already wom warned");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/wom <player> - tells a player to shut down wom.");
        }
    }
}