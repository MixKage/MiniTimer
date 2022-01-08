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
using System.Windows.Threading;

namespace MiniTimer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool StartSec = false;
        private int increment = 0;
        DispatcherTimer dt;
        DispatcherTimer globalTimer;
        private int globalTick = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Windows_Loaded(object sender, RoutedEventArgs e)
        {
            Ellipse.MouseLeftButtonDown += OnEllipseMouseLeftButtonDown;
            Ellipse.MouseRightButtonDown += OnEllipseMouseRightButtonDown;
            CreateTimers();
        }

        private void CreateTimers()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += dtTricker;
        }

        void OnEllipseMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(globalTimer != null)
                globalTimer.Stop();
            increment = 0;
            globalTick = 0;
            TimerLabel.Content = "0:00";
            TimerLabel.Opacity = 1;
            CreateTimers();
        }

        void OnEllipseMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartSec = !StartSec;
            if (StartSec)
            {
                infoText.Opacity = 0;             
                TimerLabel.Opacity = 1;
                if(globalTimer!=null)
                    globalTimer.Stop();
                dt.Start();
            }
            else
            {
                globalTimer = new DispatcherTimer();
                globalTimer.Interval = TimeSpan.FromMilliseconds(700);
                globalTimer.Tick += globalTricker;
                globalTimer.Start();
                dt.Stop();
            }
        }

        private void globalTricker(object sender, EventArgs e)
        {
            globalTick++;
            if (globalTick % 2 == 1)
                TimerLabel.Opacity = 0;
            else
                TimerLabel.Opacity = 1;
        }

        private void dtTricker(object sender, EventArgs e)
        {
            increment++;
            int min = increment / 60;
            int sec = increment % 60;
            string time;
            if (sec / 10 != 0)
                time = min.ToString() + ":" + sec.ToString();
            else
                time = min.ToString() + ":0" + sec.ToString();
            TimerLabel.Content = time;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
