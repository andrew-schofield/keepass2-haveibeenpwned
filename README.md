# keepass2-haveibeenpwned

KeePass 2.x plugin to check all entries with URLs against the haveibeenpwned breach list.

Also compares URLs against the cloudbleed vulnerbility list from here: https://github.com/pirate/sites-using-cloudflare

Download plugin zip from [here](https://github.com/andrew-schofield/keepass2-haveibeenpwned/raw/master/HaveIBeenPwned.zip).

## Usage

* Install the plugin into KeePass, this will add 2 new entries to the Tools menu, "Have I Been Pwned?" and "Cloudbleed checker"
* Clicking either of these will open a prompt asked whether to check only entries that have not been modified since the breach date. You also have the option of auto-expiring any breached entries.
* Running the check will produce a series of messages with the details of any identified breaches.

## Known Issues

* Built dll seems to be linked to a specific version of KeePass
* Currently everything happens in the UI thread
* No indication of download progress or processing progress
* There is no way of getting the "password last modified" time from KeePass, so unfortunately both checking modes have drawbacks:
 * "Only check entries that have not changed since the breach date" will produce false negatives if you have changed something other than the password since the breach date
 * If unchecked you will potentially get false positives if you have already changed entries that have been breached (as this will return all entries that have ever been breached).
 
## Notes

* HaveIBeenPwned breach data is downloaded every time the check is run as the data file is small.
* Cloudbleed data is only downloaded once and then cached here: `%PROGRAMDATA%\KeePass\cloudbleed.txt` as this is currently a ~70MB download. If you wish to refresh the cache, simply delete this file.
