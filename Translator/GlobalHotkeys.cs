using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Translator
{
    /// <summary>
    /// Κλάση που γίνεται η διαχείριση των HotKeys
    /// </summary>
    public class GlobalHotkeys : IDisposable
    {
        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hwnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32", SetLastError = true)]
        public static extern int UnregisterHotKey(IntPtr hwnd, int id);
        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalAddAtom(string lpString);
        [DllImport("kernel32", SetLastError = true)]
        public static extern short GlobalDeleteAtom(short nAtom);

        public const int MOD_ALT = 0x0001;
        public const int MOD_CONTROL = 0x0002;
        public const int MOD_SHIFT = 0x0004;

        public GlobalHotkeys()
        {
            this.Handle = Process.GetCurrentProcess().Handle;
        }

        /// <summary>Handle of the current process</summary>
        public IntPtr Handle;

        /// <summary>The ID for the hotkey</summary>
        public short HotkeyID { get; private set; }

        /// <summary>Register the hotkey</summary>
        public void RegisterGlobalHotKey(int hotkey, int modifiers, IntPtr handle)
        {
            UnregisterGlobalHotKey();
            this.Handle = handle;
            RegisterGlobalHotKey(hotkey, modifiers);
        }

        /// <summary>Register the hotkey</summary>
        public void RegisterGlobalHotKey(int hotkey, Boolean ctrl, Boolean alt, Boolean shift, IntPtr handle)
        {
            UnregisterGlobalHotKey();
            this.Handle = handle;

            RegisterGlobalHotKey(hotkey, booleansToModifiers(ctrl, alt, shift));
        }

        
        private int booleansToModifiers(Boolean ctrl, Boolean alt, Boolean shift)
        {
            if (ctrl && alt && shift)
                return MOD_CONTROL | MOD_ALT | MOD_SHIFT;
            else if (ctrl && alt && !shift)
                return MOD_CONTROL | MOD_ALT;
            else if (ctrl && !alt && shift)
                return MOD_CONTROL | MOD_SHIFT;
            else if (!ctrl && alt && shift)
                return MOD_ALT | MOD_SHIFT;
            else if (ctrl && !alt && !shift)
                return MOD_CONTROL;
            else if (!ctrl && alt && !shift)
                return MOD_ALT;
            else if (!ctrl && !alt && shift)
                return MOD_SHIFT;
            else
                return 0;
        }

        /// <summary>Register the hotkey</summary>
        public void RegisterGlobalHotKey(int hotkey, int modifiers)
        {
            UnregisterGlobalHotKey();

            try
            {
                // use the GlobalAddAtom API to get a unique ID (as suggested by MSDN)
                string atomName = Thread.CurrentThread.ManagedThreadId.ToString("X8") + this.GetType().FullName;
                HotkeyID = GlobalAddAtom(atomName);
                if (HotkeyID == 0)
                    throw new Exception("Unable to generate unique hotkey ID. Error: " + Marshal.GetLastWin32Error().ToString());

                // register the hotkey, throw if any error
                if (!RegisterHotKey(this.Handle, HotkeyID, (uint)modifiers, (uint)hotkey))
                    throw new Exception("Unable to register hotkey. Error: " + Marshal.GetLastWin32Error().ToString());

            }
            catch (Exception ex)
            {
                // clean up if hotkey registration failed
                Dispose();
                Console.WriteLine(ex);
            }
        }

        /// <summary>Unregister the hotkey</summary>
        public void UnregisterGlobalHotKey()
        {
            if (this.HotkeyID != 0)
            {
                UnregisterHotKey(this.Handle, HotkeyID);
                // clean up the atom list
                GlobalDeleteAtom(HotkeyID);
                HotkeyID = 0;
            }
        }

        public void Dispose()
        {
            UnregisterGlobalHotKey();
        }

        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        /// <summary>
        /// συνάρτηση που με την χρήση του WIN API,
        /// βρίσκει το παράθυρο που είναι επιλεγμένο από τον χρήστη
        /// και παίρνει το κείμενο που έχει επιλέξει ο χρήστης
        /// </summary>
        public string GetTextFromFocusedControl(IntPtr handle)
        {
            try
            {
                int activeWinPtr = GetForegroundWindow().ToInt32();
                int activeThreadId = 0, processId;
                activeThreadId = GetWindowThreadProcessId(activeWinPtr, out processId);
                int currentThreadId = GetCurrentThreadId();
                if (activeThreadId != currentThreadId)
                    AttachThreadInput(activeThreadId, currentThreadId, true);
                IntPtr activeCtrlId = GetFocus();

                return CopyText(activeCtrlId);
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }

        /// <summary>
        /// με την χρήση της κλάσης Keyboard και την παραπάνω συνάρτηση {GetTextFromFocusedControl},
        /// αντιγράφεται το κείμενο που έχει επιλέξει ο χρήστης, σε οποιοδήποτε παράθυρο.
        /// στην ουσία η κλάση Keyboard μας βοηθάει και κάνουμε εικονική αντιγραφή
        /// και επιστρέφουμε το αντιγραμμένο κείμενο.
        /// </summary>
        private string CopyText(IntPtr handle)
        {
            Keyboard.SimulateKeyStroke('c', true, false, false);
            Thread.Sleep(200);
            return Clipboard.GetText();
        }

        [StructLayout(LayoutKind.Sequential)]
        public class Point
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point pt);

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll", EntryPoint = "SendMessageW")]
        public static extern int SendMessageW([InAttribute] System.IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageW")]
        public static extern int SendMessageW([InAttribute] System.IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageW")]
        public static extern int SendMessageW([InAttribute] System.IntPtr hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);


        public const int WM_GETTEXT = 13;
        public const int WM_COPYDATA = 0x004A;
        public const int WM_COPY = 0x0301;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetFocus();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowThreadProcessId(int handle, out int processId);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);
        [DllImport("kernel32.dll")]
        internal static extern int GetCurrentThreadId();

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpString, int nMaxCount);
    }
}
