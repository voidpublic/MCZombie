
/*
    Copyright 2012 by void_public, MCForge Member
    You must give credit to the original author even when you edit the code.
    You may alter, edit or build new things on this
    You may not use this work for commercial purposes.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Commands
{
    public class CmdBuy : Command
    {
        public override string name { get { return "buy"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdBuy() { }
        public override void Use(Player p, string message)
        {
            string item;
            int price = 0;
            if (message == "")
            {
                Help(p);
                return;
            }
            if (p == null)
            {
                Player.SendMessage(p, c.red + "Consoles cant do that");
                return;
            }
            else if (message.Split(' ').Length < 2)
            {
                //------------------------------------------revive-----------------------------------------------------
                item = message.Split(' ')[0];
                if (item == "revive" && Server.buyableitems.Contains(item))
                {
                    price = Server.itemprices[Server.buyableitems.IndexOf(item)];
                    if (p.referee) { Player.SendMessage(p,c.red + "Referees cant do that"); return; }
                    else if (!Server.zombie.GameInProgess()) { Player.SendMessage(p, c.red + "No zombie game running at the moment"); return; }
                    else if (!p.infected) { Player.SendMessage(p, c.red + "You are a human already"); return; }
                    else if (Convert.ToInt32(Server.zombie.GetTimeLeft("minutes")) < 2) { Player.SendMessage(p, c.red + "Its too late to buy a revive"); return; }
                    else if (!p.EnoughMoney(price)) { Player.SendMessage(p, c.red + "You havent got " + price + " " + Server.moneys + " to buy " + item); return; }
                    else if ((DateTime.Now - p.infecttime).TotalMinutes > 1) { Player.SendMessage(p, c.red + "You can only revive 1 minute after you got infected"); return; }
                    else if (!p.canrevive) { Player.SendMessage(p, c.red + "Only 1 revive potion per player allowed in 1 round"); return; }
                    else if (ZombieGame.infectd.Count < 3) { Player.SendMessage(p, c.red + "A cure has not been found yet"); return; }
                    else
                    {
                        p.money -= price;
                        int chance = new Random().Next(0, 11);
                        switch (chance)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                p.canrevive = true; break;
                            case 9:
                            case 10:
                                p.canrevive = false; break;
                        }
                        if (p.canrevive)
                        {
                            Player.GlobalMessage(c.white + p.name + "%a used a revive potion - it was very effective");
                            Player.SendMessage(p, c.lime + "Congratulations, you just gave yourself a revive potion");
                            Server.zombie.DisinfectPlayer(p);
                            p.canrevive = false;
                            return;
                        }
                        else
                        {
                            Player.GlobalMessage(p.name + c.red + " tried to revive himself, but the potion was not effective");
                            Player.SendMessage(p, c.red + "The revive potion had no effect....");
                            Player.SendMessage(p, "for a price of: " + c.lime + price + " " + Server.DefaultColor + Server.moneys);
                            return;
                        }
                    }
                }
                //------------------------------------------rankup-----------------------------------------------------
                else if (item == "rankup" && Server.buyableitems.Contains(item))
                {
                    price = Server.itemprices[Server.buyableitems.IndexOf(item)];
                    if (!p.EnoughMoney(price)) { Player.SendMessage(p, c.red + "You havent got " + price + " " + Server.moneys + " to buy " + item); return; }
                    else if (Server.buyableranks.Contains(p.group.name))
                    {
                        p.money -= price;
                        Command.all.Find("promote").Use(null, p.name);
                        return;
                    }
                    else { Player.SendMessage(p, c.red + "You cant buy a higher rank"); Player.SendMessage(p, c.aqua + "Please visit %b$website in order to get a higher rank"); return; }
                }
                else if (item == "invisibility" && Server.buyableitems.Contains(item))
                {
                    price = Server.itemprices[Server.buyableitems.IndexOf(item)];
                    if (p.referee) { Player.SendMessage(p,c.red + "Referees cant do that"); return; }
                    else if (!Server.zombie.GameInProgess()) { Player.SendMessage(p,c.red + "No zombie game running at the moment"); return; }
                    else if (p.infected) { Player.SendMessage(p,c.red + "Zombies cant use that item"); return; }
                    else if (p.invisible) { Player.SendMessage(p,c.red + "You are already invisible"); return; }
                    else if (p.invisiblyused > 6) { Player.SendMessage(p, c.red + "No more invisibility potions left"); return; }
                    else if (Convert.ToInt32(Server.zombie.GetTimeLeft("minutes")) < 1) { Player.SendMessage(p, c.red + "Its too late to buy invisibility"); return; }
                    else if (!p.EnoughMoney(price + p.invisiblyused * 2)) { Player.SendMessage(p, c.red + "You havent got " + price + " " + Server.moneys + " to buy " + item); return; }
                    else
                    {
                        p.money = p.money - price - p.invisiblyused * 2;
                        if (p.invisiblyused == 5)
                            p.Achieve("Ghost");
                        Player.GlobalMessage(p.group.color + p.name + c.green + " just disappeared. POOF");
                        if (p.invisiblyused == 6) Player.SendMessage(p, c.yellow + "Warning, you have used your last invisibility potion");
                        System.Timers.Timer invisibilitytimer = new System.Timers.Timer(1000);
                        p.invisible = true;
                        Player.GlobalDie(p, false);
                        foreach (Player findref in Player.players)
                        {
                            if (findref.referee)
                            {
                                findref.SendSpawn(p.id, p.name, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1]);
                            }
                        }
                        p.invisiblyused++;
                        int count = 0;
                        invisibilitytimer.Start();
                        invisibilitytimer.Elapsed += delegate
                        {
                            count++;
                            if (count != 5 && count != 1)
                                Player.SendMessage(p, c.red + "Invisibility ends in %b" + (5 - count));
                            if (count == 5)
                            {
                                Player.SendMessage(p, c.red + "You are %bvisible %cagain");
                                Player.GlobalDie(p, false);
                                Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                                p.invisible = false;
                                invisibilitytimer.Stop();
                            }
                            if (p.infected)
                            {
                                Player.SendMessage(p, c.red + "You got infected, you are %bvisible %cagain");
                                invisibilitytimer.Stop();
                                p.invisible = false;
                                Player.Find(p.infectedfrom).Achieve("The Sixth Sense");
                                return;
                            }
                        };
                        return;
                    }

                }
                else { Help(p); return; }
            }
            item = message.Split(' ')[0];

            int pos = message.IndexOf(' ');
            string wanted = message.Substring(pos + 1);
            //------------------------------------------title-----------------------------------------------------------
            if (item == "title" && Server.buyableitems.Contains(item))
            {
                price = Server.itemprices[Server.buyableitems.IndexOf(item)];
                if (!p.EnoughMoney(price)) { Player.SendMessage(p,"%cYou havent got " + price + " " + Server.moneys + " to buy " + item); return; }
                else if (wanted.Length > 13) { Player.SendMessage(p,"%cToo long titlename, maximum is 13"); return; }
                else if (wanted.Contains("%") || wanted.Contains("&") || wanted.Contains("$")) { Player.SendMessage(p,"%cUnallowed chars like % or & or $ in title name"); return; }
                else
                {
                    Command.all.Find("title").Use(null, p.name + " " + wanted);
                    p.money -= price;
                    Player.SendMessage(p,"Congratulations, you have just purchased the title: " +c.aqua + wanted);
                    Player.SendMessage(p,"for a price of: " + c.green + price + " " + Server.DefaultColor + Server.moneys);
                    return;
                }
            }
            //------------------------------------------tcolor--------------------------------------------------------
            else if (item == "tcolor" && Server.buyableitems.Contains(item))
            {
                price = Server.itemprices[Server.buyableitems.IndexOf(item)];
                string color = c.Parse(wanted);
                if (!p.EnoughMoney(price)) { Player.SendMessage(p,"%cYou havent got " + price + " " + Server.moneys + " to buy " + item); return; }
                else if (p.title == "") { Player.SendMessage(p,"%cYou havnt got a title yet"); return; }
                else if (color == "") { Player.SendMessage(p,"%cColor not found. Try /help color"); return; }
                else
                {
                    Command.all.Find("tcolor").Use(null, p.name + " " + wanted);
                    p.money -= price;
                    Player.SendMessage(p,"Congratulations, you have just purchased the title color: " + c.aqua + wanted);
                    Player.SendMessage(p,"for a price of: " + c.lime + price + " " + Server.DefaultColor + Server.moneys);
                    return;
                }
            }
            //-------------------------------------------Blocks--------------------------------------------------
            else if (item == "10blocks" && Server.buyableitems.Contains(item))
            {
                price = Server.itemprices[Server.buyableitems.IndexOf(item)];
                int amount = 0;
                try { amount = Convert.ToInt32(wanted); }
                catch { Player.SendMessage(p,"%cNo valid amount"); return; }
                if (p.referee) { Player.SendMessage(p,"%cReferees cant do that"); return; }
                else if (!Server.zombie.GameInProgess()) { Player.SendMessage(p,"%cNo zombie game running at the moment"); return; }
                else if (p.infected) { Player.SendMessage(p,"%cZombies cant buy extra blocks"); return; }
                else if (!p.EnoughMoney(amount)) { Player.SendMessage(p,"%cYou havent got " + (amount * price) + " " + Server.moneys + " to buy " + amount * 10 + " " + item); return; }
                else
                {
                    p.blockCount += (amount * 10);
                    p.money -= amount * price;
                    Player.SendMessage(p,"Congratulations, you have just purchased: " + c.aqua + amount * 10 + "%e extra blocks");
                    Player.SendMessage(p,"for a price of: " + c.lime + (amount * price) + " " Server.DefaultColor + Server.moneys);
                    Player.SendMessage(p,"Your total blocks left are now: "+ c.aqua + p.blockCount);
                    return;
                }
            }
            //--------------------------------------------loginmsg--------------------------------------------------------------
            else if (item == "loginmsg" && Server.buyableitems.Contains(item))
            {
                price = Server.itemprices[Server.buyableitems.IndexOf(item)];
                //int pos = message.IndexOf(' ');
                //wanted = message.Substring(pos + 1);
                if (wanted.Length > 25) { Player.SendMessage(p,"%cThe loginmessage is too long"); return; }
                else if (wanted.Length < 2) { Player.SendMessage(p,"%cThe loginmessage is too short"); return; }
                else if (!p.EnoughMoney(price)) { Player.SendMessage(p,"%cYou havent got " + price + " " + Server.moneys + " to buy " + item); return; }
                else
                {
                    p.money -= price;
                    Command.all.Find("loginmessage").Use(null, p.name + " " + wanted);
                    Player.SendMessage(p,"Congratulations, you have just purchased the loginmsg: "+ c.aqua + wanted);
                    Player.SendMessage(p,"for a price of: " +c.lime + price + " " + Server.DefaultColor + Server.moneys);
                    return;
                }
            }
            //--------------------------------------------logoutmsg-----------------------------------------------------
            else if (item == "logoutmsg" && Server.buyableitems.Contains(item))
            {
                price = Server.itemprices[Server.buyableitems.IndexOf(item)];
                //int pos = message.IndexOf(' ');
                //wanted = message.Substring(pos + 1);
                if (wanted.Length > 25) { Player.SendMessage(p,"%cThe logoutmessage is too long"); return; }
                else if (wanted.Length < 2) { Player.SendMessage(p,"%cThe logoutmessage is too short"); return; }
                else if (!p.EnoughMoney(price)) { Player.SendMessage(p,"%cYou havent got " + price + " " + Server.moneys + " to buy " + item); return; }
                else
                {
                    p.money -= price;
                    Command.all.Find("logoutmessage").Use(null, p.name + " " + wanted);
                    Player.SendMessage(p,"Congratulations, you have just purchased the logoutmsg:" + c.aqua + wanted);
                    Player.SendMessage(p,"for a price of: " + c.lime + price + " " + Server.DefaultColor + Server.moneys);
                    return;
                }
            }
            //--------------------------------------------queuelevel-----------------------------------------------------
            else if (item == "queuelevel" && Server.buyableitems.Contains(item))
            {
                price = Server.itemprices[Server.buyableitems.IndexOf(item)];
                //int pos = message.IndexOf(' ');
                //wanted = message.Substring(pos + 1);
                if (Server.queLevel)
                {
                    Player.SendMessage(p, "%cSorry there is already a level queued");
                    return;
                }
                else if (Server.zombie.currentLevelName == wanted){ Player.SendMessage(p, "%cYou are not allowed to queue a level that is played on "); return; }
                else if (!p.EnoughMoney(price)) { Player.SendMessage(p, "%cYou havent got " + price + " " + Server.moneys + " to buy " + item); return; }
                if (ZombieGame.levelsplayed.Contains(wanted)) { Player.SendMessage(p, c.red + "Sorry level has been played within the last 20 rounds"); return; }
                else if (!Server.zombieRound) { Player.SendMessage(p, "%cCan only be purchased during round"); return; }
                else
                {
                    bool yes = false;
                    DirectoryInfo di = new DirectoryInfo("levels/");
                    FileInfo[] fi = di.GetFiles("*.lvl");
                    foreach (FileInfo file in fi)
                    {
                        if (file.Name.Replace(".lvl", "").ToLower().Equals(wanted.ToLower()))
                        {
                            yes = true;
                        }
                    }
                    if (yes)
                    {
                        Server.queLevel = true;
                        Server.nextLevel = wanted.ToLower();
                        p.money -= price;
                        p.Achieve("Wishes");
                        Player.GlobalMessage(p.group.color + p.name + c.aqua + " has just queued the level: " + wanted);
                        return;
                    }
                    else
                    {
                        Player.SendMessage(p, c.red + "Level " + wanted + " does not exist.");
                        return;
                    }
                }

            }
            else
            {
                Help(p);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/buy <item> <name/amount> buys the item.");
            Player.SendMessage(p, c.aqua + "Possible items:");
            int count = 0;
            foreach (string item in Server.buyableitems)
            {
                Player.SendMessage(p, String.Format("{0,-10}..............{1,-5}", c.gold + item + c.white, c.lime + Server.itemprices[count] + " " + c.white + Server.moneys));
                count++;
            };
        }
    }
}
