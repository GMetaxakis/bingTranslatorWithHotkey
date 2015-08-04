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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Translator
{
    /// <summary>
    /// Κλάση που διαχειρίζεται την λογική του UI των ρυθμίσεων.
    /// </summary>
    public partial class Preferences : Window
    {
        TranslatorUI translatorUI;
        RegUserPreferences data;
        GlobalHotkeys hotKeyToTranslate,hotKeyToStartStop;
        Boolean hotKeyToTranslateRegistered = false;

        /// <summary>
        /// δημιουργός της κλάσης
        /// </summary>
        public Preferences()
        {
            InitializeComponent();

            loadPreferences();           
        }

        /// <summary>
        /// συνάρτηση ελέγχου κλείσουμου του παραθύρου ώστε να το κρύψει απλώς και όχι να το κλείσει.
        /// </summary>
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            e.Cancel = true;
        }

        /// <summary>
        /// συνάρτηση ελέγχου ότι φορτώθηκε η φόρμα,
        /// ώστε να ξεκινήσει κάποιες ρυθμίσεις που έχουν αποθηκευτεί να ξεκινάνε στην εκκίνηση
        /// </summary>
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            source.AddHook(new HwndSourceHook(WndProc));

            if (data.StartstopENABLED)
            {
                registerHotKeyToStartStop();
            }
        }

        /// <summary>
        /// συνάρτηση διαχείρισης μυνημάτων σε επίπεδο WIN API
        /// </summary>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //is it correct to "be" here?
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    {
                        if ((long)lParam == 8060930 ) //8060930 ??
                        //if ((short)wParam == hotKeyToStartStop.HotkeyID)
                        {
                            startStopTranslate();
                        }
                        else if ((long)lParam == 7995394)
                        //else if ((short)wParam == hotKeyToTranslate.HotkeyID)
                        {
                            translate();
                        }
                        break;
                    }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// συνάρτηση για το φόρτωμα των ρυθμίσεων από την registry του υπολογιστή
        /// </summary>
        private void loadPreferences()
        {
            data = new RegUserPreferences();
            FromLanguageCombo.ItemsSource = Languages.getDisplayNameList();
            ToLanguageCombo.ItemsSource = Languages.getDisplayNameListWithoutAuto();

            translateKeyCombo.ItemsSource = Keyboard.keys;
            startstopKeyCombo.ItemsSource = Keyboard.keys;

            FromLanguageCombo.SelectedIndex = Languages.getIndexOfLanguage(data.FromLanguage);
            ToLanguageCombo.SelectedIndex = Languages.getIndexOfLanguage(data.ToLanguage);

            translateKeyCombo.Text = data.Key;
            translateKeyCTRL.IsChecked = data.CTRL;
            translateKeyALT.IsChecked = data.ALT;
            translateKeySHIFT.IsChecked = data.SHIFT;

            startstopKeyCombo.Text = data.StartstopKey;
            startstopKeyCTRL.IsChecked = data.StartstopCTRL;
            startstopKeyALT.IsChecked = data.StartstopALT;
            startstopKeySHIFT.IsChecked = data.StartstopSHIFT;
            startstopKeyEnable.IsChecked = data.StartstopENABLED;
        }

        /// <summary>
        /// συνάρτηση που χρησιμοποιείται για την μετάφραση του επιλεγμένου κειμένου
        /// </summary>
        private void translate()
        {
            string source = hotKeyToTranslate.GetTextFromFocusedControl(new WindowInteropHelper(this).Handle);
            string result = Translate.translate(source);
            if (result.Equals("TranslatorException"))
            {
                Translate.init();
                result = Translate.translate(source);
            }
            Console.WriteLine("TRANSLATE source :" + source + " , result : " + result);
            
            notifyTray.ShowBalloonTip("Translator", "Trying to translate message...");

            if (translatorUI == null)
            {
                translatorUI = new TranslatorUI();
                translatorUI.OnClose += translatorUI_OnClose;
                translatorUI.setSourceText(source);
                translatorUI.setResultText(result);
                translatorUI.Show();
            }
            else
            {
                translatorUI.setSourceText(source);
                translatorUI.setResultText(result);
                if (translatorUI.WindowState == WindowState.Minimized)
                    translatorUI.WindowState = WindowState.Normal;
                translatorUI.Focus();
            }
        }

        /// <summary>
        /// βοηθητική συνάρτηση ώστε να διαγράφει από την μνήμη το παράθυρο μετάφρασης
        /// </summary>
        private void translatorUI_OnClose()
        {
            translatorUI = null;
        }


        /// <summary>
        /// συνάρτηση που αποθηκεύει τις ρυθμίσεις στην registry του υπολογιστή
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            data.FromLanguage = Languages.getCodeNameFromDisplayName(FromLanguageCombo.Text);
            data.ToLanguage = Languages.getCodeNameFromDisplayName(ToLanguageCombo.Text);
            data.Key = translateKeyCombo.Text;
            data.CTRL = translateKeyCTRL.IsChecked.Value;
            data.ALT = translateKeyALT.IsChecked.Value;
            data.SHIFT = translateKeySHIFT.IsChecked.Value;
            data.StartstopKey = startstopKeyCombo.Text;
            data.StartstopCTRL = startstopKeyCTRL.IsChecked.Value;
            data.StartstopALT = startstopKeyALT.IsChecked.Value;
            data.StartstopSHIFT = startstopKeySHIFT.IsChecked.Value;
            bool oldStartStopEnabled = data.StartstopENABLED;
            data.StartstopENABLED = startstopKeyEnable.IsChecked.Value;

            notifyTray.ShowBalloonTip("Translator", "Settings saved!");
            data.Save();
            if (data.StartstopENABLED && !oldStartStopEnabled)
            {
                registerHotKeyToStartStop();
            }
            else if (!data.StartstopENABLED && oldStartStopEnabled)
            {
                unRegisterHotKeyToStartStop();
            }
        }
        
        /// <summary>
        /// συνάρτηση ανταλλαγής επιλεγμένων γλωσσών.
        /// </summary>
        private void SwapLanguages(object sender, RoutedEventArgs e)
        {
            int pos = FromLanguageCombo.SelectedIndex;
            FromLanguageCombo.SelectedIndex = ToLanguageCombo.SelectedIndex;
            ToLanguageCombo.SelectedIndex = pos;
        }

        /// <summary>
        /// συνάρτηση καταχώρησης HotKey για την ενεργοποίηση / απενεργοποίηση του συστήματος
        /// μαζί με την διαγραφή του χρησιμοποίεται ώστε να μην είναι ενεργοποιημένο πάντα το HotKey για την μετάφραση
        /// </summary>
        private void registerHotKeyToStartStop()
        {
            System.Windows.Input.Key myKey = (System.Windows.Input.Key)Enum.Parse(typeof(System.Windows.Input.Key), data.StartstopKey);
            //Console.WriteLine("registerHotKeyToStartStop");
            //Console.WriteLine(myKey + " " + data.StartstopCTRL + " " + data.StartstopALT + " " + data.StartstopSHIFT);
            hotKeyToStartStop = new GlobalHotkeys();
            hotKeyToStartStop.RegisterGlobalHotKey(KeyInterop.VirtualKeyFromKey(myKey), data.StartstopCTRL, data.StartstopALT, data.StartstopSHIFT, new WindowInteropHelper(this).Handle);
        }

        /// <summary>
        /// συνάρτηση διαγραφής της καταχώρησης HotKey για την ενεργοποίηση / απενεργοποίηση του συστήματος
        /// </summary>
        private void unRegisterHotKeyToStartStop()
        {
            hotKeyToStartStop.UnregisterGlobalHotKey();
        }

        /// <summary>
        /// συνάρτηση ενεργοποίησης / απενεργοποίησης δυνατότητας μετάφρασης με την χρήση HotKey
        /// </summary>
        private void startStopTranslate()
        {
            if (!hotKeyToTranslateRegistered)
            {
                notifyTray.ShowBalloonTip("Translator", "Το Hotkey για την μετάφραση ενεργοποιήθηκε");
                System.Windows.Input.Key myKey = (System.Windows.Input.Key)Enum.Parse(typeof(System.Windows.Input.Key), data.Key);
                //Console.WriteLine(myKey.ToString());
                hotKeyToTranslate = new GlobalHotkeys();
                hotKeyToTranslate.RegisterGlobalHotKey(KeyInterop.VirtualKeyFromKey(myKey), data.CTRL, data.ALT, data.SHIFT, new WindowInteropHelper(this).Handle);
                //Translate.init();
                hotKeyToTranslateRegistered = true;
            }
            else
            {
                notifyTray.ShowBalloonTip("Translator", "Το Hotkey για την μετάφραση απενεργοποιήθηκε");
                hotKeyToTranslate.UnregisterGlobalHotKey();
                hotKeyToTranslateRegistered = false;
            }
        }

        /// <summary>
        /// συνάρτηση διαχείρισης των επιλογών που έγιναν στο TrayIcon
        /// </summary>
        private void Button_OnPushed(object sender)
        {
            String msg = sender as string;
            if (msg.Equals("SETTINGS"))
            {
                loadPreferences();
                this.Visibility = Visibility.Visible;
            }
            else if (msg.Equals("TRANSLATOR"))
            {
                translatorUI = new TranslatorUI();
                translatorUI.OnClose += translatorUI_OnClose;
                translatorUI.Show();
                this.Visibility = Visibility.Collapsed;
            }
            else if (msg.Equals("START/STOP"))
            {
                startStopTranslate();

            }
            else if (msg.Equals("EXIT"))
                Application.Current.Shutdown();
        }
    }
}