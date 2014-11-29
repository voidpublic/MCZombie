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
    public class CmdLegal : Command
    {
        public override string name { get { return "legal"; } }
        public override string shortcut { get { return "l"; } }
        public override string type { get { return "operator"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdLegal() { }

        public override void Use(Player p, string message)
        {
            if (ZombieGame.alive.Count == 1)
            {
                Player.GlobalMessage("%b--------------------------------------");
                Player.GlobalMessage("%bLast is %aLEGAL");
                Player.GlobalMessage("%bLegal check done by: %a" + p.name);
                Player.GlobalMessage("%6(smile)Good Luck(smile)");  
                Player.GlobalMessage("%b--------------------------------------");
            }
            else
            {
                Player.GlobalMessage("%b--------------------------------------");
                Player.GlobalMessage("%bLast are %aLEGAL");
                Player.GlobalMessage("%bLegal check done by: %a" + p.name);
                Player.GlobalMessage("%6(smile)Good Luck(smile)");
                Player.GlobalMessage("%b--------------------------------------");

            }

        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/legal - sends out a message that everyone is legal");
        }
    }
}