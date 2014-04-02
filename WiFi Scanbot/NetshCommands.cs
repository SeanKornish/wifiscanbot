using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace WiFi_Scanbot
{
    public static class NetshCommands
    {
        public static string GetWirelessNetworksString()
        {
            try
            {
                Process p = new Process();
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "netsh.exe";
                p.StartInfo.Arguments = "wlan show networks mode=bssid";
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                return output;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Getting Network String: " + ex.Message);
                return string.Empty;
            }
        }

        public static void ConnectToNetwork(string SSID, string Key)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "netsh.exe";
                p.StartInfo.Arguments = string.Format("wlan show networks mode=bssid");
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Getting Network String: " + ex.Message);
            }
        }

        public static void Disconnect()
        {
            try
            {
                Process p = new Process();
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "netsh.exe";
                p.StartInfo.Arguments = "wlan disconnect";
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Getting Network String: " + ex.Message);
            }
        }
    }
}
