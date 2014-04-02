using System;
using System.Collections.Generic;
using System.Linq;

namespace WiFi_Scanbot
{
    public class WirelessNetwork
    {
        public string SSID;
        public string Type;
        public string Authentication;
        public string Encryption;
        public string BSSID;
        public string Signal;
        public string RadioType;
        public string Channel;
        public string BasicRates;
        public string OtherRates;
        public DateTime LastSeen;
        public DateTime FirstSeen;
        public bool isNew;
    }
}
