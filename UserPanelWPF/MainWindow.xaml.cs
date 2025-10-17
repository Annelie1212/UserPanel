using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using UserPanelWPF.Models;
using UserPanelWPF.Services;
using UserPanelWPF.ViewModels;

namespace UserPanelWPF
{



    public partial class MainWindow : Window
    {
        private string _logFilePath = "";
        private DoubleAnimation _animation;
        private ObservableCollection<string>? _eventLog;

        private double _vibrationSpeedValue = 0;
        private double _linePositionSlider_Value = 0;
        private DispatcherTimer _timer;

        public List<DeviceLog> DeviceLogs { get; set; } = new List<DeviceLog>();
        public List<double> SliderValues { get; set; } = new List<double>();
        double _previousSliderValues = -1;
        bool _hasSliderChanged = false;

        bool _skipSliderAction = true;

        MainWindowViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;    
            InitializeFeatures();
            StartWorker();

            _vm = new MainWindowViewModel();

        }

        //This methods takes everything from the ViewModel and updates the view accordingly.
        public void UpdateView()
        {
            Btn_OnOff.Content = _vm.ButtonVM.OnOffContent;
            Btn_OnOff.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_vm.ButtonVM.OnOffForeground));
            Btn_OnOff.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_vm.ButtonVM.OnOffBorderBrush));
            
            BellImage.Source = ChangeColor(BellImage.Source, _vm.ButtonVM.TrigContent);
            Btn_Trigged.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_vm.ButtonVM.TrigForeground));
            Btn_Trigged.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_vm.ButtonVM.TrigBorderBrush));
        }

        public void InitializeFeatures()
        {
            _eventLog = [];
            LB_EventLog.ItemsSource = _eventLog;
        }

        private void StartWorker()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.5);
            _timer.Tick += (s, e) => DoWork();
            _timer.Start();
        }

        public async void PostSliderValue()
        {
            //Check length of SliderValues list
            if ( (SliderValues.Count == _previousSliderValues) && (_hasSliderChanged == true) )
            {

                Console.WriteLine("Slider has not changed for 1 second -> posting new value to API.");

                var sliderValue = SliderValues.Last();

                string logMessage = await DeviceActions.SetThresholdLevel(sliderValue);
                LogMessage(logMessage);
                
                SyncUserPanelWPF();

                //double testvalueSpeed = _vibrationSpeedValue;

                SliderValues.Clear();
                _previousSliderValues = -1;
                _hasSliderChanged = false;
            }
            else
            {
                _previousSliderValues = SliderValues.Count;
                Console.WriteLine("Slider has changed, waiting for it to stabilize...");
            }
        }

        //This work is done once every second.
        private void DoWork()
        {
            //0 till 100
            //HorizontalLine.Y1 = 100;

            PostSliderValue();
            // Runs every second
            //Console.WriteLine($"Tick doing work at {DateTime.Now}");

            SyncUserPanelWPF();
        }
        public void UpdateUserPanelView(DeviceLog dl)
        {
            //Animation update utifrån slider value + threshold + vibration level

            //Vibration level
            ScaleY_VibrationLevelChanged(VibrationDetector.VibrationLevel);
            _vibrationSpeedValue = (double)VibrationDetector.VibrationLevel;
            VibrationSpeed_ValueChanged();

            //Armknapp
            Btn_OnOff.Content = DeviceActions.GetArmedState() ? "STOP" : "START";
            Btn_OnOff.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(DeviceActions.GetArmedState() ? "#F40B0B" : "#00FF00")); // Red or Green
            Btn_OnOff.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(DeviceActions.GetArmedState() ? "#F40B0B" : "#00FF00")); // Red or Green
            
            //Triggknapp
            BellImage.Source = ChangeColor(BellImage.Source, DeviceActions.GetTriggedState() ? "#F40B0B" : "#E6D825");
            Btn_Trigged.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(DeviceActions.GetTriggedState() ? "#F40B0B" : "#E6D825")); // Yellow


            


            //THRESHOLD 0-10
            //Slider Röd linje och threshold.
            //Tex Threshold = 7;
            //AnimationBox.ActualHeight = 200;
            //Röd linje = 140 (7/10 * 200) pixlar från toppen.
            Slider_Threshold.Value = VibrationDetector.VibrationLevelThreshold;
            _linePositionSlider_Value = (double) (-10*VibrationDetector.VibrationLevelThreshold) + 100;
            HorizontalLine.Y1 = _linePositionSlider_Value;
            

            //Loggning
            LogMessage(dl.LogMessage);
                       
        }
        public async void SyncUserPanelWPF()
        {
            //1. Hämtar status från modellen via API:et
            VDFetchStatusResponse statusResponse = await VDClientService.FetchStatusVDAsync();
            //2. Updaterar vår lokala model VibrationDetector
            VibrationDetector.Update(statusResponse);

            DeviceLog dl = new DeviceLog();
            dl.PopulateDeviceLog(statusResponse);
            //3 . Uppdaterar lokal vyn och ui
            UpdateUserPanelView(dl);
        }

        public async void Btn_Armed_Click(object sender, RoutedEventArgs e)
        {

            string logMessage = await DeviceActions.ToggleArmedState();
            SyncUserPanelWPF();
            _vm.ButtonVM.UpdateButtonViewModel();
            UpdateView();

        }
        public async void Btn_TriggedState_Click(object sender, RoutedEventArgs e)
        {

            string logMessage = await DeviceActions.ToggleTriggedState();
            SyncUserPanelWPF();
            _vm.ButtonVM.UpdateButtonViewModel();
            UpdateView();
        }
        public ImageSource ChangeColor(ImageSource source, string color)
        {
            // 1. Get the original DrawingImage from the Image
            var originalDrawingImage = (DrawingImage)BellImage.Source;

            // 2. Clone it so it is modifiable (unfrozen)
            var modifiableDrawingImage = originalDrawingImage.Clone();

            // 3. Access the GeometryDrawing inside the cloned DrawingImage
            var geometryDrawing = (GeometryDrawing)modifiableDrawingImage.Drawing;

            // 4. Set the new color using ColorConverter
            geometryDrawing.Brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));

            // 5. Assign the modified DrawingImage back to the Image
            return modifiableDrawingImage;

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
            //SpeedSlider.ValueChanged += VibrationSpeed_ValueChanged;

            this.Dispatcher.InvokeAsync(() =>
            {
                // Ensure layout is measured so ActualHeight is valid
                AnimationBox.UpdateLayout();
                if (AnimationBox.ActualHeight > 0)
                    _linePositionSlider_Value = AnimationBox.ActualHeight / 2;
            });

            StartAnimation();
        }

        private void ScaleY_VibrationLevelChanged(int newValueInt)
        {   
            double newValue = (double)newValueInt/5*0.85;

           // Update ScaleY on both vectors
            VectorScale1.ScaleY = newValue;
            VectorScale2.ScaleY = newValue;
        }

        private void VibrationSpeed_ValueChanged()
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


            double convertedSpeedValue = -0.188*_vibrationSpeedValue + 2.0;
            // Get duration from slider (default fallback = 3 sec)
            double durationSeconds = convertedSpeedValue;
            //
            //double durationSeconds = 2;

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

        private void ThresholdChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (_skipSliderAction)
            {
                _skipSliderAction = false;
                return; // Skip first event
            }

            var sliderValue = Slider_Threshold.Value;
            SliderValues.Add(sliderValue);
            _hasSliderChanged = true;

        }

        //private void ThresholdChanged(object sender, RoutedPropertyChangedEventArgs<T> e)
        //{

        //}
    }
}