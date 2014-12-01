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
using System.Data;
using MCForge.SQL;
//using MySql.Data.MySqlClient;
//using SData.Types;


namespace MCForge.Commands
{
    public class CmdWhowas : Command
    {
        public override string name { get { return "whowas"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "information"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdWhowas() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Player pl = Player.Find(message); 
            if (pl != null && !pl.hidden)
            { 
                Player.SendMessage(p, pl.color + pl.name + Server.DefaultColor + " is online, using /whois instead."); 
                Command.all.Find("whois").Use(p, message);
                return; 
            }

            if (message.IndexOf("'") != -1) { Player.SendMessage(p, c.red + "Cannot parse request."); return; }

            Database.AddParams("@Name", message); 
            DataTable playerDb = Database.fillData("SELECT * FROM Players WHERE Name=@Name");
            string FoundRank = Group.findPlayer(message.ToLower()); 
            if (playerDb.Rows.Count == 0) { Player.SendMessage(p, Group.Find(FoundRank).color + message + Server.DefaultColor + " has the rank of " + Group.Find(FoundRank).color + FoundRank); return; }
            int number = playerDb.Rows[0]["achievements"].ToString().Split(',').Length -1;
            if (number >= Server.s.amountofachievements)
            {
                Player.SendMessage(p, c.gold + "-------------------------------------------");
                Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.gold + "Name:", message, c.gold + "Rank:", Group.Find(FoundRank).color + FoundRank));
                Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.gold + "Logins:", c.teal + playerDb.Rows[0]["totalLogin"], c.gold + "Time:", c.teal + TotalTime(playerDb.Rows[0]["TimeSpent"].ToString())));
                Player.SendMessage(p, String.Format("{0,-13}{1,-15}", c.gold + "Lastlogin:", c.teal + playerDb.Rows[0]["LastLogin"]));
                Player.SendMessage(p, String.Format("{0,-13}{1,-15}", c.gold + "FirstLogin: ", c.teal + playerDb.Rows[0]["FirstLogin"]));
                Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.gold + "Survived:", c.lime + playerDb.Rows[0]["roundssurvived"], c.gold + "Infected:", c.red + playerDb.Rows[0]["playersinfected"]));
                Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.gold + "MaxSurvived:", c.green + playerDb.Rows[0]["maximumsurvived"], c.gold + "MaxInfected:", c.maroon + playerDb.Rows[0]["maximuminfected"]));
                Player.SendMessage(p, c.gold + "Wealth: " + c.teal + playerDb.Rows[0]["money"]);
                bool skip = false;
                if (p != null) if ((int)p.group.Permission <= CommandOtherPerms.GetPerm(this)) skip = true;
                if (!skip)
                {
                    string givenIP;
                    if (Server.bannedIP.Contains((string)playerDb.Rows[0]["IP"])) givenIP = c.black + (string)playerDb.Rows[0]["IP"] + ", which is banned";
                    else givenIP = (string)playerDb.Rows[0]["IP"];
                    Player.SendMessage(p, c.gold + "IP: " + c.blue + givenIP);
                    /*if (!Player.IPInPrivateRange(givenIP))
                    {
                        string location = Player.GetIPLocation(givenIP);
                        Player.SendMessage(p, c.white +  "From: " + c.blue + location);
                    }*/
                }
                Player.SendMessage(p, c.gold + "-------------------------------------------");
            }
            else
            {
                Player.SendMessage(p, c.white + "-------------------------------------------");
                Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.white + "Name:", message, c.white + "Rank:", Group.Find(FoundRank).color + FoundRank));
                Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.white + "Logins:", c.teal + playerDb.Rows[0]["totalLogin"], c.white + "Time:", c.teal + TotalTime(playerDb.Rows[0]["TimeSpent"].ToString())));
                Player.SendMessage(p, String.Format("{0,-13}{1,-15}", c.white + "Lastlogin:", c.teal + playerDb.Rows[0]["LastLogin"]));
                Player.SendMessage(p, String.Format("{0,-13}{1,-15}", c.white + "FirstLogin: ", c.teal + playerDb.Rows[0]["FirstLogin"]));
                Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.white + "Survived:", c.lime + playerDb.Rows[0]["roundssurvived"], c.white + "Infected:", c.red + playerDb.Rows[0]["playersinfected"]));
                Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.white + "MaxSurvived:", c.green + playerDb.Rows[0]["maximumsurvived"], c.white + "MaxInfected:", c.maroon + playerDb.Rows[0]["maximuminfected"]));
                Player.SendMessage(p, c.white + "Wealth: " + c.teal + playerDb.Rows[0]["money"]);
                bool skip = false;
                if (p != null) if ((int)p.group.Permission <= CommandOtherPerms.GetPerm(this)) skip = true;
                if (!skip)
                {
                    string givenIP;
                    if (Server.bannedIP.Contains((string)playerDb.Rows[0]["IP"])) givenIP = c.black + (string)playerDb.Rows[0]["IP"] + ", which is banned";
                    else givenIP = (string)playerDb.Rows[0]["IP"];
                    Player.SendMessage(p, c.white + "IP: " + c.blue + givenIP);
                    /*if (!Player.IPInPrivateRange(givenIP))
                    {
                        string location = Player.GetIPLocation(givenIP);
                        Player.SendMessage(p, c.white +  "From: " + c.blue + location);
                    }*/
                }
                Player.SendMessage(p, c.white + "-------------------------------------------");
            }
            playerDb.Dispose();
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/whowas <name> - Displays information about someone who left.");
        }
        public string TotalTime(string time)
        {
            try
            {
                return time.Split(' ')[0] + "d" + time.Split(' ')[1] + "h" + time.Split(' ')[2] + "m";
            }
            catch { Server.s.Log("ERROR: #010");  return "0"; }
        }
    }
}