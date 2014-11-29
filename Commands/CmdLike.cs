using System;


namespace MCForge.Commands
{
    public class CmdLike : Command
    {
        public override string name { get { return "like"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdLike() { }
        public override void Use(Player p, string message)
        {
            Server.VoteForLevel(true, p);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/like - likes a level");
        }
    }
}