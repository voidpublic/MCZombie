using System;
using System.Data;
using MCForge.SQL;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MCForge.Commands
{
    public class CmdReference : Command
    {
        public override string name { get { return "reference"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "operator"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdReference() { }
        public override void Use(Player p, string message)
        {
            if (message.Split(' ').Length > 1)
            {
                string tag = message.Split(' ')[0];
                if (tag == "remove")
                {
                    if (!Directory.Exists("text/references")) Directory.CreateDirectory("text/references");
                    string name = "";
                    Player who = Player.Find(message.Split(' ')[1]);
                    if (who == null)
                    {
                        Player.SendMessage(p,"Player not online.. searching in Database");
                        name = message.Split(' ')[1];
                    }
                    else name = who.name;
                    string path = "text/references/" + name + ".txt";
                    if (!File.Exists(path))
                    {
                        Player.SendMessage(p,c.red + "Player " + name + " has got no references (name spelt wrong?)");
                        return;
                    }
                    List<string> l = new List<string>(File.ReadAllLines(path));
                    if (l.Count() == 1)
                        File.Delete(path);
                    else
                    {
                        for (int i = 0; i < l.Count; i++)
                            if (l[i].Contains(p.name)) l.Remove(l[i]);
                        File.WriteAllLines(path, l.ToArray());
                    }
                    Player.SendMessage(p,c.lime + "Succesfully revoked your reference for: " + name);
                }
                else
                {
                    Help(p);
                    return;
                }

            }
            else
            {
                if (!Directory.Exists("text/references")) Directory.CreateDirectory("text/references");
                Player who = Player.Find(message);
                if (who == p)
                {
                    Player.SendMessage(p,c.red + "Cant let you reference yourself derp");
                    return;
                }
                if (who == null)
                {
                    Player.SendMessage(p,c.red + "Player must be online to give out reference");
                    return;
                }
                if (Group.findPlayerGroup(p.name).Permission <= Group.findPlayerGroup(who.name).Permission)
                {
                    Player.SendMessage(p,c.red + "Can only give a reference for a lower rank than yourself");
                    return;
                }
                Group foundGroup = Group.findPlayerGroup(who.name);
                Group nextGroup = null;
                bool nextOne = false;
                for (int i = 0; i < Group.GroupList.Count; i++)
                {
                    Group grp = Group.GroupList[i];
                    if(Server.name.ToLower().Contains("minemaniacs"))
                    {
                        if (nextOne && grp.name.ToLower() != "respected")
                        {
                            if (grp.Permission >= LevelPermission.Nobody) break;
                            nextGroup = grp;
                            break;
                        }
                        if (grp == foundGroup)
                            nextOne = true;
                    }
                    else
                    {
                        if (nextOne)
                        {
                            if (grp.Permission >= LevelPermission.Nobody) break;
                            nextGroup = grp;
                            break;
                        }
                        if (grp == foundGroup)
                            nextOne = true;
                    }
                }
                string path = "text/references/" + who.name + ".txt";
                if (!File.Exists(path)) File.Create(path).Dispose();
                string[] lines = File.ReadAllLines(path);
                for (int a = 0; a < lines.Count(); a++)
                {
                    if (lines[a].Contains(p.name))
                    {
                        Player.SendMessage(p,c.red + "Can only give out your reference once");
                        Player.SendMessage(p,c.red + "If you wish to remove your reference use /reference remove <player>");
                        return;
                    }
                }
                try
                {
                    StreamWriter sw = File.AppendText(path);
                    sw.WriteLine(p.group.color + p.name + Server.DefaultColor + " for: " + nextGroup.color + nextGroup.name);
                    sw.Close();

                }
                catch { Server.s.Log("Error saving Reference"); }
                Player.SendMessage(p,c.aqua + "Gave out a reference: " + who.color + who.name + c.aqua + " for: " + nextGroup.color + nextGroup.name);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/reference <player> - Gives a <player> their reference");
            Player.SendMessage(p, "/reference remove <player> - Removes your reference");
        }
    }
}