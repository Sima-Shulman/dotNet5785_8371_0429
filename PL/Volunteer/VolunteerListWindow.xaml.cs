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
            VolunteerList = (CallType == BO.Enums.CallType.None) ?
            s_bl?.Volunteer.GetVolunteersList() : s_bl?.Volunteer.GetVolunteersList(null, BO.Enums.VolunteerInListFields.CallType, CallType)!;//////////??????????????????
        }

        //public Array VolunteerFieldOptions
        //{
        //    get { return (Array)GetValue(VolunteerFieldOptionsProperty); }
        //    set { SetValue(VolunteerFieldOptionsProperty, value); }
        //}

        //public static readonly DependencyProperty VolunteerFieldOptionsProperty =
        //    DependencyProperty.Register("VolunteerFieldOptions", typeof(Array), typeof(VolunteerListWindow), new PropertyMetadata(Enum.GetValues(typeof(CallType))));

        //private void comboBoxFilterVolunteers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var comboBox = sender as ComboBox;
        //    var selectedValue = comboBox.SelectedValue as CallType?;
        //    if (selectedValue != null && selectedValue is CallType)
        //    {
        //        var filteredVolunteers = s_bl.Volunteer.GetVolunteersByCallType(selectedValue);///////////////?????????????????????????????????
        //        VolunteerList = filteredVolunteers;
        //    }
    }

}

