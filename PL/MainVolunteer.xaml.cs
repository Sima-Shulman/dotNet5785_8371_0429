using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Threading;

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
                    ShowMap((double)CurrentVolunteer.Latitude!, (double)CurrentVolunteer.Longitude!,
                            (double?)CurrentCall.Latitude, (double?)CurrentCall.Longitude);
                }
                else
                {
                    CurrentCall = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        /// <summary>
        /// Displays a map showing the volunteer's location and the call's location.
        /// </summary>
        /// <param name="volunteerLat"></param>
        /// <param name="volunteerLon"></param>
        /// <param name="callLat"></param>
        /// <param name="callLon"></param>
        private void ShowMap(double volunteerLat, double volunteerLon, double? callLat, double? callLon)
        {
            if (callLat == null || callLon == null)
                return;

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
                                        var map = L.map('map');
                                        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                                            maxZoom: 19,
                                            attribution: '© OpenStreetMap contributors'
                                        }}).addTo(map);

                                        var volunteerLatLng = [{volunteerLat}, {volunteerLon}];
                                        var callLatLng = [{callLat}, {callLon}];

                                        var volunteerMarker = L.marker(volunteerLatLng).addTo(map)
                                            .bindPopup('מיקום המתנדב').openPopup();

                                        var callMarker = L.marker(callLatLng).addTo(map)
                                            .bindPopup('מיקום הקריאה');

                                        // פוקוס אוטומטי על שני הסמנים
                                        var bounds = L.latLngBounds([volunteerLatLng, callLatLng]);
                                        map.fitBounds(bounds, {{ padding: [50, 50] }});
                                    </script>
                                </body>
                                </html>";

            string tempDir = System.IO.Path.GetTempPath();
            foreach (var file in Directory.GetFiles(tempDir, "map_*.html"))
            {
                try { File.Delete(file); } catch { }
            }

            string uniqueFileName = $"map_{Guid.NewGuid()}.html";
            string htmlPath = System.IO.Path.Combine(tempDir, uniqueFileName);
            File.WriteAllText(htmlPath, mapHtml, Encoding.UTF8);

            webView.Source = new Uri(htmlPath);
        }



        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for the specified property name.
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        /// <summary>
        /// Event handler for the Update button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentVolunteer == null)
                {
                    MessageBox.Show("Cannot update details for an empty volunteer.");
                    return;
                }

                if (!ValidateVolunteerDetails(CurrentVolunteer))
                {
                    return;
                }

                s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id, CurrentVolunteer);
                MessageBox.Show("Volunteer details updated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating details: " + ex.Message);
            }
        }
        private bool ValidateVolunteerDetails(BO.Volunteer volunteer)
        {
            if (string.IsNullOrWhiteSpace(volunteer.FullName))
            {
                MessageBox.Show("Volunteer name cannot be empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(volunteer.CellphoneNumber) || !System.Text.RegularExpressions.Regex.IsMatch(volunteer.CellphoneNumber, @"^\d{10}$"))
            {
                MessageBox.Show("Phone number must be exactly 10 digits.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(volunteer.Email) || !System.Text.RegularExpressions.Regex.IsMatch(volunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email address format.");
                return false;
            }

            if (volunteer.Latitude == null || volunteer.Longitude == null)
            {
                MessageBox.Show("Volunteer location must be defined.");
                return false;
            }

            if (volunteer.MaxDistance == null || volunteer.MaxDistance <= 0)
            {
                MessageBox.Show("Maximum distance must be a positive number.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Observer method that updates the volunteer details when changes occur.
        /// </summary>
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void volunteerObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    int id = CurrentVolunteer!.Id;
                    CurrentVolunteer = null;
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                    if (CurrentVolunteer.CallInProgress != null)
                    {
                        CurrentCall = s_bl.Call.GetCallDetails(CurrentVolunteer.CallInProgress.CallId);
                    }
                    else
                    {
                        CurrentCall = null; // Handle the case where CallInProgress is null  
                    }
                    OnPropertyChanged(nameof(CurrentVolunteer));
                    OnPropertyChanged(nameof(CurrentCall));
                    if (CurrentVolunteer.CallInProgress != null && CurrentVolunteer.CallInProgress.CallStatus != BO.Enums.CallStatus.Expired)
                    {
                        CurrentCall = s_bl.Call.GetCallDetails(CurrentVolunteer.CallInProgress.CallId);
                        ShowMap((double)CurrentVolunteer.Latitude!, (double)CurrentVolunteer.Longitude!,
                                (double?)CurrentCall.Latitude, (double?)CurrentCall.Longitude);
                    }
                });
        }

        /// <summary>
        /// Event handler for the Loaded event of the volunteer window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void volunteerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, volunteerObserver);
            if (CurrentVolunteer.CallInProgress != null)
                s_bl.Call.AddObserver(CurrentCall!.Id, volunteerObserver);

            //צריך לעקוב פה אחרי שינויים ב assignments
            // כי בישות של DO.Volunteer אין שדה שמתייחס לזה אם יש קריאה בטיפולו או לא
            //אז מעקב אחרי המתנדב עצמו לא יעזור לי כדי לדעת אם הוא לקח קריאה לטיפול.
        }


        /// <summary>
        /// Event handler for the Closed event of the volunteer window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void volunteerWindow_Closed(object sender, EventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, volunteerObserver);
            if (CurrentVolunteer.CallInProgress != null)
                s_bl.Call.RemoveObserver(CurrentCall!.Id, volunteerObserver);
        }


        /// <summary>
        ///  Event handler for the History button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            new CallHistory(CurrentVolunteer!.Id).ShowDialog();
        }

        /// <summary>
        /// Event handler for the Choose Call button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChooseCall_Click(object sender, RoutedEventArgs e)
        {
            var chooseCallWindow = new ChooseCallWindow(CurrentVolunteer!.Id);
            //צריך לעקוב פה אחרי שינויים ב assignments
            // כי בישות של DO.Volunteer אין שדה שמתייחס לזה אם יש קריאה בטיפולו או לא
            //אז מעקב אחרי המתנדב עצמו לא יעזור לי כדי לדעת אם הוא לקח קריאה לטיפול.
            //chooseCallWindow.Closed += (s, args) => RefreshCallDetails();
            chooseCallWindow.ShowDialog();
        }

        /// <summary>
        /// Event handler for the Exit button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        //צריך לעקוב פה אחרי שינויים ב assignments
        // כי בישות של DO.Volunteer אין שדה שמתייחס לזה אם יש קריאה בטיפולו או לא
        //אז מעקב אחרי המתנדב עצמו לא יעזור לי כדי לדעת אם הוא לקח קריאה לטיפול.
        /// <summary>
        /// Refreshes the call details for the current volunteer.
        /// </summary>
        //private void RefreshCallDetails()
        //{
        //    var id = CurrentVolunteer!.Id;
        //    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        //    CurrentCall = s_bl.Call.GetCallDetails(CurrentVolunteer.CallInProgress!.CallId);
        //    OnPropertyChanged(nameof(CurrentVolunteer));
        //    OnPropertyChanged(nameof(CurrentCall));
        //    if (CurrentVolunteer.CallInProgress != null && CurrentVolunteer.CallInProgress.CallStatus != BO.Enums.CallStatus.Expired)
        //    {
        //        CurrentCall = s_bl.Call.GetCallDetails(CurrentVolunteer.CallInProgress.CallId);
        //        ShowMap((double)CurrentVolunteer.Latitude!, (double)CurrentVolunteer.Longitude!,
        //                (double?)CurrentCall.Latitude, (double?)CurrentCall.Longitude);
        //    }
        //}

        /// <summary>
        /// Event handler for the End Call button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEndCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.MarkCallCompletion(CurrentVolunteer!.Id, CurrentVolunteer.CallInProgress!.AssignmentId);
                CurrentCall = null;

                MessageBox.Show("Treatment has been completed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error completing treatment " + ex.Message);
            }
        }


        /// <summary>
        /// Event handler for the Cancel Call button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.MarkCallCancellation(CurrentVolunteer!.Id, CurrentVolunteer.CallInProgress!.AssignmentId);
                CurrentCall = null;
                MessageBox.Show("Treatment was cancelled!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error canceling treatment " + ex.Message);
            }
        }

    }
}

