using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RepeatableTests
{
    public class AppSettingsLinks
    {
        public static string Root = "\\..\\..\\..\\..\\Web";

        public static string Dev => Directory.GetCurrentDirectory() + $"{Root}\\appsettings.Development.json";
        public static string Staging => Directory.GetCurrentDirectory() + $"{Root}\\appsettings.Staging.json";
        public static string Prod => Directory.GetCurrentDirectory() + $"{Root}\\appsettings.json";
    }
}
