using System;

namespace SmartGreeting.Vsix.Commands
{
    public class ShowGreetingCommand
    {
        public static void Execute()
        {
            SmartGreetingPackage.Instance.ShowGreetingManual();
        }
    }
}
