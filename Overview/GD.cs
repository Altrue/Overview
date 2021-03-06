﻿using System;
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
    // ===============================
    // Global Data class
    // ===============================
    public static class GD
    {
        // ========== DEBUG MODE CONTROL ==========
        public const bool DEBUGMODE = false;
        // ========================================

        // Constants & Variables
            // - Constants
            public const int WINDOW_WIDTH = 150;
            public const int WINDOW_HEIGHT = 52;
            public const int CPUGRAPH_WIDTH = 140;
            public const int CPUGRAPH_HEIGHT = 40;
            // - Variables
            public static int coreNumber = 0;

        // UI Elements
            // - Canvas
            public static Canvas GraphCanvasCPU = new Canvas();
            public static Canvas MainCanvas = new Canvas();
            // - Brushes
            public static Brush[] brushes = {   new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x50, 0x50)),
                                                new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x50, 0x50)), // red
                                                new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x8A, 0x50)), // orange
                                                new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xCC, 0x50)), // gold
                                                new SolidColorBrush(Color.FromArgb(0xFF, 0x72, 0xD4, 0x48)), // green
                                                new SolidColorBrush(Color.FromArgb(0xFF, 0x2A, 0xAE, 0x52)), // deeper green
                                                new SolidColorBrush(Color.FromArgb(0xFF, 0x19, 0xAB, 0x8A)), // greenish cyan
                                                new SolidColorBrush(Color.FromArgb(0xFF, 0x48, 0x9D, 0xDE)), // blue
                                                new SolidColorBrush(Color.FromArgb(0xFF, 0x5D, 0x70, 0xE3)), // violetish blue
                                                Brushes.LightPink,
                                                Brushes.Pink,
                                                Brushes.Purple,
                                                Brushes.DarkRed,
                                                Brushes.DarkOrange,
                                                Brushes.DarkGoldenrod,
                                                Brushes.DarkGreen,
                                                Brushes.DarkSlateBlue,
                                                Brushes.DarkSeaGreen };
            // - Buttons
            public static Rectangle bt_Exit = new Rectangle();
            

        // Data Storage
        // - Dictionaries
        public static Dictionary<Int16, List<int>> CoreData = new Dictionary<Int16, List<int>>();
            public static Dictionary<Int16, Polyline> CPUPolyLines = new Dictionary<Int16, Polyline>();
            // - Arrays
            public static PerformanceCounter[] pcArrayCPU = new PerformanceCounter[17];
            public static TextBlock[] tbArrayCPU = new TextBlock[32];
            public static Rectangle[] rectCPU = new Rectangle[32];
            public static Border[] borderCPU = new Border[32];
    }
}
