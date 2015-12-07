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
        // Variables
        private bool isDragMovable = true;
        private bool isClosing = false;
        private int tickCounter = 4;
        private int closeTickCount = 0;
        private double graphStep = GD.CPUGRAPH_WIDTH / 70;

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
            Width = GD.WINDOW_WIDTH;
            Height = GD.WINDOW_HEIGHT;
            ResizeMode = ResizeMode.CanMinimize;
            WindowStyle = WindowStyle.None;
            Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x22, 0x22, 0x22));
            ShowInTaskbar = false;

            // Event assignation
            MouseLeftButtonDown += MainWindow_MouseDown;
            Loaded += MainWindowLoaded;

            // Main Canvas Initialization
            AddChild(GD.MainCanvas);
            GD.MainCanvas.Width = GD.WINDOW_WIDTH;
            GD.MainCanvas.Height = GD.WINDOW_HEIGHT;
            GD.MainCanvas.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            GD.MainCanvas.VerticalAlignment = System.Windows.VerticalAlignment.Top;

            // Logo
            Rectangle bt_logo = new Rectangle();
            bt_logo.Width = 91;
            bt_logo.Height = 13;
            bt_logo.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/ressources/overviewlogo_mini.png")));
            Canvas.SetTop(bt_logo, (12));
            Canvas.SetLeft(bt_logo, (30));
            GD.MainCanvas.Children.Add(bt_logo);

            // Head Text
            TextBlock tb_head = new TextBlock();
            tb_head.Text = "";
            tb_head.Foreground = new SolidColorBrush(Colors.Gray);
            tb_head.FontSize = 10;
            Canvas.SetTop(tb_head, (25));
            Canvas.SetLeft(tb_head, (7));
            GD.MainCanvas.Children.Add(tb_head);

            TextBlock tb_head2 = new TextBlock();
            tb_head2.Text = "";
            tb_head2.Foreground = new SolidColorBrush(Colors.Gray);
            tb_head2.FontSize = 10;
            Canvas.SetTop(tb_head2, (37));
            Canvas.SetLeft(tb_head2, (7));
            GD.MainCanvas.Children.Add(tb_head2);

            // TEST
            ManagementObjectSearcher mos =
                new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                string nameFull = (string)mo["Name"];
                string[] words = nameFull.Split(' ');
                string tb1Text = "";
                string tb2Text = "";
                //int wordCount = words.Count();
                int tb1Length = 0;
                foreach (string word in words)
                {
                    int wLength = word.Length;
                    if (tb1Length + wLength < 20)
                    {
                        tb1Text += (tb1Length == 0 ? word : (" " + word));
                        tb1Length = tb1Text.Length;
                    }
                    else
                    {
                        tb2Text += (tb2Text.Length == 0 ? word : (" " + word)); ;
                    }
                }

                tb_head.Text = tb1Text;
                tb_head2.Text = tb2Text;
            }

            // Lock button
            Rectangle bt_Lock = new Rectangle();
            bt_Lock.Width = 14;
            bt_Lock.Height = 13;
            bt_Lock.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/ressources/lockopen_mini.png")));
            bt_Lock.MouseLeftButtonDown += LockButton_MouseDown;
            Canvas.SetTop(bt_Lock, (5));
            Canvas.SetLeft(bt_Lock, (6));
            GD.MainCanvas.Children.Add(bt_Lock);

            // Exit button
            GD.bt_Exit.Width = 14;
            GD.bt_Exit.Height = 14;
            GD.bt_Exit.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/ressources/exit_mini.png")));
            GD.bt_Exit.MouseLeftButtonDown += ExitButton_MouseDown;
            Canvas.SetTop(GD.bt_Exit, (5));
            Canvas.SetRight(GD.bt_Exit, (5));
            GD.MainCanvas.Children.Add(GD.bt_Exit);

            // CPU Total
            GD.tbArrayCPU[0] = new TextBlock();
            GD.tbArrayCPU[0].Text = "?";
            GD.tbArrayCPU[0].Foreground = new SolidColorBrush(Colors.White);
            Canvas.SetTop(GD.tbArrayCPU[0], (50));
            Canvas.SetLeft(GD.tbArrayCPU[0], (7));
            GD.MainCanvas.Children.Add(GD.tbArrayCPU[0]);

            // CPU Total bars
            GD.rectCPU[0] = new Rectangle();
            GD.rectCPU[0].Fill = new SolidColorBrush(Colors.Cyan);
            GD.rectCPU[0].Height = 10;
            GD.rectCPU[0].Width = 0;
            Canvas.SetTop(GD.rectCPU[0], (53));
            Canvas.SetLeft(GD.rectCPU[0], (GD.WINDOW_WIDTH / 2 + 1));
            GD.MainCanvas.Children.Add(GD.rectCPU[0]);

            GD.borderCPU[0] = new Border();
            GD.borderCPU[0].Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x11, 0x11, 0x11));
            GD.borderCPU[0].BorderThickness = new Thickness(1);
            GD.borderCPU[0].BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x33, 0x33));
            GD.borderCPU[0].Height = 12;
            GD.borderCPU[0].Width = GD.WINDOW_WIDTH/2 - 5;
            Canvas.SetTop(GD.borderCPU[0], (52));
            Canvas.SetRight(GD.borderCPU[0], (5));
            GD.MainCanvas.Children.Add(GD.borderCPU[0]);

            // Build the Processor Manager
            ProcessorManager PM = new ProcessorManager();

            // Build the CPU Graph
            GraphManager CPUGraph = new GraphManager(GD.coreNumber * 15 + 70, GD.CPUGRAPH_WIDTH, GD.CPUGRAPH_HEIGHT);
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

            if (isClosing == true)
            {
                if (isClosing == true && closeTickCount > 10)
                {
                    isClosing = false;
                    GD.bt_Exit.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/ressources/exit_mini.png")));
                    closeTickCount = 0;
                }
                else
                {
                    closeTickCount++;
                }
            }

            tickCounter++;
            if (tickCounter == 5)
            {
                // Every second, do something.
                foreach (PerformanceCounter _pc in GD.pcArrayCPU)
                {
                    if (_pc != null)
                    {
                        if (_pc.InstanceName != "_Total")
                        {
                            int tbArrayNumber = int.Parse(_pc.InstanceName) + 1;
                            short shorttbArrayNumber = (short)(tbArrayNumber - 1);
                            int _nextvalue = (int)Math.Truncate(_pc.NextValue());
                            int _instancename = tbArrayNumber - 1;
                            int listCount = GD.CoreData[shorttbArrayNumber].Count();
                            GD.tbArrayCPU[tbArrayNumber].Text = "Core " + _instancename + " : " + _nextvalue + " %";

                            GD.rectCPU[tbArrayNumber].Width = Math.Truncate((GD.WINDOW_WIDTH/2 - 6) * ((double)_nextvalue/100));
                            GD.MainCanvas.Children.Remove(GD.rectCPU[tbArrayNumber]);
                            GD.MainCanvas.Children.Add(GD.rectCPU[tbArrayNumber]);

                            GD.CoreData[shorttbArrayNumber].Insert(0, _nextvalue);
                            if (listCount > 71)
                            {
                                GD.CoreData[shorttbArrayNumber].RemoveAt(listCount - 1);
                            }
                        }
                        else
                        {
                            int _nextvalue = (int)Math.Truncate(_pc.NextValue());
                            GD.tbArrayCPU[0].Text = "Total : " + _nextvalue + "%";

                            GD.rectCPU[0].Width = Math.Truncate((GD.WINDOW_WIDTH - 7) * ((double)_nextvalue / 100));
                            GD.MainCanvas.Children.Remove(GD.rectCPU[0]);
                            GD.MainCanvas.Children.Add(GD.rectCPU[0]);
                        }
                    }
                }

                // Re-draw the data
                for (short cpuNumber = 0; cpuNumber < GD.coreNumber; cpuNumber++)
                {
                    PointCollection points = new PointCollection();
                    double x = GD.CPUGRAPH_WIDTH;
                    for (int DataIndex = 0; DataIndex < 72; DataIndex++)
                    {
                        double rawYValue = GD.CoreData[cpuNumber][DataIndex];
                        if (rawYValue >= 0)
                        {
                            int CPUValue = 40 - (int)Math.Round((rawYValue) * 0.4);
                            points.Add(new Point(x, CPUValue));
                        }
                        x -= graphStep;
                    }

                    GD.CPUPolyLines[cpuNumber].StrokeThickness = 1;
                    GD.CPUPolyLines[cpuNumber].Stroke = GD.brushes[cpuNumber];
                    GD.CPUPolyLines[cpuNumber].Points = points;

                    GD.GraphCanvasCPU.Children.Remove(GD.CPUPolyLines[cpuNumber]);
                    GD.GraphCanvasCPU.Children.Add(GD.CPUPolyLines[cpuNumber]);
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

        // Exit
        public void ExitButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isClosing == true)
            {
                Application.Current.Shutdown();
            }
            else
            {
                isClosing = true;
                GD.bt_Exit.Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/ressources/exit_mini2.png")));
            }
        }

        // DragMove
        public void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isDragMovable == true && isClosing == false)
            {
                DragMove();
            }
        }
    }
}
