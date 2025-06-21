using System.Windows;
using System.ComponentModel;

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
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (ButtonText == "Add")
        {
            try
            {
                s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                //MessageBox.Show("Volunteer added successfully");
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

    private void volunteerObserver()
    {
        int id = CurrentVolunteer!.Id;
        CurrentVolunteer = null;
        CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
    }
    private void volunteerWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
            s_bl.Volunteer.AddObserver(CurrentVolunteer!.Id, volunteerObserver);
    }
    private void volunteerWindow_Closed(object sender, EventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer!.Id, volunteerObserver);
    }
}
