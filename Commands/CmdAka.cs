using System;
using System.IO;

namespace MCForge.Commands
{
    /// <summary>
    /// This is the command /aka
    /// use /help aka in-game for more info
    /// </summary>
    public class CmdAka : Command
    {
        public override string name { get { return "aka"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "player"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdAka() { }
        public override void Use(Player p, string message)
        {
            if (p == null)
            {
                Player.SendMessage(p, "Consoles cant do that derp");
            }
            if (message == "") message = p.name;
            
            if (!p.aka)
            {
                Player.SendMessage(p,"Aka is now %aON");
                p.aka = true;
                ushort x = (ushort)((p.pos[0]));
                ushort y = (ushort)((p.pos[1]));
                ushort z = (ushort)((p.pos[2]));

                p.Loading = true;
                foreach (Player pl in Player.players) if (p.level == pl.level && p != pl) p.SendDie(pl.id);

                Player.GlobalDie(p, true);
                p.SendUserMOTD(); 
                p.SendMap();

                if (!p.hidden && !p.referee && !p.invisible)
                {
                    Player.GlobalDie(p, false);
                    Player.GlobalSpawn(p, x, y, z, p.level.rotx, p.level.roty, true);
                }
                else unchecked { p.SendPos((byte)-1, x, y, z, p.level.rotx, p.level.roty); }

                foreach (Player pl in Player.players)
                {
                    if (pl.level == p.level && p != pl && !pl.hidden && !pl.referee && !pl.invisible)
                        p.SendSpawn(pl.id, pl.color + pl.name, pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1]);
                }
                p.Loading = false;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            else if (p.aka)
            {
                Player.SendMessage(p,"Aka is now %cOFF");
                p.aka = false;
                ushort x = (ushort)((p.pos[0]));
                ushort y = (ushort)((p.pos[1]));
                ushort z = (ushort)((p.pos[2]));

                p.Loading = true;
                foreach (Player pl in Player.players) if (p.level == pl.level && p != pl) p.SendDie(pl.id);
                Player.GlobalDie(p, true);
                p.SendUserMOTD(); p.SendMap();


                if (!p.hidden && !p.referee)
                {
                    Player.GlobalDie(p, false);
                    Player.GlobalSpawn(p, x, y, z, p.level.rotx, p.level.roty, true);
                }
                else unchecked { p.SendPos((byte)-1, x, y, z, p.level.rotx, p.level.roty); }

                foreach (Player pl in Player.players)
                    if (pl.level == p.level && p != pl && !pl.hidden && !pl.referee)
                        if (pl.infected)
                            p.SendSpawn(pl.id, c.red + Server.ZombieName, pl.pos[0], pl.pos[1] , pl.pos[2], pl.level.rotx, pl.level.roty);
                        else
                            p.SendSpawn(pl.id, pl.color + pl.name, pl.pos[0], pl.pos[1], pl.pos[2], pl.level.rotx, pl.level.roty);
                p.Loading = false;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/aka - Gives players normal names.");
        }
    }
}