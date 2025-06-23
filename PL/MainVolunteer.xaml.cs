using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainVolunteer.xaml
    /// </summary>
    public partial class MainVolunteer : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.Volunteer? CurrentVolunteer
        {
            get => (BO.Volunteer?)GetValue(CurrentVolunteerProperty);
            set => SetValue(CurrentVolunteerProperty, value);
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(MainVolunteer), new PropertyMetadata(null));

        private BO.Call? _currentCall;
        public BO.Call? CurrentCall
        {
            get => _currentCall;
            set
            {
                _currentCall = value;
                OnPropertyChanged(nameof(CurrentCall));
            }
        }





        public MainVolunteer(int id)
        {

            InitializeComponent();

            try
            {
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                if (CurrentVolunteer.CallInProgress != null && CurrentVolunteer.CallInProgress.CallStatus != BO.Enums.CallStatus.Expired)
                {
                    CurrentCall = s_bl.Call.GetCallDetails(CurrentVolunteer.CallInProgress.CallId);
                    ShowMap((double)CurrentVolunteer.Latitude, (double)CurrentVolunteer.Longitude,
                            (double?)CurrentCall.Latitude, (double?)CurrentCall.Longitude);
                }
                else
                {
                    CurrentCall = null;
                    ShowMap((double)CurrentVolunteer.Latitude, (double)CurrentVolunteer.Longitude);
                }

                //CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                //ShowMap((double)CurrentVolunteer.Latitude, (double)CurrentVolunteer.Longitude);
                ////ShowMap(31.788278, 35.193187);
                //if (CurrentVolunteer.CallInProgress != null &&
                //     CurrentVolunteer.CallInProgress.CallStatus != BO.Enums.CallStatus.Expired)
                //{
                //    CurrentCall = CurrentVolunteer.CallInProgress;
                //}
                //else
                //{
                //    CurrentCall = null;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        //private void ShowMap()
        //{
        //    webView.Source = new Uri("https://www.openstreetmap.org/export/embed.html?bbox=34.7924,32.0879,34.7935,32.0886&layer=mapnik");

        //}



        private void ShowMap(double volunteerLat, double volunteerLon, double? callLat = null, double? callLon = null)
        {
            string mapHtml = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8' />
            <title>Map</title>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <link rel='stylesheet' href='https://unpkg.com/leaflet@1.7.1/dist/leaflet.css'/>
            <script src='https://unpkg.com/leaflet@1.7.1/dist/leaflet.js'></script>
        </head>
        <body>
            <div id='map' style='width: 100%; height: 100vh;'></div>
            <script>
                var map = L.map('map').setView([{volunteerLat}, {volunteerLon}], 15);
                L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                    maxZoom: 19,
                    attribution: '© OpenStreetMap'
                }}).addTo(map);

                var volunteerMarker = L.marker([{volunteerLat}, {volunteerLon}]).addTo(map)
                    .bindPopup('מיקום המתנדב').openPopup();
    ";

            if (callLat != null && callLon != null)
            {
                mapHtml += $@"
                var callMarker = L.marker([{callLat}, {callLon}]).addTo(map)
                    .bindPopup('מיקום הקריאה');
        ";
            }

            mapHtml += @"
            </script>
        </body>
        </html>";

            string htmlPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "map.html");
            System.IO.File.WriteAllText(htmlPath, mapHtml, Encoding.UTF8);
            webView.Source = new Uri(htmlPath);
        }

        //private void ShowMap(double latitude, double longitude)
        //{
        //    // גודל תצוגה סביב המיקום – ככל שקטן יותר, המפה יותר ממוקדת
        //    double delta = 0.001;

        //    double minLat = latitude - delta;
        //    double maxLat = latitude + delta;
        //    double minLon = longitude - delta;
        //    double maxLon = longitude + delta;

        //    // בניית הקישור עם התמקדות וסמן (marker)
        //    string url = $"https://www.openstreetmap.org/export/embed.html?bbox={minLon},{minLat},{maxLon},{maxLat}&layer=mapnik&marker={latitude},{longitude}";

        //    webView.Source = new Uri(url);
        //}

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer!.Id, CurrentVolunteer);
                MessageBox.Show("פרטי המתנדב עודכנו בהצלחה");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בעדכון: " + ex.Message);
            }
        }

        private void volunteerObserver()
        {
            int id = CurrentVolunteer!.Id;
            CurrentVolunteer = null;
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        }

        private void volunteerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, volunteerObserver);
            //צריך לעקוב פה אחרי שינויים ב assignments
            // כי בישות של DO.Volunteer אין שדה שמתייחס לזה אם יש קריאה בטיפולו או לא
            //אז מעקב אחרי המתנדב עצמו לא יעזור לי כדי לדעת אם הוא לקח קריאה לטיפול.
        }

        private void volunteerWindow_Closed(object sender, EventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, volunteerObserver);
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
           new CallHistory(CurrentVolunteer!.Id).ShowDialog();
        }


        private void btnChooseCall_Click(object sender, RoutedEventArgs e)
        {
            var chooseCallWindow = new ChooseCallWindow(CurrentVolunteer!.Id);
            //צריך לעקוב פה אחרי שינויים ב assignments
            // כי בישות של DO.Volunteer אין שדה שמתייחס לזה אם יש קריאה בטיפולו או לא
            //אז מעקב אחרי המתנדב עצמו לא יעזור לי כדי לדעת אם הוא לקח קריאה לטיפול.
            //chooseCallWindow.Closed += (s, args) => RefreshCallDetails();
            chooseCallWindow.ShowDialog();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        //צריך לעקוב פה אחרי שינויים ב assignments
        // כי בישות של DO.Volunteer אין שדה שמתייחס לזה אם יש קריאה בטיפולו או לא
        //אז מעקב אחרי המתנדב עצמו לא יעזור לי כדי לדעת אם הוא לקח קריאה לטיפול.
        //private void RefreshCallDetails()
        //{
        //    var id = CurrentVolunteer!.Id;
        //    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        //    CurrentCall = s_bl.Call.GetCallDetails(CurrentVolunteer.CallInProgress.CallId);
        //    OnPropertyChanged(nameof(CurrentVolunteer));
        //    OnPropertyChanged(nameof(CurrentCall));
        //}


        private void btnEndCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // אותה הבעיה של האסימנט אידי
                //s_bl.Call.MarkCallCompletion();
                //CurrentCall = null;
                MessageBox.Show("הטיפול בקריאה הסתיים.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בסיום טיפול: " + ex.Message);
            }
        }

        private void btnCancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // אותה הבעיה של האסימנט אידי
                //s_bl.Call.MarkCallCancellation();
                //CurrentCall = null;
                MessageBox.Show("הטיפול בקריאה בוטל.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בביטול טיפול: " + ex.Message);
            }
        }

    }
}

