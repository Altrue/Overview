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
    class ProcessorManager
    {
        /*
        //const
        public const int CAR_HEIGHT = 18;                       // Height of the car in pixels

        //param
        public static double MIN_DISTANCE = 45;                 // in pixels

        // Attributes - Leave public if requested very often to positively impact performances.
        private Road road;                                      

        public Road Road
        {
            get { return road; }
            set { road = value; }
        }*/

        // ===============================
        /*
        The Processor Manager fills pcArrayCPU with PerformanceCounter objects,
        fills tbArrayCPU with TextBox items to display those values,
        fills CPUPolyLines with Polylines to draw CPU graphs, and
        fills CoreData with empty CPU Core Data.
        */
        // ===============================

        public ProcessorManager()
        {
            var pc = new PerformanceCounter("Processor", "% Processor Time");
            var cat = new PerformanceCounterCategory("Processor");
            var instances = cat.GetInstanceNames();

            int instanceNumber = 0;
            short instanceNumber2 = 0;

            foreach (var s in instances)
            {
                pc.InstanceName = s;
                Console.WriteLine("Instance name is {0}", s);

                if (s == "_Total")
                {
                    GD.pcArrayCPU[0] = new PerformanceCounter("Processor", "% Processor Time", s);
                }
                else
                {
                    GD.coreNumber++;

                    instanceNumber = int.Parse(s) + 1;
                    instanceNumber2 = (short)(instanceNumber - 1);

                    GD.CoreData[instanceNumber2] = new List<int>();
                    GD.CPUPolyLines[instanceNumber2] = new Polyline();

                    GD.CoreData[instanceNumber2].Add(0);
                    for (int i = 0; i < 71; i++)
                    {
                        GD.CoreData[instanceNumber2].Add(-1);
                    }

                    GD.pcArrayCPU[instanceNumber] = new PerformanceCounter("Processor", "% Processor Time", s);
                    GD.tbArrayCPU[instanceNumber] = new TextBlock();
                    GD.tbArrayCPU[instanceNumber].Text = "?";
                    GD.tbArrayCPU[instanceNumber].Foreground = new SolidColorBrush(Colors.White);
                    Canvas.SetTop(GD.tbArrayCPU[instanceNumber], (50 + instanceNumber * 15));
                    Canvas.SetLeft(GD.tbArrayCPU[instanceNumber], (30));
                    GD.MainCanvas.Children.Add(GD.tbArrayCPU[instanceNumber]);
                }
            }

            // Will return 0 the first time, so better return it now.
            foreach (PerformanceCounter _pc in GD.pcArrayCPU)
            {
                if (_pc != null)
                {
                    Console.WriteLine(_pc.InstanceName + " : " + _pc.NextValue());
                }
            }
        }
    }
}
