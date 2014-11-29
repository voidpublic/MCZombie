
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Commands
{
    public class CmdEat : Command
    {
        public override string name { get { return "eat"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdEat() { }
        public override void Use(Player p, string message)
        {
            if (p.money > 0)
            {
                if (p.stuffed)
                {
                    Player.SendMessage(p, c.red + "You are currently stuffed from the last cookie eaten");
                }
                else
                {
                    p.stuffed = true;
                    p.cookietimer.Start();
                    p.cookietimer.Elapsed += delegate
                    {
                        p.stuffed = false;
                    };
                    Random nr = new Random();
                    int msgnr = nr.Next(0, 20);
                    string msg = "";
                    switch (msgnr)
                    {
                        case 0: msg = "ate a cookie"; break;
                        case 1: msg = "nommed a cookie"; break;
                        case 2: msg = "got eaten by a cookie"; break;
                        case 3: msg = "couldnt get enough from one cookie"; break;
                        case 4: msg = "was aroused by a cookie"; break;
                        case 5: msg = "bloodthirst increased, because a cookie was eaten"; break;
                        case 6: msg = "has killed a cookie"; break;
                        case 7: msg = "ate them all"; break;
                        case 8: msg = "tasted bacon cookies"; break;
                        case 9: msg = "accidently ate a supercookie"; break;
                        case 10: msg = "got high by eating special cookies"; break;
                        case 11: msg = "ate Fido's dog biscuit!"; break;
                        case 12: msg = "is on a diet, No cookie was eaten!"; break;
                        case 13: msg = "'s cookie was sugar free :("; break;
                        case 14: msg = "typed /eat"; break;
                        case 15: msg = "was slain by the cookie army "; break;
                        case 16: msg = "thought that brains were overrated and ate a cookie instead"; break;
                        case 17: msg = "joyfully ate a cookie in the middle of a zombie apocalypse"; break;
                        case 18: msg = "just ate a Scooby Snack!"; break;
                        case 19: msg = "had a rainbow cookie"; break;
                        case 20: msg = "tried to eat the cookie monster"; break;
                    }
                    p.money -= 1;
                    Player.GlobalMessage(p.color + p.name + Server.DefaultColor + " " + msg);
                }
            }
            else
                Player.SendMessage(p,"You have not got any cookies to eat");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/eat - Eats one of your cookies.");
            Player.SendMessage(p, "%cWARNING: You will lose it.");
        }
    }
}