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
using System.IO;
using System.Data;
using MCForge.SQL;


namespace MCForge.Commands
{
    public class CmdMapInfo : Command
    {
        public override string name { get { return "mapinfo"; } }
        public override string shortcut { get { return "mi"; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdMapInfo() { }

        public override void Use(Player p, string message)
        {
            Level dumplevel;
            if (message == "" && p != null)
                dumplevel = p.level;
            else if (message == "" && p == null)
                dumplevel = Level.Find(Server.zombie.currentLevelName);
            else
            {
                Database.AddParams("@Name", message);
                DataTable playerDb = Database.fillData("SELECT * FROM levelinfo WHERE Name=@Name");
                if (playerDb.Rows.Count == 0) { Player.SendMessage(p, c.red + "Level " + message + " not found"); return; }
                else
                {
                    Player.SendMessage(p, c.white + "-------------------------------------------");
                    Player.SendMessage(p, c.white + "Levelname:          " + c.pink + playerDb.Rows[0]["name"].ToString());
                    Player.SendMessage(p, c.white + "It was created by " + c.pink + playerDb.Rows[0]["creator"].ToString());
                    int temp1 = 0;
                    int temp2 = 0;
                    try { temp1 = Convert.ToInt32(playerDb.Rows[0]["roundtime"]); }
                    catch { temp1 = 8; }
                    Player.SendMessage(p, c.white + "Roundtime: " + c.pink + temp1 + c.white + " minutes");
                    try { temp1 = Convert.ToInt32(playerDb.Rows[0]["humanswon"]); }
                    catch { temp1 = 0; }
                    try { temp2 = Convert.ToInt32(playerDb.Rows[0]["zombieswon"]); }
                    catch { temp2 = 0; }
                    Player.SendMessage(p, c.white + "There have been   " + c.pink + (temp1 + temp2) + c.white + " rounds played on this level");
                    Player.SendMessage(p, c.white + "Humans have won " + c.lime + temp1 + c.white + " and zombies have won " + c.red + temp2 + c.white + " times");
                    Player.SendMessage(p, c.white + "This means that the win chance is " + c.lime + ((temp1 + temp2) == 0 ? "100" : ((temp1 * 100) / (temp1 + temp2)).ToString()) + "%");
                    try { temp1 = Convert.ToInt32(playerDb.Rows[0]["likes"]); }
                    catch { temp1 = 0; }
                    try { temp2 = Convert.ToInt32(playerDb.Rows[0]["dislikes"]); }
                    catch { temp2 = 0; }
                    Player.SendMessage(p, c.lime + temp1 + c.white + " like and " + c.red + temp2 + c.white + " dislike this level");
                    Player.SendMessage(p, c.white + "This means that " + c.lime + ((temp1 + temp2) == 0 ? "100" : ((temp1 * 100) / (temp1 + temp2)).ToString()) + "%" + c.white + " like this level");
                    Player.SendMessage(p, c.white + "-------------------------------------------");
                    playerDb.Dispose();
                    return;
                }
            }

            Player.SendMessage(p, c.white + "-------------------------------------------");
            Player.SendMessage(p, c.white + "Levelname:          " + c.pink + dumplevel.name);
            Player.SendMessage(p, c.white + "Sizes:                " + c.pink + dumplevel.width.ToString() + "x" + dumplevel.depth.ToString() + "x" + dumplevel.height.ToString());
            Player.SendMessage(p, c.white + "It was created by " + c.pink + dumplevel.creator);
            Player.SendMessage(p, c.white + "Roundtime: " + c.pink + dumplevel.roundtime + c.white + " minutes");
            Player.SendMessage(p, c.white + "There have been   " + c.pink + (dumplevel.humanswon + dumplevel.zombieswon) + c.white + " rounds played on this level");
            Player.SendMessage(p, c.white + "Humans have won " + c.lime + dumplevel.humanswon + c.white + " and zombies have won " + c.red + dumplevel.zombieswon + c.white + " times");
            Player.SendMessage(p, c.white + "This means that the win chance is " + c.lime + ((dumplevel.humanswon + dumplevel.zombieswon) == 0 ? "100" : ((dumplevel.humanswon * 100) / (dumplevel.humanswon + dumplevel.zombieswon)).ToString()) + "%"); 
            Player.SendMessage(p, c.lime + dumplevel.likes + c.white + " like and " + c.red + dumplevel.dislikes + c.white + " dislike this level");
            Player.SendMessage(p, c.white + "This means that " + c.lime + ((dumplevel.likes + dumplevel.dislikes) == 0 ? "100" : ((dumplevel.likes * 100) / (dumplevel.likes + dumplevel.dislikes)).ToString()) + "%" + c.white + " like this level");
            Player.SendMessage(p, c.white + "-------------------------------------------");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/mapinfo <map> - Display details of <map>");
        }
    }
}