using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MCForge.Commands
{
    public class CmdShop : Command
    {
        public override string name { get { return "shop"; } }
        public override string shortcut { get { return "item"; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdShop() { }
        public override void Use(Player p, string message)
        {
            int price = 0;
            Player.SendMessage(p,"Information to the item: %a" + message);
            if (message == "title")
            {
                Player.SendMessage(p,"Syntax is:%a /buy title <titelname>");
                Player.SendMessage(p,"Maximal Length:%a 13 letters");
                Player.SendMessage(p,"Make sure it is %anot offensive");
                price = Server.itemprices[0];
            }
            else if (message == "tcolor")
            {
                Player.SendMessage(p,"Syntax is:%a /buy tcolor <colorname>");
                Player.SendMessage(p,"Make sure you got an %aaviable color%e with /help color");
                Player.SendMessage(p,"You have to own a title to buy a color for it");
                price = Server.itemprices[1];
            }
            else if (message == "revive")
            {
                Player.SendMessage(p,"Syntax is:%a /buy revive");
                Player.SendMessage(p,"Brings you back to life %a CARE not 100%");
                Player.SendMessage(p,"%aThe more often its used in one round, the more the chance drops to be revived");
                Player.SendMessage(p, "Only available when timeleft is bigger than 2 minutes");
                Player.SendMessage(p, "Can only revive when you were infected less than 1 minute ago");
                Player.SendMessage(p, "Shortcut is /y");
                price = Server.itemprices[2];
            }
            else if (message == "blocks")
            {
                Player.SendMessage(p,"Syntax is:%a /buy blocks <amount>*10");
                Player.SendMessage(p,"Only works for %aactual round");
                Player.SendMessage(p,"Does only work for %ahumans");
                price = Server.itemprices[3];
            }
            else if (message == "rankup")
            {
                Player.SendMessage(p,"Syntax is:%a /buy rankup");
                Player.SendMessage(p,"Gives you a higher rank, but might not give you extra commands");
                price = Server.itemprices[4];
            }
            else if (message == "loginmsg")
            {
                Player.SendMessage(p,"Syntax is:%a /buy loginmsg <message>");
                Player.SendMessage(p,"Sets a message, that will be showed when you %alogin");
                Player.SendMessage(p,"Minimum Length:%a 2 letters");
                Player.SendMessage(p,"Maximal Length:%a 25 letters");
                price = Server.itemprices[5];
            }
            else if (message == "logoutmsg")
            {
                Player.SendMessage(p,"Syntax is:%a /buy logoutmsg <message>");
                Player.SendMessage(p,"Sets a message, that will be showed when you %alogout");
                Player.SendMessage(p,"Minimum Length:%a 2 letters");
                Player.SendMessage(p,"Maximal Length:%a 25 letters");
                price = Server.itemprices[6];
            }/*
            else if (message == "invisibility")
            {
                Player.SendMessage(p,"Syntax is:%a /buy invisibility");
                Player.SendMessage(p,"Makes you invisible for 8 sec");
                Player.SendMessage(p,"You can still get infected");
                Player.SendMessage(p,"Only available when timeleft is bigger than 1 minute");
                Player.SendMessage(p,"Only available for humans");
                Player.SendMessage(p,"Shortcut is /w");
                price = Server.itemprices[7];
            }*/
            else if (message == "queuelevel")
            {
                Player.SendMessage(p, "Syntax is:%a /buy queuelevel <mapname>");
                Player.SendMessage(p, "The next zombie map will be <mapname>");
                price = Server.itemprices[7];
            }
            else { Help(p); return; }
            Player.SendMessage(p, "The price is: %a"+ price + "%e "+ Server.moneys);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/shop <item> show information to that item");
            Player.SendMessage(p, c.aqua + "Possible items:");
            int count = 0;
            foreach (string item in Server.buyableitems)
            {
                Player.SendMessage(p, String.Format("{0,-10}..............{1,-5}",c.gold + item + c.white,c.lime + Server.itemprices[count] +" " + c.white + Server.moneys));
                count++;
            };
        }
    }
}
