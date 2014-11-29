using System;


namespace MCForge.Commands
{
    public class CmdDislike : Command
    {
        public override string name { get { return "dislike"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdDislike() { }
        public override void Use(Player p, string message)
        {
            Server.VoteForLevel(false, p);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/dislike - dislikes a level");
        }
    }
}