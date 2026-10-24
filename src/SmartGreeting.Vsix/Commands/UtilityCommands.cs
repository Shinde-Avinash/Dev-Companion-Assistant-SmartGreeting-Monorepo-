using System;

namespace SmartGreeting.Vsix.Commands
{
    public class SettingsCommand
    {
        public static void Execute()
        {
            SmartGreetingPackage.Instance.ShowSettings();
        }
    }

    public class AboutCommand
    {
        public static void Execute()
        {
            SmartGreetingPackage.Instance.ShowAbout();
        }
    }
}
