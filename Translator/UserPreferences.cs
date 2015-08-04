using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    /// <summary>
    /// κλάση που αντιπροσοπεύει τις δυνατότητες του χρήστη για αποθήκευση στην Registry.
    /// </summary>
    class UserPreferences
    {
        char[] separator = { ' ', '-', ' ' };
        
        internal string _FromLanguage;
        public string FromLanguage
        {
            get { return _FromLanguage; }
            set { _FromLanguage = value; }
        }
        
        public string FromLanguageShort
        {
            get
            {
                string[] temp = _FromLanguage.Split(separator);
                return temp[0];
            }
        }
        
        internal string _ToLanguage;
        public string ToLanguage
        {
            get { return _ToLanguage; }
            set { _ToLanguage = value; }
        }
        
        public string ToLanguageShort
        {
            get
            {
                string[] temp = _ToLanguage.Split(separator);
                return temp[0];
            }
        }
        
        internal string _Key;
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }
        
        internal Boolean _CTRL;
        public Boolean CTRL
        {
            get { return _CTRL; }
            set { _CTRL = value; }
        }
        
        internal Boolean _ALT;
        public Boolean ALT
        {
            get { return _ALT; }
            set { _ALT = value; }
        }
        
        internal Boolean _SHIFT;
        public Boolean SHIFT
        {
            get { return _SHIFT; }
            set { _SHIFT = value; }
        }

        internal string _startstopKey;
        public string StartstopKey
        {
            get { return _startstopKey; }
            set { _startstopKey = value; }
        }
        
        internal Boolean _startstopCTRL;
        public Boolean StartstopCTRL
        {
            get { return _startstopCTRL; }
            set { _startstopCTRL = value; }
        }
        
        internal Boolean _startstopALT;
        public Boolean StartstopALT
        {
            get { return _startstopALT; }
            set { _startstopALT = value; }
        }
        
        internal Boolean _startstopSHIFT;
        public Boolean StartstopSHIFT
        {
            get { return _startstopSHIFT; }
            set { _startstopSHIFT = value; }
        }

        internal Boolean _startstopENABLED;
        public Boolean StartstopENABLED
        {
            get { return _startstopENABLED; }
            set { _startstopENABLED = value; }
        }
    }
}
