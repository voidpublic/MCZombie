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


namespace MCForge.Commands
{
    public class CmdWhois : Command
    {
        public override string name { get { return "whois"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdWhois() { }
        public static string Indent(int count)
        {
            return "".PadLeft(count);
        }
        public override void Use(Player p, string message)
        {
            Player who = null;
            if (message == "") { who = p; message = p.name; } else { who = Player.Find(message); }
            if (who != null && !who.hidden)
            {
                int number = who.achievementnumbers.Split(',').Length - 1;
                string storedTime = Convert.ToDateTime(DateTime.Now.Subtract(who.timeLogged).ToString()).ToString("HH:mm:ss");
                if (number >= Server.s.amountofachievements)
                {
                    Player.SendMessage(p, c.gold + "-------------------------------------------");
                    Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.gold + "Name:", who.color + who.name, c.gold + "Rank:", who.group.color + who.group.name));
                    Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.gold + "Logins:", c.teal + who.totalLogins, c.gold + "Time:", c.teal + who.time.Split(' ')[0] + "d" + who.time.Split(' ')[1] + "h" + who.time.Split(' ')[2] + "m"));
                    Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.gold + "Online:", c.teal + storedTime, c.gold + "FirstLogin: ", c.teal + who.firstLogin.ToString("yyyy-MM-dd")));
                    Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.gold + "Survived:", c.lime + who.roundssurvived, c.gold + "Infected:", c.red + who.playersinfected));
                    Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.gold + "MaxSurvived: ", c.green + who.maximumsurvived, c.gold + "MaxInfected: ", c.maroon + who.maximuminfected));
                    Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.gold + "Wealth:", c.teal + who.money, c.gold + "Team:", who.teamcolor + who.teamname));
                    bool skip = false;
                    if (p != null) if ((int)p.group.Permission <= CommandOtherPerms.GetPerm(this)) skip = true;
                    if (!skip)
                    {
                        string givenIP;
                        if (Server.bannedIP.Contains(who.ip)) givenIP = "&8" + who.ip + ", which is banned";
                        else givenIP = who.ip;
                        Player.SendMessage(p, c.gold + "IP: " + c.blue + givenIP);
                        /*
                        if (!Player.IPInPrivateRange(givenIP))
                        {
                            string location = Player.GetIPLocation(givenIP);
                            Player.SendMessage(p, c.gold +  "From: " + c.blue + location);
                        }   */
                    }
                    Player.SendMessage(p, c.gold + "-------------------------------------------");
                }
                else
                {
                    Player.SendMessage(p, c.white + "-------------------------------------------");
                    Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.white + "Name:", who.color + who.name, c.white + "Rank:", who.group.color + who.group.name));
                    Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.white + "Logins:", c.teal + who.totalLogins, c.white + "Time:", c.teal + who.time.Split(' ')[0] + "d" + who.time.Split(' ')[1] + "h" + who.time.Split(' ')[2] + "m"));
                    Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.white + "Online:", c.teal + storedTime, c.white + "FirstLogin: ", c.teal + who.firstLogin.ToString("yyyy-MM-dd")));
                    Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.white + "Survived:", c.lime + who.roundssurvived, c.white + "Infected:", c.red + who.playersinfected));
                    Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.white + "MaxSurvived: ", c.green + who.maximumsurvived, c.white + "MaxInfected: ", c.maroon + who.maximuminfected));
                    Player.SendMessage(p, String.Format("{0,-13}{1,-15}{2,-12}{3,-15}", c.white + "Wealth:", c.teal + who.money, c.white + "Team:", who.teamcolor + who.teamname));
                    bool skip = false;
                    if (p != null) if ((int)p.group.Permission <= CommandOtherPerms.GetPerm(this)) skip = true;
                    if (!skip)
                    {
                        string givenIP;
                        if (Server.bannedIP.Contains(who.ip)) givenIP = "&8" + who.ip + ", which is banned";
                        else givenIP = who.ip;
                        Player.SendMessage(p, c.white + "IP: " + c.blue + givenIP);
                        /*
                        if (!Player.IPInPrivateRange(givenIP))
                        {
                            string location = Player.GetIPLocation(givenIP);
                            Player.SendMessage(p, c.white +  "From: " + c.blue + location);
                        }   */
                    }
                    Player.SendMessage(p, c.white + "-------------------------------------------");
                }

            }
            else { Player.SendMessage(p, "\"" + message + "\" is offline! Using /whowas instead."); Command.all.Find("whowas").Use(p, message); }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/whois [player] - Displays information about someone.");
        }
    }
}