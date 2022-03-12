# AntiVPN
A [TShock](https://github.com/Pryaxis/TShock) plugin that aims to prevent VPN connections using [IP Intelligence](https://getipintel.net/)'s API services.

**This plugin isn't intended for public use until I un-hardcode stuff** (is that a term?). If you currently attempt to use it on your own server, I am not responsible for rate limit issues you run into.

## Instructions
Place `AntiVPN.dll` into your `ServerPlugins` folder. If all goes well, you should see during TShock initialization.
Each time a VPN connection is detected it will be printed to TShock's console along with possible errors.

There are currently two subcommands:
 * `status` - Prints the current AntiVPN state; 'on' or 'off.' Mostly for peace of mind.
 * `toggle` - Toggles AntiVPN. Default state is 'on.'