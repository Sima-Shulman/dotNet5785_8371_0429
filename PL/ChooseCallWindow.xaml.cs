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
    public static readonly DependencyProperty MaxDistanceProperty =
    DependencyProperty.Register(
        "MaxDistance",
        typeof(double?),
        typeof(ChooseCallWindow),
        new PropertyMetadata(null, OnMaxDistanceChanged));

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

    private static void OnFullAddressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = d as ChooseCallWindow;
        if (window == null) return;

        var newValue = (string)e.NewValue;
        //if (newValue.HasValue && window.Volunteer.MaxDistance != newValue.Value)
        //{
        window.Volunteer.FullAddress = newValue;
            s_bl.Volunteer.UpdateVolunteerDetails(window._volunteerId, window.Volunteer);
            MessageBox.Show($"Max distance updated to {newValue} km.", "Update Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            window.CallList = s_bl.Call.GetOpenCallsForVolunteer(window._volunteerId);
            MessageBox.Show("CallList is updated!");
        //}
    }
    //// Using a DependencyProperty as the backing store for MaxDistance.  This enables animation, styling, binding, etc...
    //public static readonly DependencyProperty MaxDistanceProperty =
    //    DependencyProperty.Register("MaxDistance", typeof(double?), typeof(ChooseCallWindow), new PropertyMetadata(null));


    private void CallList_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (SelectedCall != null) 
        {
            MessageBox.Show($"Call {SelectedCall.Id} description {SelectedCall.Description}", "Call Details", MessageBoxButton.OK, MessageBoxImage.Information); 

        }
        else
        {
            MessageBox.Show("No call selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

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
                
                s_bl.Call.SelectCallForTreatment(_volunteerId, SelectedCall.Id);
                MessageBox.Show("Call selected successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Call selection cancelled.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
    }

    //update the max distance only after pressing Enter.
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
    private void queryCallList()
    {
        CallList = s_bl.Call.GetOpenCallsForVolunteer(_volunteerId);

    }
    private void callListObserver()
            => queryCallList();

    private void callListWindow_Loaded(object sender, RoutedEventArgs e)
        => s_bl.Call.AddObserver(callListObserver);

    private void callLisWindow_Closed(object sender, EventArgs e)
        => s_bl.Call.RemoveObserver(callListObserver);
}
