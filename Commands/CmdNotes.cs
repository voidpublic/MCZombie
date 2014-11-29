using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MCForge.Commands
{
    public class CmdNotes : Command
    {
        public override string name { get { return "notes"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "trusted"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdNotes() { }
        public override void Use(Player p, string message)
        {
            Player checking = null;
            if(message.Contains(' ')) checking = Player.Find(message.Substring(0, message.IndexOf(' '))); 
            else  checking = Player.Find(message); 
            string checkname;
            if (checking == null)
            {
                Player.SendMessage(p, "Player not online, searching for the full name");
                if (message.Contains(' ')) checkname = message.Substring(0, message.IndexOf(' '));
                else checkname = message;
            }
            else checkname = checking.name;
            string path = "text/offensenotes/" + checkname + ".txt";
            if (!File.Exists(path)) { Player.SendMessage(p, "No notes found for " + checkname); return; }
            try
            {
                List<string> l = new List<string>(File.ReadAllLines(path));
                List<string> newl = new List<string>();
                for (int i = 0; i < l.Count; i++)
                {
                    int index = l[i].IndexOf(":");
                    string notetimestring = l[i].Substring(0, index);
                    string temp = notetimestring;
                    string day = temp.Substring(0, temp.IndexOf("."));
                    temp = temp.Substring(temp.IndexOf(".") + 1);
                    string month = temp.Substring(0, temp.IndexOf("."));
                    string year = temp.Substring(temp.IndexOf(".") + 1);
                    DateTime notetime = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), Convert.ToInt32(day));
                    if ((DateTime.Now - notetime).TotalDays < 150 || l[i].ToLower().Contains("banned"))
                    {
                        newl.Add(l[i]);
                    }
                }
                if (l.Count == 0)
                    File.Delete(path);
                else
                    File.WriteAllLines(path, newl.ToArray());
                if (!File.Exists(path)) { Player.SendMessage(p, "No notes found for " + checkname); return; }
            }
            catch
            {
                string path2 = "text/problemnotes.txt";
                Player.SendMessage(p, c.red + "There was a problem with removing old notes from " + checkname);
                Player.SendMessage(p, c.red + "An automatic report has been generated, there is nothing you need to do");
                File.AppendAllText(path2, checkname + "/");
            }
            string[] lines = File.ReadAllLines(path);
            Player.SendMessage(p, "Player " + c.aqua + checkname + Server.DefaultColor + " has %b" + lines.Count() + Server.DefaultColor + " notes:");
            if (message.Contains(' '))
            {
                bool breaks = false;
                if (lines.Count() > 6) breaks = true;
                int count = 0;
                foreach (string line in lines)
                {
                    Player.SendMessage(p, c.red + line);
                    if (breaks)
                    {
                        count++;
                        if (count == 6)
                        {
                            Thread.Sleep(4000);
                            count = 0;
                        }
                    }
                }
            }
            else
            {
                if (lines.Count() > 5)
                {
                    for (int a = lines.Count() - 5; a < lines.Count(); a++)
                    {
                        Player.SendMessage(p, c.red + lines[a]);
                    }
                }
                else
                {
                    for (int a = 0; a < lines.Count(); a++)
                    {
                        Player.SendMessage(p, c.red + lines[a]);
                    }
                }
            }

        }
        public override void Help(Player p)
        {
            Player.SendMessage(p,"/notes <playername> - shows what the player has been warned/kicked/tempbanned for lately");
            Player.SendMessage(p,"/notes <playername> /all - shows all of the offenses");
        }
    }
}