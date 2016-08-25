using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PomodoroTimer.Configuration;
using PomodoroTimer.PomodoroState;

namespace PomodoroTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window//TODO: Use MVVM instead
    {
        private readonly PomodoroController m_Controller;

        private bool IsCountDownTextBoxVisible
        {
            get
            {
                PomodoroBaseState rulingState = (m_Controller?.State as Paused)?.PreviousState ?? m_Controller?.State;

                bool isVisible = rulingState is LazerFocused ||
                                 rulingState is ProgressChecking ||
                                 rulingState is Break;

                return isVisible;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            m_Controller                = new PomodoroController();
            m_Controller.StateChanged   += Controller_StateChanged;
            m_Controller.MinuteElapsed  += Controller_MinuteElapsed;
            EnableControls();
            SetTimerValues();
        }

        private void SetTimerValues()
        {
            LazerFocusedTime.Value      = ApplicationSettings.Instance.LazerFocusedTime;
            ProgressCheckingTime.Value  = ApplicationSettings.Instance.ProgressCheckingTime;
            BreakTime.Value             = ApplicationSettings.Instance.BreakTime;
        }

        private void EnableControls()
        {
            bool isStartButtonEnabled = (FocusOnTextBox.Text.Length > 0);

            StartTimer.IsEnabled = isStartButtonEnabled;

            SetStartButtonContent();

            RestoreDefaultsButton.IsEnabled = (m_Controller.State is Idle);

            SetLabelFontWeights();
            SetTimesEnabling();
            ShowCountDown();
        }

        private void SetStartButtonContent()
        {
            string startButtonContent = string.Empty;

            if (m_Controller.State is Idle)
            {
                startButtonContent = "Start";
            }

            if (string.IsNullOrEmpty(startButtonContent) && m_Controller.State is Paused)
            {
                startButtonContent = "Continue";
            }

            if (string.IsNullOrEmpty(startButtonContent))
            {
                startButtonContent = "Pause";
            }
            StartTimer.Content = startButtonContent;
        }

        private void SetBackgroundColor()
        {
            if (m_Controller.State is LazerFocused)
            {
                Background  = (Brush)FindResource("LazerFocusedBrush");
                Activate();
                return;
            }

            if (m_Controller.State is ProgressChecking)
            {
                Background  = (Brush)FindResource("ProgressCheckingBrush");
                Activate();
                return;
            }

            Background      = (Brush)FindResource("DefaultWindowBrush");
        }

        private void SetLabelFontWeights()
        {
            Label boldLabel = null;
            PomodoroBaseState rulingState = (m_Controller?.State as Paused)?.PreviousState ?? m_Controller?.State;

            if (rulingState is Idle)
            {
                boldLabel = FocusOnLabel;
            }

            if (rulingState is LazerFocused)
            {
                boldLabel = LazerFocusedTimeLabel;
            }

            if (rulingState is ProgressChecking)
            {
                boldLabel = ProgressCheckingLabel;
            }

            if (rulingState is Break)
            {
                boldLabel = BreakTimeLabel;
            }

            FocusOnLabel.FontWeight             = FontWeights.Normal;
            LazerFocusedTimeLabel.FontWeight    = FontWeights.Normal;
            ProgressCheckingLabel.FontWeight    = FontWeights.Normal;
            BreakTimeLabel.FontWeight           = FontWeights.Normal;

            if (boldLabel != null)
            {
                boldLabel.FontWeight = FontWeights.Bold;
            }
        }

        private void SetTimesEnabling()
        {
            bool isEnabled                  = m_Controller.State is Idle;

            LazerFocusedTime.IsEnabled      = isEnabled;
            ProgressCheckingTime.IsEnabled  = isEnabled;
            BreakTime.IsEnabled             = isEnabled;
        }

        private void ShowCountDown()
        {
            Visibility countDownVisibility = IsCountDownTextBoxVisible ? Visibility.Visible : Visibility.Hidden;

            CountDownTextBox.Visibility = countDownVisibility;
        }

        private void Controller_MinuteElapsed(object sender, EventArgs e)
        {
            HandleMinuteElapsed();
        }

        private void HandleMinuteElapsed()
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(HandleMinuteElapsed);
                return;
            }

            UpdateMinuteCountDown();
        }

        private void Controller_StateChanged(object sender, EventArgs e)
        {
            HandleStateChanged();
        }

        private void HandleStateChanged()
        {
            if (!CheckAccess())
            {
                Dispatcher.Invoke(HandleStateChanged);
                return;
            }


            MakeSoundAsync();
            EnableControls();
            SetBackgroundColor();
            UpdateMinuteCountDown();
        }

        private void MakeSoundAsync()
        {
            Thread thread = new Thread(MakeSound);
            thread.Start();
        }

        private void MakeSound()
        {
            if (m_Controller.State is ProgressChecking)
            {
                Console.Beep(800, 800);
                Console.Beep(1200, 400);
            }

            if (m_Controller.State is Break)
            {
                Console.Beep(1200, 500);
            }

            if (m_Controller.State is Idle)
            {
                Console.Beep(800, 1000);
            }
        }

        private void UpdateMinuteCountDown()
        {
            CountDownTextBox.Text = m_Controller.State.MinuteCountDown.ToString();
        }

        private void StartTimer_Click(object sender, RoutedEventArgs e)
        {
            if (m_Controller.State is Idle)
            {
                ApplicationSettings.Instance.LazerFocusedTime       = LazerFocusedTime.Value;
                ApplicationSettings.Instance.ProgressCheckingTime   = ProgressCheckingTime.Value;
                ApplicationSettings.Instance.BreakTime              = BreakTime.Value;
            }

            if (m_Controller.State is Idle || m_Controller.State is Paused)
            {
                m_Controller.Start(FocusOnTextBox.Text);
            }
            else
            {
                m_Controller.Pause();
            }
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            m_Controller.Stop();
            FocusOnTextBox.Focus();
        }

        private void FocusOnTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableControls();
        }

        private void SaveAsDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationSettings.Instance.LazerFocusedTime       = LazerFocusedTime.Value;
            ApplicationSettings.Instance.ProgressCheckingTime   = ProgressCheckingTime.Value;
            ApplicationSettings.Instance.BreakTime              = BreakTime.Value;

            ApplicationSettings.Instance.Save();
            MessageBox.Show("Default times saved!");//NLS
        }

        private void RestoreDefaultsButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationSettings.Instance.UseDefault();
            SetTimerValues();
        }
    }
}
