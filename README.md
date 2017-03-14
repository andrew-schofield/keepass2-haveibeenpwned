# keepass2-haveibeenpwned

**KeePass 2.x plugin to check all entries with URLs against the haveibeenpwned breach list.**

Also compares URLs against the cloudbleed vulnerability list from here: [https://github.com/pirate/sites-using-cloudflare](https://github.com/pirate/sites-using-cloudflare)

**Download plgx from [here](https://github.com/andrew-schofield/keepass2-haveibeenpwned/raw/master/HaveIBeenPwned.plgx).**

**Mono users can download the dlls from [here](https://github.com/andrew-schofield/keepass2-haveibeenpwned/tree/master/mono).**

## Usage

* Install the plugin into KeePass, this will add 2 new entries to the Tools menu, "Have I Been Pwned?" and "Cloudbleed checker"
* Clicking either of these will open a prompt asked whether to check only entries that have not been modified since the breach date. You also have the option of auto-expiring any breached entries.
* Running the check will result in a dialog listing all the breached entries. These can then be modified directly from the list.

## Known Issues

* No indication of download progress
 
## Notes

* HaveIBeenPwned breach data is downloaded every time the check is run as the data file is small.
* Cloudbleed data is only downloaded once and then cached here: `%PROGRAMDATA%\KeePass\cloudbleed.txt` as this is currently a ~70MB download. If you wish to refresh the cache, simply delete this file.
* As KeePass doesn't have a native method for determining when an entry's password was last changed, keepass2-haveibeenpwned will use the history entries if any exist and compare their passwords.