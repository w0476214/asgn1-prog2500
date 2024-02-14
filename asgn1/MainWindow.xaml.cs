using Microsoft.Win32;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using TagLib;
using System.IO;

namespace asgn1
{
    public partial class MainWindow : Window
    {
        // MediaPlayer instance for playing audio
        private MediaPlayer mediaPlayer = new MediaPlayer();
        // Store the path of the currently loaded file
        private string currentFilePath;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Event handler for opening an MP3 file
        private void OpenMp3_Click(object sender, RoutedEventArgs e)
        {
            // Open file dialog to select an MP3 file
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "MP3 Files (*.mp3)|*.mp3"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                // Get the selected file path
                currentFilePath = openFileDialog.FileName;
                // Open the selected file with MediaPlayer
                mediaPlayer.Open(new Uri(currentFilePath));
                // Update UI to display the current track information
                UpdateNowPlaying();
            }
        }

        // Event handler for exiting the application
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Event handler for playing the loaded audio file
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
        }

        // Event handler for pausing the audio playback
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        // Event handler for stopping the audio playback
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        // Event handler for editing the ID3 tags of the loaded MP3 file
        private void EditTag_Click(object sender, RoutedEventArgs e)
        {
            if (currentFilePath != null)
            {
                // Create a TagLib.File instance for the loaded MP3 file
                var file = TagLib.File.Create(currentFilePath);
                // Populate UI elements with existing tag information
                titleTextBox.Text = file.Tag.Title;
                artistTextBox.Text = file.Tag.FirstPerformer;
                albumTextBox.Text = file.Tag.Album;
                yearTextBox.Text = file.Tag.Year.ToString();
                // Set text box foreground color to black
                titleTextBox.Foreground = Brushes.Black;
                artistTextBox.Foreground = Brushes.Black;
                albumTextBox.Foreground = Brushes.Black;
                yearTextBox.Foreground = Brushes.Black;
            }
        }

        // Event handler for saving the edited ID3 tags of the MP3 file
        private void SaveTag_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                // Close the MediaPlayer to release the file
                mediaPlayer.Close();

                try
                {
                    // Open the file in edit mode and save the changes
                    using (var mp3File = TagLib.File.Create(currentFilePath))
                    {
                        mp3File.Tag.Title = titleTextBox.Text;
                        mp3File.Tag.Performers = new string[] { artistTextBox.Text };
                        mp3File.Tag.Album = albumTextBox.Text;
                        if (uint.TryParse(yearTextBox.Text, out uint year))
                        {
                            mp3File.Tag.Year = year;
                        }
                        mp3File.Save();
                    }
                }
                catch (IOException ex)
                {
                    // Display error message if an exception occurs during file save
                    MessageBox.Show($"An error occurred while saving the file: {ex.Message}");
                    return;
                }

                // Reopen the file with MediaPlayer and update UI
                mediaPlayer.Open(new Uri(currentFilePath));
                UpdateNowPlaying();
            }
        }


        // Event handler for handling text box focus event (GotFocus)
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Cast sender as TextBox
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            if (textBox != null && textBox.Foreground == Brushes.Gray)
            {
                // Clear the text box when it gains focus
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black;
            }
        }

        // Event handler for handling text box focus event (LostFocus)
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Cast sender as TextBox
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    // If text box is empty, set placeholder text and color
                    textBox.Foreground = Brushes.Gray;
                    switch (textBox.Name)
                    {
                        case "titleTextBox":
                            textBox.Text = "Title";
                            break;
                        case "artistTextBox":
                            textBox.Text = "Artist";
                            break;
                        case "albumTextBox":
                            textBox.Text = "Album";
                            break;
                        case "yearTextBox":
                            textBox.Text = "Year";
                            break;
                    }
                }
                else
                {
                    // If text box has text, set foreground color to black
                    textBox.Foreground = Brushes.Black;
                }
            }
        }


        // Update UI elements with the currently playing track information
        private void UpdateNowPlaying()
        {
            if (currentFilePath != null)
            {
                // Create a TagLib.File instance for the loaded MP3 file
                var file = TagLib.File.Create(currentFilePath);
                // Display track information in UI elements
                nowPlayingTitle.Text = file.Tag.Title;
                nowPlayingArtist.Text = file.Tag.FirstPerformer;
                nowPlayingAlbum.Text = file.Tag.Album;
                // Update album art if available
                if (file.Tag.Pictures.Length > 0)
                {
                    var bin = (byte[])(file.Tag.Pictures[0].Data.Data);
                    albumArt.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(bin);
                }
            }
        }
    }
}