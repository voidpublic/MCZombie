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
    public class CmdTop : Command
    {
        public override string name { get { return "top"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdTop() { }

        public override void Use(Player p, string message)
        {
            if (message == "1")
            {
                DataTable playerDb = Database.fillData("SELECT distinct name, totallogin FROM Players order by totallogin desc limit 5");
                Player.SendMessage(p, "%bMost logginers:");
                for (int i = 0; i < playerDb.Rows.Count; i++)
                    Player.SendMessage(p, (i + 1) + ") " + playerDb.Rows[i]["Name"] + " - [" + playerDb.Rows[i]["TotalLogin"] + "]");
                playerDb.Dispose();
                return;
            }
            if (message == "2")
            {
                DataTable playerDb = Database.fillData("SELECT distinct name, playersinfected FROM Players order by playersinfected desc limit 5");
                Player.SendMessage(p, "%cMost players infected has:");
                for (int i = 0; i < playerDb.Rows.Count; i++)
                    Player.SendMessage(p, (i + 1) + ") " + playerDb.Rows[i]["Name"] + " - [" + playerDb.Rows[i]["playersinfected"] + "]");
                playerDb.Dispose();
                return;
            }
            if (message == "3")
            {
                DataTable playerDb = Database.fillData("SELECT distinct name, roundssurvived FROM Players order by roundssurvived desc limit 5");
                Player.SendMessage(p, "%aMost rounds survived has:");
                for (int i = 0; i < playerDb.Rows.Count; i++)
                    Player.SendMessage(p, (i + 1) + ") " + playerDb.Rows[i]["Name"] + " - [" + playerDb.Rows[i]["roundssurvived"] + "]");
                playerDb.Dispose();
                return;
            }
            if (message == "4")
            {
                DataTable playerDb = Database.fillData("SELECT distinct name, money FROM Players order by money desc limit 5");
                Player.SendMessage(p, "%bMoney leaders:");
                for (int i = 0; i < playerDb.Rows.Count; i++)
                    Player.SendMessage(p, (i + 1) + ") " + playerDb.Rows[i]["Name"] + " - [" + playerDb.Rows[i]["money"] + "]");
                playerDb.Dispose();
                return;
            }
            if (message == "5")
            {
                DataTable playerDb = Database.fillData("SELECT distinct name, maximumsurvived FROM Players order by maximumsurvived desc limit 5");
                Player.SendMessage(p, "%cMost rounds survived:");
                for (int i = 0; i < playerDb.Rows.Count; i++)
                    Player.SendMessage(p, (i + 1) + ") " + playerDb.Rows[i]["Name"] + " - [" + playerDb.Rows[i]["maximumsurvived"] + "]");
                playerDb.Dispose();
                return;
            }
            if (message == "6")
            {
                DataTable playerDb = Database.fillData("SELECT distinct name, maximuminfected FROM Players order by maximuminfected desc limit 5");
                Player.SendMessage(p, "%cMost humans infected:");
                for (int i = 0; i < playerDb.Rows.Count; i++)
                    Player.SendMessage(p, (i + 1) + ") " + playerDb.Rows[i]["Name"] + " - [" + playerDb.Rows[i]["maximuminfected"] + "]");
                playerDb.Dispose();
                return;
            }
            else { Help(p); return; }

        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "%2/top [#] - Shows the top players");
            Player.SendMessage(p, "1) Most Logins");
            Player.SendMessage(p, "2) Number of players infected");
            Player.SendMessage(p, "3) Number of rounds survived");
            Player.SendMessage(p, "4) Money");
            Player.SendMessage(p, "5) Maximum rounds survived");
            Player.SendMessage(p, "6) Maximum players infected");
        }
    }
}