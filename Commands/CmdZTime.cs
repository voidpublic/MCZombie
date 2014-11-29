/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
	
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


namespace MCForge.Commands
{
    public class CmdZTime : Command
    {
        public override string name { get { return "ztime"; } }
        public override string shortcut { get { return "t"; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdZTime() { }

        public override void Use(Player p, string message)
        {
            string time = "";
            if (p == null)
            {
                time = Server.zombie.GetTimeLeft("");
                Player.SendMessage(p, time + " left");
                return;
            }
            if (!Server.zombieRound) { Player.SendMessage(p, c.red + "The current zombie round hasn't started yet!"); return; }
            time = Server.zombie.GetTimeLeft("");
            if (p.referee)
            {
                message = c.blue + time + Server.DefaultColor + " remaining for the current round!";
            }
            else message = (p.infected ? c.red : c.lime) + time + Server.DefaultColor + " remaining for the current round!";
            Player.SendMessage(p, message);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/ztime - Shows the current zombie survival round time left.");
        }
    }
}