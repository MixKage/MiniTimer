using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Windows.Shell;
using System.Windows.Threading;

namespace MiniTimer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isStopWatchMode = true;
        bool StartSec = false;
        private int increment = 0;
        DispatcherTimer dt;
        DispatcherTimer globalTimer;
        private int globalTick = 0;
        public MainWindow()
        {
            InitializeComponent();
            CreateJumpList();
        }
        /// <summary>
        ///Переключение режимов
        /// </summary>
        private void Render()
        {
            if (isStopWatchMode)
            {
                Buttons.Visibility = Visibility.Hidden;
            }
            else
            {
                Buttons.Visibility = Visibility.Visible;
            }
            StartSec = false;
            CreateTimers();
        }
        /// <summary>
        ///При создании окна определяет функции 
        /// </summary>
        private void Windows_Loaded(object sender, RoutedEventArgs e)
        {
            Ellipse.MouseLeftButtonDown += OnEllipseMouseLeftButtonDown;
            Ellipse.MouseRightButtonDown += OnEllipseMouseRightButtonDown;
            CreateTimers();
        }
        /// <summary>
        ///Создаёт таймер для подсчёта времени
        /// </summary>
        private void CreateTimers()
        {
            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(900);
            dt.Tick += dtTricker;
        }
        /// <summary>
        /// Нажатие на циферблат левая кнопка мыши
        /// </summary>
        void OnEllipseMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isStopWatchMode)
            {
                DelTime();
            }
            else
            {
                infoText.Opacity = 1;
                if (globalTimer != null)
                    globalTimer.Stop();
                StartSec = false;
                increment = 0;
                globalTick = 0;
                TimerLabel.Content = "0:00";
                TimerLabel.Opacity = 1;
                dt.Stop();
                CreateTimers();
            }
        }
        /// <summary>
        /// Очищает время
        /// </summary>
        private void DelTime()
        {
            infoText.Opacity = 1;
            if (globalTimer != null)
                globalTimer.Stop();
            StartSec = false;
            increment = 0;
            globalTick = 0;
            TimerLabel.Content = "0:00";
            TimerLabel.Opacity = 1;
            dt.Stop();
            CreateTimers();
        }
        /// <summary>
        /// Нажатие на циферблат правая кнопка мыши
        /// </summary>
        void OnEllipseMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Двойное нажатие мыши
            if (e.ClickCount == 2)
            {
                Window.Topmost = !Window.Topmost;
                if (!Window.Topmost)
                    Window.WindowState = WindowState.Minimized;
            }
            StartSec = !StartSec;
            if (StartSec)
            {
                if (!isStopWatchMode && increment == 0) { StartSec = !StartSec; return; }
                infoText.Opacity = 0;
                TimerLabel.Opacity = 1;
                if (globalTimer != null)
                    globalTimer.Stop();
                dt.Start();
            }
            else
            {
                //Пересоздание таймера для мигания часов во время остановки
                globalTimer = new DispatcherTimer();
                globalTick = 0;
                globalTimer.Interval = TimeSpan.FromMilliseconds(700);
                globalTimer.Tick += globalTricker;
                globalTimer.Start();
                dt.Stop();
            }
        }
        /// <summary>
        /// Мерцание
        /// </summary>
        private void globalTricker(object sender, EventArgs e)
        {
            globalTick++;
            if (globalTick % 2 == 1)
                TimerLabel.Opacity = 0;
            else
                TimerLabel.Opacity = 1;
        }
        /// <summary>
        /// Подсчёт минут и секунд для отрисовки на циферблате
        /// </summary>
        private void dtTricker(object sender, EventArgs e)
        {
            if (isStopWatchMode)
                increment++;
            else
                increment--;
            ShowTime();
        }
        /// <summary>
        /// Отрисовка времени
        /// </summary>
        private void ShowTime()
        {
            if (!isStopWatchMode && increment == 0)
            {
                var notificationManager = new NotificationManager();
                if (StartSec)
                    notificationManager.Show(new NotificationContent
                    {
                        Title = "MiniTimer",
                        Message = "The time is over!",
                        Type = NotificationType.Information
                    });
                DelTime();
            }
            int min = increment / 60;
            int sec = increment % 60;
            string time;
            if (sec / 10 != 0)
                time = min.ToString() + ":" + sec.ToString();
            else
                time = min.ToString() + ":0" + sec.ToString();
            TimerLabel.Content = time;
        }
        /// <summary>
        /// Перемещение окна из любой точки вызова
        /// </summary>        
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
        /// <summary>
        /// Контекстное меню в панели задач
        /// </summary>
        private void CreateJumpList()
        {
            JumpList jumpList = new JumpList();
            JumpList.SetJumpList(Application.Current, jumpList);
            JumpTask Stopwatch = new JumpTask();
            Stopwatch.Title = "Stopwatch";
            Stopwatch.IconResourcePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            Stopwatch.Description = "version 1.0";
            Stopwatch.ApplicationPath = Assembly.GetEntryAssembly().Location;
            Stopwatch.Arguments = "/Stopwatch";
            jumpList.JumpItems.Add(Stopwatch);

            JumpTask Timer = new JumpTask();
            Timer.Title = "Timer";
            Timer.Description = "by NNCompany";
            Timer.ApplicationPath = Assembly.GetEntryAssembly().Location;
            Timer.Arguments = "/Timer";
            Timer.IconResourcePath = Assembly.GetEntryAssembly().Location;
            Stopwatch.IconResourceIndex = 0;
            jumpList.JumpItems.Add(Timer);

            jumpList.Apply();
        }
        /// <summary>
        /// Приём ответа от контекстного меню
        /// </summary>
        public bool ProcessCommandLineArgs(IList<string> args)
        {
            if (args == null || args.Count == 0)
                return true;
            if ((args.Count > 1))
            {
                //the first index always contains the location of the exe so we need to check the second index
                if ((args[1].ToLowerInvariant() == "/stopwatch"))
                {
                    isStopWatchMode = true;
                    DelTime();
                    Render();
                }
                else if ((args[1].ToLowerInvariant() == "/timer"))
                {
                    isStopWatchMode = false;
                    DelTime();
                    Render();
                }
            }
            return true;
        }

        void Increase(object sender, RoutedEventArgs e)
        {
            increment += 10;
            ShowTime();
        }

        void Decrease(object sender, RoutedEventArgs e)
        {
            if (increment == 0) { return; }
            increment -= 10;
            if (increment < 0) { increment = 0; StartSec = false; }
            ShowTime();
        }
    }
}