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
using System.IO;


namespace MCForge.Commands
{
    public class CmdQueue : Command
    {
        public override string name { get { return "queue"; } }
        public override string shortcut { get { return "qz"; } }
        public override string type { get { return "headop"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdQueue() { }

        public override void Use(Player p, string message)
        {
            int number = message.Split(' ').Length;
            if (number > 2) { Help(p); return; }
            if (number == 2)
            {
                Server.s.Log(message);
                string t = message.Split(' ')[0];
                string s = message.Split(' ')[1];
                if (t == "zombie")
                {
                    if (Server.queZombie)
                    {
                        Player.SendMessage(p, "%cSorry there is already a zombie queued");
                        return;
                    }
                    Player queuedzombie = Player.Find(s);
                    if (queuedzombie == null) { Player.SendMessage(p, s + " is not online"); return; }
                    else
                    {
                        Player.GlobalMessage(queuedzombie.group.color + queuedzombie.name + c.red + " will start the infection next round");
                        Server.queZombie = true;
                        Server.nextZombie = queuedzombie.name;
                        return;
                    }
                }
                else if (t == "level")
                {
                    if (Server.queLevel) { Player.SendMessage(p, "%cSorry there is already a level queued"); return; }
                    if (Server.zombie.currentLevelName == s) { Player.SendMessage(p, "%cYou are not allowed to queue a level that is played on "); return; }
                    if (ZombieGame.levelsplayed.Contains(s)) { Player.SendMessage(p, c.red + "Sorry level has been played within the last 20 rounds"); return; }
                    bool yes = false;
                    DirectoryInfo di = new DirectoryInfo("levels/");
                    FileInfo[] fi = di.GetFiles("*.lvl");
                    foreach (FileInfo file in fi)
                    {
                        if (file.Name.Replace(".lvl", "").ToLower().Equals(s.ToLower()))
                        {
                            yes = true;
                        }
                    }
                    if (!Server.zombieRound)
                    {
                        Player.SendMessage(p, "%cCan only queue while game going on!");
                        return;
                    }
                    if (yes)
                    {
                        Player.GlobalMessage(c.white + "The level " + c.aqua + s + c.white + " was queued");
                        Server.queLevel = true;
                        Server.nextLevel = s.ToLower();
                        return;
                    }
                    else
                    {
                        Player.SendMessage(p, "%cLevel does not exist.");
                        return;
                    }
                }
                else if (t == "xlevel")
                {
                    if (Server.zombie.currentLevelName == s) { Player.SendMessage(p, "%cYou are not allowed to queue a level that is played on "); return; }
                    bool yes = false;
                    DirectoryInfo di = new DirectoryInfo("levels/");
                    FileInfo[] fi = di.GetFiles("*.lvl");
                    foreach (FileInfo file in fi)
                    {
                        if (file.Name.Replace(".lvl", "").ToLower().Equals(s.ToLower()))
                        {
                            yes = true;
                        }
                    }
                    if (yes)
                    {
                        Player.SendMessage(p, c.lime + " Queue level " + s.ToLower() + " silently+forced it");
                        Server.queLevel = true;
                        Server.nextLevel = s.ToLower();
                        return;
                    }
                    else
                    {
                        Player.SendMessage(p, "%cLevel does not exist.");
                        return;
                    }
                }
                else
                {
                    Player.SendMessage(p, "You did not enter a valid option.");
                }
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/queue zombie [name] - Next round [name] will be infected");
            Player.SendMessage(p, "/queue level [name] - Next round [name] will be the round loaded");
        }
    }
}