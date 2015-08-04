using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Translator
{
    /// <summary>
    /// Κλάση που διαχειρίζεται την λογική του UI του μεταφραστή.
    /// </summary>
    public partial class TranslatorUI : Window
    {
        /// <summary>
        /// οι ρυθμίσεις του χρήστη
        /// </summary>
        RegUserPreferences data;
        public TranslatorUI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// συνάρτηση ελέγχου για το φόρτωμα τις φόρμας, 
        /// ώστε να φορτώσει τις επιλογές του χρήστη στα comboboxeσ
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            data = new RegUserPreferences();
            
            FromLanguageCombo.ItemsSource = Languages.getDisplayNameList();
            ToLanguageCombo.ItemsSource = Languages.getDisplayNameListWithoutAuto();

            FromLanguageCombo.SelectedIndex = Languages.getIndexOfLanguage(data.FromLanguage);
            ToLanguageCombo.SelectedIndex = Languages.getIndexOfLanguage(data.ToLanguage);

            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        /// <summary>
        /// συνάρτηση ελέγχου του πλήκτρου που πατιέται από το πληκτρολόγιο,
        /// ώστε να κλείνει το παράθυρο με την χρήση του κουμπιού ESC
        /// </summary>
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        /// <summary>
        /// συνάρτηση μετάφρασης
        /// </summary>
        private void TranslateText(object sender, RoutedEventArgs e)
        {
            String textToTranslate = TextFrom.Text;

            string result = Translate.translate(textToTranslate);

            if (result.Equals("TranslatorException"))
            {
                Translate.init();
                result = Translate.translate(textToTranslate);
            }
            TextTo.Text = result;
        }

        /// <summary>
        /// συνάρτηση ανταλλαγής επιλεγμένων γλώσσων
        /// </summary>
        private void SwapLanguages(object sender, RoutedEventArgs e)
        {
            int pos = FromLanguageCombo.SelectedIndex;
            FromLanguageCombo.SelectedIndex = ToLanguageCombo.SelectedIndex;
            ToLanguageCombo.SelectedIndex = pos;
        }

        /// <summary>
        /// συνάρτηση εμφάνισης κειμένου "προς μετάφραση"
        /// </summary>
        internal void setSourceText(string source)
        {
            TextFrom.Text = source;
        }

        /// <summary>
        /// συνάρτηση εμφάνισης κειμένου "από μετάφραση"
        /// </summary>
        internal void setResultText(string result)
        {
            TextTo.Text = result;
        }

        /// <summary>
        /// συνάρτηση ελέγχου για το combobox της γλώσσας προς μετάφραση,
        /// ώστε να αποθηκεύει στις ρυθμίσεις την επιλεγμένη γλώσσα,
        /// αλλά και να κάνει αυτόματη μετάφραση εφόσον είναι επιλεγμένη
        /// </summary>
        private void ToLanguageCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string combotext = (sender as ComboBox).SelectedValue as string;

            data.ToLanguage = Languages.getCodeNameFromDisplayName(combotext);
            //Console.WriteLine(data.ToLanguage + " " + combotext);
            data.Save();

            if (AutoTranslateCheckBox.IsChecked.Value)
                TranslateText(this, null);
        }

        /// <summary>
        /// συνάρτηση ελέγχου για το combobox της γλώσσας από μετάφραση,
        /// ώστε να αποθηκεύει στις ρυθμίσεις την επιλεγμένη γλώσσα,
        /// αλλά και εμφανίζεται την δυνατότητα ανταλλαγής γλωσσών, 
        /// εφόσον δεν έχει επιλεχθεί η αυτόματη αναγνώριση.
        /// Επίσης εμφανίζει την δυνατότητα για ηχητικό άκουσμα του κειμένου,
        /// εφόσον αυτό υποστηρίζεται από την συγκεκριμένη γλώσσα
        /// </summary>
        private void FromLanguageCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string combotext = (sender as ComboBox).SelectedValue as string;

            data.FromLanguage = Languages.getCodeNameFromDisplayName(combotext);
            Console.WriteLine(data.FromLanguage + " " + combotext);
            data.Save();
            if (data.FromLanguage.Equals("auto"))
                disableSwap();
            else
                enableSwap();

            if (Languages.IsSpeakLanguage(data.FromLanguageShort))
                SpeakImage.Visibility = Visibility.Visible;
            else
                SpeakImage.Visibility = Visibility.Hidden;

        }

        /// <summary>
        /// ενεργοποίηση ανταλλαγής γλώσσας
        /// </summary>
        private void enableSwap()
        {
            SwapButton.IsEnabled = true;
        }

        /// <summary>
        /// απενεργοποίηση ανταλλαγής γλώσσας
        /// </summary>
        private void disableSwap()
        {
            SwapButton.IsEnabled = false;
        }


        public delegate void ButtonPushedEventHandler();
        public event ButtonPushedEventHandler OnClose;

        /// <summary>
        /// συνάρτηση ελέγχου για το κλείσιμο του παραθύρου
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (OnClose != null)
            {
                OnClose();
            }
        }

        /// <summary>
        /// συνάρτηση ελέγχου για την ρύθμιση αυτόματης μετάφρασης,
        /// ώστε να κάνει την απενεργοποίηση του κουμπιού μετάφρασης
        /// </summary>
        private void AutoTranslateCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TranslateButton.IsEnabled = false;
        }

        /// <summary>
        /// συνάρτηση ελέγχου για την ρύθμιση αυτόματης μετάφρασης,
        /// ώστε να κάνει την ενεργοποίηση του κουμπιού μετάφρασης
        /// </summary>
        private void AutoTranslateCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TranslateButton.IsEnabled = true;
        }

        /// <summary>
        /// συνάρτηση για ξεκινήσει το ηχητικό άκουσμα του κειμένου
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Translate.SpeakMethod(TextFrom.Text);
        }

        /// <summary>
        /// συνάρτηση ελέγχου του κειμένου που γράφεται προς μετάφραση,
        /// αν πατηθεί το ENTER και είναι η ενεργοποιημένη η αυτόματη μετάφραση,
        /// μεταφράζει το μέχρι τώρα κείμενο.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextFrom_TextChanged(object sender, TextChangedEventArgs e)
        {
            string str = TextFrom.Text;
            if (str.Length > 1)
            {
                char ch = str[str.Length - 1];

                if (ch == '\n')
                {
                    if (AutoTranslateCheckBox.IsChecked.Value)
                        TranslateText(this, null);
                }
            }
        }
    }
}
