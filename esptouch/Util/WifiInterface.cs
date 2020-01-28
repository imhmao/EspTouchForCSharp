using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace EspTouchForCSharp.Util
{
    public static class WifiInterface
    {
        private static Guid? getWindowsWifiId(EsptouchInfo info)
        {
            Guid? id = null;

            var process = new Process
            {
                StartInfo =
                    {
                    FileName = "netsh.exe",
                    Arguments = "wlan show interfaces",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                    }
            };

            process.Start();

            var output = process.StandardOutput.ReadToEnd();
            var ssid_line = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(l => l.Contains("SSID") && !l.Contains("BSSID"));

            var bssid_line = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(l => l.Contains("BSSID"));

            var guid_line = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(l => l.Contains("GUID"));

            if (ssid_line != null && bssid_line != null && guid_line != null)
            {
                info.SSID = ssid_line.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].TrimStart();
                info.BSSID = bssid_line.Substring(bssid_line.IndexOf(":") + 1).Trim();

                id = Guid.Parse(guid_line.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].TrimStart());
            }

            return id;
        }

        public static EsptouchInfo GetInfo()
        {
            EsptouchInfo r = new EsptouchInfo();
            NetworkInterface apt = null;
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var aptId = getWindowsWifiId(r);
                if (aptId != null)
                {
                    apt = interfaces.FirstOrDefault(ap => Guid.Parse(ap.Id) == aptId.Value);
                }
            }
            else
            {
                apt = interfaces.FirstOrDefault(ap => apt.OperationalStatus == OperationalStatus.Up && apt.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);
            }

            if (apt != null)
            {
                var ips = apt.GetIPProperties().UnicastAddresses.FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork);

                if (ips != null)
                {
                    r.IP = ips.Address;
                }
            }

            return r;
        }
    }
}
