using System;
using System.Net.Http;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace VPNPlugin
{
    [ApiVersion(2, 1)]
    public class VPNPlugin : TerrariaPlugin
    {
        public override string Name => "AntiVPN";

        public override string Description => "Jayton repellent";

        public override string Author => "numanuma";

        public override Version Version => new Version(1, 1);

        static readonly string[] ignoredIPs = { "10.", "127", "172.16.", "172.17.", "172.18.", "172.19.", "172.20.", "172.21.", "172.22.", "172.23.",
                                                "172.24.", "172.25.", "172.26", "172.27", "172.28", "172.29", "172.30", "172.31", "192.168"};

        static readonly HttpClient client = new HttpClient();

        static string responseString;

        static bool usingIPintel = true;

        static bool antiVPNtoggle = true;

        public VPNPlugin(Main game) : base(game)
        {
         
        }

        void OnInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("antivpn", ToggleVPN, "antivpn")
            {
                HelpText = "Toggles the AntiVPN plugin. Default state is enabled."
            });
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
        }

        async void OnJoin(JoinEventArgs args)
        {
            bool isIgnored = false;

            foreach (string IP in ignoredIPs)
            {
                if (isIgnored == true)
                {
                    break;
                }
                else
                {
                    isIgnored = TShock.Players[args.Who].IP.StartsWith(IP);
                }
            }

            if (isIgnored || !antiVPNtoggle)
            {
                return;
            }
            
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    if (usingIPintel)
                    {
                        responseString = await client.GetStringAsync($"http://check.getipintel.net/check.php?ip={TShock.Players[args.Who].IP}&contact=jacobaustin.sec@gmail.com&flags=m");
                        float.TryParse(responseString, out float responseResult);
                        if (responseResult >= 0.9) 
                        {
                            TShock.Players[args.Who].Disconnect("[AntiVPN] VPN connections are not permitted.");
                        }
                    }
                    else
                    {
                        HttpRequestMessage requestMessage = BuildRequest(TShock.Players[args.Who].IP);
                        HttpResponseMessage response = await client.SendAsync(requestMessage);
                        responseString = await response.Content.ReadAsStringAsync();
                        if (responseString.Contains("k\":1,"))
                        {
                            TShock.Players[args.Who].Disconnect("[AntiVPN] VPN connections are not permitted.");
                        }
                    }

                    i++;
                }
                catch (Exception e)
                {
                    if (e is HttpRequestException)
                    {
                        Console.WriteLine($"[AntiVPN] An error occurred verifying {TShock.Players[args.Who].Name}'s IP:\n{e.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"[AntiVPN] An error occurred verifying {TShock.Players[args.Who].Name}'s IP:\n{e}");
                    }

                    if (i == 0)
                    {
                        usingIPintel = !usingIPintel;
                        Console.WriteLine($"[AntiVPN] Attempting alternative VPN lookup using {HostCheck()}...");
                    }
                    else
                    {
                        Console.WriteLine("[AntiVPN] Both APIs attempted; giving up.");
                    }
                }
            }
        }

        void ToggleVPN(CommandArgs args)
        {
            string subcmd = args.Parameters.Count == 0 ? null : args.Parameters[0].ToLower();

            switch (subcmd)
            {
                case "toggle":
                    antiVPNtoggle = !antiVPNtoggle;
                    args.Player.SendSuccessMessage($"AntiVPN is now {((antiVPNtoggle) ? $"enabled; using {HostCheck()}" : "disabled")}.");
                    break;

                case "status":
                    args.Player.SendSuccessMessage($"AntiVPN is currently {((antiVPNtoggle) ? $"enabled; using {HostCheck()}" : "disabled")}.");
                    break;

                case "switchapi":
                    usingIPintel = !usingIPintel;
                    args.Player.SendSuccessMessage($"AntiVPN API switched to {HostCheck()}.");
                    break;

                default:
                    args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}antivpn [status, switchapi, toggle]", TShock.Config.Settings.CommandSpecifier);
                    break;
            }
        }

        string HostCheck()
        {
            string host = usingIPintel ? "GetIPIntel" : "IPHub";
            return host;
        }

        HttpRequestMessage BuildRequest(string IP)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri($"http://v2.api.iphub.info/ip/{IP}"),
                Method = HttpMethod.Get
            };
            requestMessage.Headers.Add("X-Key", "MTcwMTc6VG5YWmpCTVhRZUY1NnNvRkNDSk5jOEdvMU5XOThiRjc=");

            return requestMessage;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
            }

            base.Dispose(disposing);
        }
    }
}