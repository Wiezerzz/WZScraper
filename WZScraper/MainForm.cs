using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Forms;

namespace WZScraper
{
    public partial class MainForm : MetroForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private string LadderURL = "";
        private int curSiteID;

        CancellationTokenSource _cts;
        Stopwatch stopwatch = new Stopwatch();
        List<string> passList = new List<string>(); 

        private void btScrape_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Application.ProductVersion);
            //Stupid visual focus problem with buttons.
            lbCount.Focus();

            //Trimming to make sure people don't copy paste white spaces.
            lbStartPage.Text = lbStartPage.Text.Trim();
            lbStopPage.Text = lbStopPage.Text.Trim();

            //Check if region is selected.
            if (cbRegion.Text == "")
            {
                MetroMessageBox.Show(
                    this, "You forgot to select a region", "Whoops!", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            //Check if page numbers are specified
            if (lbStartPage.Text == "" || lbStopPage.Text == "")
            {
                MetroMessageBox.Show(this, "You forgot to choose page numbers.", "Whoops!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //Check if moron or nah.
            if (int.Parse(lbStopPage.Text) < int.Parse(lbStartPage.Text))
            {
                MetroMessageBox.Show(this, "Math is hard man..." + Environment.NewLine + "Start Page # cannot be higher than the Stop Page #", "Moron!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Don't allow more than 300k pages to be scraped(Low memory).
            if (int.Parse(lbStopPage.Text) - int.Parse(lbStartPage.Text) > 300000)
            {
                MetroMessageBox.Show(
                    this,
                    "You can only scrape 300k pages at a time cause of memory limitation." + Environment.NewLine +
                    "The only time you need to scrape more than 300k pages is when you want to scrape all GLOBAL usernames from LoL Summoners.",
                    "Careful!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //Enable the stopwatch obviously.
            stopwatchTimer.Enabled = true;

            //Start the async thread.
            Thread mainThread = new Thread(new ThreadStart(StartScraping));
            mainThread.Start();
        }

        async void StartScraping()
        {
            //Stopwatch starting.
            stopwatch.Reset();
            stopwatch.Start();

            //GUI stuff
            Invoke(
                (MethodInvoker) delegate()
                {
                    btScrape.Enabled = false;
                    btStop.Enabled = true;
                    cbRegion.Enabled = false;
                    cbSite.Enabled = false;
                    lbStartPage.Enabled = false;
                    lbStopPage.Enabled = false;
                    loadingSpinner.Style = MetroColorStyle.Teal;
                    loadingSpinner.Visible = true;
                });

            //Creating Cancellation Token.
            _cts = new CancellationTokenSource();

            //Creating tasks.
            Task[] tasks = new Task[int.Parse(lbStopPage.Text) - int.Parse(lbStartPage.Text) + 1];
            for (int i = 0; i < int.Parse(lbStopPage.Text) - int.Parse(lbStartPage.Text) + 1; i++)
            {
                var i1 = int.Parse(lbStartPage.Text) + i;
                tasks[i] = Task.Factory.StartNew(() => ScrapePage(i1, curSiteID, _cts.Token));
            }

            //---Wait till all the tasks are completed, then continue.
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            stopwatchTimer.Enabled = false;

            //GUI stuff
            Invoke(
                (MethodInvoker) delegate()
                {
                    btScrape.Enabled = true;
                    btStop.Enabled = false;
                    cbRegion.Enabled = true;
                    cbSite.Enabled = true;
                    lbStartPage.Enabled = true;
                    lbStopPage.Enabled = true;
                    loadingSpinner.Visible = false;
                });
            //---END

            //If Cancellation is not request, show a message box that all pages have been scraped.
            if (!_cts.IsCancellationRequested)
                MetroMessageBox.Show(this, "All pages has been scraped, ready to export!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        async Task ScrapePage(int pageNumber, int siteType, CancellationToken ct)
        {
            await Task.Run(
                () =>
                {
                    ct.ThrowIfCancellationRequested();


                    WebClient webClient = new WebClient { Encoding = Encoding.UTF8 };

                    if (siteType == 0 || siteType == 1)
                    {
                        MatchCollection Matches = null;
                        if (siteType == 0)
                        {
                            Matches =
                                new Regex(@"<td class=""name""><a href=""/leagues/.*"">(.*)</a></td>").Matches(
                                    webClient.DownloadString(LadderURL + pageNumber));
                        }
                        else if (siteType == 1)
                        {
                            Matches =
                                new Regex("class=\\\"ajax-tooltip shadow radius lazy\\\" alt=\\\"\\\">(.*)</a>").Matches
                                    (
                                        webClient.DownloadString(
                                            LadderURL + "?q=analyze%2Franking%2Fpro-ism-ranking%2F1&page=" + pageNumber));
                        }

                        List<string> namesList = new List<string>();

                        foreach (Match singleMatch in Matches)
                        {
                            string cleanMatch = singleMatch.Groups[1].Value.Replace(" ", string.Empty);
                            if (cleanMatch.Length < 24 && cleanMatch.Length > 4)
                            {
                                namesList.Add(RemoveDiacritics(cleanMatch));
                            }
                        }

                        Invoke(
                            (MethodInvoker) delegate()
                            {
                                ltbUsernames.Items.AddRange(namesList.ToArray());
                                lbCount.Text = "Usernames: " + ltbUsernames.Items.Count.ToString();
                            });
                    }
                    else if (siteType == 2)
                    {
                        string htmlsource = webClient.DownloadString(LadderURL + pageNumber + ".json");
                        LoLKing jsonLoLKing = new JavaScriptSerializer().Deserialize<LoLKing>(htmlsource);

                        List<string> namesList = new List<string>();

                        foreach (Usernames usernames in jsonLoLKing.data)
                        {
                            string cleanMatch = usernames.name.Replace(" ", string.Empty);
                            if (cleanMatch.Length < 24 && cleanMatch.Length > 4)
                            {
                                namesList.Add(RemoveDiacritics(cleanMatch));
                            }
                        }

                                            Invoke(
                        (MethodInvoker)delegate()
                        {
                            ltbUsernames.Items.AddRange(namesList.ToArray());
                            lbCount.Text = "Usernames: " + ltbUsernames.Items.Count.ToString();
                        });
                    }
                }, ct);
        }

        #region Form Events
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(_cts != null)
                _cts.Cancel();
        }
        #endregion

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        #region Controls
        private void cbRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSite.SelectedIndex == 0)
            {
                switch (cbRegion.Text)
                {
                    case "NA":
                        LadderURL = "http://www.lolsummoners.com/ladders/na/";
                        break;
                    case "EUW":
                        LadderURL = "http://www.lolsummoners.com/ladders/euw/";
                        break;
                    case "EUNE":
                        LadderURL = "http://www.lolsummoners.com/ladders/eune/";
                        break;
                    case "BR":
                        LadderURL = "http://www.lolsummoners.com/ladders/br/";
                        break;
                    case "LAN":
                        LadderURL = "http://www.lolsummoners.com/ladders/lan/";
                        break;
                    case "LAS":
                        LadderURL = "http://www.lolsummoners.com/ladders/las/";
                        break;
                    case "OCE":
                        LadderURL = "http://www.lolsummoners.com/ladders/oce/";
                        break;
                    case "RU":
                        LadderURL = "http://www.lolsummoners.com/ladders/ru/";
                        break;
                    case "TR":
                        LadderURL = "http://www.lolsummoners.com/ladders/tr/";
                        break;
                    case "KR":
                        LadderURL = "http://www.lolsummoners.com/ladders/kr/";
                        break;
                    case "GLOBAL":
                        LadderURL = "http://www.lolsummoners.com/ladders/all/";
                        break;
                }
            }
            else if (cbSite.SelectedIndex == 1)
            {
                switch (cbRegion.Text)
                {
                    case "NA":
                        LadderURL = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/1";
                        break;
                    case "EUW":
                        LadderURL = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/2";
                        break;
                    case "EUNE":
                        LadderURL = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/3";
                        break;
                    case "BR":
                        LadderURL = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/4";
                        break;
                    case "LAN":
                        LadderURL = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/5";
                        break;
                    case "LAS":
                        LadderURL = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/6";
                        break;
                    case "OCE":
                        LadderURL = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/7";
                        break;
                    case "RU":
                        LadderURL = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/8";
                        break;
                    case "TR":
                        LadderURL = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/9";
                        break;
                    case "KR":
                        LadderURL = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/10";
                        break;
                    case "GLOBAL":
                        cbRegion.SelectedIndex = 0;
                        MetroMessageBox.Show(
                            this, "LOLDB does not support GLOBAL.. :(", "Whoops!", MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        break;
                }
            }
            else if (cbSite.SelectedIndex == 2)
            {
                switch (cbRegion.Text)
                {
                    case "NA":
                        LadderURL = "http://lolking.net/leaderboards/wiezerzz/na/";
                        break;
                    case "EUW":
                        LadderURL = "http://lolking.net/leaderboards/wiezerzz/euw/";
                        break;
                    case "EUNE":
                        LadderURL = "http://lolking.net/leaderboards/wiezerzz/eune/";
                        break;
                    case "BR":
                        LadderURL = "http://lolking.net/leaderboards/wiezerzz/br/";
                        break;
                    case "LAN":
                        LadderURL = "http://lolking.net/leaderboards/wiezerzz/lan/";
                        break;
                    case "LAS":
                        LadderURL = "http://lolking.net/leaderboards/wiezerzz/las/";
                        break;
                    case "OCE":
                        LadderURL = "http://lolking.net/leaderboards/wiezerzz/oce/";
                        break;
                    case "RU":
                        LadderURL = "http://lolking.net/leaderboards/wiezerzz/ru/";
                        break;
                    case "TR":
                        LadderURL = "http://lolking.net/leaderboards/wiezerzz/tr/";
                        break;
                    case "KR":
                        cbRegion.SelectedIndex = 0;
                        MetroMessageBox.Show(
                            this, "LoLKing does not support KR(Korean).. :(", "Whoops!", MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        break;
                    case "GLOBAL":
                        cbRegion.SelectedIndex = 0;
                        MetroMessageBox.Show(
                            this, "LoLKing does not support GLOBAL.. :(", "Whoops!", MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        break;
                }   
            }
        }

        private void lbStartPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) && e.KeyChar == 8)
                e.Handled = false;
            else
            {
                e.Handled = true;
            }
        }

        private void lbStartPage_TextChanged(object sender, EventArgs e)
        {
            int parsedValue;
            if (!int.TryParse(lbStartPage.Text, out parsedValue) && lbStartPage.Text != "")
            {
                lbStartPage.Text = "";
            }
        }

        private void lbStopPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) && e.KeyChar == 8)
                e.Handled = false;
            else
            {
                e.Handled = true;
            }
        }

        private void lbStopPage_TextChanged(object sender, EventArgs e)
        {
            int parsedValue;
            if (!int.TryParse(lbStopPage.Text, out parsedValue) && lbStopPage.Text != "")
            {
                lbStopPage.Text = "";
            }
        }
        #endregion

        private void stopwatchTimer_Tick(object sender, EventArgs e)
        {
            lbTimer.Text = "Elapsed Time: " + stopwatch.Elapsed.ToString("mm\\:ss\\.ff");
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            //Stupid visual focus problem with buttons.
            lbCount.Focus();

            Invoke((MethodInvoker)delegate()
            { loadingSpinner.Style = MetroColorStyle.Yellow; });
            if (_cts != null)
                _cts.Cancel();
        }

        private void cbSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cbSite.Text))
            {
                curSiteID = cbSite.SelectedIndex;
                cbRegion.Enabled = true;
            }
            else
            {
                cbRegion.Enabled = false;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ltbUsernames.SelectedIndex > -1)
            Clipboard.SetText(ltbUsernames.SelectedItem.ToString());
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ltbUsernames.SelectedIndex > -1)
            ltbUsernames.Items.Remove(ltbUsernames.SelectedItem);
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ltbUsernames.Items.Count > 0)
                ltbUsernames.Items.Clear();
            lbCount.Text = "Usernames: " + ltbUsernames.Items.Count.ToString();
        }

        private void metroRadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (metroRadioButton3.Checked)
                btImport.Enabled = true;
            else
            {
                btImport.Enabled = false;
            }
        }

        public static bool IsAlphaNum(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            bool conLetter = false;
            bool conNumber = false;

            for (int i = 0; i < str.Length; i++)
            {
                if ((char.IsLetter(str[i])))
                conLetter = true;

                if (char.IsNumber(str[i]))
                    conNumber = true;
            }

            if (conLetter && conNumber)
                return true;
            else
            {
                return false;
            }
        }

        private void btImport_Click(object sender, EventArgs e)
        {
            //Stupid visual focus problem with buttons.
            lbCount.Focus();

            passList.Clear();

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text Files|*.txt|All Files|*.*";
                ofd.FileName = "";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamReader passStreamReader = new StreamReader(ofd.FileName))
                    {
                        for (string str = passStreamReader.ReadLine(); !string.IsNullOrEmpty(str); str = passStreamReader.ReadLine())
                        {
                            str = str.Trim();
                            if (str.Length < 24 && str.Length > 6 && IsAlphaNum(str))
                            {
                                passList.Add(str);
                            }
                        }
                        MetroMessageBox.Show(
                            this, "All passwords(" + passList.Count + ") have been imported!", "Imported!", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void btExport_Click(object sender, EventArgs e)
        {
            //Stupid visual focus problem with buttons.
            lbCount.Focus();

            if (ltbUsernames.Items.Count <= 0)
            {
                MetroMessageBox.Show(
                    this, "The scraped usernames list is empty, you cannot export a empty list!", "Error!", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }


            if (metroRadioButton1.Checked)
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Text Files|*.txt|All Files|*.*";
                    sfd.FileName = "";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter writer = new StreamWriter(sfd.FileName))
                        {
                            foreach (string listboxItem in ltbUsernames.Items)
                            {
                                writer.Write(listboxItem + Environment.NewLine);
                            }
                        }
                        MetroMessageBox.Show(
                            this, "Usernames have been exported!", "Completed!", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            else if (metroRadioButton2.Checked)
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Text Files|*.txt|All Files|*.*";
                    sfd.FileName = "";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter writer = new StreamWriter(sfd.FileName))
                        {
                            foreach (string listboxItem in ltbUsernames.Items)
                            {
                                if (IsAlphaNum(listboxItem))
                                {
                                    writer.Write(listboxItem + ':' + listboxItem + Environment.NewLine);
                                }
                                writer.Write(listboxItem + ':' + listboxItem + '1' + Environment.NewLine);
                                writer.Write(listboxItem + ':' + listboxItem + "12" + Environment.NewLine);
                                writer.Write(listboxItem + ':' + listboxItem + "123" + Environment.NewLine);
                            }
                        }
                        MetroMessageBox.Show(
                            this, "Username:pass[numbers(123)] list has been exported!", "Completed!", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            else if (metroRadioButton3.Checked)
            {
                if (passList.Count <= 0)
                {
                    MetroMessageBox.Show(
                        this, "You need to import a password list first.", "Error!", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "Text Files|*.txt|All Files|*.*";
                        sfd.FileName = "";
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            using (StreamWriter writer = new StreamWriter(sfd.FileName))
                            {
                                foreach (string username in ltbUsernames.Items)
                                {
                                    foreach (string password0 in passList)
                                    {
                                        string password = password0.ToLower().Replace("%user%", username);

                                        if (password.Length < 24 && password.Length > 6 && IsAlphaNum(password))
                                        writer.Write(username + ':' + password + Environment.NewLine);
                                    }
                                }
                            }
                            MetroMessageBox.Show(
                                this, "Username:pass list has been exported!", "Completed!", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }
    }
}
