using System;
using System.Runtime.InteropServices;
using System.Security;

namespace StructureInterfaces
{
    public static class SecureHelper
    {
        public static string ToString(SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException("secureString");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static SecureString ToSecureString(string unsecureString)
        {
            var ss = new SecureString();
            foreach (var ch in unsecureString)
            {
                ss.AppendChar(ch);
            }
            return ss;
        }
    }
}