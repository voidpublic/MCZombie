
/*
    Copyright 2012 by void_public, MCForge Member
    You must give credit to the original author even when you edit the code.
    You may alter, edit or build new things on this
    You may not use this work for commercial purposes.
    tbh this code is REALLY messy, i don't know what i thought when doing this. There should be a class called team
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace MCForge.Commands
{
    class CmdTeam : Command
    {
        public override string name { get { return "team"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public static string keywords { get { return ""; } }
        public CmdTeam() { }
        public int Permissioncheck(string check, string team)
        {
            if (team != "")
            {
                string[] admincheck = File.ReadAllLines("teams/" + team + ".txt");
                foreach (string line in admincheck)
                {
                    if (line == ("(TLeader)" + check))
                        return 10;
                    if (line == ("(TCaptain)" + check))
                        return 5;
                    if (line == ("(THelper)" + check))
                        return 2;
                    if (line == ("(TMember)" + check))
                        return 1;
                }
                return 0;
            }
            else return 0;
        }
        public override void Use(Player p, string message)
        {
            string action = message.Split(' ')[0];
            string todo = "";

            if (!Directory.Exists("teams")) Directory.CreateDirectory("teams");
            if (message.Split(' ').Length == 2) todo = message.Split(' ')[1];
            DirectoryInfo teamlist = new DirectoryInfo("teams/");
            switch (action)
            {
                case "create":
                    if (todo.Length != 3) { Player.SendMessage(p,"Unallowed length. Must be 3 letters"); return; }
                    if (todo.Contains("%") || todo.Contains("&")) { Player.SendMessage(p,"Unallowed characters"); return; }
                    if (File.Exists("teams/" + todo + ".txt")) { Player.SendMessage(p,"Teamname already taken"); return; }
                    if (p.teamname!="") { Player.SendMessage(p,"You are already in a team."); return; }
                    if (!p.EnoughMoney(200)) { Player.SendMessage(p,"You havent got 200" + Server.moneys + " to buy a new team"); return; }
                    StreamWriter newteam = new StreamWriter(File.Create("teams/" + todo + ".txt"));
                    newteam.WriteLine("(TLeader)" + p.name);
                    newteam.Close();
                    Player.SendMessage(p, "Successfully created a new team!");
                    Player.GlobalMessage("A team named %a" + todo + Server.DefaultColor + " has just been created!");
                    p.money -= 200;
                    p.teamname = todo;
                    p.SetPrefix();
                    return;
                case "color":
                    if (p.teamname == "") { Player.SendMessage(p, c.red + "You are not in a team"); return; }
                    if (Permissioncheck(p.name, p.teamname) < 5){Player.SendMessage(p, c.red + "Must be TCaptain or higher to buy color");return;}
                    string teamcolor = c.Parse(todo);
                    if (teamcolor == "") { Player.SendMessage(p, c.red + "There is no color: " + todo); return; }
                    if (!p.EnoughMoney(150)) { Player.SendMessage(p, c.red + "You havent got 150" + Server.moneys + " to buy a team color"); return; }
                    teamcolor = todo;
                    if (Player.GetTeamColor(p) != "")
                    {
                        List<string> l = new List<string>(File.ReadAllLines("teams/" + p.teamname + ".txt"));
                        for (int i = 0; i < l.Count; i++)
                            if (l[i].Contains("(TColor)")) l.Remove(l[i]);
                        File.WriteAllLines("teams/" + p.teamname + ".txt", l.ToArray());
                    }
                    StreamWriter teamcolors = File.AppendText("Teams/" + p.teamname + ".txt");
                    teamcolors.WriteLine("(TColor)" + teamcolor);
                    teamcolors.Close();
                    p.money -= 150;
                    p.teamcolor = c.Parse(teamcolor);
                    p.SetPrefix();          
                    Player.SendMessage(p,c.lime+ "Sucessfully bought " + c.Parse(teamcolor) + teamcolor + c.lime + " as your teamcolor");
                    break;
                case "del":
                    todo = p.teamname;
                    if (todo == "") { Player.SendMessage(p, c.red + "You are not in a team"); return; }
                    if (Permissioncheck(p.name, todo) == 10)
                    {
                        Player.players.ForEach(delegate(Player tofix)
                        {
                            if(tofix.teamname == p.teamname)
                            {
                                tofix.teamname = "";
                                tofix.SetPrefix();
                            }
                        });
                        File.Delete("teams/" + todo + ".txt");
                        if(!File.Exists("teams/" + todo +".txt"))
                            Player.SendMessage(p,"Team %a" + todo + Server.DefaultColor + " successfully deleted");
                        return;
                    }
                    Player.SendMessage(p, c.red + "Cannot delete the team because you are not leader");
                    return;
                case "invite":
                    Player toinvite = null;
                    string toinviteteam = p.teamname;
                    toinvite = Player.Find(todo);
                    if (toinvite == null) { Player.SendMessage(p,c.red + "Player not found"); return; };
                    if (toinvite == p) { Player.SendMessage(p,c.red + "Cannot invite yourself to your team"); return; };
                    if (toinvite.teamname != "") { Player.SendMessage(p, c.red + "That player is already in a team"); return; }
                    if (toinvite.invitedtoteam != "") { Player.SendMessage(p, c.red + "That player already has another invitation"); return; }
                    if (Permissioncheck(p.name, toinviteteam) >= 2)
                    {
                        toinvite.invitedtoteam = toinviteteam;
                        toinvite.SendMessage("You have been invited to the team %a" + toinviteteam);
                        toinvite.SendMessage("Join by typing %a/team join");
                        toinvite.SendMessage("Or decline by typing %a/team decline");
                        Player.SendMessage(p,"You have successfully invited %a" + toinvite.name  +Server.DefaultColor + " to your team");
                        return;
                    }
                    Player.SendMessage(p,c.red + "You need to be THelper or higher to use this");
                    return;
                case "decline":
                    if (p.invitedtoteam == ""){ Player.SendMessage(p, c.red + "You are not invited"); return;}
                    Player.SendMessage(p,"Successfully declined");
                    p.invitedtoteam = "";
                    return;
                case "join":
                    todo = p.invitedtoteam;
                    if (todo == "") { Player.SendMessage(p,c.red + "You cannot join because you are not invited"); return; }
                    if (p.teamname != "") { Player.SendMessage(p,c.red + "You already are in a team"); return; }
                    StreamWriter playerjoin = File.AppendText("Teams/" + todo + ".txt");
                    playerjoin.WriteLine("(TMember)" + p.name);
                    playerjoin.Close();
                    Player.SendMessage(p, "Successfully joined the, " + todo + " team!");
                    Player.GlobalMessage(p.color + p.name + Server.DefaultColor + " has joined: %a" + todo + Server.DefaultColor + " team!");
                    p.invitedtoteam = "";
                    p.teamname = todo;
                    p.teamcolor = c.Parse(Player.GetTeamColor(p));
                    p.SetPrefix();
                    return;
                case "kick":
                    Player tokick = Player.Find(todo);
                    if (tokick != null)
                    {
                        todo = p.teamname;
                        if (todo != tokick.teamname) { Player.SendMessage(p, c.red + "Can only kick someone from the same team"); return; }
                        if (Permissioncheck(p.name, todo) > Permissioncheck(tokick.name, todo))
                        {
                            List<string> l = new List<string>(File.ReadAllLines("teams/" + todo + ".txt"));
                            for (int i = 0; i < l.Count; i++)
                            {
                                if (l[i] == "(TMember)" + tokick.name
                                   || l[i] == "(THelper)" + tokick.name
                                   || l[i] == "(TCaptain)" + tokick.name)
                                {
                                    l.Remove(l[i]);
                                    File.WriteAllLines("teams/" + todo + ".txt", l.ToArray());
                                    tokick.SendMessage(c.red + "You have been kicked out of the team %a" + todo);
                                    Player.SendMessage(p, "You have successfully kicked out %a" + tokick.name + Server.DefaultColor + " from your team");
                                    Player.GlobalMessage(tokick.name + Server.DefaultColor + " was kicked out of: %a" + todo + Server.DefaultColor + " team!");
                                    tokick.teamname = "";
                                    tokick.teamcolor = "";
                                    tokick.SetPrefix();
                                }
                            }
                            return;
                        }
                        Player.SendMessage(p, c.red + "You need to be a higher Teamrank to do this");
                        return;
                    }
                    else
                    {
                        string tokicks = todo;
                        todo = p.teamname;
                        if (Permissioncheck(p.name, todo) > Permissioncheck(tokicks, todo))
                        {
                            bool found = false;
                            List<string> l = new List<string>(File.ReadAllLines("teams/" + todo + ".txt"));
                            for (int i = 0; i < l.Count; i++)
                            {
                                if (l[i] == "(TMember)" + tokicks
                                   || l[i] == "(THelper)" + tokicks
                                   || l[i] == "(TCaptain)" + tokicks)
                                {
                                    l.Remove(l[i]);
                                    File.WriteAllLines("teams/" + todo + ".txt", l.ToArray());
                                    found = true;
                                }

                            }
                            if (!found) Player.SendMessage(p, c.red + "Player " + tokicks + " not in your team");
                            else
                            {
                                Player.SendMessage(p, "You have successfully kicked out %a" + tokicks + Server.DefaultColor + " from your team");
                                Player.GlobalMessage(tokicks + Server.DefaultColor + " was kicked out of: %a" + todo + Server.DefaultColor + " team!");
                            }
                            return;
                        }
                        Player.SendMessage(p, c.red + "You need to be a higher Teamrank to do this");
                        return;
                    }
                case "leave":
                    todo = p.teamname;
                    if (todo == "") { Player.SendMessage(p, c.red + "You are not in a team"); return; }
                    if (Permissioncheck(p.name, todo) == 10) { Player.SendMessage(p, c.red + "You can only /team del if you want to delete your team"); return; }
                    List<string> l1 = new List<string>(File.ReadAllLines("teams/" + todo + ".txt"));
                    for (int i = 0; i < l1.Count; i++)
                    {
                        if (l1[i] == "(TMember)" + p.name
                                || l1[i] == "(THelper)" + p.name
                                || l1[i] == "(TCaptain)" + p.name)
                        {
                            l1.Remove(l1[i]);
                            File.WriteAllLines("teams/" + todo + ".txt", l1.ToArray());
                        }
                    }
                    Player.SendMessage(p,"You have succesfully left your team %a" + todo);
                    Player.GlobalMessage(p.color + p.name + Server.DefaultColor + " has left: %a" + todo + Server.DefaultColor + " team!");
                    p.teamname = "";
                    p.SetPrefix();
                    return;
                case "promote":
                    Player promote = Player.Find(todo);
                    todo = p.teamname;
                    if (promote == null) { Player.SendMessage(p, c.red + "Player not found"); return; }
                    if (todo != promote.teamname) { Player.SendMessage(p, c.red + "Cannot promote a player from another team"); return; }
                    if (Permissioncheck(p.name, todo) < 5) { Player.SendMessage(p, c.red + "Must be atleast TCaptain to do this"); return; }
                    if (Permissioncheck(p.name, todo) < Permissioncheck(promote.name, todo)) { Player.SendMessage(p, c.red + "Cannot promote someone to the same rank or higher"); return; }
                    List<string> find = new List<string>(File.ReadAllLines("teams/" + todo + ".txt"));
                    switch (Permissioncheck(p.name, todo))
                    {
                        case 10:
                            for (int i = 0; i < find.Count; i++)
                            {
                                if (find[i] == "(TMember)" + promote.name)
                                {
                                    find.Remove(find[i]);
                                    File.WriteAllLines("teams/" + todo + ".txt", find.ToArray());
                                    StreamWriter promoteplayer = File.AppendText("Teams/" + todo + ".txt");
                                    promoteplayer.WriteLine("(THelper)" + promote.name);
                                    promoteplayer.Close();
                                    p.SendMessage(c.lime + "Successfully promoted " + promote.name + " to THelper");
                                    promote.SendMessage(c.lime + "You were promoted to THelper");
                                    return;
                                }
                                if (find[i] == "(THelper)" + promote.name)
                                {
                                    find.Remove(find[i]);
                                    File.WriteAllLines("teams/" + todo + ".txt", find.ToArray());
                                    StreamWriter promoteplayer = File.AppendText("Teams/" + todo + ".txt");
                                    promoteplayer.WriteLine("(TCaptain)" + promote.name);
                                    promoteplayer.Close();
                                    p.SendMessage(c.lime + "Successfully promoted " + promote.name + " to TCaptain");
                                    promote.SendMessage(c.lime + "You were promoted to TCaptain");
                                    return;
                                }
                            }
                            return;
                        case 5:
                            for (int i = 0; i < find.Count; i++)
                            {
                                if (find[i] == "(TMember)" + promote.name)
                                {
                                    find.Remove(find[i]);
                                    File.WriteAllLines("teams/" + todo + ".txt", find.ToArray());
                                    StreamWriter promoteplayer = File.AppendText("Teams/" + todo + ".txt");
                                    promoteplayer.WriteLine("(THelper)" + promote.name);
                                    promoteplayer.Close();
                                    p.SendMessage(c.lime + "Successfully promoted " + promote.name + " to THelper");
                                    promote.SendMessage(c.lime + "You were promoted to THelper");
                                    return;
                                }
                            }
                            return;
                        default:
                            Player.SendMessage(p, "You need to be TCaptain or higher to use this");
                            return;
                    }
                case "demote":
                    Player demote = Player.Find(todo);
                    todo = p.teamname;
                    if (demote == null) { Player.SendMessage(p, c.red + "Player not found"); return; }
                    if (todo != demote.teamname) { Player.SendMessage(p, c.red + "Cannot demote a player from another team"); return; }
                    if (Permissioncheck(p.name, todo) < 5) { Player.SendMessage(p, c.red + "Must be atleast TCaptain to do this"); return; }
                    if (Permissioncheck(p.name, todo) < Permissioncheck(demote.name, todo)) { Player.SendMessage(p, c.red + "Cannot demote someone to the same rank or higher"); return; }
                    List<string> find2 = new List<string>(File.ReadAllLines("teams/" + todo + ".txt"));
                    switch (Permissioncheck(p.name, todo))
                    {
                        case 10:
                            for (int i = 0; i < find2.Count; i++)
                            {
                                if (find2[i] == "(TCaptain)" + demote.name)
                                {
                                    find2.Remove(find2[i]);
                                    File.WriteAllLines("teams/" + todo + ".txt", find2.ToArray());
                                    StreamWriter demoteplayer = File.AppendText("Teams/" + todo + ".txt");
                                    demoteplayer.WriteLine("(THelper)" + demote.name);
                                    demoteplayer.Close();
                                    p.SendMessage(c.lime + "Successfully demoted " + demote.name + " to THelper");
                                    demote.SendMessage(c.lime + "You were demoted to THelper");
                                    return;
                                }
                                if (find2[i] == "(THelper)" + demote.name)
                                {
                                    find2.Remove(find2[i]);
                                    File.WriteAllLines("teams/" + todo + ".txt", find2.ToArray());
                                    StreamWriter demoteplayer = File.AppendText("Teams/" + todo + ".txt");
                                    demoteplayer.WriteLine("(TMember)" + demote.name);
                                    demoteplayer.Close();
                                    p.SendMessage(c.lime + "Successfully demoted " + demote.name + " to TMember");
                                    demote.SendMessage(c.lime + "You were demote to TMember");
                                    return;
                                }
                            }
                            Player.SendMessage(p, c.red + "Cannot demote " + demote.name + " further");
                            return;
                        case 5:
                            for (int i = 0; i < find2.Count; i++)
                            {
                                if (find2[i] == "(THelper)" + demote.name)
                                {
                                    find2.Remove(find2[i]);
                                    File.WriteAllLines("teams/" + todo + ".txt", find2.ToArray());
                                    StreamWriter demoteplayer = File.AppendText("Teams/" + todo + ".txt");
                                    demoteplayer.WriteLine("(TMember)" + demote.name);
                                    demoteplayer.Close();
                                    p.SendMessage(c.lime + "Successfully demoted " + demote.name + " to TMember");
                                    demote.SendMessage(c.lime + "You were demote to TMember");
                                    return;
                                }
                            }
                            Player.SendMessage(p, c.red + "Cannot demote " + demote.name + " further");
                            return;
                        default:
                            Player.SendMessage(p, "You need to be TCaptain or higher to use this");
                            return;
                    }
                    /*
                case "list":
                    string cutted = "";
                    Player.SendMessage(p,"Available Teams:");
                    string allFiles = "";
                    foreach (FileInfo fi in teamlist.GetFiles("*.txt"))
                    {
                        cutted = fi.Name.Substring(0, 3);
                        allFiles += cutted + ",";
                    }
                    Player.SendMessage(p, allFiles);
                    return;*/
                case "members":
                    if (!File.Exists("Teams/" + todo + ".txt"))
                    {
                        Player.SendMessage(p, c.red +  "Could not find the Team.");
                    }
                    else
                    {
                        string members = "";
                        using (StreamReader wacha = new StreamReader("Teams/" + todo + ".txt"))
                        {
                            string[] lines = File.ReadAllLines("Teams/" + todo + ".txt");
                            foreach (string line in lines)
                            {
                                members += line + ",";   
                            }
                            Player.SendMessage(p, members);
                        }
                    }
                    return;
                case "leader":
                    Player newleader = Player.Find(todo);
                    Player oldleader = p;
                    if (newleader != null)
                    {
                        todo = p.teamname;
                        if (todo != newleader.teamname) { Player.SendMessage(p, c.red + "Can only promote someone from the same team"); return; }
                        if (Permissioncheck(p.name, todo) == 10)
                        {
                            List<string> l = new List<string>(File.ReadAllLines("teams/" + todo + ".txt"));
                            for (int i = 0; i < l.Count; i++)
                            {
                                if (l[i] == "(TMember)" + newleader.name
                                   || l[i] == "(THelper)" + newleader.name
                                   || l[i] == "(TCaptain)" + newleader.name)
                                {
                                    l.Remove(l[i]);
                                    l.Add("(TLeader)"+newleader.name);
                                    File.WriteAllLines("teams/" + todo + ".txt", l.ToArray());
                                    newleader.SendMessage(c.lime + "You have been promoted to TLeader for team %a" + todo);
                                    Player.SendMessage(p, "You have successfully promoted %a" + newleader.name + Server.DefaultColor + " to TLeader");
                                }
                                if (l[i] == "(TLeader)" + oldleader.name)
                                {
                                    l.Remove(l[i]);
                                    l.Add("(TCaptain)" + oldleader.name);
                                    File.WriteAllLines("teams/" + todo + ".txt", l.ToArray());
                                }
                            }
                            return;
                        }
                        Player.SendMessage(p, c.red + "You need to be TLeader in order to hand over your team");
                        return;
                    }
                    else
                    {
                        string toleader = todo;
                        todo = p.teamname;
                        bool found = false;
                        if (Permissioncheck(p.name, todo) == 10)
                        {
                            List<string> l = new List<string>(File.ReadAllLines("teams/" + todo + ".txt"));
                            for (int i = 0; i < l.Count; i++)
                            {
                                if (l[i] == "(TMember)" + toleader
                                   || l[i] == "(THelper)" + toleader
                                   || l[i] == "(TCaptain)" + toleader)
                                {
                                    l.Remove(l[i]);
                                    l.Add("(TLeader)" + toleader);
                                    File.WriteAllLines("teams/" + todo + ".txt", l.ToArray());
                                    Player.SendMessage(p, "You have successfully promoted %a" + toleader + Server.DefaultColor + " to TLeader");
                                    found = true;
                                }
                            }
                            if (!found) Player.SendMessage(p, c.red + "Player " + toleader + " is not in your team");
                            else
                            {
                                List<string> a = new List<string>(File.ReadAllLines("teams/" + todo + ".txt"));
                                for (int i = 0; i < a.Count; i++)
                                {
                                    if (a[i] == "(TLeader)" + oldleader.name)
                                    {
                                        a.Remove(a[i]);
                                        a.Add("(TCaptain)" + oldleader.name);
                                        File.WriteAllLines("teams/" + todo + ".txt", a.ToArray());
                                    }
                                }
                            }
                            return;
                        }
                        Player.SendMessage(p, c.red + "You need to be TLeader in order to hand over your team");
                        return;
                    }
                case "":
                case " ":
                    Help(p);
                    return;
                default:
                    if (p.teamname == "") { Player.SendMessage(p,"You are not in a team"); return; }
                    Player.players.ForEach(delegate(Player tosent)
                    {
                        if (p.teamname == tosent.teamname)
                        {
                            tosent.SendMessage(c.purple + "- to team - " +  p.group.color + p.name + ":" + c.white + " " + message); 
                        }
                    });
                    return;
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/team <yourmessage> - sends a message to all teammembers");
            Player.SendMessage(p, "/team create <teamname> - creates a new team - costs 200");
            Player.SendMessage(p, "/team join - joins a team when invited");
            Player.SendMessage(p, "/team decline - decline a team when invited");
            Player.SendMessage(p, "/team del - deletes your team, no money will be refunded");
            Player.SendMessage(p, "/team kick <teammember> - kicks a teammember out of a team");
            Player.SendMessage(p, "/team leave - leaves your current team");
            Player.SendMessage(p, "/team invite <playername> - invites a player to your team");
            Player.SendMessage(p, "/team promote/demote <playername> - promotes/demote a player in your team");
            Player.SendMessage(p, "/team leader <playername> - gives your team away");
            Player.SendMessage(p, "/team members <teamname> - shows players inside a team");
            Player.SendMessage(p, "/team color <colorname> - buys a rank color for your team - costs 150 cookies");
        }
    }
}