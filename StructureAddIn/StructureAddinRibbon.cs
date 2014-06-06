using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using StructureAddIn.Annotations;
using StructureInterfaces;
using Office = Microsoft.Office.Core;

namespace StructureAddIn
{


    [ComVisible(true)]
    public class StructureAddinRibbon : Office.IRibbonExtensibility
    {
        [UsedImplicitly]        
        private Office.IRibbonUI ribbon;
        private readonly AddinSettings addinSettings;

        public delegate void ImportHandler(AddinSettings settings);

        public event ImportHandler OnImport;
        
        public StructureAddinRibbon()
        {
            addinSettings = new SettingsPersister().ReadSettings();
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("StructureAddIn.StructureAddinRibbon.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit http://go.microsoft.com/fwlink/?LinkID=271226

        [UsedImplicitly]
        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            ribbon = ribbonUI;            

            //Setup UI:
            
//            this.ribbon.=           this.addinSettings.JIRAURL;
        }

        [UsedImplicitly]
        public string GetText_Username(Office.IRibbonControl control)
        {
            return addinSettings.Username;
        }

        [UsedImplicitly]
        public void UsernameChanged(Office.IRibbonControl control, string isPressed)
        {
            addinSettings.Username = isPressed;
        }

        [UsedImplicitly]
        public string GetText_Password(Office.IRibbonControl control)
        {
            return new string('*', addinSettings.Password.Length);
        }

        [UsedImplicitly]
        public void PasswordChanged(Office.IRibbonControl control, string password)
        {
            addinSettings.Password = SecureHelper.ToSecureString(password);
        }

        [UsedImplicitly]
        public void RememberPasswordChanged(Office.IRibbonControl control, bool isChecked)
        {
            addinSettings.SavePassword = isChecked;
        }

        [UsedImplicitly]
        public bool GetPressed_SavePassword(Office.IRibbonControl control)
        {
            return addinSettings.SavePassword;
        }

        [UsedImplicitly]
        public void JIRAURLChanged(Office.IRibbonControl control, string isPressed)
        {
            addinSettings.JIRAURL = isPressed;
        }

        [UsedImplicitly]
        public string GetText_JIRAURL(Office.IRibbonControl control)
        {
            return addinSettings.JIRAURL;
        }


        [UsedImplicitly]
        public void StructureChanged(Office.IRibbonControl control, string structureName)
        {
            addinSettings.Structure = structureName;
        }

        [UsedImplicitly]
        public string GetText_Structure(Office.IRibbonControl control)
        {
            return addinSettings.Structure;
        }

        [UsedImplicitly]
        public void Import(Office.IRibbonControl control)
        {
            OnImport(addinSettings);
        }
        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Stream manifestResourceStream = asm.GetManifestResourceStream(resourceNames[i]);
                    if (manifestResourceStream != null)
                    using (var resourceReader = new StreamReader(manifestResourceStream))
                    {
                        return resourceReader.ReadToEnd();
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
