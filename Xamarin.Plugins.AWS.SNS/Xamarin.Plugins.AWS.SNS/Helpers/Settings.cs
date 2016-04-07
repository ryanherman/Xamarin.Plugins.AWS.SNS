// Helpers/Settings.cs

using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Xamarin.Plugins.AWS.SNS.Helpers
{
    /// <summary>
    ///     This is the Settings static class that can be used in your Core solution or in any
    ///     of your client applications. All settings are laid out the same exact way with getters
    ///     and setters.
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get { return CrossSettings.Current; }
        }

        public static string Arnsns
        {
            get { return AppSettings.GetValueOrDefault(ARNKey, ARNDefault); }
            set { AppSettings.AddOrUpdateValue(ARNKey, value); }
        }
        public static string Uid
        {
            get { return AppSettings.GetValueOrDefault(memberUidKey, memberUidDefault); }
            set { AppSettings.AddOrUpdateValue(memberUidKey, value); }
        }
        #region Setting Constants

        private static readonly string ARNKey = "ARN";
        private static readonly string ARNDefault = string.Empty;
        private static readonly string memberUidKey = "Uid";
        private static readonly string memberUidDefault = string.Empty;
        #endregion
    }
}