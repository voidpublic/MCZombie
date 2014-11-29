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
using System.Threading;
using System.Linq;

namespace MCForge.Commands
{
    class CmdRule : Command
    {
        public override string name { get { return "rule"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdRule() { }

        public override void Use(Player p, string message)
        {
            int number = 0;
            try { number = Convert.ToInt32(message); }
            catch { Player.SendMessage(p, "Need to insert a number"); return; }
            string path = "text/rulesmore.txt";
            if (!File.Exists(path)) File.Create(path);
            string[] lines = File.ReadAllLines(path);
            if (number > lines.Count())
            {
                Player.SendMessage(p, "Rulenumber does not exist");
                return;
            }
            else
            {
                Player.SendMessage(p, lines[number - 1]);
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/rules [player]- Displays server rules to a player");
            Player.SendMessage(p, "/rule <rulenumber> - Shows more information about a rule");
        }
    }
}
