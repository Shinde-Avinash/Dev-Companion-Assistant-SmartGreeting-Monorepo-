namespace SmartGreeting.Core.Models
{
    public enum TimePeriod
    {
        Morning,
        Afternoon,
        Evening,
        LateNight
    }

    public enum WorkContext
    {
        BeforeWork,
        DuringWork,
        AfterWork,
        LateNight,
        Weekend
    }

    public enum GreetingFrequency
    {
        OncePerSession,
        OncePerDay,
        EveryStartup,
        ManualOnly
    }

    public enum GreetingTheme
    {
        Auto,
        Light,
        Dark,
        Morning,
        Afternoon,
        Evening,
        Night
    }
}
