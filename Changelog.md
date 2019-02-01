# keepass2-haveibeenpwned Changelog

### v1.3.1 - 2019-02-01
* Allow cancelling a running breach check.
* Performance improvements. Thanks to SlightlyMadGargoyle.
* Add option to check all breach types at once. Thanks to SlightlyMadGargoyle.
* Show more details about the breach in the results list. Thanks to SlightlyMadGargoyle.

### v1.3.0 - 2019-01-19
* Prevent every breach type being checked when "check all breaches" is selected
* Allow users to continue working while the breach results are shown. Thanks to strayge.
* Add a cancel button to the message box if an error occurs during checks. Thanks to strayge.
* Cache password hashes while checking to prevent identical passwords needing an online check. Thanks to strayge.
* Fix to keepass2-developer extensions to correctly check recycle bin entries. Thanks to strayge.
* Allow ignoring of expired entries. Thanks to strayge.
* Follow KeePass proxy settings. Requires restart on proxy change. Thanks to strayge.
* Allow running HIBP on groups, or individual entries. Thanks to Matt Schneeberger.

### v1.2.4 - 2018-02-23
* Enable password check mode using the new HIBP v2 password API. Thanks to Matt Schneeberger.
* Add breach description to checker dialog

### v1.2.3 - 2017-10-27
* Temporarily disable the password check mode as it sends weakly hashed passwords (although encrypted) to HIBP.

### v1.2.2 - 2017-10-21
* Alter the way developer extensions are referenced to prevent dependency on a specific version of KeePass

### v1.2.1 - 2017-09-23
* Disable the check entries not changed option when checking for breach passwords as this is not relevant for that breach type

### v1.2.0 - 2017-09-14
* Add new check method to allow searching for breaches based on entry password

### v1.1.0 - 2017-09-14
* Add new check method to allow searching for breaches based on entry username
* Moved menu item into submenu to allow for new username checking option

### v1.0 - 2017-06-23
* v1 release.
* Moved password entry extensions into a separate library
* Fix password last changed algorithm to match canonical KeePass 2.36 implementation

### v0.3.1 - 2017-05-23
* Fix password-last-modified algorithm

### v0.3 - 2017-05-12
* Combine breach selection dialogs into one
* Allow checking of all supported breaches

### v0.2.10 - 2017-04-20
* Add breach count to results dialog
* Show entry icon in results dialog

### v0.2.9.2 - 2017-04-13
* Alter HTTPS negotiation protocol for HIBP (only supports TLS)

### v0.2.9.1 - 2017-04-10
* Fix how domains are extracted from the URL of an entry

### v0.2.9 - 2017-04-10
* Fix potential data security issue

### v0.2.8.1 - 2017-04-08
* Allow resizing of breach check results list

### v0.2.8 - 2017-04-08
* Allow ignoring of deleted entries (entries in the recycle bin)

### v0.2.7 - 2017-04-07
* Group entries in the breach check results list

### v0.2.6 - 2017-04-04
* Add breach name to breach check results list

### v0.2.5 - 2017-04-01
* Fix crash when URL is invalid

### v0.2.4.1 - 2017-03-15
* Alter cloudbleed download location for Linux

### v0.2.4 - 2017-03-14
* Modify entries directly from the breach check results list

### v0.2.3 - 2017-03-13
* Add ability to check password-last-modified date

### v0.2.2.1 - 2017-03-10
* Increase timeout again
* Fix typo

### v0.2.2 - 2017-03-10
* Add results dialog
* All processing now asynchronous

### v0.2.1 - 2017-03-09
* Create plgx version of plugin
* Fix incompatabilities
* Make downloading asynchronous

### v0.2 - 2017-03-09
* Add cloudbleed vulnerability support
* General cleanup
* Increased timeout for slow connections
* Add processing dialog
* Add precompiled dll

### v0.1 - 2017-02-24
* Initial release of plugin
* Basic HIBP checking