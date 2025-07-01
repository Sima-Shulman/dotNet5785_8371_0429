using BO;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using static BO.Enums;

namespace PL;

/// <summary>  
/// Interaction logic for ChooseCallWindow.xaml  
/// </summary>  
public partial class ChooseCallWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private readonly int _volunteerId;
    private BO.Volunteer Volunteer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChooseCallWindow"/> class.
    /// </summary>
    /// <param name="volunteerId"> The current volunteer's Id</param>
    public ChooseCallWindow(int volunteerId)
    {
        InitializeComponent();
        _volunteerId = volunteerId;

        CallList = s_bl.Call.GetOpenCallsForVolunteer(_volunteerId);
        Volunteer = s_bl.Volunteer.GetVolunteerDetails(_volunteerId);
        SetValue(MaxDistanceProperty, Volunteer.MaxDistance);
        SetValue(FullAddressProperty, Volunteer.FullAddress);

    }






    public BO.OpenCallInList? SelectedCall { get; set; }
    public IEnumerable<BO.OpenCallInList> CallList
    {
        get { return (IEnumerable<BO.OpenCallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<BO.OpenCallInList>), typeof(PL.ChooseCallWindow), new PropertyMetadata(null));



    public double? MaxDistance
    {
        get { return (double?)GetValue(MaxDistanceProperty); }
    }
    /// <summary>
    /// Using a DependencyProperty as the backing store for MaxDistance. This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty MaxDistanceProperty =
    DependencyProperty.Register(
        "MaxDistance",
        typeof(double?),
        typeof(ChooseCallWindow),
        new PropertyMetadata(null, OnMaxDistanceChanged));

    /// <summary>
    /// Handles the MaxDistance property change event.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnMaxDistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = d as ChooseCallWindow;
        if (window == null) return;

        var newValue = (double?)e.NewValue;
        if (newValue.HasValue && window.Volunteer.MaxDistance != newValue.Value)
        {
            window.Volunteer.MaxDistance = newValue.Value;
            s_bl.Volunteer.UpdateVolunteerDetails(window._volunteerId, window.Volunteer);
            MessageBox.Show($"Max distance updated to {newValue.Value} km.", "Update Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            window.CallList = s_bl.Call.GetOpenCallsForVolunteer(window._volunteerId);
            MessageBox.Show("CallList is updated!");
        }
    }


    public string FullAddress
    {
        get { return (string)GetValue(FullAddressProperty); }
    }
    public static readonly DependencyProperty FullAddressProperty =
    DependencyProperty.Register(
        "FullAddress",
        typeof(string),
        typeof(ChooseCallWindow),
        new PropertyMetadata(null, OnFullAddressChanged));

    /// <summary>
    /// Handles the FullAddress property change event.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnFullAddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = d as ChooseCallWindow;
        if (window == null) return;

        var newValue = (string)e.NewValue;
        window.Volunteer.FullAddress = newValue;
        s_bl.Volunteer.UpdateVolunteerDetails(window._volunteerId, window.Volunteer);
        MessageBox.Show($"Max distance updated to {newValue} km.", "Update Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        window.CallList = s_bl.Call.GetOpenCallsForVolunteer(window._volunteerId);
        MessageBox.Show("CallList is updated!");
    }

    /// <summary>
    /// Handles the selection change event of the call list.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CallList_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (SelectedCall != null)
        {
            MessageBox.Show($"Call {SelectedCall.Id} description {SelectedCall.Description}", "Call Details", MessageBoxButton.OK, MessageBoxImage.Information);
            var Call = s_bl.Call.GetCallDetails(SelectedCall.Id);
            ShowMap((double)Volunteer.Latitude, (double)Volunteer.Longitude,
        (double?)Call.Latitude, (double?)Call.Longitude);

        }
        else
        {
            MessageBox.Show("No call selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Handles the button click event to choose a volunteer for a call.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnChooseVolunteer_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedCall != null)
        {
            var result = MessageBox.Show(
                $"Are you sure? You are choosing call {SelectedCall.Id} at {SelectedCall.FullAddress}.",
                "Confirm Selection",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
             );

            if (result == MessageBoxResult.Yes)
            {
                try
                {

                    s_bl.Call.SelectCallForTreatment(_volunteerId, SelectedCall.Id);
                    MessageBox.Show("Call selected successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception err)
                {
                    MessageBox.Show("Error selecting call: " + err);
                }
            }
            else
            {
                MessageBox.Show("Call selection cancelled.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
    }

    /// <summary>
    /// Handles the KeyDown event for the TextBox to update the source when Enter is pressed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (Keyboard.FocusedElement is TextBox focusedTextBox)
            {
                var binding = BindingOperations.GetBindingExpression(focusedTextBox, TextBox.TextProperty);
                binding?.UpdateSource();
            }
        }
    }

    /// <summary>
    /// Queries the call list for open calls assigned to the volunteer.
    /// </summary>
    private void queryCallList()
    {
        CallList = s_bl.Call.GetOpenCallsForVolunteer(_volunteerId);

    }

    /// <summary>
    /// Calls the observer to update the call list when there are changes.
    /// </summary>
    private volatile DispatcherOperation? _observerOperation = null; //stage 7

    private void callListObserver()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                queryCallList();
            });
    }

    /// <summary>
    /// Handles the Loaded event of the call list window to add an observer for call updates.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void callListWindow_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(callListObserver);
        s_bl.Admin.AddConfigObserver(callListObserver); //stage 7
        s_bl.Admin.AddClockObserver(callListObserver); //stage 7}
    }

    /// <summary>
    /// Handles the Closed event of the call list window to remove the observer for call updates.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void callLisWindow_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(callListObserver);
        s_bl.Admin.RemoveConfigObserver(callListObserver); //stage 7
        s_bl.Admin.RemoveClockObserver(callListObserver); //stage 7
    }


    /// <summary>
    /// Displays a map showing the volunteer's location and the call's location.
    /// </summary>
    /// <param name="volunteerLat">The volunteer's coordinates </param>
    /// <param name="volunteerLon">The volunteer's coordinates </param>
    /// <param name="callLat">The call's coordinates</param>
    /// <param name="callLon">The call's coordinates</param>
    private void ShowMap(double volunteerLat, double volunteerLon, double? callLat, double? callLon)
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

                <!-- Routing machine CSS & JS -->
                <link rel='stylesheet' href='https://unpkg.com/leaflet-routing-machine@latest/dist/leaflet-routing-machine.css' />
                <script src='https://unpkg.com/leaflet-routing-machine@latest/dist/leaflet-routing-machine.js'></script>
            </head>
            <body>
                <div id='map' style='width: 100%; height: 100vh;'></div>
                <script>
                    var map = L.map('map');
                    L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                        maxZoom: 19,
                        attribution: '© OpenStreetMap'
                    }}).addTo(map);

                    var volunteerLatLng = [{volunteerLat}, {volunteerLon}];
                    var callLatLng = [{callLat}, {callLon}];

                    var volunteerMarker = L.marker(volunteerLatLng).addTo(map)
                        .bindPopup('מיקום המתנדב').openPopup();

                    var callMarker = L.marker(callLatLng).addTo(map)
                        .bindPopup('מיקום הקריאה');

                    // קו אווירי (Polyline) בין המתנדב לקריאה
                    var latlngs = [volunteerLatLng, callLatLng];
                    var polyline = L.polyline(latlngs, {{color: 'red'}}).addTo(map);

                    // מיקוד על שני הסמנים
                    var bounds = L.latLngBounds(latlngs);
                    map.fitBounds(bounds, {{ padding: [50, 50] }});

                    // מסלול הליכה/נסיעה (Routing)
                    L.Routing.control({{
                        waypoints: [
                            L.latLng(volunteerLatLng),
                            L.latLng(callLatLng)
                        ],
                        lineOptions: {{
                            styles: [{{ color: 'blue', opacity: 0.6, weight: 4 }}]
                        }},
                        router: L.Routing.osrmv1({{ serviceUrl: 'https://router.project-osrm.org/route/v1' }}),
                        createMarker: function() {{ return null; }},
                        draggableWaypoints: false,
                        addWaypoints: false
                    }}).addTo(map);
                </script>
            </body>
            </html>";

        string uniqueFileName = $"map_{Guid.NewGuid()}.html";
        string htmlPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), uniqueFileName);
        System.IO.File.WriteAllText(htmlPath, mapHtml, Encoding.UTF8);
        webView.Source = new Uri(htmlPath);
    }
};

