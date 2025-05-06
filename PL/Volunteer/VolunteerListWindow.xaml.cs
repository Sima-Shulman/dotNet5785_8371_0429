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
        //public BO.VolunteerInList? SelectedVolunteer
        //{
        //    get => (BO.VolunteerInList?)GetValue(SelectedVolunteerProperty);
        //    set => SetValue(SelectedVolunteerProperty, value);
        //}

        //public static readonly DependencyProperty SelectedVolunteerProperty =
        //    DependencyProperty.Register("SelectedVolunteer", typeof(BO.VolunteerInList), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public BO.VolunteerInList? SelectedVolunteer { get; set; }


        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(PL.Volunteer.VolunteerListWindow), new PropertyMetadata(null));

        public BO.Enums.CallType CallType { get; set; } = BO.Enums.CallType.None;

        private void comboBoxFilterVolunteers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VolunteerList = FilterVolunteerListList();
        }
        private void queryVolunteerList()
        {
            VolunteerList = FilterVolunteerListList();
        }

        private void courseListObserver()
            => queryVolunteerList();

        private void volunteerListWindow_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(courseListObserver);

        private void volunteerLisWindow_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(courseListObserver);

        private IEnumerable<BO.VolunteerInList> FilterVolunteerListList()
        {
            return (CallType == BO.Enums.CallType.None) ?
                            s_bl?.Volunteer.GetVolunteersList() ?? Enumerable.Empty<BO.VolunteerInList>() :
                            s_bl?.Volunteer.GetVolunteersFilterList(CallType) ?? Enumerable.Empty<BO.VolunteerInList>();
        }
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer is BO.VolunteerInList volunteer)
                new VolunteerWindow(volunteer.Id).Show();



        }
        private void btnAddVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow().Show();
        }
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
    }

}

