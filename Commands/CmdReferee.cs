using System;
using System.IO;


namespace MCForge.Commands
{
    /// <summary>
    /// This is the command /referee
    /// use /help referee in-game for more info
    /// </summary>
    public class CmdReferee : Command
    {
        public override string name { get { return "ref"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "operator"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdReferee() { }
        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game!"); return; }
            if (p.referee)
            {
                if (p.hidden)
                {
                    Player.SendMessage(p, "Hidden persons may not unref");
                    return;
                }
                p.referee = false;
                p.invincible = false;
                if (p.modeType != 0)
                {
                    Player.SendMessage(p, "&b" + Block.Name(p.modeType)[0].ToString().ToUpper() + Block.Name(p.modeType).Remove(0, 1).ToLower() + Server.DefaultColor + " mode: &cOFF");
                    p.modeType = 0;
                    p.BlockAction = 0;
                }
                p.modemode = false;
                //-------------------------------------------------------------------
                ushort x = (ushort)((p.pos[0]));
                ushort y = (ushort)((p.pos[1]));
                ushort z = (ushort)((p.pos[2]));
                p.SendUserMOTD();
                p.SendUserMOTD();
                p.SendMap();
                Player.GlobalDie(p, false);
                Player.GlobalSpawn(p, x, y, z, p.level.rotx, p.level.roty, true);
                //--------------------------------------------------------------------
                Player.SendMessage(p,"Referee mode is now %cOFF");
                LevelPermission perm = Group.findPlayerGroup(name).Permission;
                Player.GlobalDie(p, false);
                Player.GlobalChat(p, p.group.color + p.name + c.blue + " is no longer a referee", false);
                if (p.isFlying) p.isFlying = !p.isFlying;
                if (Server.zombie.GameInProgess())
                {
                    x = (ushort)((0.5 + p.level.spawnx) * 32);
                    y = (ushort)((1 + p.level.spawny) * 32);
                    z = (ushort)((0.5 + p.level.spawnz) * 32);
                    unchecked
                    {
                        p.SendPos((byte)-1, x, y, z, p.level.rotx, p.level.roty);
                    }
                    Server.zombie.InfectPlayer(p);
                }
                else
                {
                    Player.GlobalDie(p, false);
                    Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                    if(!ZombieGame.alive.Contains(p))ZombieGame.alive.Add(p);
                    p.color = p.group.color;
                    p.SetPrefix();
                }
            }
            else
            {
                p.referee = true;
                p.invincible = true;
                //---------------------------------------------------------------------
                ushort x = (ushort)((p.pos[0]));
                ushort y = (ushort)((p.pos[1]));
                ushort z = (ushort)((p.pos[2]));
                p.SendUserMOTD();
                p.SendUserMOTD();
                p.SendMap();
                Player.GlobalDie(p, false);
                Player.GlobalSpawn(p, x, y, z, p.level.rotx, p.level.roty, true);
                //-------------------------------------------------------------------------
                Player.SendMessage(p, "Referee mode is now " + c.lime + "ON");
                if(!p.hidden) Player.GlobalChat(p, p.group.color + p.name + c.blue + " is now a referee", false);
                if(!p.hidden) Player.GlobalDie(p, false);
                p.winstreakcount = 0;
                if (Server.zombie.GameInProgess())
                {
                    p.color = p.group.color;
                    p.infected = false;
                    p.SetPrefix();
                    bool debug = false;
                    try { ZombieGame.infectd.Remove(p); } catch { }
                    try { 
                        if(ZombieGame.alive.Contains(p)) debug = true;
                        ZombieGame.alive.Remove(p); 
                    }
                    catch { }
                    if (debug) Server.zombie.humangone();
                    if (ZombieGame.infectd.Count == 0) Server.zombie.firstinfectdc();
                }
                else
                {
                    try { ZombieGame.infectd.Remove(p); } catch {}
                    try { ZombieGame.alive.Remove(p); } catch {}
                    p.color = p.group.color;
                    p.infected = false;
                    p.SetPrefix();
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/referee - Turns referee mode on/off.");
        }
    }
}