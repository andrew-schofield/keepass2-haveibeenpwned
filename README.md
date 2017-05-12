# keepass2-haveibeenpwned

**KeePass 2.x plugin to check all entries with URLs against various breach lists.**

**Download plgx from [here](https://github.com/andrew-schofield/keepass2-haveibeenpwned/raw/master/HaveIBeenPwned.plgx).**

**Mono users can download the dlls from [here](https://github.com/andrew-schofield/keepass2-haveibeenpwned/tree/master/mono).**

## Currently Supported Breach Lists

* [Have I Been Pwned (HIBP)](https://haveibeenpwned.com/) - Checks the domains of any entries against the Have I Been Pwned? list curated by Troy Hunt
* [Cloudbleed vulnerability list](https://github.com/pirate/sites-using-cloudflare) - Checks the domains of any entries that appear in the Cloudbleed vulnerability list. This has potential to produce false positives due to the way this list was produced.

## Usage

* Install the plugin into KeePass, this will add an entry to the Tools menu for "Have I Been Pwned?"
* Clicking this entry will open a prompt asking which breach to check, or all, whether to check only entries that have not been modified since the breach date. You also have the option of auto-expiring any breached entries and including any deleted entries.
* Running the check will result in a dialog listing all the breached entries, and from which breach they originated (entries can appear multiple times if they appear in multiple breach lists). These can then be modified directly from the list.


## Notes

* HaveIBeenPwned breach data is downloaded every time the check is run as the data file is small.
* Cloudbleed data is only downloaded once and then cached here: `%PROGRAMDATA%\KeePass\cloudbleed.txt` (Windows) or `%LOCALAPPDATA%\KeePass\cloudbleed.txt` (Linux) as this is currently a ~70MB download. If you wish to refresh the cache, simply delete this file.
* As KeePass doesn't have a native method for determining when an entry's password was last changed, keepass2-haveibeenpwned will use the history entries if any exist and compare their passwords.

## Copyright

&copy; 2017 Andrew Schofield