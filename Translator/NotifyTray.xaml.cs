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
    /// Κλάση που διαχειρίζεται την λογική του UI για το trayIcon.
    /// </summary>
    public partial class NotifyTray : UserControl
    {
        public NotifyTray()
        {
            InitializeComponent();
        }

        /// <summary>
        /// συνάρτηση να προωθήσει στην μητρική κλάση το πάτημα κάποιου κουμπιού
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Console.WriteLine(b.Content);
            if (OnPushed != null)
            {
                OnPushed(b.Content.ToString());
            }
        }

        public delegate void ButtonPushedEventHandler(object sender);
        public event ButtonPushedEventHandler OnPushed;

        /// <summary>
        /// Συνάρτηση εμφάνισης "μπαλονιού" μηνύματος
        /// </summary>
        internal void ShowBalloonTip(string title, string message)
        {
            MyNotifyIcon.ShowBalloonTip(title, message, MyNotifyIcon.Icon);
        }
    }
}
