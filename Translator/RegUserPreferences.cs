using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    /// <summary>
    /// Κλάση που αποθηκεύει τις ρυθμίσεις του χρήστη
    /// </summary>
    class RegUserPreferences : UserPreferences
    {
        /// <summary>
        /// δημιουργός της κλάσης,
        /// σε πρώτη φάση ελέγχει αν υπάρχουν αποθηκευμένες ρυθμίσεις και τις φορτώνει,
        /// αλλιώς εμφανίζει κάποιες προεπιλεγμένες
        /// </summary>
        public RegUserPreferences()
        {
            RegistryKey UserPrefs = Registry.CurrentUser.OpenSubKey("SOFTWARE\\TEIKAV\\Translator", true);

            if (UserPrefs != null)
            {
                _ToLanguage = UserPrefs.GetValue("ToLanguage").ToString();
                _FromLanguage = UserPrefs.GetValue("FromLanguage").ToString();
                _Key = UserPrefs.GetValue("Key").ToString();
                _CTRL = Convert.ToBoolean(UserPrefs.GetValue("CTRL").ToString());
                _ALT = Convert.ToBoolean(UserPrefs.GetValue("ALT").ToString());
                _SHIFT = Convert.ToBoolean(UserPrefs.GetValue("SHIFT").ToString());

                _startstopKey = UserPrefs.GetValue("startstopKey").ToString();
                _startstopCTRL = Convert.ToBoolean(UserPrefs.GetValue("startstopCTRL").ToString());
                _startstopALT = Convert.ToBoolean(UserPrefs.GetValue("startstopALT").ToString());
                _startstopSHIFT = Convert.ToBoolean(UserPrefs.GetValue("startstopSHIFT").ToString());
                _startstopENABLED = Convert.ToBoolean(UserPrefs.GetValue("startstopENABLED").ToString());

            }
            else
            {
                // Key did not exist so use defaults
                _ToLanguage = "el";
                _FromLanguage = "en";
                _Key = "F11";
                _CTRL = true;
                _ALT = false;
                _SHIFT = false;

                _startstopKey = "F12";
                _startstopCTRL = true;
                _startstopALT = false;
                _startstopSHIFT = false;
                _startstopENABLED = true;
            }
        }

        /// <summary>
        /// συνάρτηση αποθήκευσης των ρυθμίσεων
        /// </summary>
        public void Save()
        {
            RegistryKey UserPrefs = Registry.CurrentUser.OpenSubKey("SOFTWARE\\TEIKAV\\Translator", true);

            if (UserPrefs == null)
            {
                // Value does not already exist so create it
                RegistryKey newKey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\TEIKAV");
                UserPrefs = newKey.CreateSubKey("Translator");
            }

            UserPrefs.SetValue("ToLanguage", _ToLanguage);
            UserPrefs.SetValue("FromLanguage", _FromLanguage);
            UserPrefs.SetValue("Key", _Key);
            UserPrefs.SetValue("CTRL", _CTRL);
            UserPrefs.SetValue("ALT", _ALT);
            UserPrefs.SetValue("SHIFT", _SHIFT);

            UserPrefs.SetValue("startstopKey", _startstopKey);
            UserPrefs.SetValue("startstopCTRL", _startstopCTRL);
            UserPrefs.SetValue("startstopALT", _startstopALT);
            UserPrefs.SetValue("startstopSHIFT", _startstopSHIFT);
            UserPrefs.SetValue("startstopENABLED", _startstopENABLED);

        }
    }
}
