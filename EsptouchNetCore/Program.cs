using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using EspTouchForCSharp;
using EspTouchForCSharp.Util;

namespace EsptouchNetCore
{
    internal class Program
    {
        static void usage()
        {
            var r = System.Reflection.Assembly.GetExecutingAssembly().GetName();

            string progname = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);

            Console.WriteLine($"Usage: {progname} ...");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("  [ --(ssid | s) SSID ]");
                Console.WriteLine("  [ --(bssid | b) BSSID ]");
                Console.WriteLine("  [ --(address | a) LOCALADDRESS ]");
                Console.WriteLine("  [ --(broadcast | br) <1|0> ,default:1 ]");
                Console.WriteLine("  [ --(devices | d) NUMBER ,default:1 ]");
            }
            else
            {

                Console.WriteLine("  --(ssid | s) SSID");
                Console.WriteLine("  --(bssid | b) BSSID");
                Console.WriteLine("  --(address | a) LOCALADDRESS");
            }

            Console.WriteLine("  [ --(password | p) PASSWORD ]");
            Console.WriteLine("  [ --(help | h) USAGE ]");
        }

        static bool ARG_EQU(string arg, string s, string l)
        {
            var nr = arg.Replace("-", "").ToLower();
            return nr == s.ToLower() || nr == l.ToLower();
        }

        static void scanargs(string[] args, EsptouchInfo r)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (ARG_EQU(arg, "s", "ssid"))
                {
                    i++;
                    r.SSID = args[i];
                }
                else if (ARG_EQU(arg, "b", "bssid"))
                {
                    i++;
                    r.BSSID = args[i];
                }
                else if (ARG_EQU(arg, "p", "password"))
                {
                    i++;
                    r.Password = args[i];
                }
                else if (ARG_EQU(arg, "d", "devices"))
                {
                    i++;
                    int tmp = 1;
                    if (int.TryParse(args[i], out tmp))
                    {
                        r.Devices = tmp;
                    }
                }
                else if (ARG_EQU(arg, "br", "broadcast"))
                {
                    i++;
                    int tmp = 1;
                    if (int.TryParse(args[i], out tmp))
                    {
                        r.Broadcast = tmp == 1;
                    }
                }
                else if (ARG_EQU(arg, "a", "address"))
                {
                    i++;

                    try
                    {
                        r.IP = IPAddress.Parse(args[i]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"IP ({args[i]}) Parse Error: {e.Message} ");
                    }

                }
            }

        }

        static void Main(string[] args)
        {
            if (args.Any(arg => arg.IndexOf("help") > -1 || arg.IndexOf("-h") > -1))
            {
                usage();
            }
            else
            {
                var info = WifiInterface.GetInfo();
                scanargs(args, info);

                Console.WriteLine("EsptouchInfo:");
                Console.WriteLine($"    {info}");
                Console.WriteLine();

                if (info == null || info.IP == null || string.IsNullOrEmpty(info.SSID) || string.IsNullOrEmpty(info.BSSID) || info.Devices == 0)
                {
                    Console.WriteLine("Exception: Argument Incomplete!");
                    Console.WriteLine();
                    usage();
                }
                else
                {
                    try
                    {
                        Console.WriteLine($"ESPTOUCH VERSION: {EsptouchTask.ESPTOUCH_VERSION} \n");
                        /// broadcast = false  组播模式下， 必须跟esp8266 在一个 wifi 路由下 esp8266 才能收到
                        EsptouchTask eh = new EsptouchTask(info.SSID, info.BSSID, info.Password, info.Broadcast);

                        eh.Find += Eh_Find;

                        Console.Write("Search ");

                        var lst = eh.ExecuteForResults(info.Devices, info.IP);

                        Console.WriteLine("\n");

                        if (lst.Count == 0)
                        {
                            Console.WriteLine("No device response");
                        }
                        else
                        {
                            Console.WriteLine($"{lst.Count} devices were configured successfully");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Exception: {e.Message}");
                    }
                }
            }

        }

        private static void Eh_Find(object sender, EsptouchEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"Find: {e.Driver}");
            Console.WriteLine();
            Console.WriteLine();
        }

    }
}
