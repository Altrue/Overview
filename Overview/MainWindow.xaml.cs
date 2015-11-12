using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace Overview
{
    public partial class MainWindow : Window
    {
        // ===============================
        // DEBUG MODE CONTROL
        public const bool DEBUGMODE = false;
        // ===============================

        // Constants
        private const int WINDOW_WIDTH = 150;           // in pixels
        private const int WINDOW_HEIGHT = 400;           // in pixels

        [DllImport("user32.dll")]
        static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(
            IntPtr hWndChild,      // handle to window
            IntPtr hWndNewParent   // new parent window
        );

        public MainWindow()
        {
            IntPtr hwndf = Process.GetCurrentProcess().MainWindowHandle;
            IntPtr hwndParent = GetDesktopWindow();
            SetParent(hwndf, hwndParent);

            InitializeComponent();
            Width = WINDOW_WIDTH;
            Height = WINDOW_HEIGHT;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.None;
            Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x22, 0x22, 0x22));
        }

        public void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }
    }
}
