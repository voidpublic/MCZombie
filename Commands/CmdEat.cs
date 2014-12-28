
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
            if (p == null){  Player.SendMessage(p, c.red + "Sorry consoles can't eat cookies ;(");  return; }
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
                    p.money -= 1;
                    Player.GlobalMessage(GetEatMessage(p));
                }
            }
            else
                Player.SendMessage(p,"You have not got any cookies to eat");
        }

        public string GetEatMessage(Player eater)
        {
            string path = "text/eatmessages.txt";
            if (!File.Exists(path))
            {
                string[] prepared = new string[] 
                { "#Use <name> to display eat messages", 
                  "<name> ate a cookie",
                  "<name> nommed a cookie",
                  "<name> got eaten by a cookie", 
                  "<name> ate them all"
                };
                File.WriteAllLines(path, prepared);
            }
            string[] lines = File.ReadAllLines(path);
            List<string> messages = new List<string>();
            foreach (string msg in lines)
            {
                if (!msg.StartsWith("#")) messages.Add(msg);
            }
            int useline = new Random().Next(0, messages.Count());
            string toreturn = messages[useline];
            toreturn = toreturn.Replace("<name>", eater.group.color + eater.name + Server.DefaultColor);
            return toreturn;
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/eat - Eats one of your cookies.");
            Player.SendMessage(p, "%cWARNING: You will lose it.");
        }
    }
}