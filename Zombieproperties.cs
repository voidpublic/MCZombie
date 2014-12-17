using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MCForge
{
    public static class Zombieproperties
    {
        public static void ZPropLoad(string givenPath)
        {
            if (File.Exists(givenPath))
            {
                Server.itemprices.Clear();
                Server.buyableranks.Clear();
                Server.buyableitems.Clear();
                try
                {
                    string[] lines = File.ReadAllLines(givenPath);

                    foreach (string line in lines)
                    {
                        if (line != "" && line[0] != '#')
                        {
                            string item = line.Split('=')[0].Trim();
                            string obj = "";
                            if (line.IndexOf('=') >= 0)
                                obj = line.Substring(line.IndexOf('=') + 1).Trim();
                            switch (item.ToLower())
                            {
                                case "blocklimithuman":
                                    try { Server.blocklimithuman = Convert.ToInt32(obj);}
                                    catch { Server.s.Log("invalid blocklimit-human"); Server.blocklimithuman = 55; }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "blocklimitzombie":
                                    try { Server.blocklimitzombies = Convert.ToInt32(obj); }
                                    catch { Server.s.Log("invalid blocklimit-zombies"); Server.blocklimitzombies = 3; }
                                    break;
                                //------------------------------------------------------------------------------------
                                /*case "round-time1":
                                    try { Server.roundtime1 = Convert.ToInt32(obj); }
                                    catch { Server.s.Log("invalid round time1"); Server.roundtime1 = 6; }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "round-time2":
                                    try { Server.roundtime2 = Convert.ToInt32(obj); }
                                    catch { Server.s.Log("invalid round time2"); Server.roundtime2 = 8; }
                                    break;*/
                                // roundtime is no longer in use as this is dynamic depending on winchance
                                //------------------------------------------------------------------------------------
                                case "zombie-name-while-infected":
                                    if (obj != "")
                                        Server.ZombieName = obj;
                                    break;
                                //------------------------------------------------------------------------------------
                                case "no-respawning-during-zombie":
                                    try { Server.noRespawn = bool.Parse(obj); }
                                    catch { Server.s.Log("error "+ item + " switch"); }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "no-pillaring-during-zombie":
                                    try { Server.noPillaring = bool.Parse(obj); }
                                    catch { Server.s.Log("error " + item + " switch"); }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "zombie-survival-only-server":
                                    try { Server.ZombieOnlyServer = bool.Parse(obj); }
                                    catch { Server.s.Log("error " + item + " switch"); }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "zombie-on-server-start":
                                    try { Server.startZombieModeOnStartup = bool.Parse(obj); }
                                    catch { Server.s.Log("error " + item + " switch"); }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "enable-changing-levels":
                                    try { Server.ChangeLevels = bool.Parse(obj); }
                                    catch { Server.s.Log("error " + item + " switch"); }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "human-prefix":
                                    if (obj != "")
                                        Server.humanprefix = obj;
                                    break;
                                //------------------------------------------------------------------------------------
                                case "zombie-prefix":
                                    if (obj != "")
                                        Server.zombieprefix = obj;
                                    break;
                                //------------------------------------------------------------------------------------
                                case "referee-prefix":
                                    if (obj != "")
                                        Server.refprefix = obj;
                                    break;
                                case "zombielevelpath":
                                    if (obj != "")
                                        Server.zombielevelpath = obj;
                                    break;
                                case "zombiedefaultlevelpath":
                                    if(obj != "")
                                        Server.zombiedefaultlevelpath = obj;
                                    break;
                                case "buildlevelpath":
                                    if(obj != "")
                                        Server.buildlevelpath = obj;
                                    break;
                                //------------------------------------------------------------------------------------
                                case "skipreference":
                                    try
                                    {
                                        if (obj != "" || obj != " ")
                                        {
                                            string input = obj.Replace(" ", "").ToString();
                                            int itndex = input.IndexOf("#");
                                            if (itndex > 0)
                                                input = input.Substring(0, itndex);
                                            Server.skipreference = input.Split(',').ToList<string>();
                                        }
                                    }
                                    catch { Server.s.Log("error buyableranks reading"); }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "title":
                                    try { if (obj.ToLower() == "true") Server.buyableitems.Add("title"); }
                                    catch { Server.s.Log("error " + item + " switch"); }
                                    break;
                                case "titlep":
                                    try
                                    {
                                        int temp = Convert.ToInt32(obj);
                                        if (Server.buyableitems.Contains("title"))
                                            Server.itemprices.Add(temp);
                                    }
                                    catch { Server.s.Log("invalid title price"); Server.itemprices.Add(400); }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "tcolor":
                                    try { if (obj.ToLower() == "true") Server.buyableitems.Add("tcolor"); }
                                    catch { Server.s.Log("error " + item + " switch"); }
                                    break;
                                case "tcolorp":
                                    try
                                    {
                                        int temp = Convert.ToInt32(obj);
                                        if (Server.buyableitems.Contains("tcolor"))
                                            Server.itemprices.Add(temp);
                                    }
                                    catch { Server.s.Log("invalid tcolor price"); Server.itemprices.Add(200); }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "revive":
                                    try { if (obj.ToLower() == "true") Server.buyableitems.Add("revive"); }
                                    catch { Server.s.Log("error " + item + " switch"); }
                                    break;
                                case "revivep":
                                    try
                                    {
                                        int temp = Convert.ToInt32(obj);
                                        if (Server.buyableitems.Contains("revive"))
                                            Server.itemprices.Add(temp);
                                    }
                                    catch { Server.s.Log("invalid revive price"); Server.itemprices.Add(7); }
                                    break;
                                case "revivesperround":
                                    try{Server.revivesperround = Convert.ToInt32(obj);}
                                    catch { Server.s.Log("invalid revivesperround"); Server.revivesperround = 1; }
                                    break;
                                case "revivetimeleft":
                                    try { Server.revivetimeleft = Convert.ToInt32(obj); }
                                    catch { Server.s.Log("invalid revivetimeleft"); Server.revivetimeleft = 2; }
                                    break;
                                case "revivetimeinfected":
                                    try { Server.revivetimeinfected = Convert.ToInt32(obj); }
                                    catch { Server.s.Log("invalid revivetimeinfected"); Server.revivetimeinfected = 1; }
                                    break;
                                case "reviveminimuminfected":
                                    try { Server.reviveminimuminfected = Convert.ToInt32(obj); }
                                    catch { Server.s.Log("invalid reviveminimuminfected"); Server.reviveminimuminfected = 1; }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "10blocks":
                                    try { if (obj.ToLower() == "true") Server.buyableitems.Add("10blocks"); }
                                    catch { Server.s.Log("error " + item + " switch"); }
                                    break;
                                case "10blocksp":
                                    try
                                    {
                                        int temp = Convert.ToInt32(obj);
                                        if (Server.buyableitems.Contains("10blocks"))
                                            Server.itemprices.Add(temp);
                                    }
                                    catch { Server.s.Log("invalid blocks price"); Server.itemprices.Add(1); }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "rankup":
                                    try { if (obj.ToLower() == "true") Server.buyableitems.Add("rankup"); }
                                    catch { Server.s.Log("error " + item + " switch"); }
                                    break;
                                case "rankupp":
                                    try
                                    {
                                        int temp = Convert.ToInt32(obj);
                                        if (Server.buyableitems.Contains("rankup"))
                                        Server.itemprices.Add(temp);
                                    }
                                    catch { Server.s.Log("invalid rankup price"); Server.itemprices.Add(300); }
                                    break;
                                case "buyableranks":
                                    try
                                    {
                                        if (obj != "" || obj != " ")
                                        {
                                            string input = obj.Replace(" ", "").ToString();
                                            int itndex = input.IndexOf("#");
                                            if (itndex > 0)
                                                input = input.Substring(0, itndex);
                                            Server.buyableranks = input.Split(',').ToList<string>();
                                        }
                                    }
                                    catch { Server.s.Log("error buyableranks reading"); }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "loginmsg":
                                    try { if (obj.ToLower() == "true") Server.buyableitems.Add("loginmsg"); }
                                    catch { Server.s.Log("error " + item + " switch"); }
                                    break;
                                case "loginmsgp":
                                    try
                                    {
                                        int temp = Convert.ToInt32(obj);
                                        if (Server.buyableitems.Contains("loginmsg"))
                                            Server.itemprices.Add(temp);
                                    }
                                    catch { Server.s.Log("invalid loginmsg price"); Server.itemprices.Add(300); }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "logoutmsg":
                                    try { if (obj.ToLower() == "true") Server.buyableitems.Add("logoutmsg"); }
                                    catch { Server.s.Log("error " + item + " switch"); }
                                    break;
                                case "logoutmsgp":
                                    try
                                    {
                                        int temp = Convert.ToInt32(obj);
                                        if (Server.buyableitems.Contains("logoutmsg"))
                                            Server.itemprices.Add(temp);
                                    }
                                    catch { Server.s.Log("invalid logoutmsg price"); Server.itemprices.Add(300); }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "invisibility":
                                    try { if (obj.ToLower() == "true") Server.buyableitems.Add("invisibility"); }
                                    catch { Server.s.Log("error " + item + " switch"); }
                                    break;
                                case "invisibilityp":
                                    try
                                    {
                                        int temp = Convert.ToInt32(obj);
                                        if (Server.buyableitems.Contains("invisibility"))
                                            Server.itemprices.Add(temp);
                                    }
                                    catch { Server.s.Log("invalid invisibility price"); Server.itemprices.Add(3); }
                                    break;
                                case "invisibilityduration":
                                    try { Server.invisibilityduration = Convert.ToInt32(obj); }
                                    catch { Server.s.Log("invalid invisibilityduration"); Server.invisibilityduration = 5; }
                                    break;
                                case "invisiblityperround":
                                    try { Server.invisiblityperround = Convert.ToInt32(obj); }
                                    catch { Server.s.Log("invalid invisiblityperround"); Server.invisiblityperround = 6; }
                                    break;
                                //------------------------------------------------------------------------------------
                                case "queuelevel":
                                    try { if (obj.ToLower() == "true") Server.buyableitems.Add("queuelevel"); }
                                    catch { Server.s.Log("error " + item + " switch"); }
                                    break;
                                case "queuelevelp":
                                    try
                                    {
                                        int temp = Convert.ToInt32(obj);
                                        if (Server.buyableitems.Contains("queuelevel"))
                                        Server.itemprices.Add(temp);
                                    }
                                    catch { Server.s.Log("invalid invisibility price"); Server.itemprices.Add(200); }
                                    break;
                                //------------------------------------------------------------------------------------
                            }
                        }
                    }
                    ZSave("properties/zombie.properties");
                }
                catch (Exception e) { Server.ErrorLog(e); }
            }
            else if(!File.Exists(givenPath))
            {
                File.Create("properties/zombie.properties").Dispose();
                ZSave("properties/zombie.properties");
                ZPropLoad("properties/zombie.properties");
            }
        }
        public static void ZSave(string givenPath)
        {
            try
            {
                File.Create(givenPath).Dispose();
                using (StreamWriter w = File.CreateText(givenPath))
                {
                    //if (givenPath.IndexOf("zombieproperties") != -1)
                    //{
                        w.WriteLine("#--------------------------------------------------------------------");
                        w.WriteLine("#General settings:");
                        w.WriteLine("");
                        w.WriteLine("blocklimithuman = " + Server.blocklimithuman);
                        w.WriteLine("blocklimitzombie= " + Server.blocklimitzombies);
                        //w.WriteLine("round-time1 = " + Server.roundtime1);
                        //w.WriteLine("round-time2 = " + Server.roundtime2);
                        //roundtime is dynamic depending on the winchance, therefor this is no longer in use
                        w.WriteLine("zombie-name-while-infected = " + Server.ZombieName);
                        w.WriteLine("no-respawning-during-zombie = " + Server.noRespawn);
                        w.WriteLine("no-pillaring-during-zombie = " + Server.noPillaring);
                        w.WriteLine("zombie-survival-only-server = " + Server.ZombieOnlyServer);
                        w.WriteLine("zombie-on-server-start = " + Server.startZombieModeOnStartup);
                        w.WriteLine("enable-changing-levels = " + Server.ChangeLevels);
                        w.WriteLine("human-prefix = " + Server.humanprefix);
                        w.WriteLine("zombie-prefix = " + Server.zombieprefix);
                        w.WriteLine("referee-prefix = " + Server.refprefix);
                        w.WriteLine("zombielevelpath = " + Server.zombielevelpath);
                        w.WriteLine("zombiedefaultlevelpath = " + Server.zombiedefaultlevelpath);
                        w.WriteLine("buildlevelpath = " + Server.buildlevelpath);
                        string skipreference = string.Join(",", Server.skipreference.ToArray());
                        w.WriteLine("skipreference = " + skipreference + "#must be comma seperated and no spaces");
                        w.WriteLine("");
                        w.WriteLine("#--------------------------------------------------------------------");
                        w.WriteLine("#Shop");
                        w.WriteLine("");
                        //------------------------------------------------------------------------------------------------
                        w.WriteLine("title = " + (Server.buyableitems.Contains("title") ? true : false));
                        try { w.WriteLine("titlep = " + Server.itemprices[Server.buyableitems.IndexOf("title")]); }
                        catch { w.WriteLine("titlep = 100"); }
                        //------------------------------------------------------------------------------------------------
                        w.WriteLine("tcolor = " + (Server.buyableitems.Contains("tcolor") ? true : false));
                        try { w.WriteLine("tcolorp = " + Server.itemprices[Server.buyableitems.IndexOf("tcolor")]); }
                        catch { w.WriteLine("tcolorp = 100"); }
                        //------------------------------------------------------------------------------------------------
                        w.WriteLine("revive = " + (Server.buyableitems.Contains("revive") ? true : false));
                        try { w.WriteLine("revivep = " + Server.itemprices[Server.buyableitems.IndexOf("revive")]); }
                        catch { w.WriteLine("revivep = 7"); }
                        try { w.WriteLine("revivesperround = " + Server.revivesperround.ToString()); }
                        catch { w.WriteLine("revivesperround = 1"); }
                        try { w.WriteLine("revivetimeleft = " + Server.revivetimeleft.ToString()); }
                        catch { w.WriteLine("revivetimeleft = 2"); }
                        try { w.WriteLine("revivetimeinfected = " + Server.revivetimeinfected.ToString()); }
                        catch { w.WriteLine("revivetimeinfected = 1"); }
                        try { w.WriteLine("reviveminimuminfected = " + Server.reviveminimuminfected.ToString()); }
                        catch { w.WriteLine("reviveminimuminfected = 3"); }
                        //------------------------------------------------------------------------------------------------
                        w.WriteLine("10blocks = " + (Server.buyableitems.Contains("10blocks") ? true : false));
                        try { w.WriteLine("10blocksp = " + Server.itemprices[Server.buyableitems.IndexOf("10blocks")]); }
                        catch { w.WriteLine("blocksp= 1"); }
                        //------------------------------------------------------------------------------------------------
                        w.WriteLine("rankup = " + (Server.buyableitems.Contains("rankup") ? true : false));
                        try { w.WriteLine("rankupp = " + Server.itemprices[Server.buyableitems.IndexOf("rankup")]); }
                        catch { w.WriteLine("rankupp = 100"); }
                        string buyableranks = string.Join(",", Server.buyableranks.ToArray());
                        w.WriteLine("buyableranks = " + buyableranks + "#must be comma seperated and no spaces");
                        //------------------------------------------------------------------------------------------------
                        w.WriteLine("loginmsg = " + (Server.buyableitems.Contains("loginmsg") ? true : false));
                        try { w.WriteLine("loginmsgp = " + Server.itemprices[Server.buyableitems.IndexOf("loginmsg")]); }
                        catch { w.WriteLine("loginnmsgp = 100"); }
                        //------------------------------------------------------------------------------------------------
                        w.WriteLine("logoutmsg = " + (Server.buyableitems.Contains("logoutmsg") ? true : false));
                        try { w.WriteLine("logoutmsgp = " + Server.itemprices[Server.buyableitems.IndexOf("logoutmsg")]); }
                        catch { w.WriteLine("logoutmsgp = 100"); }
                        //------------------------------------------------------------------------------------------------
                        w.WriteLine("invisibility = " + (Server.buyableitems.Contains("invisibility") ? true : false));
                        try { w.WriteLine("invisibilityp = " + Server.itemprices[Server.buyableitems.IndexOf("invisibility")]); }
                        catch { w.WriteLine("invisibilityp = 3"); }
                        try { w.WriteLine("invisibilityduration = " + Server.invisibilityduration.ToString()); }
                        catch { w.WriteLine("invisibilityduration = 5"); }
                        try { w.WriteLine("invisiblityperround = " + Server.invisiblityperround.ToString()); }
                        catch { w.WriteLine("invisiblityperround = 6"); }
                        //------------------------------------------------------------------------------------------------
                        w.WriteLine("queuelevel = " + (Server.buyableitems.Contains("queuelevel") ? true : false));
                        try { w.WriteLine("queuelevelp = " + Server.itemprices[Server.buyableitems.IndexOf("queuelevel")]); }
                        catch { w.WriteLine("queuelevelp = 200"); }
                    //}
                }
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }
    }
}
