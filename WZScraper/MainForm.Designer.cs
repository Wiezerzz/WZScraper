using System.Security.AccessControl;

namespace WZScraper
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.cbRegion = new MetroFramework.Controls.MetroComboBox();
            this.btScrape = new MetroFramework.Controls.MetroButton();
            this.ltbUsernames = new System.Windows.Forms.ListBox();
            this.contextMenuListBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbCount = new System.Windows.Forms.Label();
            this.lbStartPage = new MetroFramework.Controls.MetroTextBox();
            this.lbStopPage = new MetroFramework.Controls.MetroTextBox();
            this.lbSeperator = new MetroFramework.Controls.MetroLabel();
            this.lbTimer = new System.Windows.Forms.Label();
            this.stopwatchTimer = new System.Windows.Forms.Timer(this.components);
            this.btStop = new MetroFramework.Controls.MetroButton();
            this.loadingSpinner = new MetroFramework.Controls.MetroProgressSpinner();
            this.cbSite = new MetroFramework.Controls.MetroComboBox();
            this.metroRadioButton1 = new MetroFramework.Controls.MetroRadioButton();
            this.metroRadioButton2 = new MetroFramework.Controls.MetroRadioButton();
            this.metroRadioButton3 = new MetroFramework.Controls.MetroRadioButton();
            this.btImport = new MetroFramework.Controls.MetroButton();
            this.btExport = new MetroFramework.Controls.MetroButton();
            this.contextMenuListBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbRegion
            // 
            resources.ApplyResources(this.cbRegion, "cbRegion");
            this.cbRegion.FormattingEnabled = true;
            this.cbRegion.Items.AddRange(new object[] {
            resources.GetString("cbRegion.Items"),
            resources.GetString("cbRegion.Items1"),
            resources.GetString("cbRegion.Items2"),
            resources.GetString("cbRegion.Items3"),
            resources.GetString("cbRegion.Items4"),
            resources.GetString("cbRegion.Items5"),
            resources.GetString("cbRegion.Items6"),
            resources.GetString("cbRegion.Items7"),
            resources.GetString("cbRegion.Items8"),
            resources.GetString("cbRegion.Items9"),
            resources.GetString("cbRegion.Items10")});
            this.cbRegion.Name = "cbRegion";
            this.cbRegion.PromptText = "Region";
            this.cbRegion.Style = MetroFramework.MetroColorStyle.Teal;
            this.cbRegion.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.cbRegion.UseSelectable = true;
            this.cbRegion.UseStyleColors = true;
            this.cbRegion.SelectedIndexChanged += new System.EventHandler(this.CbRegionSelectedIndexChanged);
            // 
            // btScrape
            // 
            resources.ApplyResources(this.btScrape, "btScrape");
            this.btScrape.Name = "btScrape";
            this.btScrape.Style = MetroFramework.MetroColorStyle.Teal;
            this.btScrape.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btScrape.UseSelectable = true;
            this.btScrape.UseStyleColors = true;
            this.btScrape.Click += new System.EventHandler(this.BtScrapeClick);
            // 
            // ltbUsernames
            // 
            this.ltbUsernames.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(22)))), ((int)(((byte)(22)))));
            this.ltbUsernames.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ltbUsernames.ContextMenuStrip = this.contextMenuListBox;
            resources.ApplyResources(this.ltbUsernames, "ltbUsernames");
            this.ltbUsernames.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.ltbUsernames.Name = "ltbUsernames";
            // 
            // contextMenuListBox
            // 
            this.contextMenuListBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.clearAllToolStripMenuItem});
            this.contextMenuListBox.Name = "contextMenuListBox";
            resources.ApplyResources(this.contextMenuListBox, "contextMenuListBox");
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItemClick);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItemClick);
            // 
            // clearAllToolStripMenuItem
            // 
            this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
            resources.ApplyResources(this.clearAllToolStripMenuItem, "clearAllToolStripMenuItem");
            this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.ClearAllToolStripMenuItemClick);
            // 
            // lbCount
            // 
            resources.ApplyResources(this.lbCount, "lbCount");
            this.lbCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            this.lbCount.Name = "lbCount";
            // 
            // lbStartPage
            // 
            this.lbStartPage.Lines = new string[0];
            resources.ApplyResources(this.lbStartPage, "lbStartPage");
            this.lbStartPage.MaxLength = 9;
            this.lbStartPage.Name = "lbStartPage";
            this.lbStartPage.PasswordChar = '\0';
            this.lbStartPage.PromptText = "Start #";
            this.lbStartPage.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.lbStartPage.SelectedText = "";
            this.lbStartPage.Style = MetroFramework.MetroColorStyle.Teal;
            this.lbStartPage.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.lbStartPage.UseSelectable = true;
            this.lbStartPage.TextChanged += new System.EventHandler(this.LbStartPageTextChanged);
            this.lbStartPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LbStartPageKeyPress);
            // 
            // lbStopPage
            // 
            this.lbStopPage.Lines = new string[0];
            resources.ApplyResources(this.lbStopPage, "lbStopPage");
            this.lbStopPage.MaxLength = 9;
            this.lbStopPage.Name = "lbStopPage";
            this.lbStopPage.PasswordChar = '\0';
            this.lbStopPage.PromptText = "Stop #";
            this.lbStopPage.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.lbStopPage.SelectedText = "";
            this.lbStopPage.Style = MetroFramework.MetroColorStyle.Teal;
            this.lbStopPage.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.lbStopPage.UseSelectable = true;
            this.lbStopPage.TextChanged += new System.EventHandler(this.LbStopPageTextChanged);
            this.lbStopPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LbStopPageKeyPress);
            // 
            // lbSeperator
            // 
            resources.ApplyResources(this.lbSeperator, "lbSeperator");
            this.lbSeperator.Name = "lbSeperator";
            this.lbSeperator.Style = MetroFramework.MetroColorStyle.Teal;
            this.lbSeperator.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.lbSeperator.UseStyleColors = true;
            // 
            // lbTimer
            // 
            resources.ApplyResources(this.lbTimer, "lbTimer");
            this.lbTimer.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lbTimer.Name = "lbTimer";
            // 
            // stopwatchTimer
            // 
            this.stopwatchTimer.Interval = 10;
            this.stopwatchTimer.Tick += new System.EventHandler(this.StopwatchTimerTick);
            // 
            // btStop
            // 
            resources.ApplyResources(this.btStop, "btStop");
            this.btStop.Name = "btStop";
            this.btStop.Style = MetroFramework.MetroColorStyle.Teal;
            this.btStop.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btStop.UseSelectable = true;
            this.btStop.UseStyleColors = true;
            this.btStop.Click += new System.EventHandler(this.BtStopClick);
            // 
            // loadingSpinner
            // 
            resources.ApplyResources(this.loadingSpinner, "loadingSpinner");
            this.loadingSpinner.Maximum = 100;
            this.loadingSpinner.Name = "loadingSpinner";
            this.loadingSpinner.Speed = 2F;
            this.loadingSpinner.Style = MetroFramework.MetroColorStyle.Teal;
            this.loadingSpinner.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.loadingSpinner.UseSelectable = true;
            this.loadingSpinner.Value = 40;
            // 
            // cbSite
            // 
            this.cbSite.FormattingEnabled = true;
            resources.ApplyResources(this.cbSite, "cbSite");
            this.cbSite.Items.AddRange(new object[] {
            resources.GetString("cbSite.Items"),
            resources.GetString("cbSite.Items1"),
            resources.GetString("cbSite.Items2")});
            this.cbSite.Name = "cbSite";
            this.cbSite.PromptText = "Site";
            this.cbSite.Style = MetroFramework.MetroColorStyle.Teal;
            this.cbSite.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.cbSite.UseSelectable = true;
            this.cbSite.UseStyleColors = true;
            this.cbSite.SelectedIndexChanged += new System.EventHandler(this.CbSiteSelectedIndexChanged);
            // 
            // metroRadioButton1
            // 
            resources.ApplyResources(this.metroRadioButton1, "metroRadioButton1");
            this.metroRadioButton1.Name = "metroRadioButton1";
            this.metroRadioButton1.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroRadioButton1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroRadioButton1.UseSelectable = true;
            // 
            // metroRadioButton2
            // 
            resources.ApplyResources(this.metroRadioButton2, "metroRadioButton2");
            this.metroRadioButton2.Name = "metroRadioButton2";
            this.metroRadioButton2.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroRadioButton2.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroRadioButton2.UseSelectable = true;
            // 
            // metroRadioButton3
            // 
            resources.ApplyResources(this.metroRadioButton3, "metroRadioButton3");
            this.metroRadioButton3.Name = "metroRadioButton3";
            this.metroRadioButton3.Style = MetroFramework.MetroColorStyle.Teal;
            this.metroRadioButton3.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.metroRadioButton3.UseSelectable = true;
            this.metroRadioButton3.CheckedChanged += new System.EventHandler(this.MetroRadioButton3CheckedChanged);
            // 
            // btImport
            // 
            resources.ApplyResources(this.btImport, "btImport");
            this.btImport.Name = "btImport";
            this.btImport.Style = MetroFramework.MetroColorStyle.Teal;
            this.btImport.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btImport.UseSelectable = true;
            this.btImport.UseStyleColors = true;
            this.btImport.Click += new System.EventHandler(this.BtImportClick);
            // 
            // btExport
            // 
            resources.ApplyResources(this.btExport, "btExport");
            this.btExport.Name = "btExport";
            this.btExport.Style = MetroFramework.MetroColorStyle.Teal;
            this.btExport.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btExport.UseSelectable = true;
            this.btExport.UseStyleColors = true;
            this.btExport.Click += new System.EventHandler(this.BtExportClick);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackImage = global::WZScraper.Properties.Resources.WZ_Scraper;
            this.BackImagePadding = new System.Windows.Forms.Padding(0, 13, 57, 0);
            this.BackLocation = MetroFramework.Forms.BackLocation.TopRight;
            this.BackMaxSize = 50;
            this.Controls.Add(this.btExport);
            this.Controls.Add(this.btImport);
            this.Controls.Add(this.metroRadioButton3);
            this.Controls.Add(this.metroRadioButton2);
            this.Controls.Add(this.metroRadioButton1);
            this.Controls.Add(this.cbSite);
            this.Controls.Add(this.loadingSpinner);
            this.Controls.Add(this.btStop);
            this.Controls.Add(this.lbTimer);
            this.Controls.Add(this.lbStopPage);
            this.Controls.Add(this.lbStartPage);
            this.Controls.Add(this.lbCount);
            this.Controls.Add(this.ltbUsernames);
            this.Controls.Add(this.btScrape);
            this.Controls.Add(this.cbRegion);
            this.Controls.Add(this.lbSeperator);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.DropShadow;
            this.Style = MetroFramework.MetroColorStyle.Teal;
            this.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
            this.Load += new System.EventHandler(this.MainFormLoad);
            this.contextMenuListBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroComboBox cbRegion;
        private MetroFramework.Controls.MetroButton btScrape;
        private System.Windows.Forms.ListBox ltbUsernames;
        private System.Windows.Forms.Label lbCount;
        private MetroFramework.Controls.MetroTextBox lbStartPage;
        private MetroFramework.Controls.MetroTextBox lbStopPage;
        private MetroFramework.Controls.MetroLabel lbSeperator;
        private System.Windows.Forms.Label lbTimer;
        private System.Windows.Forms.Timer stopwatchTimer;
        private MetroFramework.Controls.MetroButton btStop;
        private MetroFramework.Controls.MetroProgressSpinner loadingSpinner;
        private MetroFramework.Controls.MetroComboBox cbSite;
        private System.Windows.Forms.ContextMenuStrip contextMenuListBox;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearAllToolStripMenuItem;
        private MetroFramework.Controls.MetroRadioButton metroRadioButton1;
        private MetroFramework.Controls.MetroRadioButton metroRadioButton2;
        private MetroFramework.Controls.MetroRadioButton metroRadioButton3;
        private MetroFramework.Controls.MetroButton btImport;
        private MetroFramework.Controls.MetroButton btExport;
    }
}

