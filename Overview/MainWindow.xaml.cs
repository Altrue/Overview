using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

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
        private const int WINDOW_HEIGHT = 400;          // in pixels

        // Variables
        private bool isDragMovable = true;
        PerformanceCounter pc0;
        PerformanceCounter pc1;
        PerformanceCounter pc2;
        PerformanceCounter pc3;

        // Handle Recuperation Tools
        [DllImport("user32.dll")]
        static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(
            IntPtr hWndChild,      // Child Handle
            IntPtr hWndNewParent   // New Parent Handle
        );

        public MainWindow()
        {
            // Handle recuperation for current window
            GCHandle handle1 = GCHandle.Alloc(Process.GetCurrentProcess().MainWindowHandle);
            IntPtr hwndf = (IntPtr) handle1;
            IntPtr hwndParent = GetDesktopWindow();
            SetParent(hwndf, hwndParent);

            // Initialization
            InitializeComponent();
            Width = WINDOW_WIDTH;
            Height = WINDOW_HEIGHT;
            ResizeMode = ResizeMode.CanMinimize;
            WindowStyle = WindowStyle.None;
            Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x22, 0x22, 0x22));

            // Event assignation
            MouseLeftButtonDown += MainWindow_MouseDown;
            Loaded += MainWindowLoaded;

            // Main Canvas Initialization
            MainCanvas.Width = WINDOW_WIDTH;
            MainCanvas.Height = WINDOW_HEIGHT;
            MainCanvas.HorizontalAlignment = HorizontalAlignment.Left;
            MainCanvas.VerticalAlignment = VerticalAlignment.Top;

            // Lock button
            Rectangle bt_Lock = new Rectangle();
            bt_Lock.Width = 25;
            bt_Lock.Height = 25;
            bt_Lock.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x44, 0x22, 0x22));
            bt_Lock.MouseLeftButtonDown += LockButton_MouseDown;
            Canvas.SetTop(bt_Lock, (0));
            Canvas.SetRight(bt_Lock, (0));
            MainCanvas.Children.Add(bt_Lock);

            // Test button
            Rectangle bt_Test = new Rectangle();
            bt_Test.Width = 25;
            bt_Test.Height = 25;
            bt_Test.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x44, 0x88, 0x99));
            bt_Test.MouseLeftButtonDown += TestButton_MouseDown;
            Canvas.SetTop(bt_Test, (0));
            Canvas.SetLeft(bt_Test, (0));
            MainCanvas.Children.Add(bt_Test);

            // Processor Manager
            var pc = new PerformanceCounter("Processor", "% Processor Time");
            var cat = new PerformanceCounterCategory("Processor");
            var instances = cat.GetInstanceNames();

            foreach (var s in instances)
            {
                pc.InstanceName = s;
                Console.WriteLine("instance name is {0}", s);
            }

            pc0 = new PerformanceCounter("Processor", "% Processor Time", "0");
            pc1 = new PerformanceCounter("Processor", "% Processor Time", "1");
            pc2 = new PerformanceCounter("Processor", "% Processor Time", "2");
            pc3 = new PerformanceCounter("Processor", "% Processor Time", "3");

            // Will return 0 the first time, so better return it now.
            Console.WriteLine(pc0.NextValue());
            Console.WriteLine(pc1.NextValue());
            Console.WriteLine(pc2.NextValue());
            Console.WriteLine(pc3.NextValue());
        }

        // Timer
        void MainWindowLoaded(object _sender, RoutedEventArgs _e)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200); // Timer Length
            dispatcherTimer.Start();
        }

        // Action to execute each tick
        void dispatcherTimer_Tick(object sender, EventArgs _e)
        {
            if (WindowState != WindowState.Normal)
            {
                WindowState = WindowState.Normal;
            }
        }

        // Lock / Unlock
        public void LockButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDragMovable = (isDragMovable == true ? false : true);
            Rectangle bt_Lock = (Rectangle)e.Source;
            bt_Lock.Fill = new SolidColorBrush((isDragMovable == true ? Color.FromArgb(0xFF, 0x88, 0x22, 0x22) : Color.FromArgb(0xFF,0x44, 0x22, 0x22)));
        }

        // Test
        public void TestButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Click Enregistré");

            Console.WriteLine(pc0.NextValue());
            Console.WriteLine(pc1.NextValue());
            Console.WriteLine(pc2.NextValue());
            Console.WriteLine(pc3.NextValue());
        }

        // DragMove
        public void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isDragMovable == true)
            {
                DragMove();
            }
        }
    }
}
