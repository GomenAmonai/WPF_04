using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MediaPlayerApp
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            MediaPlayer.Volume = 0.5;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += UpdatePositionSlider;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MediaPlayer.Source = new Uri(FilePathTextBox.Text, UriKind.Absolute);
                MediaPlayer.LoadedBehavior = MediaState.Manual;
                MediaPlayer.UnloadedBehavior = MediaState.Manual;
                MediaPlayer.Stop();
                PlayPauseButton.Content = "Play";
                PositionSlider.Value = 0;
                StatusBarText.Text = "File loaded successfully.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusBarText.Text = "Error loading file.";
            }
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.Source == null)
            {
                MessageBox.Show("Please load a video file first.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MediaPlayer.CanPause && MediaPlayer.Position != TimeSpan.Zero)
            {
                MediaPlayer.Pause();
                PlayPauseButton.Content = "Play";
                StatusBarText.Text = "Paused.";
                _timer.Stop();
            }
            else
            {
                MediaPlayer.Play();
                PlayPauseButton.Content = "Pause";
                StatusBarText.Text = "Playing...";
                _timer.Start();
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaPlayer.Volume = VolumeSlider.Value / 100;
            VolumeStatus.Text = $"Volume: {VolumeSlider.Value}%";
        }

        private void UpdatePositionSlider(object sender, EventArgs e)
        {
            if (MediaPlayer.NaturalDuration.HasTimeSpan)
            {
                PositionSlider.Maximum = MediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                PositionSlider.Value = MediaPlayer.Position.TotalSeconds;
            }
        }

        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MediaPlayer.NaturalDuration.HasTimeSpan && PositionSlider.IsMouseOver)
            {
                MediaPlayer.Position = TimeSpan.FromSeconds(PositionSlider.Value);
            }
        }

        private void MediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("Failed to load video. Please check the file path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            StatusBarText.Text = "Media load failed.";
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            LoadButton_Click(sender, e);
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Media Player\nVersion 1.0\nBy MediaPlayerApp", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
