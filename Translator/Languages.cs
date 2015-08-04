using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator
{
    /// <summary>
    /// βοηθητική κλάση που "κρατάει" λίστα με της υποστηριζόμενες γλώσσες για μετάφραση και για ήχο.
    /// </summary>
    class Languages
    {
        /// <summary>
        /// οι υποστηριζόμενες γλώσσες για μετάφραση σε μορφή πίνακα
        /// </summary>
        private static String[] supportedLanguages = new String[] { "ar - Arabic", "cs - Czech", "ar - Arabic", "cs - Czech", "da - Danish", 
            "de - German", "en - English", "et - Estonian", "fi - Finnish", "fr - French", "nl - Dutch", "el - Greek", "he - Hebrew", 
            "ht - Haitian", "hu - Hungarian", "id - Indonesian", "it - Italian", "ja - Japanese", "ko - Korean", "lt - Lithuanian", 
            "lv - Latvian", "no - Norwegian", "pl - Polish	", "pt - Portuguese", "ro - Romanian", "es - Spanish", "ru - Russian", 
            "sk - Slovak", "sl - lovene", "sv - Swedish", "th - Thai", "tr - Turkish", "uk - Ukrainian", "vi - Vietnamese", 
            "zh-CHS - S Chinese", "zh-CHT - T Chinese" };
        /// <summary>
        /// οι υποστηριζόμενες γλώσσες για ήχο σε μορφή πίνακα
        /// </summary>
        private static String[] supportedVoiceLanguages = new String[] { "ca", "da", "de", "en", "es", "fi", "fr", "it", "ja", "ko", "nl", "no", "pl", "pt", "ru", "sv" };

        /// <summary>
        /// η λίστα που εμφανίζεται στα "comboboxes" της εφαρμογή μας,
        /// όπως φαίνεται και στον static δημιουργό της κλάσσης εμπεριέχει ότι και η λίστα supportedLanguages,
        /// απλά και το "auto" που είναι για την αυτόματη αναγνώριση.
        /// </summary>
        public static ArrayList list = new ArrayList();
        
        /// <summary>
        /// static δημιουργός
        /// </summary>
        static Languages()
        {
            foreach (String language in supportedLanguages)
            {
                String displayName = language.Substring(language.IndexOf("-") + 2);
                list.Add(new Language(language, displayName));
            }
            list.Add(new Language("auto","Αυτόματη αναγνώριση"));
        }

        /// <summary>
        /// λίστα μόνο με το πως "ονομάζετε" η γλώσσα και όχι τον κωδικό αναφοράς.
        /// π.χ. για τα Ελληνικά εμφανίζεται το "Greek" και όχι το "el"
        /// </summary>
        internal static ArrayList getDisplayNameList()
        {
            ArrayList tempList = new ArrayList();
            foreach (Language language in list)
            {
                tempList.Add(language.DisplayName);
            }
            return tempList;
        }

        /// <summary>
        /// λίστα με τις γλώσσες χωρίς το auto
        /// </summary>
        internal static ArrayList getDisplayNameListWithoutAuto()
        {
            ArrayList tempList = getDisplayNameList();

            tempList.RemoveAt(list.Count - 1);
            return tempList;
        }

        /// <summary>
        /// επιστροφή δείκτη θέσης συγκεκριμένης γλώσσας στην λίστα
        /// </summary>
        internal static int getIndexOfLanguage(string codeName)
        {
            int rv=0;
            for (int i = 0; i < list.Count; i++)
            {
                if (codeName.Equals(((Language)list[i]).CodeName))
                    rv = i;
            }
            return rv;
        }

        /// <summary>
        /// επιστροφή κωδικού αναφοράς γλώσσας με βάση την περιγραφή γλώσσας
        /// </summary>
        internal static String getCodeNameFromDisplayName(string displayName)
        {
            String codeName = ((Language)list[0]).CodeName;
            foreach (Language language in list)
            {
                if (language.DisplayName.Equals(displayName))
                    codeName = language.CodeName;
            }
            return codeName;
        }

        /// <summary>
        /// επιστρέφει αν η γλώσσα υποστηρίζει ήχο
        /// </summary>
        internal static bool IsSpeakLanguage(string languageShort)
        {
            bool rv = false;
            foreach (String language in supportedVoiceLanguages)
            {
                if (language.Equals(languageShort))
                    rv = true;
            }
            return rv;
        }
    }

    /// <summary>
    /// υποκλάσση για την κάθε γλώσσα χωριστά
    /// </summary>
    class Language
    {
        private String codeName;

        public String CodeName
        {
            get { return codeName; }
        }
        private String displayName;

        public String DisplayName
        {
            get { return displayName; }
        }

        public Language(String codeName, String displayName)
        {
            this.codeName = codeName;
            this.displayName = displayName;

        }
    }
}
