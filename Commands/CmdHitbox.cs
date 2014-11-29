using System;

namespace MCForge.Commands
{
    public class CmdHitbox : Command
    {
        public override string name { get { return "hitbox"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return ""; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdHitbox() { }
        public override void Use(Player p, string message)
        {
            
            string variable = "";
            int value = 0;
            int pos = message.IndexOf(' ');
            value = Convert.ToInt32(message.Substring(pos + 1));
            variable = message.Split(' ')[0];
            if (variable == "show")
            {
                Player.SendMessage(p, "set values are:");
                Player.SendMessage(p, "time: " + Server.hitboxtime);
                Player.SendMessage(p, "rangex: " + Server.hitboxrangex);
                Player.SendMessage(p, "rangey: " + Server.hitboxrangey);
                Player.SendMessage(p, "rangez: " + Server.hitboxrangez);
            }
            if (variable == "time")
            {
                Server.hitboxtime = value;
                Player.SendMessage(p, "SET!");
            }
            if (variable == "rangex")
            {
                Server.hitboxrangex = value;
                Player.SendMessage(p, "SET!");
            }
            if (variable == "rangey")
            {
                Server.hitboxrangey = value;
                Player.SendMessage(p, "SET!");
            }
            if (variable == "rangeyz")
            {
                Server.hitboxrangez = value;
                Player.SendMessage(p, "SET!");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/hitbox - make changes to hitbox (debug command!!!)");
            Player.SendMessage(p, "/hitbox variable value");
            Player.SendMessage(p, "variables: time, rangex, rangey, rangez");
            Player.SendMessage(p, "defaults: 700, 32, 64, 32");
        }
    }
}