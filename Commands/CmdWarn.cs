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
using System.Collections.Generic;
using System.Data;
using MCForge.SQL;
using System.Text;
using System.IO;

namespace MCForge.Commands
{
    public class CmdWarn : Command
    {
        public override string name { get { return "warn"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "trusted"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        string reason;

        public override void Use(Player p, string message)
        {
            string warnedby;

            if (message == "") { Help(p); return; }

            Player who = Player.Find(message.Split(' ')[0]);

            // Make sure we have a valid player
            if (who == null)
            {
                Player.SendMessage(p,"Player not online, searching in Database...");
                string offlinename = message.Split(' ')[0].Trim();
                DataTable playerDb = Server.useMySQL ? MySQL.fillData("SELECT * FROM Players WHERE Name='" + offlinename + "'") : SQLite.fillData("SELECT * FROM Players WHERE Name='" + offlinename + "'");
                if (playerDb.Rows.Count == 0) { Player.SendMessage(p,"That player " + offlinename + " does not exist in the database"); playerDb.Dispose(); return; }
                playerDb.Dispose();
                if (message.Split(' ').Length == 1)
                {
                    Player.SendMessage(p, "No reason entered");
                    return;
                }
                else
                    reason = message.Substring(message.IndexOf(' ') + 1).Trim();
                if (reason.Contains("@"))
                {
                    string testing = "0";
                    testing = Server.GetRuleReason(p, reason);
                    if (testing == "0") return;
                    else
                    {
                        reason = testing;
                        warnedby = (p == null) ? "<CONSOLE>" : p.color + p.name;
                        Player.GlobalMessage(warnedby + " %ewarned (offline)" + c.white + offlinename + Server.DefaultColor + " for breaking the rule:");
                        Player.GlobalMessage("%c" + reason);
                    }
                }
                else
                {
                warnedby = (p == null) ? "<CONSOLE>" : p.color + p.name;
                Player.GlobalMessage(warnedby + " %ewarned (offline)" + c.white + offlinename);
                Player.GlobalMessage("%c" + reason);
                }
                if (!Directory.Exists("text/offensenotes")) Directory.CreateDirectory("text/offensenotes");
                string path = "text/offensenotes/" + message.Split(' ')[0] + ".txt";
                if (!File.Exists(path)) File.Create(path).Dispose();
                try
                {
                    StreamWriter sw = File.AppendText(path);
                    sw.WriteLine(DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + ": " + "%dwarned%c" + " : " + reason + " by " + p.name);
                    sw.Close();

                }
                catch { Server.s.Log("Error saving Offensenote"); }
                return;
            }
            // Don't warn yourself... derp
            if (who == p)
            {
                Player.SendMessage(p, "%cYou can't warn yourself");
                return;
            }
            // Check the caller's rank
            if (p != null && p.group.Permission <= who.group.Permission)
            {
                Player.SendMessage(p, "%cYou can't warn a player equal or higher rank.");
                return;
            }
            // We need a reason
            if (message.Split(' ').Length == 1)
            {
                Player.SendMessage(p, "%cNo reason entered");
                return;
            }
            if ((DateTime.Now - who.warnedtime).TotalSeconds < 10)
            {
                Player.SendMessage(p,"%cPlayer was recently warned!");
                return;
            }
            else
            {
                reason = message.Substring(message.IndexOf(' ') + 1).Trim();
            }
            if (reason.Contains("@"))
            {
                string testing = "0";
                testing = Server.GetRuleReason(p, reason);
                if (testing == "0") return;
                else
                {
                    reason = testing;
                    warnedby = (p == null) ? "<CONSOLE>" : p.color + p.name;
                    Player.GlobalMessage(warnedby + " %ewarned " + who.color + who.name + Server.DefaultColor + " for breaking the rule:");
                    Player.GlobalMessage("%c" + reason);
                }
            }
            else
            {
                warnedby = (p == null) ? "<CONSOLE>" : p.color + p.name;
                Player.GlobalMessage(warnedby + " %ewarned " + who.color + who.name);
                Player.GlobalMessage("%c" + reason);
            }
            who.OffenseNote(DateTime.Now, "%dwarned%c", reason, p.name);
            who.warnedtime = DateTime.Now;
            if (who.money >= 5) who.money -= 5;
            else who.money = 0;

            if (who.warn == 0)
            {
                Player.SendMessage(who, "%aDont do it again or you will be kicked");
                who.warn = 1;
                return;
            }
            if (who.warn == 1)
            {
                Player.SendMessage(who, "%aDont do it again or you will be kicked");
                who.warn = 2;
                return;
            }
            if (who.warn == 2)
            {
                Player.GlobalMessage(who.color + who.name + " " + Server.DefaultColor + "was warn-kicked by " + warnedby);
                who.warn = 0;
                if (who.money >= 10) who.money -= 10;
                else who.money = 0;
                who.OffenseNote(DateTime.Now, "kick", "warnkick", p.name);
                who.Kick("Kicked for: Reached too high warning level");
                return;
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/warn <player> <reason> - Warns a player.");
            Player.SendMessage(p, "You can use for reason @rulenumber to point to a special rule");
            Player.SendMessage(p, "Player will get kicked after 3 warnings.");
        }
    }
}