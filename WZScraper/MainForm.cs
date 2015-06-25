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

        private string _ladderUrl = "";
        private int _curSiteId;

        CancellationTokenSource _cts;
        Stopwatch stopwatch = new Stopwatch();
        List<string> passList = new List<string>();


        #region Scraping
        private void BtScrapeClick(object sender, EventArgs e)
        {
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

            if (IsSiteOffline())
                return;

            //Enable the stopwatch obviously.
            stopwatchTimer.Enabled = true;

            //Start the async thread.
            Thread mainThread = new Thread(StartScraping);
            mainThread.Start();
        }

        private bool IsSiteOffline()
        {
            HttpStatusCode copyStatusCode;
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(_ladderUrl);
                webRequest.AllowAutoRedirect = false;
                HttpWebResponse response = (HttpWebResponse) webRequest.GetResponse();
                copyStatusCode = response.StatusCode;
            }
            catch (WebException we)
            {
                copyStatusCode = ((HttpWebResponse)we.Response).StatusCode;
            }

            Console.WriteLine((int)copyStatusCode);
            if ((int)copyStatusCode != 200 && (int)copyStatusCode != 301)
            {
                DialogResult dialogResult = MetroMessageBox.Show(
                    this,
                    "It appears that " + cbSite.Text + " might be offline. (" + (int)copyStatusCode + "/" + copyStatusCode.ToString() + ")" + Environment.NewLine + Environment.NewLine +
                    "Do you want to continue? (If you get no usernames than the website is offline.", "Site offline?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (dialogResult == DialogResult.Yes)
                    return false;
                else if(dialogResult == DialogResult.No)
                    return true;
            }

            return false;
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
                tasks[i] = Task.Factory.StartNew(() => ScrapePage(i1, _curSiteId, _cts.Token));
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
                        MatchCollection matches = null;
                        if (siteType == 0)
                        {
                            matches =
                                new Regex(@"<td class=""name""><a href=""/leagues/.*"">(.*)</a></td>").Matches(
                                    webClient.DownloadString(_ladderUrl + pageNumber));
                        }
                        else if (siteType == 1)
                        {
                            matches =
                                new Regex("class=\\\"ajax-tooltip shadow radius lazy\\\" alt=\\\"\\\">(.*)</a>").Matches
                                    (
                                        webClient.DownloadString(
                                            _ladderUrl + "?q=analyze%2Franking%2Fpro-ism-ranking%2F1&page=" + pageNumber));
                        }

                        List<string> namesList = new List<string>();

                        if (matches != null)
                        {
                            foreach (Match singleMatch in matches)
                            {
                                string cleanMatch = singleMatch.Groups[1].Value.Replace(" ", string.Empty);
                                if (cleanMatch.Length <= 24 && cleanMatch.Length >= 4)
                                {
                                    namesList.Add(RemoveDiacritics(cleanMatch));
                                }
                            }
                        }
                            if (ltbUsernames != null)
                            {

                                if (!ltbUsernames.IsHandleCreated)
                                    ltbUsernames.CreateControl();
                                ltbUsernames.Invoke(
                                    (MethodInvoker) delegate { ltbUsernames.Items.AddRange(namesList.ToArray()); });

                                if (!lbCount.IsHandleCreated)
                                    lbCount.CreateControl();
                                lbCount.Invoke(
                                    (MethodInvoker)
                                        delegate
                                        {
                                            lbCount.Text = string.Format(
                                                "Usernames: {0}", ltbUsernames.Items.Count.ToString());
                                        });
                            }
                    }
                    else if (siteType == 2)
                    {
                        string htmlsource = webClient.DownloadString(_ladderUrl + pageNumber + ".json");
                        LoLKing jsonLoLKing = new JavaScriptSerializer().Deserialize<LoLKing>(htmlsource);

                        List<string> namesList = new List<string>();

                        foreach (Usernames usernames in jsonLoLKing.Data)
                        {
                            string cleanMatch = usernames.Name.Replace(" ", string.Empty);
                            if (cleanMatch.Length <= 24 && cleanMatch.Length >= 4)
                            {
                                namesList.Add(RemoveDiacritics(cleanMatch));
                            }
                        }

                        if (!ltbUsernames.IsHandleCreated)
                            ltbUsernames.CreateControl();
                        ltbUsernames.Invoke(
                            (MethodInvoker)delegate { ltbUsernames.Items.AddRange(namesList.ToArray()); });

                        if (!lbCount.IsHandleCreated)
                            lbCount.CreateControl();
                        lbCount.Invoke(
                            (MethodInvoker)
                                delegate
                                {
                                    lbCount.Text = string.Format(
                                        "Usernames: {0}", ltbUsernames.Items.Count.ToString());
                                });
                    }
                }, ct);
        }

        #endregion

        #region Import and Export
        private void BtImportClick(object sender, EventArgs e)
        {
            //Stupid visual focus problem with buttons.
            lbCount.Focus();

            passList.Clear();

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = @"Text Files|*.txt|All Files|*.*";
                ofd.FileName = "";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamReader passStreamReader = new StreamReader(ofd.FileName))
                    {
                        for (string str = passStreamReader.ReadLine(); !string.IsNullOrEmpty(str); str = passStreamReader.ReadLine())
                        {
                            str = str.Trim();
                            if (!str.ToLower().Contains("%user%"))
                            {
                                if (str.Length <= 16 && str.Length >= 6 && IsAlphaNum(str))
                                {
                                    passList.Add(str);
                                }
                            }
                            else if (str.ToLower().Contains("%user%"))
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

        private void BtExportClick(object sender, EventArgs e)
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
                    sfd.Filter = @"Text Files|*.txt|All Files|*.*";
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
                    sfd.Filter = @"Text Files|*.txt|All Files|*.*";
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
                                writer.Write(listboxItem + ':' + listboxItem + "1234" + Environment.NewLine);
                                writer.Write(listboxItem + ':' + listboxItem + "12345" + Environment.NewLine);								
                            }
                        }
                        MetroMessageBox.Show(
                            this, "Username:pass[numbers(12345)] list has been exported!", "Completed!", MessageBoxButtons.OK,
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
                }
                else
                {
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = @"Text Files|*.txt|All Files|*.*";
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

                                        if (password.Length <= 16 && password.Length >= 6 && IsAlphaNum(password))
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
            else if (metroRadioButton4.Checked)
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = @"Text Files|*.txt|All Files|*.*";
                    sfd.FileName = "";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (StreamWriter writer = new StreamWriter(sfd.FileName))
                        {
                            foreach (string listboxItem in ltbUsernames.Items)
                            {
                                writer.Write(listboxItem + ':' + listboxItem + Environment.NewLine);
                            }
                        }
                        MetroMessageBox.Show(
                            this, "username:username list has been exported!", "Completed!", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
        }
        #endregion

        #region Form Events
        private void MainFormLoad(object sender, EventArgs e)
        {
            //Basic Update checker
            Thread updateThread = new Thread(
                delegate()
                {
                    string dVersion =
                        new WebClient().DownloadString(
                            "https://raw.githubusercontent.com/Wiezerzz/WZScraper/master/version.txt").Trim();
                    if (Application.ProductVersion != dVersion)
                    {
                        MetroMessageBox.Show(
                            this,
                            "You have an older version(" + Application.ProductVersion + "). Go to: https://github.com/Wiezerzz/WZScraper/releases to download the latest version(" + dVersion + ").",
                            "Outdated!");
                    }
                });

            updateThread.IsBackground = true;
            updateThread.Start();
            //---END
        }

        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            //Cancel all threads when user presses close.
            if(_cts != null)
                _cts.Cancel();
        }
        #endregion

        #region Utils
        static string RemoveDiacritics(string text)
        {
            //This removes accents and weird shit from the string.

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

        public static bool IsAlphaNum(string str)
        {
            //Check if string is AlphaNumeric(Contains letters AND numbers), This method is used cause Regex didn't work.

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

        #endregion

        #region Controls
        private void CbRegionSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_curSiteId == 0)
            {
                switch (cbRegion.Text)
                {
                    case "NA":
                        _ladderUrl = "http://www.lolsummoners.com/ladders/na/";
                        break;
                    case "EUW":
                        _ladderUrl = "http://www.lolsummoners.com/ladders/euw/";
                        break;
                    case "EUNE":
                        _ladderUrl = "http://www.lolsummoners.com/ladders/eune/";
                        break;
                    case "BR":
                        _ladderUrl = "http://www.lolsummoners.com/ladders/br/";
                        break;
                    case "LAN":
                        _ladderUrl = "http://www.lolsummoners.com/ladders/lan/";
                        break;
                    case "LAS":
                        _ladderUrl = "http://www.lolsummoners.com/ladders/las/";
                        break;
                    case "OCE":
                        _ladderUrl = "http://www.lolsummoners.com/ladders/oce/";
                        break;
                    case "RU":
                        _ladderUrl = "http://www.lolsummoners.com/ladders/ru/";
                        break;
                    case "TR":
                        _ladderUrl = "http://www.lolsummoners.com/ladders/tr/";
                        break;
                    case "KR":
                        _ladderUrl = "http://www.lolsummoners.com/ladders/kr/";
                        break;
                    case "GLOBAL":
                        _ladderUrl = "http://www.lolsummoners.com/ladders/all/";
                        break;
                }
            }
            else if (_curSiteId == 1)
            {
                switch (cbRegion.Text)
                {
                    case "NA":
                        _ladderUrl = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/1";
                        break;
                    case "EUW":
                        _ladderUrl = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/2";
                        break;
                    case "EUNE":
                        _ladderUrl = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/3";
                        break;
                    case "BR":
                        _ladderUrl = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/4";
                        break;
                    case "LAN":
                        _ladderUrl = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/5";
                        break;
                    case "LAS":
                        _ladderUrl = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/6";
                        break;
                    case "OCE":
                        _ladderUrl = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/7";
                        break;
                    case "RU":
                        _ladderUrl = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/8";
                        break;
                    case "TR":
                        _ladderUrl = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/9";
                        break;
                    case "KR":
                        _ladderUrl = "http://loldb.gameguyz.com/analyze/ranking/pro-ism-ranking/10";
                        break;
                    case "GLOBAL":
                        cbRegion.SelectedIndex = 0;
                        MetroMessageBox.Show(
                            this, "LOLDB does not support GLOBAL.. :(", "Whoops!", MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        break;
                }
            }
            else if (_curSiteId == 2)
            {
                switch (cbRegion.Text)
                {
                    case "NA":
                        _ladderUrl = "http://lolking.net/leaderboards/wiezerzz/na/";
                        break;
                    case "EUW":
                        _ladderUrl = "http://lolking.net/leaderboards/wiezerzz/euw/";
                        break;
                    case "EUNE":
                        _ladderUrl = "http://lolking.net/leaderboards/wiezerzz/eune/";
                        break;
                    case "BR":
                        _ladderUrl = "http://lolking.net/leaderboards/wiezerzz/br/";
                        break;
                    case "LAN":
                        _ladderUrl = "http://lolking.net/leaderboards/wiezerzz/lan/";
                        break;
                    case "LAS":
                        _ladderUrl = "http://lolking.net/leaderboards/wiezerzz/las/";
                        break;
                    case "OCE":
                        _ladderUrl = "http://lolking.net/leaderboards/wiezerzz/oce/";
                        break;
                    case "RU":
                        _ladderUrl = "http://lolking.net/leaderboards/wiezerzz/ru/";
                        break;
                    case "TR":
                        _ladderUrl = "http://lolking.net/leaderboards/wiezerzz/tr/";
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

        private void LbStartPageKeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) && e.KeyChar == 8)
                e.Handled = false;
            else
            {
                e.Handled = true;
            }
        }

        private void LbStartPageTextChanged(object sender, EventArgs e)
        {
            int parsedValue;
            if (!int.TryParse(lbStartPage.Text, out parsedValue) && lbStartPage.Text != "")
            {
                lbStartPage.Text = "";
            }
        }

        private void LbStopPageKeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) && e.KeyChar == 8)
                e.Handled = false;
            else
            {
                e.Handled = true;
            }
        }

        private void LbStopPageTextChanged(object sender, EventArgs e)
        {
            int parsedValue;
            if (!int.TryParse(lbStopPage.Text, out parsedValue) && lbStopPage.Text != "")
            {
                lbStopPage.Text = "";
            }
        }

        private void StopwatchTimerTick(object sender, EventArgs e)
        {
            lbTimer.Text = string.Format("Elapsed Time: {0}", stopwatch.Elapsed.ToString("mm\\:ss\\.ff"));
        }

        private void BtStopClick(object sender, EventArgs e)
        {
            //Stupid visual focus problem with buttons.
            lbCount.Focus();

            Invoke((MethodInvoker)delegate { loadingSpinner.Style = MetroColorStyle.Yellow; });
            if (_cts != null)
                _cts.Cancel();
        }

        private void CbSiteSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cbSite.Text))
            {
                _curSiteId = cbSite.SelectedIndex;
                CbRegionSelectedIndexChanged(this, EventArgs.Empty); //fixed.
                cbRegion.Enabled = true;
            }
            else
            {
                cbRegion.Enabled = false;
            }
        }

        private void CopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (ltbUsernames.SelectedIndex > -1)
            Clipboard.SetText(ltbUsernames.SelectedItem.ToString());
        }

        private void DeleteToolStripMenuItemClick(object sender, EventArgs e)
        {
            if(ltbUsernames.SelectedIndex > -1)
            ltbUsernames.Items.Remove(ltbUsernames.SelectedItem);
        }

        private void ClearAllToolStripMenuItemClick(object sender, EventArgs e)
        {
            if(ltbUsernames.Items.Count > 0)
                ltbUsernames.Items.Clear();
            lbCount.Text = string.Format("Usernames: {0}", ltbUsernames.Items.Count.ToString());
        }

        private void MetroRadioButton3CheckedChanged(object sender, EventArgs e)
        {
            if (metroRadioButton3.Checked)
                btImport.Enabled = true;
            else
            {
                btImport.Enabled = false;
            }
        }
        #endregion

        private void btAdd_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = @"Text Files|*.txt|All Files|*.*";
                ofd.FileName = "";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var lines = File.ReadLines(ofd.FileName);
                    foreach (string str in lines)
                    {
                        string str1 = str.Trim();
                        if (!string.IsNullOrEmpty(str1) && str1.Length <= 24 && str1.Length >= 4)
                        {
                            ltbUsernames.Items.Add(str1);
                        }
                    }

                    lbCount.Text = "Usernames: " + ltbUsernames.Items.Count;
                        MetroMessageBox.Show(
                            this, "All usernames have been imported!", "Imported!", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    
                }
            }
        }
    }
}
