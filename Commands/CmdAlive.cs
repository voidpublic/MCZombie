/*
	Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCForge)
 
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
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
    public class CmdAlive : Command
    {
        public override string name { get { return "alive"; } }
        public override string shortcut { get { return "humans"; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdAlive() { }
        public override void Use(Player p, string message)
        {
            if (p == null)
            {
                Player.SendMessage(null, "%aPeople that are alive: " + ZombieGame.alive.Count);
                string playerstring = "";
                ZombieGame.alive.ForEach(delegate(Player player)
                {
                    playerstring = playerstring + player.group.color + player.name + Server.DefaultColor + ", ";
                });
                Player.SendMessage(null, playerstring);
                return;
            }
            Player who = null;
            if (message == "") { who = p; message = p.name; } else { who = Player.Find(message); }
            if (ZombieGame.alive.Count == 0)
            {
                Player.SendMessage(p, "All are alive");
            }
            else
            {
                Player.SendMessage(p, "%aPeople that are alive: "+ZombieGame.alive.Count);
                string playerstring = "";
                ZombieGame.alive.ForEach(delegate(Player player)
                {
                    playerstring = playerstring + player.group.color + player.name + Server.DefaultColor + ", ";
                });
                Player.SendMessage(p, playerstring);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/alive /humans - shows who is alive");
        }
    }
}