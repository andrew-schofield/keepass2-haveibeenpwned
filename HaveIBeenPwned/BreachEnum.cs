namespace HaveIBeenPwned
{
    public enum BreachEnum
    {
        [CheckerType(Name = "Have I Been Pwned", Type = CheckTypeEnum.SiteDomain)]
        HIBPSite,

        [CheckerType(Name = "Cloudbleed", Type = CheckTypeEnum.SiteDomain)]
        CloudBleedSite
    }
}
