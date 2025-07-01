using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerWindow.xaml
/// </summary>
public partial class VolunteerWindow : Window, INotifyPropertyChanged
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.Volunteer? CurrentVolunteer
    {
        get => (BO.Volunteer?)GetValue(CurrentVolunteerProperty);
        set => SetValue(CurrentVolunteerProperty, value);
    }

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    private string _buttonText;
    public string ButtonText
    {
        get => _buttonText;
        set
        {
            _buttonText = value;
            OnPropertyChanged(nameof(ButtonText));
        }
    }


    /// <summary>
    /// Constructor for VolunteerWindow.
    /// </summary>
    /// <param name="id"></param>
    public VolunteerWindow(int id = 0)
    {
        _buttonText = string.Empty;
        PropertyChanged = delegate { };

        ButtonText = id == 0 ? "Add" : "Update";
        InitializeComponent();
        DataContext = this;

        try
        {
            CurrentVolunteer = id != 0
                ? s_bl.Volunteer.GetVolunteerDetails(id)!
                : new BO.Volunteer()
                {
                    Id = 0,
                    FullName = string.Empty,
                    CellphoneNumber = string.Empty,
                    Email = string.Empty,
                    Password = string.Empty,
                    FullAddress = string.Empty,
                    Latitude = null,
                    Longitude = null,
                    Role = BO.Enums.Role.None,
                    IsActive = true,
                    DistanceType = BO.Enums.DistanceType.None,
                    MaxDistance = null,
                    TotalHandledCalls = 0,
                    TotalCanceledCalls = 0,
                    TotalExpiredCalls = 0,
                    CallInProgress = null
                };
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            Close();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Method to raise the PropertyChanged event for data binding.
    /// </summary>
    /// <param name="propertyName"></param>
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Event handler for the Add/Update button click event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer == null)
        {
            MessageBox.Show("Volunteer details are missing.");
            return;
        }

        if (string.IsNullOrWhiteSpace(CurrentVolunteer.FullName))
        {
            MessageBox.Show("Full Name cannot be empty.");
            return;
        }

        if (string.IsNullOrWhiteSpace(CurrentVolunteer.CellphoneNumber) || !System.Text.RegularExpressions.Regex.IsMatch(CurrentVolunteer.CellphoneNumber, @"^\d{10}$"))
        {
            MessageBox.Show("Cellphone Number must be a valid 10-digit number.");
            return;
        }

        if (string.IsNullOrWhiteSpace(CurrentVolunteer.Email) || !System.Text.RegularExpressions.Regex.IsMatch(CurrentVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            MessageBox.Show("Email must be in a valid format.");
            return;
        }

        if (CurrentVolunteer.MaxDistance.HasValue && CurrentVolunteer.MaxDistance <= 0)
        {
            MessageBox.Show("Max Distance must be greater than 0.");
            return;
        }

        if (ButtonText == "Add")
        {
            try
            {
                s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        else if (ButtonText == "Update")
        {
            try
            {
                s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer!.Id, CurrentVolunteer!);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        else
        {
            MessageBox.Show("Invalid operation");
        }
    }


    /// <summary>
    /// Observer method to refresh the volunteer details when changes occur.
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
            });
    }

    /// <summary>
    /// Event handler for the window loaded event to add the observer for volunteer updates.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void volunteerWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
            s_bl.Volunteer.AddObserver(CurrentVolunteer!.Id, volunteerObserver);
        if (CurrentVolunteer.CallInProgress != null)
            s_bl.Call.AddObserver(CurrentVolunteer.CallInProgress.CallId, volunteerObserver);
    }

    /// <summary>
    /// Event handler for the window closed event to remove the observer for volunteer updates.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void volunteerWindow_Closed(object sender, EventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer!.Id, volunteerObserver);
        if (CurrentVolunteer.CallInProgress != null)
            s_bl.Call.RemoveObserver(CurrentVolunteer.CallInProgress.CallId, volunteerObserver);
    }
}
