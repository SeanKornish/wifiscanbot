using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace WiFi_Scanbot
{
    public partial class Form1 : Form
    {
        List<WirelessNetwork> networks = new List<WirelessNetwork>();
        int count = 15;
        int refresh = 15;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += " " + Application.ProductVersion;
            LoadSettings();
            RefreshNetworkList();
        }        

        private void RefreshNetworkList()
        {
            GetWirelessNetworks();
            foreach (WirelessNetwork network in networks)
            {
                if (network.isNew)
                {
                    ListViewItem lvi = new ListViewItem(network.SSID);
                    lvi.SubItems.Add(network.Channel);
                    lvi.SubItems.Add(network.Encryption);
                    lvi.SubItems.Add(network.Authentication);
                    lvi.SubItems.Add(network.Signal);
                    lvi.SubItems.Add(network.Type);
                    lvi.SubItems.Add(network.RadioType);
                    lvi.SubItems.Add(network.LastSeen.ToString("R"));
                    lvi.SubItems.Add(network.FirstSeen.ToString("R"));
                    lstNetworks.Items.Add(lvi);
                }
                else
                {
                    for (int i = 0; i < lstNetworks.Items.Count; i++)
                    {
                        if (lstNetworks.Items[i].SubItems[0].Text == network.SSID)
                        {
                            lstNetworks.Items[i].SubItems[1].Text = network.Channel;
                            lstNetworks.Items[i].SubItems[2].Text = network.Encryption;
                            lstNetworks.Items[i].SubItems[3].Text = network.Authentication;
                            lstNetworks.Items[i].SubItems[4].Text = network.Signal;
                            lstNetworks.Items[i].SubItems[5].Text = network.Type;
                            lstNetworks.Items[i].SubItems[6].Text = network.RadioType;
                            lstNetworks.Items[i].SubItems[7].Text = network.LastSeen.ToString("R");
                            break;
                        }
                    }
                }
            }
        }

        private void GetWirelessNetworks()
        {
            List<WirelessNetwork> localList = new List<WirelessNetwork>();

            #region LoadNetworkList
            string lanData = NetshCommands.GetWirelessNetworksString();

            if (String.IsNullOrEmpty(lanData))
            {
                Console.WriteLine("No Data Recieved from GetNetworkString");
                return;
            }

            string[] strItems = lanData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            WirelessNetwork wn = new WirelessNetwork();
            foreach (string item in strItems)
            {
                string strItem = item.Trim();
                if (strItem.StartsWith("SSID"))
                {
                    wn = new WirelessNetwork();
                    wn.SSID = strItem.Split(':').GetValue(1).ToString().Trim();
                }
                else if (strItem.StartsWith("Network type"))
                    wn.Type = strItem.Split(':').GetValue(1).ToString().Trim();
                else if (strItem.StartsWith("Authentication"))
                    wn.Authentication = strItem.Split(':').GetValue(1).ToString().Trim();
                else if (strItem.StartsWith("Encryption"))
                    wn.Encryption = strItem.Split(':').GetValue(1).ToString().Trim();
                else if (strItem.StartsWith("BSSID"))
                    wn.BSSID = strItem.Substring(strItem.IndexOf(":") + 1).Trim();
                else if (strItem.StartsWith("Signal"))
                    wn.Signal = strItem.Split(':').GetValue(1).ToString().Trim();
                else if (strItem.StartsWith("Radio type"))
                    wn.RadioType = strItem.Split(':').GetValue(1).ToString().Trim();
                else if (strItem.StartsWith("Channel"))
                    wn.Channel = strItem.Split(':').GetValue(1).ToString().Trim();
                else if (strItem.StartsWith("Basic rates"))
                    wn.BasicRates = strItem.Split(':').GetValue(1).ToString().Trim();
                else if (strItem.StartsWith("Other rates"))
                {
                    wn.OtherRates = strItem.Split(':').GetValue(1).ToString().Trim();
                    wn.LastSeen = DateTime.Now;
                    localList.Add(wn);
                }
            } 
            #endregion

            #region UpdateList
            foreach (WirelessNetwork newNetwork in localList)
            {
                bool isInNetworkList = false;

                foreach (WirelessNetwork oldNetwork in networks)
                {
                    if (newNetwork.SSID == oldNetwork.SSID)
                    {
                        isInNetworkList = true;
                        oldNetwork.Channel = newNetwork.Channel;
                        oldNetwork.Authentication = newNetwork.Authentication;
                        oldNetwork.BasicRates = newNetwork.BasicRates;
                        oldNetwork.BSSID = newNetwork.BSSID;
                        oldNetwork.Encryption = newNetwork.Encryption;
                        oldNetwork.LastSeen = newNetwork.LastSeen;
                        oldNetwork.OtherRates = newNetwork.OtherRates;
                        oldNetwork.RadioType = newNetwork.RadioType;
                        oldNetwork.Signal = newNetwork.Signal;
                        oldNetwork.Type = newNetwork.Type;
                        oldNetwork.isNew = false;
                        break;
                    }
                }

                if (!isInNetworkList)
                {
                    newNetwork.FirstSeen = DateTime.Now;
                    newNetwork.isNew = true;
                    networks.Add(newNetwork);
                }
            } 
            #endregion
        }        

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            
        }

        private void LoadSettings()
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (count == 0)
            {
                tsStatus.Text = "Refreshing...";
                Application.DoEvents();
                RefreshNetworkList();
                count = refresh;
            }
            else
            {
                tsStatus.Text = string.Format("Refreshing in {0} seconds.", count);
                Application.DoEvents();
                count--;
            }                    
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Application.Exit();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipData = string.Empty;
            for (int i = 0; i < lstNetworks.SelectedIndices.Count; i++)
            {
                foreach (ListViewItem.ListViewSubItem item in lstNetworks.SelectedItems[i].SubItems)
                {
                    clipData = String.Format("{0}{1}, ", clipData, item.Text);
                }
                lstNetworks.Items.RemoveAt(lstNetworks.SelectedIndices[i]);

                clipData = clipData + Environment.NewLine;
            }

            if (clipData.EndsWith(Environment.NewLine))
                clipData = clipData.Substring(0, clipData.Length - 4);

            if (!string.IsNullOrEmpty(clipData))
                Clipboard.SetText(clipData);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clipData = string.Empty;
            for (int i = 0; i < lstNetworks.SelectedIndices.Count; i++)
            {
                foreach (ListViewItem.ListViewSubItem item in lstNetworks.SelectedItems[i].SubItems)
                {
                    clipData = String.Format("{0}{1}, ", clipData, item.Text);
                }

                clipData = clipData + Environment.NewLine;
            }

            if (clipData.EndsWith(Environment.NewLine))
                clipData = clipData.Substring(0, clipData.Length - 4);

            if (!string.IsNullOrEmpty(clipData))
                Clipboard.SetText(clipData);
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstNetworks.Items.Count; i++)
			{
                lstNetworks.SelectedIndices.Add(i);
			}            
        }

        private void ClearTimes()
        {
            fiveSecondsToolStripMenuItem.Checked = false;
            fifteenSecondsToolStripMenuItem1.Checked = false;
            thirtySecondsToolStripMenuItem.Checked = false;
            minuteToolStripMenuItem.Checked = false;
        }

        private void visitWwwkornishnetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://seankornish.com/");
        }

        private void fiveSecondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            refresh = 5;
            ClearTimes();
            fiveSecondsToolStripMenuItem.Checked = true;
        }

        private void fifteenSecondsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            refresh = 15;
            ClearTimes();
            fifteenSecondsToolStripMenuItem1.Checked = true;
        }

        private void thirtySecondsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            refresh = 30;
            ClearTimes();
            thirtySecondsToolStripMenuItem.Checked = true;
        }

        private void minuteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            refresh = 60;
            ClearTimes();
            minuteToolStripMenuItem.Checked = true;
        }

    }
}