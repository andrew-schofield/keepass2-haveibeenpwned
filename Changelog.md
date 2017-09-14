# keepass2-haveibeenpwned Changelog

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