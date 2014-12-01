
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCForge.Commands
{
    public class CmdLottery : Command
    {
        public override string name { get { return "lottery"; } }
        public override string shortcut { get { return "luck"; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdLottery() { }
        public override void Use(Player p, string message)
        {
            if (p == null)
                Player.SendMessage(p, c.red + "Consoles cant do that derp");
            /*else if (!Server.zombieRound)
                Player.SendMessage(p, "Can only be used while round is going on");*/
            else if (Server.zombie.lottery.Contains(p))
                Player.SendMessage(p, c.red + "You already are in the lottery with " + Server.zombie.lotterycount + " people");
            else if (p.EnoughMoney(10))
            {
                Server.zombie.lotterycount++;
                Server.zombie.lottery.Add(p);
                p.money -= 10;
                Player.SendMessage(p, c.lime + "Succesfully joined the lottery. The winner will be chosen at the end of the round");
                Player.GlobalMessage(p.group.color + p.name + Server.DefaultColor + " has joined the lottery");

            }
            else
            {
                Player.SendMessage(p, c.red + "Sorry you dont have enough money to join the lottery");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/lottery - joins the lottery for 10 cookies");
            Player.SendMessage(p, "If you leave/get kicked you wont be refunded any money");
        }
    }
}