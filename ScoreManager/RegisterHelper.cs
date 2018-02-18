using Microsoft.Win32;
using System;

namespace ScoreManager
{
    public class RegisterHelper
    {
        public static string GetRegValue(string path, string name)
        {
            var value = "";
            try
            {
                var mainKey = Registry.LocalMachine;
                var subKey = mainKey.OpenSubKey(path);
                if (subKey != null)
                    value = (string)subKey.GetValue(name);
                if (value == null) value = "";
            }
            catch (Exception)
            {
                value = "";
            }
            return value;

        }
    }
}
