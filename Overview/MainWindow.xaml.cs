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
        private PerformanceCounter[] pcArray = new PerformanceCounter[17];
        private int tickCounter = 0;

        // UI Elements
        private TextBlock[] tbArray = new TextBlock[32];

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

            // Logo
            Rectangle bt_logo = new Rectangle();
            bt_logo.Width = 91;
            bt_logo.Height = 13;
            bt_logo.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/ressources/overviewlogo_mini.png")));
            Canvas.SetTop(bt_logo, (12));
            Canvas.SetLeft(bt_logo, (30));
            MainCanvas.Children.Add(bt_logo);

            // Lock button
            Rectangle bt_Lock = new Rectangle();
            bt_Lock.Width = 14;
            bt_Lock.Height = 13;
            bt_Lock.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/ressources/lockopen_mini.png")));
            bt_Lock.MouseLeftButtonDown += LockButton_MouseDown;
            Canvas.SetTop(bt_Lock, (5));
            Canvas.SetLeft(bt_Lock, (6));
            MainCanvas.Children.Add(bt_Lock);

            // Exit button
            Rectangle bt_Exit = new Rectangle();
            bt_Exit.Width = 14;
            bt_Exit.Height = 14;
            bt_Exit.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/ressources/exit_mini.png")));
            bt_Exit.MouseLeftButtonDown += ExitButton_MouseDown;
            Canvas.SetTop(bt_Exit, (5));
            Canvas.SetRight(bt_Exit, (6));
            MainCanvas.Children.Add(bt_Exit);

            // CPU Usage
            tbArray[0] = new TextBlock();
            tbArray[0].Text = "?";
            tbArray[0].Foreground = new SolidColorBrush(Colors.White);
            Canvas.SetTop(tbArray[0], (50));
            Canvas.SetLeft(tbArray[0], (30));
            MainCanvas.Children.Add(tbArray[0]);

            // Processor Manager
            var pc = new PerformanceCounter("Processor", "% Processor Time");
            var cat = new PerformanceCounterCategory("Processor");
            var instances = cat.GetInstanceNames();

            int instanceNumber = 0;

            foreach (var s in instances)
            {
                pc.InstanceName = s;
                Console.WriteLine("Instance name is {0}", s);

                if (s == "_Total")
                {
                    pcArray[0] = new PerformanceCounter("Processor", "% Processor Time", s);
                }
                else
                {
                    instanceNumber = int.Parse(s) + 1;
                    pcArray[instanceNumber] = new PerformanceCounter("Processor", "% Processor Time", s);
                    tbArray[instanceNumber] = new TextBlock();
                    tbArray[instanceNumber].Text = "?";
                    tbArray[instanceNumber].Foreground = new SolidColorBrush(Colors.White);
                    Canvas.SetTop(tbArray[instanceNumber], (50 + instanceNumber * 50));
                    Canvas.SetLeft(tbArray[instanceNumber], (30));
                    MainCanvas.Children.Add(tbArray[instanceNumber]);
                }
            }

            // Will return 0 the first time, so better return it now.
            foreach (PerformanceCounter _pc in pcArray)
            {
                if (_pc != null)
                {
                    Console.WriteLine(_pc.InstanceName + " : " + _pc.NextValue());
                }
            }
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

            tickCounter++;
            if (tickCounter == 5)
            {
                // Every second, do something.
                foreach (PerformanceCounter _pc in pcArray)
                {
                    if (_pc != null)
                    {
                        if (_pc.InstanceName != "_Total")
                        {
                            int tbArrayNumber = int.Parse(_pc.InstanceName) + 1;
                            tbArray[tbArrayNumber].Text = _pc.InstanceName + " : " + Math.Truncate(_pc.NextValue()) + " %";
                        }
                        else
                        {
                            tbArray[0].Text = "CPU Total : " + Math.Truncate(pcArray[0].NextValue()) + "%";
                        }
                    }
                }
                tickCounter = 0;
            }
        }

        // Lock / Unlock
        public void LockButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDragMovable = (isDragMovable == true ? false : true);
            Rectangle bt_Lock = (Rectangle)e.Source;
            bt_Lock.Fill = new SolidColorBrush((isDragMovable == true ? Color.FromArgb(0xFF, 0x88, 0x22, 0x22) : Color.FromArgb(0xFF,0x44, 0x22, 0x22)));
            bt_Lock.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/ressources/lock" + (isDragMovable == true ? "open" : "closed") + "_mini.png")));
        }

        // Test
        public void ExitButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Exit Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
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
