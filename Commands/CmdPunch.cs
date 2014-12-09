using System;

namespace MCForge
{
    public class CmdPunch : Command
    {
        public override string name { get { return "punch"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            Player who = Player.Find(message);

            if (p != null && who.group.Permission > p.group.Permission) { Player.SendMessage(p, "Cannot punch someone of greater rank"); return; }


            if (who == null)
            {
                Player.SendMessage(p, "Could not find player specified");
                return;
            }

            ushort currentX = (ushort)(who.pos[0] / 32);
            ushort currentY = (ushort)(who.pos[1] / 32);
            ushort currentZ = (ushort)(who.pos[2] / 32);
            ushort foundDirection = 0;

            for (ushort xx = currentX; xx <= 1000; xx++)
            {
                if (!Block.Walkthrough(p.level.GetTile(currentY, xx, currentZ)) && p.level.GetTile(currentY, xx, currentZ) != Block.Zero)
                {
                    foundDirection = (ushort)(xx - 1);
                    who.level.ChatLevel(who.color + who.name + Server.DefaultColor + " was punched into the wall by " + p.color + p.name);
                    break;
                }
            }

            if (foundDirection == 0)
            {
                who.level.ChatLevel(who.color + who.name + Server.DefaultColor + " was punched across the map by " + p.color + p.name);
                foundDirection = 1000;
            }

            unchecked { who.SendPos((byte)-1, (ushort)(foundDirection * 32), who.pos[1], who.pos[2], who.rot[1], who.rot[1]); }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/punch <name> - Punches <name>, knocking them into the wall.");
        }
    }
}
