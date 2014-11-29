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
using System.Collections.Generic;
using System.Text;
using System.IO;
using MCForge.SQL;
using System.Data;


namespace MCForge.Commands
{
    public class CmdTempBan : Command
    {
        public override string name { get { return "tempban"; } }
        public override string shortcut { get { return "tb"; } }
        public override string type { get { return "operator"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdTempBan() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            if (message.IndexOf(' ') == -1) message = message + " 60";

            Player who = Player.Find(message.Split(' ')[0]);
            string name = "";
            if (who != null) name = who.name;
            if (who == null)
            {
                name = message.Split(' ')[0];
                Player.SendMessage(p, "Player not online, searching in Database...");
                Database.AddParams("@Name", name);
                DataTable playerDb = Database.fillData("SELECT * FROM Players WHERE Name=@Name");
                if (playerDb.Rows.Count == 0) { Player.SendMessage(p, "That player " + name + " does not exist in the database"); playerDb.Dispose(); return; }
                playerDb.Dispose();
            }
            if(who != null) 
                if (p != null && who.group.Permission >= p.group.Permission) { Player.SendMessage(p, "Cannot ban someone of the same rank"); return; }
            int minutes;
            try
            {
                minutes = int.Parse(message.Split(' ')[1]);
            } catch { Player.SendMessage(p, "Invalid minutes"); return; }
            if (minutes > 1440) { Player.SendMessage(p, "Cannot ban for more than a day"); return; }
            if (minutes < 1) { Player.SendMessage(p, "Cannot ban someone for less than a minute"); return; }
            
            string msg = "";
            if (message.Split(' ').Length < 3)
            {
                Help(p);
                return;
            }
            if (who != null)
            {
                if (who.money >= 30) who.money -= 30;
                else who.money = 0;
            }
            int pos = message.IndexOf(' ');
            msg = message.Substring(pos);
            if (msg.Contains("@"))
            {
                string testing = "0";
                testing = Server.GetRuleReason(p, msg);
                if (testing == "0") return;
                else
                {
                    msg = testing;
                    Server.TempBan tBan;
                    tBan.name = name;
                    tBan.allowedJoin = DateTime.Now.AddMinutes(minutes);
                    if (who != null) tBan.ip = who.ip;
                    else
                    {
                        Database.AddParams("@Name", name);
                        DataTable ipdatatable = Database.fillData("SELECT * FROM Players WHERE Name=@Name");
                        if (ipdatatable.Rows.Count == 0)
                            tBan.ip = "0.0.0.0";
                        else
                            tBan.ip = ipdatatable.Rows[0]["IP"].ToString();
                        ipdatatable.Dispose();
                    }
                    Server.tempBans.Add(tBan);
                    if(who!=null) who.Kick("Banned for " + minutes.ToString() + "minutes for breaking rule: " + msg);
                }
            }
            else
            {
                Server.TempBan tBan;
                tBan.name = name;
                tBan.allowedJoin = DateTime.Now.AddMinutes(minutes);
                if (who != null) tBan.ip = who.ip;
                else
                {
                    Database.AddParams("@Name", name);
                    DataTable ipdatatable = Database.fillData("SELECT * FROM Players WHERE Name=@Name");
                    if (ipdatatable.Rows.Count == 0)
                        tBan.ip = "0.0.0.0";
                    else
                        tBan.ip = ipdatatable.Rows[0]["IP"].ToString();
                    ipdatatable.Dispose();
                }
                Server.tempBans.Add(tBan);
                if(who!=null) who.Kick("Banned for " + minutes.ToString() + "minutes for: " + msg);
            }
            if (who != null)
                who.OffenseNote(DateTime.Now, "%8tempbanned " + minutes + "min%c", msg, p.name);
            else
            {
                Player.GlobalMessage(c.white + name + Server.DefaultColor + " has been temporary banned (offline) by " + p.group.color + p.name);
                if (!Directory.Exists("text/offensenotes")) Directory.CreateDirectory("text/offensenotes");
                string path = "text/offensenotes/" + name + ".txt";
                if (!File.Exists(path)) File.Create(path).Dispose();
                try
                {
                    StreamWriter sw = File.AppendText(path);
                    sw.WriteLine(DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + ": %8tempbanned: " + minutes + "min%c" + " : " + msg + " by " + p.name);
                    sw.Close();

                }
                catch { Server.s.Log("Error saving Offensenote"); }
            }
        
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/tempban <name> <minutes> <reason>- Bans <name> for <minutes>");
            Player.SendMessage(p, "You can use for reason @rulenumber to point to a special rule");
            Player.SendMessage(p, "Max time is 1440 (1 day). Default is 60");
            Player.SendMessage(p, "Temp bans will reset on server restart");
        }
    }
}