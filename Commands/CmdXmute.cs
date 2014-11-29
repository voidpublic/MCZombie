/*
	Copyright 2011 MCForge
	
	Written by GamezGalaxy (hypereddie10)
		
	Licensed under the
	Educational Community License, Version 2.0 (the "License"); you may
	not use this file except in compliance with the License. You may
	obtain a copy of the License at
	
	http://www.opensource.org/licenses/ecl2.php
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the License is distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the License for the specific language governing
	permissions and limitations under the License.
*/
using System;
using System.Threading;


namespace MCForge.Commands
{
        public class CmdXmute : Command
        {
                public override string name { get { return "xmute"; } }
                public override string shortcut { get { return ""; } }
                public override string type { get { return "trusted"; } }
                public override bool museumUsable { get { return false; } }
                public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
                public override void Use(Player p, string message)
                {
					if(message == "")
					{
						Help(p);
						return;
					}

                    /*if (p == null)
                    {
                        Player.SendMessage(p, "This command can only be used in-game. Use /mute [Player] instead.");
                        return;
                    }*/

					var split = message.Split(' ');
            		Player muter = Player.Find(split[0]);
		            if (muter == null)
        		    {
                		Player.SendMessage(p, "Player not found.");
						return;
		            }
    		        if (p != null && muter.group.Permission > p.group.Permission)
	                {
	                    Player.SendMessage(p, "You cannot xmute someone ranked higher than you!");
						return;
                    }
                    int time = 120;
                    try
                    {
                        time = Convert.ToInt32(message.Split(' ')[1]);
                        if (time < 1)
                        {
                            Player.SendMessage(p, c.red + "Invalid time given");
                            return;
                        }
                    }
                    catch
                    {
                        Player.SendMessage(p, c.red + "Invalid time given.");
                        Help(p);
                        return;
                    }
                    if (muter == p)
                    {
                        Player.SendMessage(p, c.red + "Cannot mute yourself");
                        return;
                    }
                    if(muter.muted)
                    {
                        Player.SendMessage(p, c.red + "Player is already muted");
                        return;
                    }
                    if (time > 300)
                    {
                        Player.SendMessage(p, "Setting maximum mute time : 300");
                        time = 300;
                    }
                    Command.all.Find("mute").Use(p, muter.name);
                    string nameofmuted = muter.name;
					
					
                    Player.GlobalMessage(muter.color + muter.name + " has been muted for " + time + " seconds");

                    Thread.Sleep(time * 1000);

                    try { if(Player.Find(nameofmuted).muted) Command.all.Find("mute").Use(p, muter.name); }
                    catch { Player.SendMessage(p,"Player not online anymore"); }
                }

                // This one controls what happens when you use /help [commandname].
                public override void Help(Player p)
                {
                        Player.SendMessage(p, "/xmute <player> <seconds> - Mutes <player> for <seconds> seconds");
                }
        }
}


