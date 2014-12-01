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
using System.Data;
using MCForge.SQL;

namespace MCForge.Commands
{
	public class CmdMoney : Command
	{
		public override string name { get { return "money"; } }
		public override string shortcut { get { return "cookies"; } }
        public override string type { get { return "player"; } }
		public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
		public override void Use(Player p, string message)
		{
            if (message == "")
            {
                Player.SendMessage(p, c.lime + "You currently have " + c.aqua + p.money + " " + c.lime + Server.moneys + ".");
            }
            else
            {
                Player who = Player.Find(message);
                if (who == null)
                {
                    DataTable money = Database.fillData("SELECT Money FROM Players WHERE name='" + message + "'");
                    if (money.Rows.Count == 0)
                        Player.SendMessage(p, c.red + "Player could not be found");
                    else
                        Player.SendMessage(p, message + Server.DefaultColor + " currently has " + money.Rows[0]["Money"] + " " + Server.moneys + ".");
                    money.Dispose();
                    return;
                }
                if (who.group.Permission >= p.group.Permission)
                {
                    Player.SendMessage(p, c.red + "Cannot see the money of someone of equal or greater rank.");
                    return;
                }
                Player.SendMessage(p, who.color + who.name + Server.DefaultColor + " currently has " + who.money + " " + Server.moneys + ".");
            }  
        }

		public override void Help(Player p)
		{
			Player.SendMessage(p, "/money <player> - Shows how much " + Server.moneys + " <player> has");
		}
	}
}