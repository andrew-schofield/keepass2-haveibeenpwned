namespace HaveIBeenPwned
{
    public enum BreachEnum
    {
        [CheckerType(Name = "Have I Been Pwned", Type = CheckTypeEnum.SiteDomain, Description = "This checker will compare the domains of your entries against the Have I Been Pwned breach list.")]
        HIBPSite,

        [CheckerType(Name = "Cloudbleed", Type = CheckTypeEnum.SiteDomain, Description = "This checker will compare the domains of your entries against the cloudbleed breach list.")]
        CloudBleedSite,

        [CheckerType(Name = "Have I Been Pwned", Type = CheckTypeEnum.Username, Description = "This checker will compare the usernames of your entries against the Have I Been Pwned breach list")]
        HIBPUsername,

        [CheckerType(Name = "Have I Been Pwned", Type = CheckTypeEnum.Password, Description = "This checker will send a partial hash of the password for each entry to the Have I Been Pwned password checker. Your passwords or complete hashes are not disclosed to this service.")]
        HIBPPassword,

        [CheckerType(Name = "CheckAll", Type = CheckTypeEnum.CheckAll, Description = "This checker will return all results from all other checkers.")]
        CheckAll
    }
}
