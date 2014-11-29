using System;
using MCForge.SQL;
using System.Data;

namespace MCForge.Commands
{
    public class CmdAchievements : Command
    {
        public override string name { get { return "achievements"; } }
        public override string shortcut { get { return "awards"; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdAchievements() { }

        public override void Use(Player p, string message)
        {
            
            Player who = null;
            if (message == "")
                who = p;
            else
                who = Player.Find(message);
            if (who == null)
            {
                Player.SendMessage(p, c.red + "Cannot find player: " + message);
            }
            else
            {
                DataTable achievement = Database.fillData("SELECT * FROM Achievements;");
                Player.SendMessage(p, "Showing achievements for: " + who.name);
                int number = achievement.Rows.Count;
                for (int i = 0; i < number; i++)
                {
                    string prefix = "";
                    string name = achievement.Rows[i]["name"].ToString();
                    string description = achievement.Rows[i]["description"].ToString();
                    if (who.achievements.Contains(name))
                        prefix = "%a";
                    else
                        prefix = "%c";
                    Player.SendMessage(p, prefix + "[" + (i + 1).ToString() + "]" + name + " : " + description);
                }
            }

        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/achievements - Shows your achievements.");
        }
    }
}