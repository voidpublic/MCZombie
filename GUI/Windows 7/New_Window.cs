using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace MCForge.GUI
{
    public partial class New_Window : Form
    {
        System.Timers.Timer updatetimer = new System.Timers.Timer(100);
        PlayerCollection pc = new PlayerCollection(new PlayerListView());
        LevelCollection lc = new LevelCollection(new LevelListView());
        LevelCollection lcTAB = new LevelCollection(new LevelListViewForTab());
        delegate void StringCallback(string s);
        delegate void PlayerListCallback(List<Player> players);
        delegate void ReportCallback(Report r);
        delegate void VoidDelegate();
        internal static Server s;
        public New_Window()
        {
            InitializeComponent();
        }
        delegate void LogDelegate(string message);
        public void WriteLine(string s)
        {
            if (Server.shuttingDown) return;
            if (this.InvokeRequired)
            {
                LogDelegate d = new LogDelegate(WriteLine);
                this.Invoke(d, new object[] { s });
            }
            else
            {
                //txtLog.AppendText(Environment.NewLine + s);
                txtLog.AppendTextAndScroll(s);
            }
        }
        /// <summary>
        /// Updates the list of client names in the window
        /// </summary>
        /// <param name="players">The list of players to add</param>
        public void UpdateClientList(List<Player> players)
        {

            if (this.InvokeRequired)
            {
                PlayerListCallback d = UpdateClientList;
                Invoke(d, new List<Player>[] { players });
            }
            else
            {

                if (dgvPlayers.DataSource == null)
                    dgvPlayers.DataSource = pc;

                // Try to keep the same selection on update
                string selected = null;
                if (pc.Count > 0 && dgvPlayers.SelectedRows.Count > 0)
                {
                    selected = (from DataGridViewRow row in dgvPlayers.Rows where row.Selected select pc[row.Index]).First().name;
                }

                // Update the data source and control
                //dgvPlayers.SuspendLayout();

                pc = new PlayerCollection(new PlayerListView());
                Player.players.ForEach(p => pc.Add(p));

                //dgvPlayers.Invalidate();
                dgvPlayers.DataSource = pc;
                // Reselect player
                if (selected != null)
                {
                    for (int i = 0; i < Player.players.Count; i++)
                        for (int j = 0; j < dgvPlayers.Rows.Count; j++)
                            if (String.Equals(dgvPlayers.Rows[j].Cells[0].Value, selected))
                                dgvPlayers.Rows[j].Selected = true;
                }

                dgvPlayers.Refresh();
                //dgvPlayers.ResumeLayout();
            }

        }
        public void newCommand(string p)
        {
            if (txtCommandsUsed.InvokeRequired)
            {
                LogDelegate d = new LogDelegate(newCommand);
                this.Invoke(d, new object[] { p });
            }
            else
            {
                txtCommandsUsed.AppendTextAndScroll(p);
            }
        }
        private void New_Window_Load(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                s = new Server();
                s.OnLog += WriteLine;
                s.OnCommand += newCommand;
                //s.OnError += newError;
                //s.OnSystem += newSystem;
                //s.OnAdmin += WriteAdmin;
                //s.OnOp += WriteOp;

                /*foreach (TabPage tP in tabControl1.TabPages)
                    tabControl1.SelectTab(tP);
                tabControl1.SelectTab(tabControl1.TabPages[0]);*/

                //s.HeartBeatFail += HeartBeatFail;
                //s.OnURLChange += UpdateUrl;
                //s.OnPlayerListChange += UpdateClientList;
               // s.OnSettingsUpdate += SettingsUpdate;
                s.Start();
                Player.PlayerConnect += new Player.OnPlayerConnect(Player_PlayerConnect);
                Player.PlayerDisconnect += new Player.OnPlayerDisconnect(Player_PlayerDisconnect);
                Level.LevelLoad += new Level.OnLevelLoad(Level_LevelLoad);
                Level.LevelUnload += new Level.OnLevelUnload(Level_LevelUnload);
                //btnProperties.Enabled = true;
                //if (btnProperties.InvokeRequired)
                //{
                //    VoidDelegate d = btnPropertiesenable;
                //    Invoke(d);
                //}
                //else
                //{
                //    btnProperties.Enabled = true;
                //}
            }).Start();
            dgvPlayers.DataSource = pc;
            dgvPlayers.Font = new Font("Calibri", 8.25f);

            dgvMaps.DataSource = new LevelCollection(new LevelListView());
            dgvMaps.Font = new Font("Calibri", 8.25f);

            dgvMapsTab.DataSource = new LevelCollection(new LevelListViewForTab());
            dgvMapsTab.Font = new Font("Calibri", 8.25f);

            updatetimer.Elapsed += delegate
            {
                UnloadedlistUpdate();
            };
        }

        void Level_LevelUnload(Level l)
        {
            UpdateMapList("'");
        }

        void Level_LevelLoad(string level)
        {
            UpdateMapList("'");
        }

        void Player_PlayerDisconnect(Player p, string reason)
        {
            UpdateClientList(Player.players);
            foreach (RibbonTab r in this.ribbon2.Tabs.ToArray())
            {
                if (r.Text.ToLower() == p.name.ToLower())
                {
                    ribbon2.ActiveTab = ribbonTab8;
                    ribbon2.Tabs.Remove(r);
                }
            }
        }
        public void UnloadedlistUpdate()
        {
            UnloadedList.Items.Clear();
            string name;
            FileInfo[] fi = new DirectoryInfo("levels/").GetFiles("*.lvl");
            foreach (FileInfo file in fi)
            {
                name = file.Name.Replace(".lvl", "");
                if (Level.Find(name.ToLower()) == null)
                    UnloadedList.Items.Add(name);
            }
        }
        internal static Color FromHex(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length != 6) throw new Exception("Color not valid");

            return Color.FromArgb(
                int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
        }
        void Player_PlayerConnect(Player p)
        {
            
            UpdateClientList(Player.players);
        }

        private void dgvPlayers_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPlayers.ColumnCount == 0)
                return;
            foreach (RibbonTab r in this.ribbon2.Tabs.ToArray())
            {
                switch (r.Text)
                {
                    case "Home":
                    case "System":
                    case "Maps":
                    case "Players":
                    case "Chat":
                    case "Plugins/Commands":
                        break;
                    default:
                        ribbon2.ActiveTab = ribbonTab8;
                        this.ribbon2.Tabs.Remove(r);
                        break;
                }
            }
            RibbonTab rb = new RibbonTab(ribbon2, (from DataGridViewRow row in dgvPlayers.Rows where row.Selected select pc[row.Index]).First().name);
            RibbonPanel p = new RibbonPanel("Moderation");
            RibbonButton b = new RibbonButton("Kick");
            b.Image = Image.FromFile("gui/kick.png");
            b.Click += delegate
            {
                try
                {
                    string selected = (from DataGridViewRow row in dgvPlayers.Rows where row.Selected select pc[row.Index]).First().name;
                    Command.all.Find("kick").Use(null, selected);
                }
                catch (Exception ex)
                {
                    Server.ErrorLog(ex);
                    Server.s.Log("Command Falied");
                }
            };
            p.Items.Add(b);
            b = new RibbonButton("Ban");
            b.Image = Image.FromFile("gui/delete.png");
            b.Click += delegate
            {
                try
                {
                    string selected = (from DataGridViewRow row in dgvPlayers.Rows where row.Selected select pc[row.Index]).First().name;
                    Command.all.Find("ban").Use(null, selected);
                }
                catch (Exception ex)
                {
                    Server.ErrorLog(ex);
                    Server.s.Log("Command Falied");
                }
            };
            p.Items.Add(b);
            b = new RibbonButton("Warn");
            b.Image = Image.FromFile("gui/warn.png");
            b.Click += delegate
            {
                try
                {
                    string selected = (from DataGridViewRow row in dgvPlayers.Rows where row.Selected select pc[row.Index]).First().name;
                    Command.all.Find("warn").Use(null, selected);
                }
                catch (Exception ex)
                {
                    Server.ErrorLog(ex);
                    Server.s.Log("Command Falied");
                }
            };
            p.Items.Add(b);
            rb.Panels.Add(p);
            ribbon2.Tabs.Add(rb);
            ribbon2.ActiveTab = rb;
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvMaps_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtCommandsUsed_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCommands_TextChanged(object sender, EventArgs e)
        {

        }

        private void ribbon2_ActiveTabChanged(object sender, EventArgs e)
        {
            try
            {
                switch (ribbon2.ActiveTab.Text)
                {
                    case "Home":
                    case "System":
                        tabControl1.SelectedTab = tabPage1;
                        break;
                    case "Maps":
                        tabControl1.SelectedTab = tabPage2;
                        break;
                    default:
                        tabControl1.SelectedTab = tabPage1;
                        break;
                }
            }
            catch (Exception ee) { Server.s.Log("" + ee);  }
        }

        private void ribbon2_Click(object sender, EventArgs e)
        {

        }

        private void loadmap_Click(object sender, EventArgs e)
        {
            
        }

        private void UnloadedList_SelectedIndexChanged(object sender, EventArgs e)
        {
            RibbonPanel p = new RibbonPanel("Options");
            RibbonButton b = new RibbonButton("Load");
            b.Image = Image.FromFile("gui/open32.png");
            p.Items.Add(b);
            b = new RibbonButton("Delete");
            b.Image = Image.FromFile("gui/delete.png");
            p.Items.Add(b);
            ribbonTab10.Panels.Add(p);
        }

        private void ribbonButton8_Click(object sender, EventArgs e)
        {
            Command.all.Find("killphysics").Use(null, "");
            try { UpdateMapList("'"); }
            catch { }
        }
        public void UpdateMapList(string unused)
        {
            /*
            if (this.InvokeRequired) {
                LogDelegate d = new LogDelegate(UpdateMapList);
                this.Invoke(d, new object[] { blah });
            } else {
                LevelCollection lc = new LevelCollection(new LevelListView());
                Server.levels.ForEach(delegate(Level l) { lc.Add(l); });
                dgvMaps.SuspendLayout();
                dgvMaps.DataSource = lc;
                //dgvMaps.Invalidate();
                dgvMaps.ResumeLayout();
            }
            */
            if (InvokeRequired)
            {
                LogDelegate d = new LogDelegate(UpdateMapList);
                Invoke(d, new Object[] { " " });
            }
            else
            {

                if (dgvMaps.DataSource == null)
                    dgvMaps.DataSource = lc;

                // Try to keep the same selection on update
                string selected = null;
                if (lc.Count > 0 && dgvMaps.SelectedRows.Count > 0)
                {
                    selected = (from DataGridViewRow row in dgvMaps.Rows where row.Selected select lc[row.Index]).First().name;
                }

                // Update the data source and control
                //dgvPlayers.SuspendLayout();
                lc.Clear();
                //lc = new LevelCollection(new LevelListView());
                Server.levels.ForEach(delegate(Level l) { lc.Add(l); });

                //dgvPlayers.Invalidate();
                dgvMaps.DataSource = null;
                dgvMaps.DataSource = lc;
                // Reselect map
                if (selected != null)
                {
                    foreach (DataGridViewRow row in Server.levels.SelectMany(l => dgvMaps.Rows.Cast<DataGridViewRow>().Where(row => (string)row.Cells[0].Value == selected)))
                        row.Selected = true;
                }

                dgvMaps.Refresh();
                //dgvPlayers.ResumeLayout();

                if (dgvMapsTab.DataSource == null)
                    dgvMapsTab.DataSource = lcTAB;

                // Try to keep the same selection on update
                string selected2 = null;
                if (lcTAB.Count > 0 && dgvMapsTab.SelectedRows.Count > 0)
                {
                    selected2 = (from DataGridViewRow row in dgvMapsTab.Rows where row.Selected select lcTAB[row.Index]).First().name;
                }

                // Update the data source and control
                //dgvPlayers.SuspendLayout();
                lcTAB.Clear();
                //lcTAB = new LevelCollection(new LevelListViewForTab());
                Server.levels.ForEach(delegate(Level l) { lcTAB.Add(l); });

                //dgvPlayers.Invalidate();
                dgvMapsTab.DataSource = null;
                dgvMapsTab.DataSource = lcTAB;
                // Reselect map
                if (selected2 != null)
                {
                    foreach (Level l in Server.levels)
                        foreach (DataGridViewRow row in dgvMapsTab.Rows)
                            if (String.Equals(row.Cells[0].Value, selected2))
                                row.Selected = true;
                }

                dgvMapsTab.Refresh();
            }
        }

        private void New_Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Server.shuttingDown == true || MessageBox.Show("Really Shutdown the Server? All Connections will break!", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (!Server.shuttingDown)
                {
                    MCForge_.Gui.Program.ExitProgram(false);
                }
            }
            else
            {
                // Prevents form from closing when user clicks the X and then hits 'cancel'
                e.Cancel = true;
            }
        }

        private void ribbonColorChooser1_ColorChanged(object sender, EventArgs e)
        {
            /*RibbonProfessionalRenderer rend = new RibbonProfessionalRenderer();
            rend.ColorTable.RibbonBackground = ribbonColorChooser1.Color;
            rend.ColorTable.PanelOverflowBackgroundSelectedNorth = ribbonColorChooser1.Color;
            this.BackColor = ribbonColorChooser1.Color;
            ribbon2.Renderer = rend;*/
        }
        //CUSTOM COLORS ===========================
        /*RibbonProfessionalRenderer rend = new RibbonProfessionalRenderer();
        rend.ColorTable.RibbonBackground = FromHex("#30fd30");
        ribbon2.Renderer = rend;*/
        //CUSTOM COLORS ===========================
        private void ribbonColorChooser1_Click(object sender, EventArgs e)
        {

        }

        private void blue_Click(object sender, EventArgs e)
        {
            RibbonProfessionalRenderer rend = new RibbonProfessionalRenderer();
            rend.ColorTable.PanelOverflowBackgroundSelectedNorth = FromHex("#B8D7FD");
            this.BackColor = FromHex("#B8D7FD");
            ribbon2.Renderer = rend;
        }

        private void black_Click(object sender, EventArgs e)
        {
            RibbonProfessionalRenderer rend = new RibbonProfessionalRenderer();
            rend.ColorTable.TabNorth = FromHex("#000000");
            rend.ColorTable.TabContentNorth = FromHex("#000000");
            rend.ColorTable.TabContentSouth = Color.DarkGray;
            rend.ColorTable.TabBorder = FromHex("#000000");
            rend.ColorTable.TabGlow = FromHex("#000000");
            rend.ColorTable.RibbonBackground = FromHex("#000000");
            rend.ColorTable.PanelOverflowBackgroundSelectedNorth = FromHex("#000000");
            this.BackColor = FromHex("#000000");
            tabPage1.BackColor = this.BackColor;
            tabPage2.BackColor = this.BackColor;
            label1.ForeColor = FromHex("#FFFFFF");
            label2.ForeColor = FromHex("#FFFFFF");
            label3.ForeColor = FromHex("#FFFFFF");
            label5.ForeColor = FromHex("#FFFFFF");
            label6.ForeColor = FromHex("#FFFFFF");
            label11.ForeColor = FromHex("#FFFFFF");
            label12.ForeColor = FromHex("#FFFFFF");
            label13.ForeColor = FromHex("#FFFFFF");
            label14.ForeColor = FromHex("#FFFFFF");
            label15.ForeColor = FromHex("#FFFFFF");
            label16.ForeColor = FromHex("#FFFFFF");
            label17.ForeColor = FromHex("#FFFFFF");
            label18.ForeColor = FromHex("#FFFFFF");
            label19.ForeColor = FromHex("#FFFFFF");
            label20.ForeColor = FromHex("#FFFFFF");
            label21.ForeColor = FromHex("#FFFFFF");
            label22.ForeColor = FromHex("#FFFFFF");
            label23.ForeColor = FromHex("#FFFFFF");
            label24.ForeColor = FromHex("#FFFFFF");
            label25.ForeColor = FromHex("#FFFFFF");
            label26.ForeColor = FromHex("#FFFFFF");
            label27.ForeColor = FromHex("#FFFFFF");
            label35.ForeColor = FromHex("#FFFFFF");
            label36.ForeColor = FromHex("#FFFFFF");
            label37.ForeColor = FromHex("#FFFFFF");
            label38.ForeColor = FromHex("#FFFFFF");
            label39.ForeColor = FromHex("#FFFFFF");
            ribbon2.Renderer = rend;
        }
    }
}
