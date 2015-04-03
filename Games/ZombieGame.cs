/*
	Copyright 2010 MCLawl Team - 
    Created by Snowl (David D.) and Cazzar (Cayde D.)

	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
 
    This script was modified by void_public. All credits to the creators.
*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Timers;
using System.Threading;
using System.Diagnostics;

namespace MCForge
{
    public class ZombieGame
    {
        public int amountOfRounds = 0;
        public int limitRounds = 0;
        public int amountOfMilliseconds = 0;
        public string currentZombieLevel = "";
        public string combomessage = "";
        public string article = "";
        public static System.Timers.Timer timer;
        public static System.Timers.Timer starttimer;
        public static System.Timers.Timer timeleftannouncer;
        public bool initialChangeLevel = false;
        public int aliveCount = 0;
        public DateTime StartTime;
        public string currentLevelName = "";
        public string lastzombie = "";
        public static List<Player> alive = new List<Player>();
        public static List<Player> infectd = new List<Player>();
        public static List<string> levelsplayed = new List<string>();
        public int lotterycount = 0;
        public bool lastseconds = false;
        Random rand = new Random();
        public List<Player> lottery = new List<Player>();
        public ZombieGame() { }
        public void StartGame(int status, int amount)
        {
            //status: 0 = not started, 1 = always on, 2 = one time, 3 = certain amount of rounds, 4 = stop round next round
            if (status == 0) return;
            //SET ALL THE VARIABLES!
            /*if (Server.UseLevelList && Server.LevelList == null)
                Server.ChangeLevels = false;*/
            Server.ZombieModeOn = true;
            Server.gameStatus = status;
            Server.zombieRound = false;
            initialChangeLevel = false;
            limitRounds = amount + 1;
            amountOfRounds = 0;
            Thread t = new Thread(MainLoop);
            t.Start();
        }
        public void InfectPlayer(Player p)
        {
            if (p == null && infectd.Count == 0) {firstinfectdc(); return;}
            else if(p==null) return;
            try { alive.Remove(p); }
            catch { }
            if (!infectd.Contains(p)) infectd.Add(p);
            p.infected = true;
            p.color = c.red;
            p.winstreakcount = 0;
            p.SetPrefix();
            p.blockCount = Server.blocklimitzombies;
            p.infecttime = DateTime.Now;
            Player.GlobalDie(p, false);
            Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
            Player.SendMessage(p, c.red + "--------------------------------------");
            Player.SendMessage(p, c.red + "You are a zombie now !");
            Player.SendMessage(p, c.red + "--------------------------------------");
        }
        public void DisinfectPlayer(Player p)
        {
            if (p == null) return;
            try { infectd.Remove(p); }
            catch { }
            if (!alive.Contains(p)) alive.Add(p);
            p.infected = false;
            p.color = p.group.color;
            p.SetPrefix();
            p.blockCount = Server.blocklimithuman;
            Player.GlobalDie(p, false);
            Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
            if (infectd.Count == 0) firstinfectdc();
        }
        public void firstinfectdc()
        {
            jump:
            Player newzombie = null;
            if (Server.queZombie == true)
            {
                Server.queZombie = false;
                newzombie = Player.Find(Server.nextZombie);
            }
            else if (newzombie == null)
            {
                Random random = new Random();
                newzombie = Player.Find(alive[random.Next(alive.Count)].name);
            }
            if (newzombie == null) goto jump;
            if (infectd.Count==0) 
            {
                InfectPlayer(newzombie);
                Player.GlobalMessage("%cThe disease continues with " + newzombie.color + newzombie.name);
            }
        }
        public void humangone()
        {
            if (alive.Count > 0) Player.GlobalMessage("%aHumans left: " + alive.Count);
            if (alive.Count == 1)
            {
                string timeleft = Server.zombie.GetTimeLeft("");
                Player.GlobalMessage("Now its up to: %f" + alive[0].name + "%e Can they survive for " + c.red + timeleft);
            }
        }
        private void MainLoop()
        {
            if (Server.gameStatus == 0) return;
            bool cutVariable = true;

            if (initialChangeLevel == false)
            {
                ChangeLevel();
                initialChangeLevel = true;
            }
            while (cutVariable == true)
            {
                int gameStatus = Server.gameStatus;
                Server.zombieRound = false;
                amountOfRounds = amountOfRounds + 1;

                if (gameStatus == 0) { cutVariable = false; return; }
                else if (gameStatus == 1)
                {
                    MainGame();
                    if (Server.ChangeLevels) ChangeLevel();
                }
                else if (gameStatus == 2) { MainGame(); if (Server.ChangeLevels) ChangeLevel(); cutVariable = false; Server.gameStatus = 0; return; }
                else if (gameStatus == 3)
                {
                    if (limitRounds == amountOfRounds) { cutVariable = false; Server.gameStatus = 0; limitRounds = 0; initialChangeLevel = false; Server.ZombieModeOn = false; Server.zombieRound = false; return; }
                    else { MainGame(); if (Server.ChangeLevels) ChangeLevel(); }
                }
                else if (gameStatus == 4)
                { cutVariable = false; Server.gameStatus = 0; Server.gameStatus = 0; limitRounds = 0; initialChangeLevel = false; Server.ZombieModeOn = false; Server.zombieRound = false; return; }
            }
        }
        private void MainGame()
        {
            if (Server.gameStatus == 0) return;
        GoBack:
            Player.GlobalMessage("Starting in %c30%e seconds"); Thread.Sleep(10000); if (!Server.ZombieModeOn) { return; }
            Player.GlobalMessage("Starting in %c20%e seconds"); Thread.Sleep(10000); if (!Server.ZombieModeOn) { return; }
            Player.GlobalMessage("Starting in %c10%e seconds"); Thread.Sleep(10000); if (!Server.ZombieModeOn) { return; }
            Player.GlobalMessage("Starting in %c5%e seconds"); Thread.Sleep(1000); if (!Server.ZombieModeOn) { return; }
            Player.GlobalMessage("Starting in %c4%e seconds"); Thread.Sleep(1000); if (!Server.ZombieModeOn) { return; }
            Player.GlobalMessage("Starting in %c3%e seconds"); Thread.Sleep(1000); if (!Server.ZombieModeOn) { return; }
            Player.GlobalMessage("Starting in %c2%e seconds"); Thread.Sleep(1000); if (!Server.ZombieModeOn) { return; }
            Player.GlobalMessage("Starting in %c1%e second"); Thread.Sleep(1000); if (!Server.ZombieModeOn) { return; }
            int playerscountminusref = 0;
            //Get players
            try { alive.Clear(); infectd.Clear(); }
            catch { }
            for (int index = Player.players.Count(); index > 0; index--)
            {
                Player playere = Player.players[index - 1];
                playere.canrevive = true;
                playere.invisiblyused = 0;
                playere.infectThisRound = 0;
                playere.infected = false;
                playere.infectedfrom = "";
                if (playere.referee)
                    Player.GlobalDie(playere, false);
                else if (playere.level.name == currentLevelName)
                {
                    if (!alive.Contains(playere)) alive.Add(playere);
                    playerscountminusref += 1;
                }
                else
                {
                    try { Command.all.Find("goto").Use(playere, currentLevelName); }
                    catch { Server.s.Log("ERROR: #006"); }
                    if (!alive.Contains(playere)) alive.Add(playere);
                    playerscountminusref += 1;
                }
            }
            if (playerscountminusref < 2) 
            {
                Server.s.Log("ERROR: #007");
                Player.GlobalMessage(c.red + "ERROR: Need more than 2 players to play"); goto GoBack; 
            }
            StartTime = DateTime.Now;
            //Choose the first Zombie to be infected
            Player player = null;
            if (Server.queZombie == true)
            {
                Server.queZombie = false;
                player = Player.Find(Server.nextZombie);
            }
            if (player == null || player.referee)
                player = alive[new Random().Next(0, alive.Count())];
            // Timer Initializing
            Level actualevel = Level.Find(currentLevelName);
            //int amountOfMinutes = 1;
            int amountOfMinutes = actualevel.roundtime;
            //-----------------------------------------------
            //int amountOfMinutes = new Random().Next(Server.roundtime1, Server.roundtime2);
            Player.GlobalMessage("The round will last for %c" + amountOfMinutes + "%e minutes!");
            amountOfMilliseconds = (60000 * amountOfMinutes);
            timer = new System.Timers.Timer(amountOfMilliseconds);
            timer.Elapsed += new ElapsedEventHandler(EndRound);
            timer.Enabled = true;
            Server.zombieRound = true;
            timeleftannouncer = new System.Timers.Timer(60000);
            timeleftannouncer.Elapsed += new ElapsedEventHandler(timleftannouncerfunction);
            timeleftannouncer.Enabled = true;
            Player.GlobalMessage(player.group.color + player.name + Server.DefaultColor + " started the infection!");
            InfectPlayer(player);
            humangone();
            //Main Loop for Game
            try
            {
                while (alive.Count > 0)
                {
                    for (int index = infectd.Count(); index > 0; index--)
                    {

                        Player player1 = infectd[index - 1];
                        if (player1.color != c.red)
                        {
                            player1.color = c.red;
                            player1.SetPrefix();
                            Player.GlobalDie(player1, false);
                            Player.GlobalSpawn(player1, player1.pos[0], player1.pos[1], player1.pos[2], player1.rot[0], player1.rot[1], false);
                        }
                        for (int index2 = alive.Count(); index2 > 0; index2--)
                        {
                            Player player2 = alive[index2 - 1];
                            if (player2.color != c.white)
                            {
                                player2.color = c.white;
                                player2.SetPrefix();
                                Player.GlobalDie(player2, false);
                                Player.GlobalSpawn(player2, player2.pos[0], player2.pos[1], player2.pos[2], player2.rot[0], player2.rot[1], false);
                            }
                            // Hitbox Detection

                            /*if (player2.pos[0] / 32 == player1.pos[0] / 32 || player2.pos[0] / 32 == player1.pos[0] / 32 + 1 || player2.pos[0] / 32 == player1.pos[0] / 32 - 1)
                              {
                                  if (player2.pos[1] / 32 == player1.pos[1] / 32 || player2.pos[1] / 32 == player1.pos[1] / 32 - 1 || player2.pos[1] / 32 == player1.pos[1] / 32 + 1)
                                  {
                                      if (player2.pos[2] / 32 == player1.pos[2] / 32 || player2.pos[2] / 32 == player1.pos[2] / 32 + 1 || player2.pos[2] / 32 == player1.pos[2] / 32 - 1)
                                      {
                             */
                            if (Math.Abs(player2.pos[0] - player1.pos[0]) <= Server.hitboxrangex
                                && Math.Abs(player2.pos[1] - player1.pos[1]) <= Server.hitboxrangey
                                && Math.Abs(player2.pos[2] - player1.pos[2]) <= Server.hitboxrangez
                                )
                            {
                                if (!player2.infected && player1.infected && !player2.referee && !player1.referee && player1 != player2)
                                {
                                    //Infection Combo + Award
                                    if (Server.lastPlayerToInfect == player1.name)
                                    {
                                        Server.infectCombo++;
                                        if (Server.infectCombo > 0)
                                        {
                                            switch (Server.infectCombo + 1)
                                            {
                                                case 2: combomessage = "is awesome - doublekill"; break;
                                                case 3: combomessage = "is epic - triplekill"; break;
                                                case 4: combomessage = "is pro - quadruplekill"; break;
                                                case 5: combomessage = "is crazy - quintuplekill"; break;
                                                case 6: combomessage = "is unstoppable - sextuplekill"; break;
                                                case 7: combomessage = "is unbeatable - septuplekill"; break;
                                                case 8: combomessage = "is legendary  - octuplekill"; break;
                                                case 9: combomessage = "is god like - nonuplekill"; break;
                                                case 10: Player.Find(player1.name).Achieve("Chuck Norris"); break;
                                            }
                                            if ((Server.infectCombo + 1) < 10) Player.GlobalMessage(c.red + player1.name + "%b " + combomessage);
                                            else Player.GlobalMessage(c.red + player1.name + "%b kills like Chuck Norris himself: " + (Server.infectCombo + 1) + " kills in a row");
                                            Player.SendMessage(player1, "%aYou got additional " + ((Server.infectCombo + 1)) + " " + Server.moneys + " for your killstreak");
                                            player1.money += ((Server.infectCombo + 1) * 2);
                                        }
                                    }
                                    else
                                        Server.infectCombo = 0;
                                    if (player1.maximuminfected < Server.infectCombo + 1) player1.maximuminfected = Server.infectCombo + 1;
                                    Server.lastPlayerToInfect = player1.name;
                                    player1.infectThisRound++;
                                    player1.playersinfected++;
                                    if (player2.infectedfrom == player1.name)
                                        player1.Achieve("Deja Vu");
                                    player2.infectedfrom = player1.name;
                                    if (alive.Count == 1)
                                        player1.Achieve("Finisher");
                                    if (player2.winstreakcount >= 3)
                                        player1.Achieve("Dream Destroyer");
                                    if (player2.autoafk)
                                        player1.Achieve("Assassin");
                                    if (lastseconds)
                                        player2.Achieve("Unlucky");
                                    Player.SendMessage(player1, "Brains eaten this round: %c" + player1.infectThisRound);
                                    if (Server.infectCombo == 0)
                                    {
                                        Player.SendMessage(player1, "%aYou gained 1 extra cookie for eating brains");
                                        player1.money += 1;
                                    }
                                    Player.GlobalMessage(GetInfectedmessage(player1.name, player2.name));
                                    Server.s.Log(player1.name + " infected " + player2.name);
                                    InfectPlayer(player2);
                                    humangone();
                                    Thread.Sleep(300);
                                    //Thread.Sleep(200);
                                    //Thread.Sleep(200);
                                }
                            }
                            //  }
                            //}
                        }
                    }
                    Thread.Sleep(400);
                }
            }
            catch
            {
                Server.s.Log("Error: #016");
            }
            if (Server.gameStatus == 0)
            {
                Server.gameStatus = 4;
                return;
            }
            else if (Server.zombieRound == true)
                HandOutRewards();

        }

        void timleftannouncerfunction(object sender, ElapsedEventArgs e)
        {
            string timeleft = Server.zombie.GetTimeLeft("");
            int timeleftminutes = Convert.ToInt32(Server.zombie.GetTimeLeft("minutes"));
            if (timeleftminutes > 0)
            {
                Player.GlobalMessage("Time left: " + c.red + timeleft);
                for (int index = alive.Count(); index > 0; index--)
                {
                    Player test = alive[index - 1];
                    Player b = null;
                    b = Player.Find(test.name);
                    if (b == null) alive.Remove(alive[index - 1]);
                }
            }
            
        }
        public void EndRound(object sender, ElapsedEventArgs e)
        {
            if (Server.gameStatus == 0) return;
            if (!Server.zombieRound) return;
            lastseconds = true;
            Player.GlobalMessage("%4Round End:%f 5"); Thread.Sleep(1000);
            if (!Server.zombieRound) return;
            Player.GlobalMessage("%4Round End:%f 4"); Thread.Sleep(1000);
            if (!Server.zombieRound) return;
            Player.GlobalMessage("%4Round End:%f 3"); Thread.Sleep(1000);
            if (!Server.zombieRound) return;
            Player.GlobalMessage("%4Round End:%f 2"); Thread.Sleep(1000);
            if (!Server.zombieRound) return;
            Player.GlobalMessage("%4Round End:%f 1"); Thread.Sleep(1000);
            if (!Server.zombieRound) return;
            lastseconds = false;
            HandOutRewards();
        }
        public void HandOutRewards()
        {
            Server.zombieRound = false;
            lastseconds = false;
            timer.Enabled = false;
            timeleftannouncer.Enabled = false;
            amountOfMilliseconds = 0;   
            string playersString = "";
            if (Server.gameStatus == 0) return;
            Level actuallevel = Level.Find(currentLevelName); 
            if (alive.Count != 0)
            {
                actuallevel.humanswon++;
                Level.SaveSettings(actuallevel);
                Player.GlobalMessage(c.green + "----------------------------------");
                Player.GlobalMessage(c.green + "Humans have won this round");
                Player.GlobalMessage(c.green + "Congratulations to our survivor(s)");
                Server.s.Log("Humans have won on map " + currentLevelName);
                try
                {
                    for (int index = alive.Count(); index > 0; index--)
                    {
                        Player winner = alive[index - 1];
                        playersString += winner.group.color + winner.name + c.white + ", ";
                        switch (winner.winstreakcount)
                        {
                            case 0: winner.money += 15;
                                Player.SendMessage(winner, c.green + "You got 15 " + Server.moneys + " for surviving");
                                break;
                            case 1: winner.money += 17;
                                Player.SendMessage(winner, c.green + "You got 17 " + Server.moneys + " for surviving");
                                break;
                            case 2: winner.money += 20;
                                Player.SendMessage(winner, c.green + "You got 20 " + Server.moneys + " for surviving");
                                break;
                            default: winner.money += 24;
                                Player.SendMessage(winner, c.green + "You got 24 " + Server.moneys + " for surviving");
                                break;
                        }
                        winner.winstreakcount++;
                        if (winner.maximumsurvived < winner.winstreakcount) winner.maximumsurvived = winner.winstreakcount;
                        winner.roundssurvived++;
                        if (!winner.canrevive)
                            winner.Achieve("Second Chance");
                        if (winner.winstreakcount == 5)
                            winner.Achieve("Bear Grylls");
                        if((((actuallevel.humanswon + actuallevel.zombieswon) == 0 ? 100 : ((actuallevel.humanswon * 100) / (actuallevel.humanswon + actuallevel.zombieswon)))<=10) 
                            && ((actuallevel.humanswon + actuallevel.zombieswon) >= 10))
                            winner.Achieve("Impossible");
                        if (alive.Count == 1)
                            alive[0].Achieve("Cant touch this");
                    }
                }
                catch { Server.s.Log("ERROR: #001"); }
                Player.GlobalMessage(playersString);
                Player.GlobalMessage(c.green + "----------------------------------");
            }
            else if (alive.Count == 0)
            {
                actuallevel.zombieswon++;
                Level.SaveSettings(actuallevel);
                Player.GlobalMessage(c.red + "----------------------------------");
                Player.GlobalMessage(c.red + "Zombies have won this round.");
                Server.s.Log("Zombies have won on map " + currentLevelName);
                try
                {
                    int maxinfect = infectd.Max(obj => obj.infectThisRound);
                    var maxinfectList = infectd.Where(obj => obj.infectThisRound == maxinfect);
                    string maxinfectname = "";
                    foreach (Player bestzombie in maxinfectList)
                        maxinfectname += bestzombie.name + ",";
                    Player.GlobalMessage("Best zombie(s): " + c.red + maxinfectname + Server.DefaultColor + " with " + c.red + maxinfect + Server.DefaultColor + " kills");
                }
                catch
                {
                    Server.s.Log("Error: #017");
                }
                Player.GlobalMessage(c.red + "----------------------------------");
                try
                {
                    for (int index = Player.players.Count(); index > 0; index--)
                    {
                        Player player = Player.players[index - 1];
                        if (player.infected && player.infectThisRound > 0)
                        {
                            player.SendMessage(c.green + "You helped the zombies win, here is a little reward: 2 " + Server.moneys);
                            player.money = player.money + 2;
                        }
                    }
                }
                catch
                {
                    Server.s.Log("ERROR: #012");
                }
            }
            try { alive.Clear(); } catch { Server.s.Log("ERROR: #003"); }
            try { infectd.Clear(); } catch { Server.s.Log("ERROR: #004"); }
            try
            {
                for (int index = Player.players.Count(); index > 0; index--)
                {
                    Player player = Player.players[index - 1];
                    player.infected = false;
                    player.color = player.group.color;
                    player.SetPrefix();
                    player.blockCount = Server.blocklimithuman;
                    player.infectThisRound = 0;
                    player.infectedfrom = "";
                    player.revivesused = 0;
                    Player.GlobalDie(player, false);
                    Player.GlobalSpawn(player, player.pos[0], player.pos[1], player.pos[2], player.rot[0], player.rot[1], false);
                    if (player.referee)
                    {
                        player.SendMessage("You gained 1 " + Server.moneys + " because you're a ref. Thank you!");
                        player.money += 1;
                    }
                }
            }
            catch { Server.s.Log("ERROR: #002"); }
            try
            {
                if (lottery.Count > 1)
                {
                    int number = rand.Next(lottery.Count - 1);
                    Player lotterywinner = lottery[number];
                    Player.GlobalMessage(lotterywinner.name + " won the lottery with a price of: " + c.gold + Convert.ToString(9 * lotterycount) + Server.moneys);
                    lotterywinner.money += 9 * lotterycount;
                    Server.s.Log("Lottery winner " + lotterywinner.name + " with " + lottery.Count + " players");
                    if (lotterycount == 7)
                        lotterywinner.Achieve("Lucky Number 7");
                }
                else if (lottery.Count == 1)
                {
                    Player.SendMessage(lottery[0], "Too less people joined the lottery, you get your money back");
                    Server.s.Log("Lottery aborted - too less players");
                    lottery[0].money += 10;
                }
            }
            catch
            {
                Server.s.Log("Error: #020");
            }
            try { lottery.Clear(); lotterycount = 0; } catch { Server.s.Log("ERROR: #018"); }
            try
            {
                if((actuallevel.humanswon + actuallevel.zombieswon) > 10)
                {
                    int percantagewin = (actuallevel.humanswon*100 / (actuallevel.humanswon + actuallevel.zombieswon));
                    if (percantagewin > 95)
                        actuallevel.roundtime = 12;
                    else if (percantagewin > 90)
                        actuallevel.roundtime = 10;
                    else if (percantagewin > 85)
                        actuallevel.roundtime = 9;
                    else if (percantagewin > 75)
                        actuallevel.roundtime = 8;
                    else if (percantagewin > 30)
                        actuallevel.roundtime = 7;
                    else if (percantagewin > 25)
                        actuallevel.roundtime = 6;
                    else if (percantagewin > 15)
                        actuallevel.roundtime = 6;
                    else
                        actuallevel.roundtime = 5;
                    Level.SaveSettings(actuallevel);
                }
            }
            catch
            {
                Server.s.Log("Error: #022");
            }
            if (Math.Round((double)Process.GetCurrentProcess().PrivateMemorySize64 / 1048576) > 100)
            {
                Server.s.Log("Server restarting -> Memory overflow");
                MCForge_.Gui.Program.ExitProgram(true);
            }
            return;
        }
        public void CopyStandartLevel(string lvlname)
        {
            if (!Directory.Exists(Server.zombiedefaultlevelpath)) Directory.CreateDirectory(Server.zombiedefaultlevelpath);
            if (!File.Exists(Server.zombiedefaultlevelpath + lvlname + ".lvl")) { Server.s.Log("The standartlevel " + lvlname + " doesnt exist!"); return; }
            string from = Server.zombiedefaultlevelpath + lvlname + ".lvl";
            string to = Server.zombielevelpath + lvlname + ".lvl";
            File.Copy(from, to, true);
        }
        public void ChangeLevel()
        {
            if (Server.queLevel == true)
            {
                ChangeLevel(Server.nextLevel, Server.ZombieOnlyServer);
            }
            else
            {
                Thread.Sleep(10000);
                try
                {
                    if (Server.ChangeLevels)
                    {
                        ArrayList al = new ArrayList();
                        DirectoryInfo di = new DirectoryInfo(Server.zombielevelpath);
                        FileInfo[] fi = di.GetFiles("*.lvl");
                        foreach (FileInfo fil in fi)
                        {
                            al.Add(fil.Name.Split('.')[0]);
                        }

                        if (al.Count <= 2 && !Server.UseLevelList) { Server.s.Log("You must have more than 2 levels to change levels in Zombie Survival"); return; }

                        if (Server.LevelList.Count < 2 && Server.UseLevelList) { Server.s.Log("You must have more than 2 levels in your level list to change levels in Zombie Survival"); return; }

                        string selectedLevel1 = "";
                        string selectedLevel2 = "";
                        string selectedLevel3 = "";
                        int tries = 0;

                    LevelChoice:
                        Random r = new Random();
                        int x = 0;
                        int x2 = 1;
                        int x3 = 2;
                        string level = ""; string level2 = ""; string level3 = "";
                        if (!Server.UseLevelList)
                        {
                            x = r.Next(0, al.Count);
                            x2 = r.Next(0, al.Count);
                            x3 = r.Next(0, al.Count);
                            level = al[x].ToString();
                            level2 = al[x2].ToString();
                            level3 = al[x3].ToString();
                        }
                        else
                        {
                            x = r.Next(0, Server.LevelList.Count());
                            x2 = r.Next(0, Server.LevelList.Count());
                            x3 = r.Next(0, Server.LevelList.Count());
                            level = Server.LevelList[x].ToString();
                            level2 = Server.LevelList[x2].ToString();
                            level3 = Server.LevelList[x3].ToString();
                        }
                        Level current = Server.mainLevel;

                        if (level == level2 
                            || level == level3 
                            || level2 == level3
                            || Server.lastLevelVote1 == level 
                            || Server.lastLevelVote1 == level2
                            || Server.lastLevelVote1 == level3
                            || Server.lastLevelVote2 == level
                            || Server.lastLevelVote2 == level2
                            || Server.lastLevelVote2 == level3
                            || Server.lastLevelVote3 == level
                            || Server.lastLevelVote3 == level2
                            || Server.lastLevelVote3 == level3
                            || current == Level.Find(level)
                            || current == Level.Find(level2)
                            || current == Level.Find(level3) 
                            || currentZombieLevel == level
                            || currentZombieLevel == level2
                            || currentZombieLevel == level3
                            )
                            goto LevelChoice;
                        if ((levelsplayed.Contains(level)
                            || levelsplayed.Contains(level2)
                            || levelsplayed.Contains(level3)
                            ) && tries < 40)
                        {
                            tries++;
                            goto LevelChoice;
                        }
                        Server.s.Log("Level choice was made after " + tries + " tries");
                        if (selectedLevel1 == "") selectedLevel1 = level;  
                        if (selectedLevel2 == "") selectedLevel2 = level2; 
                        if (selectedLevel3 == "") selectedLevel3 = level3;

                        Server.Level1Vote = 0; 
                        Server.Level2Vote = 0; 
                        Server.Level3Vote = 0;
                        Server.lastLevelVote1 = selectedLevel1; 
                        Server.lastLevelVote2 = selectedLevel2; 
                        Server.lastLevelVote3 = selectedLevel3;

                        if (Server.gameStatus == 4 || Server.gameStatus == 0) { return; }

                        if (initialChangeLevel == true)
                        {
                            Server.votingforlevel = true;
                            Player.GlobalMessage(c.aqua + "---------------------------------------------");
                            Player.GlobalMessage(c.aqua + "Vote: " + Server.DefaultColor + "(" + c.lime + "1" + Server.DefaultColor + "/" + c.red + "2" + Server.DefaultColor + "/" + c.blue + "3" + Server.DefaultColor + ")");
                            Player.GlobalMessage(c.lime + selectedLevel1 + Server.DefaultColor + " / " + c.red + selectedLevel2 + Server.DefaultColor + " / " + c.blue + selectedLevel3);
                            Player.GlobalMessage(c.aqua + "---------------------------------------------");
                            System.Threading.Thread.Sleep(15000);
                            Server.votingforlevel = false;
                        }
                        else { Server.Level1Vote = 1; Server.Level2Vote = 0; Server.Level3Vote = 0; }

                        if (Server.gameStatus == 4 || Server.gameStatus == 0) { return; }
                       
                        if (Server.Level1Vote >= Server.Level2Vote)
                        {
                            if (Server.Level3Vote > Server.Level1Vote && Server.Level3Vote > Server.Level2Vote)
                            {
                                ChangeLevel(selectedLevel3, Server.ZombieOnlyServer);
                            }
                            else ChangeLevel(selectedLevel1, Server.ZombieOnlyServer);
                        }
                        else
                        {
                            if (Server.Level3Vote > Server.Level1Vote && Server.Level3Vote > Server.Level2Vote)
                            {
                                ChangeLevel(selectedLevel3, Server.ZombieOnlyServer);
                            }
                            else ChangeLevel(selectedLevel2, Server.ZombieOnlyServer);
                        }
                        Player.players.ForEach(delegate(Player voter)
                        {
                            voter.voted = false;
                        });
                    }
                }
                catch { }
            }
        }
        public int ZombieStatus()
        {
            return Server.gameStatus;
        }
        public bool GameInProgess()
        {
            return Server.zombieRound;
        }
        public string GetTimeLeft(string what)
        {
            int minutesgone = DateTime.Now.Minute - Server.zombie.StartTime.Minute;
            int secondsgone = DateTime.Now.Second - Server.zombie.StartTime.Second;
            int getroundtimeinseconds = Server.zombie.amountOfMilliseconds / 1000;

            int totalsecondsleft = getroundtimeinseconds - minutesgone * 60 - secondsgone + 5;

            int minutesleft = totalsecondsleft / 60;
            int secondsleft = totalsecondsleft % 60;

            if (minutesleft >= 60) minutesleft -= 60;

            if(what == "minutes") return minutesleft.ToString();
            if (what == "seconds") return secondsleft.ToString();
            else
            {
                string time = string.Format("{0:D2}:{1:D2}", minutesleft, secondsleft);
                return time;
            }
        }
        public string GetInfectedmessage(string zombiename, string humanname)
        {
            string path = "text/infectmessages.txt";
            if (!File.Exists(path))
            {
                string[] prepared = new string[] {"#Use <human> and <zombie> for the people in the infected messages" , "<zombie> ate up <human>" };
                File.WriteAllLines(path, prepared);
            }
            string[] lines = File.ReadAllLines(path);
            List<string> messages = new List<string>();
            foreach (string msg in lines)
            {
                if (!msg.StartsWith("#")) messages.Add(msg);
            }
            int useline = new Random().Next(0, messages.Count());
            string toreturn = messages[useline];
            toreturn = toreturn.Replace("<zombie>", c.red  + zombiename + Server.DefaultColor);
            toreturn = toreturn.Replace("<human>", c.white + humanname + Server.DefaultColor);
            return toreturn;
        }
        public void ChangeLevel(string LevelName, bool changeMainLevel)
        {
            String next = LevelName;
            try
            {
                levelsplayed.Add(next);
                string firstlevelinlevelsplayed = levelsplayed.First();
                if(levelsplayed.Count>20) levelsplayed.Remove(firstlevelinlevelsplayed);
            }
            catch
            {
                Server.s.Log("Error: #023");
            }
            currentLevelName = next;
            if (!Server.queLevel)
            {
                int number1 = 0, number2 = 0, number3 = 0;
                if (Server.Level1Vote != 0) number1 = Server.Level1Vote * 100 / (Server.Level1Vote + Server.Level2Vote + Server.Level3Vote);
                if (Server.Level2Vote != 0) number2 = Server.Level2Vote * 100 / (Server.Level1Vote + Server.Level2Vote + Server.Level3Vote);
                if (Server.Level3Vote != 0) number3 = Server.Level3Vote * 100 / (Server.Level1Vote + Server.Level2Vote + Server.Level3Vote);
                Player.GlobalMessage(c.aqua + "---------------------------------------------");
                Player.GlobalMessage(c.aqua + "Vote results are in:");
                Player.GlobalMessage(c.lime + Server.lastLevelVote1 + "(" + number1 + "%" + ")" + Server.DefaultColor + " / "
                                    + c.red + Server.lastLevelVote2 + "(" + number2 + "%" + ")" + Server.DefaultColor + " / "
                                    + c.blue + Server.lastLevelVote3 + "(" + number3 + "% )");
                Player.GlobalMessage(c.aqua + "---------------------------------------------");
            }
            Server.queLevel = false;
            Thread.Sleep(1000);
            Server.nextLevel = "";
            Command.all.Find("load").Use(null, next.ToLower() + " 0");
            Player.GlobalMessage("The next map has been chosen - " + c.lime + currentLevelName);
            Heart.Init();
            Level nextlvl = Level.Find(currentLevelName);
            Player.GlobalMessage("It was created by " + c.lime + nextlvl.creator);
            int percentagelike = 0;
            if(nextlvl.likes + nextlvl.dislikes != 0) 
                percentagelike = (nextlvl.likes * 100) / (nextlvl.likes + nextlvl.dislikes);
            Player.GlobalMessage(c.lime + percentagelike + Server.DefaultColor + "% like this map");
            int winchance = 0;
            if (nextlvl.humanswon + nextlvl.zombieswon != 0)
                winchance = (nextlvl.humanswon * 100) / (nextlvl.humanswon + nextlvl.zombieswon);
            Player.GlobalMessage(c.lime + winchance + Server.DefaultColor + "% win chance on this map");
            Thread.Sleep(1000);
            Player.GlobalMessage("Type "+ c.aqua + "/g " + currentLevelName + Server.DefaultColor + " to go there right now");
            Player.GlobalMessage("Or wait " + c.aqua + "10 seconds " + Server.DefaultColor + "to be transfered");
            Thread.Sleep(10000);
            String oldLevel = Server.mainLevel.name;
            if (changeMainLevel)
            {
                Server.mainLevel = Level.Find(next.ToLower());
                try
                {
                    for (int index = Player.players.Count(); index > 0; index--)
                    {
                        Player playere = Player.players[index - 1];
                        if (playere.level.name != next)
                        {
                            playere.SendMessage("Going to the next map!");
                            Command.all.Find("goto").Use(playere, next);
                            while (playere.Loading) { }
                        }
                    }
                }
                catch
                {
                    Server.s.Log("ERROR: #005");
                    MCForge_.Gui.Program.ExitProgram(true);
                }
                Thread.Sleep(500);
                Command.all.Find("unload").Use(null, oldLevel);
                CopyStandartLevel(oldLevel);
            }
            else
            {
                Player.GlobalMessage("Type /goto " + next + " to play the next round of Zombie Survival");
            }
            return;
        }
        public void ChangeTime(object sender, ElapsedEventArgs e)
        {
            amountOfMilliseconds = amountOfMilliseconds - 10;
        }
        public bool IsInZombieGameLevel(Player p)
        {
            return p.level.name == currentLevelName;
        }

    }
}
