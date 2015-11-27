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
    class GraphManager
    {
        // Variables
        private double step = 10;

        public GraphManager(int _topSpacing, int _width, int _height)
        {
            GD.GraphCanvasCPU.Width = _width;
            GD.GraphCanvasCPU.Height = _height;
            GD.GraphCanvasCPU.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x11, 0x11, 0x11));

            Canvas.SetTop(GD.GraphCanvasCPU, (_topSpacing));
            Canvas.SetLeft(GD.GraphCanvasCPU, (5));
            GD.MainCanvas.Children.Add(GD.GraphCanvasCPU);

            // Invisible grey border against rebelious polylines
            Border BorderCPUGraphCanvas2 = new Border();
            BorderCPUGraphCanvas2.Width = _width + 10;
            BorderCPUGraphCanvas2.Height = _height + 10;
            BorderCPUGraphCanvas2.BorderThickness = new Thickness(4);
            BorderCPUGraphCanvas2.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x22, 0x22, 0x22));
            Canvas.SetTop(BorderCPUGraphCanvas2, (_topSpacing - 5));
            Canvas.SetLeft(BorderCPUGraphCanvas2, (0));
            GD.MainCanvas.Children.Add(BorderCPUGraphCanvas2);

            // Light grey border
            Border BorderCPUGraphCanvas = new Border();
            BorderCPUGraphCanvas.Width = _width + 2;
            BorderCPUGraphCanvas.Height = _height + 2;
            BorderCPUGraphCanvas.BorderThickness = new Thickness(1);
            BorderCPUGraphCanvas.BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x55, 0x55, 0x55));
            Canvas.SetTop(BorderCPUGraphCanvas, (_topSpacing - 1));
            Canvas.SetLeft(BorderCPUGraphCanvas, (4));
            GD.MainCanvas.Children.Add(BorderCPUGraphCanvas);

            // Vertical lines background
            GeometryGroup xaxis_geom = new GeometryGroup();
            for (double x = 0 + step;
                x <= GD.GraphCanvasCPU.Width - step; x += step)
            {
                xaxis_geom.Children.Add(new LineGeometry(
                    new Point(x, 0),
                    new Point(x, GD.CPUGRAPH_HEIGHT)));
            }

            Path xaxis_path = new Path();
            xaxis_path.StrokeThickness = 1;
            xaxis_path.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x33, 0x33));
            xaxis_path.Data = xaxis_geom;
            xaxis_path.SnapsToDevicePixels = true;
            xaxis_path.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

            GD.GraphCanvasCPU.Children.Add(xaxis_path);

            // Horizontal lines background
            GeometryGroup yaxis_geom = new GeometryGroup();
            for (double y = step; y <= GD.GraphCanvasCPU.Height - step; y += step)
            {
                yaxis_geom.Children.Add(new LineGeometry(
                    new Point(0, y),
                    new Point(GD.CPUGRAPH_WIDTH, y)));
            }

            Path yaxis_path = new Path();
            yaxis_path.StrokeThickness = 1;
            yaxis_path.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x33, 0x33));
            yaxis_path.Data = yaxis_geom;
            yaxis_path.SnapsToDevicePixels = true;
            yaxis_path.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);

            GD.GraphCanvasCPU.Children.Add(yaxis_path);

            //
            // --- END OF GRAPH STUFF ---
            //
        }
    }
}
