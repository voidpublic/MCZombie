using System;


namespace MCForge.Commands
{
    /// <summary>
    /// This is the command /vote
    /// </summary>
    public class CmdVote : Command
    {
        public override string name { get { return "vote"; } }
        public override string shortcut { get { return "vo"; } }
        public override string type { get { return "headop"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdVote() { }

        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                if (!Server.voting && !Server.votingforlevel)
                {
                    Server.voting = true;
                    Server.NoVotes = 0;
                    Server.YesVotes = 0;
                    Player.GlobalMessage(c.aqua + "VOTE: " + message + Server.DefaultColor + " (" + c.green + "Yes " + Server.DefaultColor + "/" + c.red + "No" + Server.DefaultColor + ")");
                    System.Threading.Thread.Sleep(15000);
                    Server.voting = false;
                    int yespercent, nopercent;
                    yespercent = Server.YesVotes == 0 ? 0 : (Server.YesVotes * 100 / (Server.YesVotes + Server.NoVotes));
                    nopercent = Server.NoVotes == 0 ? 0 : (Server.NoVotes * 100 / (Server.YesVotes + Server.NoVotes));
                    Player.GlobalMessage(c.aqua + "Results: " + c.green + "Y: " + Server.YesVotes + " ( " + yespercent + "% )" + c.red + " N: " + Server.NoVotes + " ( " + nopercent + "% )");
                    Player.players.ForEach(delegate(Player winners)
                    {
                        winners.voted = false;
                    });
                }
                else
                {
                    Player.SendMessage(p,"A vote is in progress!");
                }
            }
            else
            {
                Help(p);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p,"/vote [message] - Obviously starts a vote!");
        }
    }
}