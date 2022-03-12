# AntiVPN
A [TShock](https://github.com/Pryaxis/TShock) plugin that aims to prevent VPN connections using [ipdata](https://getipintel.net/) and [IPHub](https://iphub.info/)'s API services.

**This plugin isn't intended for public use until I un-hardcode stuff** (is that a term?). If you currently attempt to use it on your own server, I am not responsible for rate limit issues you run into.

## Instructions
Place `AntiVPN.dll` into your `ServerPlugins` folder. If all goes well, you should see during TShock initialization.
Each time a VPN connection is detected it will be printed to TShock's console along with possible errors.

`/antivpn` will list subcommands. You can allow a TShock group to utilize this command by adding the `antivpn` permission to it.

By default AntiVPN will be enabled at server start and will function every time a player joins, using the ipdata API to check their IP. Once the rate limit is hit on ipdata,
IPHub will be used instead until the same occurs. **It is unlikely both APIs would be rate limited at any given time; if this happens, the IP will be skipped.**

### There are currently three subcommands:
 * `status` - Prints the current AntiVPN state (enabled/disabled) and the API being used.
 * `switchapi` - Allows manual switching of APIs. Might be useful if a certain API is letting VPN connections through (which I've observed).
 * `toggle` - Disables and re-enables AntiVPN.