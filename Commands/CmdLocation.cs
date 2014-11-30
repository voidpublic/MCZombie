using System;
using MCForge.SQL;
using System.Data;
namespace MCForge.Commands
{
    public class CmdLocation : Command
    {
        public override string name { get { return "location"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "headop"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdLocation() { }
        public override void Use(Player p, string message)
        {
            Player who = null;
            string searchip = "";
            who = Player.Find(message);
            if (who == null)
            {
                Player.SendMessage(p, c.red + "Could not find player " + message + ".. searching in database");
                Database.AddParams("@Name", message);
                DataTable playerDb = Database.fillData("SELECT * FROM Players WHERE Name=@Name");
                if (playerDb.Rows.Count == 0)
                {
                    Player.SendMessage(p, c.red + "Could not find player at ALL");
                    return;
                }
                else
                    searchip = (string)playerDb.Rows[0]["IP"];
            }
            else
                searchip = who.ip;
            if (Player.IPInPrivateRange(searchip))
            {
                Player.SendMessage(p, c.red + "Player has an internal IP, cannot trace");
                return;
            }
            Player.SendMessage(p, c.lime + "Ip " + c.aqua + searchip + c.lime + " has been traced to: " + c.aqua + Player.GetIPLocation(searchip));
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/location <name> - Tracks down the location of an IP.");
        }
    }
}