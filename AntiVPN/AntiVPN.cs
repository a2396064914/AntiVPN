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
        public override Version Version => new Version(1, 0);

        private static string[] ignoredIPs = { "10.", "127", "172.16.", "172.17.", "172.18.", "172.19.", "172.20.", "172.21.", "172.22.", "172.23.", 
                                               "172.24.", "172.25.", "172.26", "172.27", "172.28", "172.29", "172.30", "172.31", "192.168"};

        private static bool toggleAntiVPN = true;

        private static readonly HttpClient client = new HttpClient();

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
            ServerApi.Hooks.ServerJoin.Register(this, OnJoinAsync);
        }

        async void OnJoinAsync(JoinEventArgs args)
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

            if (isIgnored || !toggleAntiVPN)
            {
                return;
            }

            try
            {
                var responseString = await client.GetStringAsync($"http://check.getipintel.net/check.php?ip={TShock.Players[args.Who].IP}&contact=jacobaustin.sec@gmail.com&flags=m");
                
                float.TryParse(responseString, out float responseResult);

                if (responseResult >= 0.90)
                    TShock.Players[args.Who].Disconnect("[AntiVPN] VPN connections are not permitted.");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"[AntiVPN] An error occurred verifying {TShock.Players[args.Who].Name}'s IP:\n{e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[AntiVPN] An error occurred verifying {TShock.Players[args.Who].Name}'s IP:\n{e}");
            }
        }

        void ToggleVPN(CommandArgs args)
        {
            string subcmd = args.Parameters.Count == 0 ? null : args.Parameters[0].ToLower();

            switch (subcmd)
            {
                case "toggle":
                    toggleAntiVPN = !toggleAntiVPN;
                    args.Player.SendSuccessMessage($"AntiVPN is now {((toggleAntiVPN) ? "enabled" : "disabled")}.");
                    break;

                case "status":
                    args.Player.SendSuccessMessage($"AntiVPN is currently {((toggleAntiVPN) ? "enabled" : "disabled")}.");
                    break;

                default:
                    args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}antivpn [status, toggle]", TShock.Config.Settings.CommandSpecifier);
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoinAsync);
            }

            base.Dispose(disposing);
        }
    }
}