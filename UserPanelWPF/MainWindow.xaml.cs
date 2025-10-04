using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using UserPanelWPF.Models;
using UserPanelWPF.Services;
using System.Windows.Media.Animation;

namespace UserPanelWPF
{

    public partial class MainWindow : Window
    {
        private string _logFilePath = "";
        private DoubleAnimation _animation;
        private ObservableCollection<string>? _eventLog;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;    
            InitializeFeatures();

        }

        public void InitializeFeatures()
        {
            _eventLog = [];
            LB_EventLog.ItemsSource = _eventLog;
        }

        public void UpdateUserPanelView()
        {
            //VibrationDetector.VibrationDetectorId;
            //VibrationDetector.DeviceName;
            //VibrationDetector.Location;

            //if( VibrationDetector.AlarmArmed)
            //{

            //}

            //Btn_OnOff.Content = "STOP";
            //Btn_OnOff.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F40B0B")); // Red
            //Btn_OnOff.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F40B0B")); // Red

            //LogMessage(DeviceActions.GetDeviceName() + " has started.");
        }

        public async void Btn_Armed_Click(object sender, RoutedEventArgs e)
        {
            VDFetchStatusResponse statusResponse = await VDClientService.FetchStatusVDAsync();

            VibrationDetector.Update(statusResponse);

            //UpdateUserPanelView();

            //LogMessage(statusString);

            //Get to be able to synch with other device.

            //tillfällig
            //string testMeddelande = await DeviceActions.SetVibrationLevel();
            //LogMessage(testMeddelande);

            DeviceActions.ToggleArmedState();

            if (DeviceActions.GetArmedState())
            {
                Btn_OnOff.Content = "STOP";
                Btn_OnOff.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F40B0B")); // Red
                Btn_OnOff.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F40B0B")); // Red

                LogMessage(DeviceActions.GetDeviceName() + " has started.");
            }
            else
            {
                Btn_OnOff.Content = "START";
                Btn_OnOff.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF00")); // Green
                Btn_OnOff.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00FF00")); // Green

                LogMessage(DeviceActions.GetDeviceName() + " has stopped.");

                if (DeviceActions.GetTriggedState() == true)
                {

                    DeviceActions.ToggleTriggedState();

                    Btn_Trigged.Content = "TRIGGER ALARM";
                    Btn_Trigged.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6D825")); // Yellow
                    Btn_Trigged.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6D825")); // Yellow
                    LogMessage(DeviceActions.GetDeviceName() + " alarm has been reset.");
                }
            }


        }
        public void Btn_TriggedState_Click(object sender, RoutedEventArgs e)
        {

            if (!DeviceActions.GetArmedState())
            {
                LogMessage("Cannot trigger alarm when device is not armed.");

            }
            else
            {
                DeviceActions.ToggleTriggedState();

                if (DeviceActions.GetTriggedState() == true)
                {
                    Btn_Trigged.Content = "RESET ALARM";
                    Btn_Trigged.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F40B0B")); // Red
                    Btn_Trigged.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F40B0B")); // Red
                    LogMessage(DeviceActions.GetDeviceName() + " has triggered the alarm!");
                }
                else
                {
                    Btn_Trigged.Content = "TRIGGER ALARM";
                    Btn_Trigged.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6D825")); // Yellow
                    Btn_Trigged.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6D825")); // Yellow
                    LogMessage(DeviceActions.GetDeviceName() + " alarm has been reset.");
                }
            }


        }

        private void Slider_Speed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
        private void LogMessage(string message)
        {
            var line = @$"{DateTime.Now:yyy-MM-dd HH:mm:ss} : {message}";
            _eventLog?.Add(line);

            try
            {
                //File.AppendAllText(_logFilePath, line + Environment.NewLine);
            }
            catch
            {

            }

            if (LB_EventLog.Items.Count > 0)
                LB_EventLog.ScrollIntoView(LB_EventLog.Items[^1]);

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Hook up slider events
            //ScaleYSlider.ValueChanged += ScaleYSlider_ValueChanged;
            //SpeedSlider.ValueChanged += SpeedSlider_ValueChanged;

            StartAnimation();
        }

        private void ScaleYSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Update ScaleY on both vectors
            VectorScale1.ScaleY = e.NewValue;
            VectorScale2.ScaleY = e.NewValue;
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Update speed by restarting the animation
            StartAnimation();
        }

        private void StartAnimation()
        {
            // Ensure layout is ready
            AnimationBox.UpdateLayout();
            MovingVectorContainer.UpdateLayout();

            double containerWidth = MovingVectorContainer.ActualWidth;

            if (containerWidth <= 0)
            {
                MovingVectorContainer.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                containerWidth = MovingVectorContainer.DesiredSize.Width;
            }

            // Get duration from slider (default fallback = 3 sec)
            //double durationSeconds = SpeedSlider?.Value ?? 3.0;
            double durationSeconds = 3;

            double from = 20;
            double to = -containerWidth / 2 + 20;

            VectorTransform.Y = 24;

            _animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                RepeatBehavior = RepeatBehavior.Forever
            };

            VectorTransform.BeginAnimation(TranslateTransform.XProperty, _animation);
        }
    }
}