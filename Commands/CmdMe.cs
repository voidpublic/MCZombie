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


namespace MCForge.Commands
{
    public class CmdMe : Command
    {
        public override string name { get { return "me"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdMe() { }

        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game!"); return; }
            if (message == "")
            {
                if (!Server.zombieRound && p.referee)
                    Player.SendMessage(p, "You are a " + c.blue + "referee");
                else if (!Server.zombieRound)
                    Player.SendMessage(p, c.red + "Round has not started yet");
                else if (p.infected == false && !p.referee)
                    Player.SendMessage(p, "You are " + c.lime + "human" + Server.DefaultColor + " with " + c.lime + Server.zombie.GetTimeLeft("") + Server.DefaultColor + " left");
                else if (!p.referee)
                    Player.SendMessage(p, "You are " + c.red + "zombie" + Server.DefaultColor + " with " + c.red + p.infectThisRound.ToString() + Server.DefaultColor + " kills and " + c.red + Server.zombie.GetTimeLeft("") + Server.DefaultColor + " left");
                else
                    Player.SendMessage(p, "You are " + c.blue + "referee" + Server.DefaultColor + " with " + c.blue + Server.zombie.GetTimeLeft("") + Server.DefaultColor + " left");
                return;
            }

            if (p.muted) { Player.SendMessage(p, "You are currently muted and cannot use this command."); return; }
            if (Server.chatmod && !p.voice) { Player.SendMessage(p, "Chat moderation is on, you cannot emote."); return; }
            if (Player.CapsDetection(message))
            {
                Player.SendMessage(p,"%cToo much caps in your message!");
                Player.SendMessage(p,"%cPlease do not spam caps");
                return;
            }
            if(message.Contains("%")) 
            {
                Player.SendMessage(p,"%cNo color codes with /me please");
                return;
            }

            if (Server.worldChat)
            {
                Player.GlobalChat(p, p.color + "*" + p.name + " " + message, false);
            }
            else
            {
                Player.GlobalChatLevel(p, p.color + "*" + p.name + " " + message, false);
            }
            //IRCBot.Say("*" + p.name + " " + message);
            Server.IRC.Say("*" + p.name + " " + message);


        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/me - displays if you are human or zombie");
            Player.SendMessage(p, "/me <text> - makes you make the emote <text>");
        }
    }
}