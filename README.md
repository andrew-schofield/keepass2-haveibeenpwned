# keepass2-haveibeenpwned

**KeePass 2.x plugin to check all entries with URLs against various breach lists.**

**Download plgx from [here](https://github.com/andrew-schofield/keepass2-haveibeenpwned/raw/master/HaveIBeenPwned.plgx).**

**Mono users can download the dlls from [here](https://github.com/andrew-schofield/keepass2-haveibeenpwned/tree/master/mono).**

## Currently Supported Breach Lists

### Site/Domain based
* [Have I Been Pwned (HIBP)](https://haveibeenpwned.com/) - Checks the domains of any entries against the Have I Been Pwned? list curated by Troy Hunt
* [Cloudbleed vulnerability list](https://github.com/pirate/sites-using-cloudflare) - Checks the domains of any entries that appear in the Cloudbleed vulnerability list. This has potential to produce false positives due to the way this list was produced.

### Username based
* [Have I Been Pwned (HIBP)](https://haveibeenpwned.com/) - Checks the usernames of any entries against the Have I Been Pwned? list curated by Troy Hunt

### Password based
* [Have I Been Pwned (HIBP)](https://haveibeenpwned.com/) - Checks the passwords of any entries against the Have I Been Pwned? list curated by Troy Hunt

**This checker sends a small portion of the password hash to HIBP and then checks the full hash locally against the list of hashes returned by HIBP. This service does not send your password, nor enough of the hash to expose your password to HIBP.**

## Usage

* Install the plugin into KeePass, this will add an entry to the Tools menu for "Have I Been Pwned?"
* Clicking this entry will open a sub-menu with entries for the different breach types to check
* Clicking these entries will open a prompt asking which breach to check, or all, whether to check only entries that have not been modified since the breach date. You also have the option of auto-expiring any breached entries and including any deleted entries.
* Running the check will result in a dialog listing all the breached entries, and from which breach they originated (entries can appear multiple times if they appear in multiple breach lists). These can then be modified directly from the list.
* In the case of username breaches the dialog will also list accounts that have been breached but are not stored in the database


## Notes

* HaveIBeenPwned breach data is downloaded every time the check is run as the data file is small.
* Cloudbleed data is only downloaded once and then cached here: `%PROGRAMDATA%\KeePass\cloudbleed.txt` (Windows) or `%LOCALAPPDATA%\KeePass\cloudbleed.txt` (Linux) as this is currently a ~70MB download. If you wish to refresh the cache, simply delete this file.
* As KeePass doesn't have a native method for determining when an entry's password was last changed, keepass2-haveibeenpwned will use the history entries if any exist and compare their passwords.
* Username/password checking could take a while to complete as HIBP applies a rate limit on requests, which means we can only check one username/password every 1.6s
* Common usernames (such as admin & root) are not removed from the check and will likely result in false positives in your results, however these should be immediately obvious.

## Donate

keepass2-haveibeenpwned is developed entirely in my own time. If you wish to support development you can donate via PayPal here.

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=S2DVYTS47PX4S)

## Contributers

* **Andrew Schofield**
* **Matt Schneeberger**
* **strayge**