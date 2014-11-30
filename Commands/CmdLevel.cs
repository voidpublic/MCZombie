using System;
using System.IO;
using System.Collections.Generic;
using MCForge.SQL;


namespace MCForge.Commands
{
    public class CmdLevel : Command
    {
        public override string name { get { return "level"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "vetop"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdLevel() { }

        public override void Use(Player p, string message)
        {
            string zombielevelpath = Server.zombielevelpath;
            string zombiestandartlevelpath = Server.zombiedefaultlevelpath; 
            string buildlevelpath = Server.buildlevelpath;
            if (!Directory.Exists(zombielevelpath)) Directory.CreateDirectory(zombielevelpath);
            if (!Directory.Exists(zombiestandartlevelpath)) Directory.CreateDirectory(zombiestandartlevelpath);
            if (!Directory.Exists(buildlevelpath)) Directory.CreateDirectory(buildlevelpath);
            if (message.Split(' ').Length == 3)
            {
                string what = message.Split(' ')[0];
                string mapnameonbuild = message.Split(' ')[1];
                string mapnameonzomb = message.Split(' ')[2].ToLower();
                bool yes = false;
                DirectoryInfo di = new DirectoryInfo(buildlevelpath);
                FileInfo[] fi = di.GetFiles("*.lvl");
                foreach (FileInfo file in fi)
                {
                    if (file.Name.Replace(".lvl", "").ToLower().Equals(mapnameonbuild.ToLower()))
                    {
                        yes = true;
                    }
                }
                if (yes)
                {
                    if (what == "add")
                    {
                        if (File.Exists(zombielevelpath + mapnameonzomb + ".lvl"))
                        {
                            Player.SendMessage(p, "Level " + mapnameonzomb + " already exists - wanted to fix?");
                            return;
                        }
                        File.Copy(buildlevelpath + mapnameonbuild + ".lvl", zombielevelpath + mapnameonzomb + ".lvl", false);
                        File.Copy(buildlevelpath + mapnameonbuild + ".lvl", zombiestandartlevelpath + mapnameonzomb.ToLower() + ".lvl", false);
                        Player.SendMessage(p, c.lime + "Map " + c.aqua + mapnameonzomb.ToLower() + c.lime + " sucessfully added");
                    }
                    else if (what == "fix")
                    {
                        if (!File.Exists(zombielevelpath + mapnameonzomb + ".lvl"))
                        {
                            Player.SendMessage(p, c.red + "Level does not exist on zombie so cannot fix it");
                            return;
                        }
                        Level test = null;
                        test = Level.Find(mapnameonzomb);
                        if (test == null)
                        {
                            File.Copy(buildlevelpath + mapnameonbuild + ".lvl", zombielevelpath + mapnameonzomb + ".lvl", true);
                            File.Copy(buildlevelpath + mapnameonbuild + ".lvl", zombiestandartlevelpath + mapnameonzomb.ToLower() + ".lvl", true);
                            Player.SendMessage(p, c.lime + "Map " + c.aqua + mapnameonzomb.ToLower() + c.lime + " sucessfully fixed");
                        }
                        else
                            Player.SendMessage(p, c.red + "Level MUST NOT be loaded");
                    }
                }
                else
                    Player.SendMessage(p, c.red + "There is no level called " + mapnameonbuild + " on build server");
            }
            else if (message.Split(' ').Length == 2)
            {
                string what = message.Split(' ')[0];
                if (what == "remove")
                {
                    string mapnameonbuild = message.Split(' ')[1];
                    string mapname = mapnameonbuild;
                    Level test = null;
                    test = Level.Find(mapname);
                    if (test == null)
                    {
                        File.Delete(zombielevelpath + mapname + ".lvl");
                        File.Delete(zombielevelpath + "level properties/" + mapname + ".properties");
                        MySQL.executeQuery("DELETE FROM levelinfo WHERE name='" + mapname + "';");
                        Player.SendMessage(p, "Succesfully removed " + mapname + " + all data that has to do with it");
                    }
                    else
                        Player.SendMessage(p, c.red + "Level MUST NOT be loaded");
                }
                else
                    Help(p);

            }
            else
                Help(p);

        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/level add <levelnamebuild> <levelnamezomb> - adds a level from build to zombie");
            Player.SendMessage(p, "/level fix <levelnamebuild> <levelnamezomb> - fixes a level that is already on zombie");
            Player.SendMessage(p, "/level remove <levelnamezomb> - removes a level from zombie");
            Player.SendMessage(p,"Only works when name is written EXACTLY");
        }
    }
}