namespace HaveIBeenPwned
{
    public enum CheckTypeEnum
    {
        [Display(Name = "Site/Service Checker")]
        SiteDomain,

        [Display(Name = "Username Checker")]
        Username,

        [Display(Name = "Password Checker")]
        Password
    }
}
