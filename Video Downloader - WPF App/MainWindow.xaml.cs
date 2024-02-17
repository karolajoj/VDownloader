using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Common;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Effects;

namespace VDownloader {

    public class List_Element {
        public string URL { get; set; }
        public string Title { get; set; }
        public string Progress { get; set; }
        public string Status { get; set; }
        public BitmapImage Thumbnail { get; set; }

        public List_Element() {
            Thumbnail = new BitmapImage(new Uri("pack://application:,,,/Resources/NoThumbnail.png"));
        }
    }


    public partial class MainWindow : Window {

        ObservableCollection<List_Element> downloadingItem = new ObservableCollection<List_Element>();
        ObservableCollection<List_Element> downloadedItem = new ObservableCollection<List_Element>();
        ObservableCollection<List_Element> convertingItem = new ObservableCollection<List_Element>();
        ObservableCollection<List_Element> convertedItem = new ObservableCollection<List_Element>();


        public MainWindow() {
            InitializeComponent();

            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            Downloading_Tab.IsChecked = true;

            downloadingItem.Add(new List_Element { URL = "https://www.youtube.com/watch?v=_U-cncVYIxI", Status  = "" });
            downloadingItem.Add(new List_Element { Title = "Mój Tytul", Status  = "Nie można pobrać filmu" });
            downloadingItem.Add(new List_Element { Title = "Mój Tytul", Status  = "Nie można pobrać filmu" });
            downloadingItem.Add(new List_Element { Title = "Mój Tytul", Status  = "Nie można pobrać filmu" });
            downloadingItem.Add(new List_Element { Title = "Mój Tytul", Status  = "Nie można pobrać filmu" });
            downloadingItem.Add(new List_Element { Title = "Mój Tytul", Status  = "Nie można pobrać filmu" });
            downloadingItem.Add(new List_Element { Title = "Mój Tytul", Status  = "Nie można pobrać filmu" });
            downloadingItem.Add(new List_Element  { Title = "Mój Tytul", Status  = "Nie można pobrać filmu" });

            downloadedItem.Add(new List_Element { Title = "Pobrane List_Element", Status  = "" });
            downloadedItem.Add(new List_Element { Title = "Pobrane List_Element", Status  = "Nie można pobrać filmu" });
            downloadedItem.Add(new List_Element { Title = "Pobrane List_Element", Status  = "Nie można pobrać filmu" });

            convertingItem.Add(new List_Element { Title = "List_Element do konwersji", Status  = "" });

            convertedItem.Add(new List_Element { Title = "Przekonwertowane List_Element", Status  = "" });
            convertedItem.Add(new List_Element { Title = "Przekonwertowane List_Element", Status  = "Nie można pobrać filmu" });
            convertedItem.Add(new List_Element { Title = "Przekonwertowane List_Element", Status  = "Nie można pobrać filmu" });
            convertedItem.Add(new List_Element { Title = "Przekonwertowane List_Element", Status  = "Nie można pobrać filmu" });
        }

        #region Tab Control
        private void Tab_Checked(object sender, RoutedEventArgs e) {
            switch (((RadioButton)sender).Name) {
                case "Downloading_Tab":
                    Display_Downloading_Tab_Content();
                    break;
                case "Downloaded_Tab":
                    Display_Downloaded_Tab_Content();
                    break;
                case "Converting_Tab":
                    Display_Converting_Tab_Content();
                    break;
                case "Converted_Tab":
                    Display_Converted_Tab_Content();
                    break;
            }
        }

        private void Display_Downloading_Tab_Content() {
            VideoList_DataGrid.ItemsSource = null;
            VideoList_DataGrid.Items.Clear();

            VideoList_DataGrid.ItemsSource = downloadingItem;

            VideoList_DataGrid.Items.Refresh();
        }

        private void Display_Downloaded_Tab_Content() {
            VideoList_DataGrid.ItemsSource = null;
            VideoList_DataGrid.Items.Clear();

            VideoList_DataGrid.ItemsSource = downloadedItem;

            VideoList_DataGrid.Items.Refresh();
        }

        private void Display_Converting_Tab_Content() {
            VideoList_DataGrid.ItemsSource = null;
            VideoList_DataGrid.Items.Clear();

            VideoList_DataGrid.ItemsSource = convertingItem;

            VideoList_DataGrid.Items.Refresh();
        }

        private void Display_Converted_Tab_Content() {
            VideoList_DataGrid.ItemsSource = null;
            VideoList_DataGrid.Items.Clear();

            VideoList_DataGrid.ItemsSource = convertedItem;

            VideoList_DataGrid.Items.Refresh();
        }
        #endregion

        #region Window Controll

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                DragMove();
            }
        }

        private bool IsMaximized = false;

        private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2){
                Top_Maximize_Button_Click(sender, new RoutedEventArgs(Button.ClickEvent, sender));
            }
        }

        private void Top_Settings_Button_Click(object sender, RoutedEventArgs e) {

        }

        private void Top_Menu_Button_Click(object sender, RoutedEventArgs e) {
            if (Top_Menu_Button.ContextMenu != null) {
                Top_Menu_Button.ContextMenu.PlacementTarget = Top_Menu_Button;
                Top_Menu_Button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                Top_Menu_Button.ContextMenu.IsOpen = true;
            }
        }

        private void Top_Minimize_Button_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void Top_Maximize_Button_Click(object sender, RoutedEventArgs e) {
            if (IsMaximized) {
                WindowState = WindowState.Normal;
                Width = 1070;
                Height = 680;

                IsMaximized = false;
            }
            else {
                WindowState = WindowState.Maximized;


                IsMaximized = true;
            }
        }

        private void Top_Close_Button_Click(object sender, RoutedEventArgs e) {
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

        #region Video Downloading
        private async void Paste_URL_Button_Click(object sender, RoutedEventArgs e) {
            string clipboardText = Clipboard.GetText();

            if (IsYouTubeLink(clipboardText)) {

                string URL = clipboardText;

                List_Element yt_video = new List_Element { URL = URL, Title = URL};

                if (IsYouTubePlaylist(yt_video.URL)) await DownloadPlaylist(yt_video);
                else DownloadVideo(yt_video);
            }
        }

        private void More_Options_Button_Click(object sender, RoutedEventArgs e) {
            if (More_Options_Button.ContextMenu != null) {
                More_Options_Button.ContextMenu.PlacementTarget = More_Options_Button;
                More_Options_Button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                More_Options_Button.ContextMenu.IsOpen = true;
            }
        }

        private void Download_Playlist_Button_Click(object sender, RoutedEventArgs e) {

        }

        private void Download_MP3_Button_Click(object sender, RoutedEventArgs e) {

        }

        private void Multiple_URLs_Button_Click(object sender, RoutedEventArgs e) {

        }

        private void Resume_All_Button_Click(object sender, RoutedEventArgs e) {

        }

        private void Pause_All_Button_Click(object sender, RoutedEventArgs e) {
            foreach (var item in downloadingItem) {
                item.Status = "Paused";
            }
            VideoList_DataGrid.Items.Refresh();
        }

        private void Delete_All_Button_Click(object sender, RoutedEventArgs e) {
            
            // Najpierw spytać o potwierdzenie

            if (VideoList_DataGrid.ItemsSource == downloadingItem) {
                // Czyszczenie elementów aktualnie wyświetlanych dla zakładki Downloading_Tab
                downloadingItem.Clear();
            }
            else if (VideoList_DataGrid.ItemsSource == downloadedItem) {
                // Czyszczenie elementów aktualnie wyświetlanych dla zakładki Downloaded_Tab
                downloadedItem.Clear();
            }
            else if (VideoList_DataGrid.ItemsSource == convertingItem) {
                // Czyszczenie elementów aktualnie wyświetlanych dla zakładki Converting_Tab
                convertingItem.Clear();
            }
            else if (VideoList_DataGrid.ItemsSource == convertedItem) {
                // Czyszczenie elementów aktualnie wyświetlanych dla zakładki Converted_Tab
                convertedItem.Clear();
            }
        }

        #endregion

        #region Helper Functions

        async Task DownloadPlaylist(List_Element video) {
            try {
                YoutubeClient youtubeClient = new YoutubeClient();
                var videos = await youtubeClient.Playlists.GetVideosAsync(video.URL);

                //WindowSettings settingsForm = new WindowSettings();
                //string saveDirectory = settingsForm.GetSaveDirectory();

                foreach (var file in videos) {
                    List_Element playlistVideo = new List_Element {
                        URL = file.Id,
                        Title = file.Title,
                        Status = "Paused"
                    };

                    // Tworzymy obrazek
                    BitmapImage bitmap = new BitmapImage(new Uri(file.Thumbnails[0].Url));

                    downloadingItem.Add(playlistVideo);
                }
                VideoList_DataGrid.Items.Refresh();
            }
            catch (Exception ex) {
                video.Status = ex.Message;
            }
        }

        async void DownloadVideo(List_Element video) {
            try {

                List_Element file = new List_Element {
                    URL = video.URL,
                    Title = video.URL,
                    Status = "Analyzing..."
                };
                
                
                downloadingItem.Add(file);

                VideoList_DataGrid.Items.Refresh();

                // Rozpoczynamy pobieranie danych w tle
                YoutubeClient youtubeClient = new YoutubeClient();
                var videoInfo = await youtubeClient.Videos.GetAsync(video.URL);

                // Po pobraniu danych aktualizujemy miniaturę i tytuł wideo
                file.Title = videoInfo.Title;
                file.Thumbnail = new BitmapImage(new Uri(videoInfo.Thumbnails[0].Url));


                // Odświeżamy widok DataGrid, aby uwzględnić zaktualizowany element
                VideoList_DataGrid.Items.Refresh();
            }
            catch (Exception ex) {
                video.Status = ex.Message;
            }
        }

        static async Task DownloadVideoAsync(string videoUrl, Format format, string saveDirectory, ProgressBar downloadProgressBar) {
            try {
                var youtubeClient = new YoutubeClient();

                if (string.IsNullOrWhiteSpace(videoUrl)) {
                    MessageBox.Show("Please enter a valid YouTube video URL.");
                    return;
                }

                var video = await youtubeClient.Videos.GetAsync(videoUrl);
                var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(videoUrl);

                IStreamInfo streamInfo = null;

                if (format == Format.MP3) {
                    streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                }
                else if (format == Format.MP4) {
                    streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
                }

                if (streamInfo == null) {
                    MessageBox.Show("No suitable stream found for the selected format.");
                    return;
                }

                var sanitizedTitle = SanitizeFileName(video.Title);
                var extension = format == Format.MP3 ? "mp3" : "mp4";
                var outputFilePath = Path.Combine(saveDirectory, $"{sanitizedTitle}.{extension}");

                var progressHandler = new Progress<double>(progress => {
                    // Update the progress bar value
                    downloadProgressBar.Value = (int)(progress * 100); // Update the progress as a percentage
                });

                // Download the video stream to the specified directory
                await youtubeClient.Videos.Streams.DownloadAsync(streamInfo, outputFilePath, progressHandler);

                // Ensure the progress bar displays 100% upon completion
                downloadProgressBar.Value = 100;
                //MessageBox.Show($"Downloaded and saved as {outputFilePath}");
                downloadProgressBar.Value = 0;
            }
            catch (Exception ex) {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private static readonly char[] InvalidFileNameChars = { '<', '>', ':', '"', '/', '\\', '|', '?', '*' };

        private static string SanitizeFileName(string fileName) {
            foreach (var invalidChar in InvalidFileNameChars) {
                fileName = fileName.Replace(invalidChar.ToString(), string.Empty);
            }

            return fileName;
        }

        static bool IsYouTubeLink(string link) {
            // Check if the link contains the domain "youtube.com"
            if (link.Contains("youtube.com")) {
                // Check if the link includes a valid video ID
                if (link.Contains("watch?v=")) {
                    return true;
                }
            }

            return false;
        }

        static bool IsYouTubePlaylist(string link) {
            // Check if the link contains the domain ""
            if (link.Contains("list"))
                return true;
            return false;
        }

        private enum Format {
            MP3,
            MP4
        }




        #endregion

    }
}