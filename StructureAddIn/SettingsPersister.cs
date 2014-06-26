using System;
using StructureAddIn.Properties;
using StructureInterfaces;

namespace StructureAddIn
{
    /// <summary>
    /// Responsible for saving and restoring the settings.
    /// </summary>
    class SettingsPersister
    {
        internal AddinSettings ReadSettings()
        {
            var settings = new AddinSettings
            {
                JIRAURL = Settings.Default.JIRAURL,
                Password = SecureHelper.ToSecureString(Settings.Default.Password),//TODO improve.
                Username = Settings.Default.Username,
                SavePassword = Settings.Default.Password != null,
                Structure = Settings.Default.Structure,
                JQL = Settings.Default.JQL
            };
            settings.PropertyChanged += addinSettings_PropertyChanged;

            return settings;
        }

        private void addinSettings_PropertyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            var addinSettings = (AddinSettings) sender;
            switch (e.PropertyName)
            {
                case "JIRAURL":
                    Settings.Default.JIRAURL = addinSettings.JIRAURL;
                    break;
                case "JQL":
                    Settings.Default.JQL = addinSettings.JQL;
                    break;
                case "Password":
                    if (addinSettings.SavePassword)
                    {
                        //TODO encrypt with public key...
                        Settings.Default.Password = SecureHelper.ToString(addinSettings.Password);
                    }
                    break;
                case "Username":
                    Settings.Default.Username = addinSettings.Username;
                    break;
                case "Structure":
                    Settings.Default.Structure = addinSettings.Structure;
                    break;
                default:
                    throw new NotSupportedException(e.PropertyName);
            }
            Settings.Default.Save();
        }
    }
}