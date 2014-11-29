/*
	Copyright 2011 MCForge
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.IO;


namespace MCForge.Commands
{
    public class CmdFreeze : Command
    {
        public override string name { get { return "freeze"; } }
        public override string shortcut { get { return "f"; } }
        public override string type { get { return "operator"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdFreeze() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Player who = Player.Find(message);
            if (who == null) { Player.SendMessage(p, "Could not find player."); return; }
            else if (who == p) { Player.SendMessage(p, "Cannot freeze yourself."); return; }
            else if (p != null) { if (who.group.Permission >= p.group.Permission) { Player.SendMessage(p, "Cannot freeze someone of equal or greater rank."); return; } }
            if ((DateTime.Now - who.freezetime).TotalSeconds < 6)
            {
                Player.SendMessage(p, "%cPlayer was recently frozen!");
                return;
            }
            if (!who.frozen)
            {
                who.frozen = true;
                who.freezetime = DateTime.Now;
                Player.GlobalChat(who, who.color + who.name + Server.DefaultColor + " has been &bfrozen" + Server.DefaultColor + " by " + p.color + p.name + Server.DefaultColor + ".", false);
                Player.SendMessage(who, c.red + "--------------------------------------------------------------");
                Player.SendMessage(who, c.red + " You have been frozen please listen to a staff member that wants to talk to you");
                Player.SendMessage(who, c.red + "--------------------------------------------------------------");
            }
            else
            {
                who.frozen = false;
                Player.GlobalChat(who, who.color + who.name + Server.DefaultColor + " has been &adefrosted" + Server.DefaultColor + " by " + p.color + p.name + Server.DefaultColor + ".", false);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/freeze <name> - Stops <name> from moving until unfrozen.");
        }
    }
}