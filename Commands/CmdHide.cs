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
    public class CmdHide : Command
    {
        public override string name { get { return "hide"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdHide() { }

        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game!"); return; }
            if (message == "check")
            {
                if (p.hidden)
                {
                    Player.SendMessage(p, "You are currently hidden!");
                    return;
                }
                else
                {
                    Player.SendMessage(p, "You are not currently hidden!");
                    return;
                }
            }
            else
                if (message != "")
                    if (p.possess != "")
                    {
                        Player.SendMessage(p, "Stop your current possession first.");
                return;
            }
            Command opchat = Command.all.Find("opchat");
            p.hidden = !p.hidden;
            if (p.hidden)
            {
                Player.GlobalDie(p, true);
                if (!p.referee) Command.all.Find("ref").Use(p, "");
                Player.GlobalMessageOps("To Ops -" + p.color + p.name + "-" + Server.DefaultColor + " is now &finvisible" + Server.DefaultColor + ".");
                Player.GlobalChat(p, "&c- " + p.color + p.prefix + p.name + Server.DefaultColor + " " + (File.Exists("text/logout/" + p.name + ".txt") ? File.ReadAllText("text/logout/" + p.name + ".txt") : "disconnected."), false);
                Server.IRC.Say(p.name + " left the game (disconnected.)");
                if (!p.opchat) opchat.Use(p, message);
                //Player.SendMessage(p, "You're now &finvisible&e.");
            }
            else
            {
                Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                if(p.referee)
                {
                    p.referee = false;
                    ushort x = (ushort)((p.pos[0]));
                    ushort y = (ushort)((p.pos[1]));
                    ushort z = (ushort)((p.pos[2]));
                    p.SendUserMOTD();
                    p.SendUserMOTD();
                    p.SendMap();
                    Player.GlobalDie(p, false);
                    Player.GlobalSpawn(p, x, y, z, p.level.rotx, p.level.roty, true);
                    if (p.isFlying) p.isFlying = !p.isFlying;
                    if (Server.zombie.GameInProgess())
                    {
                        x = (ushort)((0.5 + p.level.spawnx) * 32);
                        y = (ushort)((1 + p.level.spawny) * 32);
                        z = (ushort)((0.5 + p.level.spawnz) * 32);
                        unchecked
                        {
                            p.SendPos((byte)-1, x, y, z, p.level.rotx, p.level.roty);
                        }
                        Server.zombie.InfectPlayer(p);
                    }
                    else
                    {
                        Player.GlobalDie(p, false);
                        Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                        if (!ZombieGame.alive.Contains(p)) ZombieGame.alive.Add(p);
                        p.color = p.group.color;
                        p.SetPrefix();
                    }
                }
                Player.GlobalMessageOps("To Ops -" + p.color + p.name + "-" + Server.DefaultColor + " is now &8visible" + Server.DefaultColor + ".");
                Player.GlobalChat(p, "&a+ " + p.color + p.prefix + p.name + Server.DefaultColor + " " + (File.Exists("text/login/" + p.name + ".txt") ? File.ReadAllText("text/login/" + p.name + ".txt") : "joined the game."), false);
                Server.IRC.Say(p.name + " joined the game");
                if (p.opchat) opchat.Use(p, message);
                //Player.SendMessage(p, "You're now &8visible&e.");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/hide - Makes yourself (in)visible to other players also turns opchat on and off.");
        }
    }
}