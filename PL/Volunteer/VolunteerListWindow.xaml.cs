using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public VolunteerListWindow()
        {
            InitializeComponent();

        }
        public BO.VolunteerInList? SelectedVolunteer { get; set; }


        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(PL.Volunteer.VolunteerListWindow), new PropertyMetadata(null));


        public BO.Enums.CallType CallType { get; set; } = BO.Enums.CallType.None;

        /// <summary>
        /// Handles the SelectionChanged event of the comboBoxFilterVolunteers control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxFilterVolunteers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VolunteerList = FilterVolunteerList();
        }

        /// <summary>
        /// Queries the volunteer list based on the selected filter.
        /// </summary>
        private void queryVolunteerList()
        {
            VolunteerList = FilterVolunteerList();
        }


        /// <summary>
        /// Registers an observer to update the volunteer list when changes occur.
        /// </summary>
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void VolunteerListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryVolunteerList();
                });
        }


        /// <summary>
        /// Handles the Loaded event of the volunteerListWindow control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void volunteerListWindow_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(VolunteerListObserver);

        /// <summary>
        /// Handles the Closed event of the volunteerLisWindow control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void volunteerListWindow_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(VolunteerListObserver);

        /// <summary>
        /// Filters the volunteer list based on the selected call type.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<BO.VolunteerInList> FilterVolunteerList()
        {
            return (CallType == BO.Enums.CallType.None) ?
                            s_bl?.Volunteer.GetVolunteersList() ?? Enumerable.Empty<BO.VolunteerInList>() :
                            s_bl?.Volunteer.GetVolunteersFilterList(CallType) ?? Enumerable.Empty<BO.VolunteerInList>();
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the DataGrid control to open the VolunteerWindow for the selected volunteer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer is BO.VolunteerInList volunteer)
                new VolunteerWindow(volunteer.Id).Show();
        }

        /// <summary>
        /// Handles the Click event of the btnAddVolunteer control to open a new VolunteerWindow for adding a volunteer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow().Show();
        }

        /// <summary>
        /// Handles the Click event of the btnDeleteVolunteer control to delete the selected volunteer after confirmation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteVolunteer_Click(Object sender, RoutedEventArgs e)
        {
            if (SelectedVolunteer is BO.VolunteerInList volunteer)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {volunteer.FullName}?", "Delete Volunteer", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                try
                {
                    if (result == MessageBoxResult.Yes)
                        s_bl.Volunteer.DeleteVolunteer(volunteer.Id);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

}

