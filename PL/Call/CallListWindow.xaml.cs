using BO;
using PL.Volunteer;
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

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public CallListWindow()
        {
            InitializeComponent();
        }
        public BO.Enums.CallType CallType { get; set; } = BO.Enums.CallType.None;
        public BO.Enums.CallStatus CallStatus { get; set; } = BO.Enums.CallStatus.None;

        public BO.CallInList? SelectedCall { get; set; }


        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(PL.Call.CallListWindow), new PropertyMetadata(null));

        private void comboBoxFilterCallType_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => queryVolunteerList();

        private void comboBoxFilterCallStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
             => queryVolunteerList();



        private void btnDeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall is BO.CallInList call)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {call.CallId}?", "Delete Call", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                try
                {
                    if (result == MessageBoxResult.Yes)
                        s_bl.Call.DeleteCall(call.CallId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnUnassignCall_Click(object sender, RoutedEventArgs e)
        {
            //if (SelectedCall is BO.CallInList call)
            //{
            //    MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {call.CallId}?", "Delete Call", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            //    try
            //    {
            //        if (result == MessageBoxResult.Yes)
            //            s_bl.Call.MarkCallCancellation(call.CallId);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}
            MessageBox.Show("לא עובד בגלל המורה");
        }


        private void DataGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            new CallWindow(SelectedCall!.CallId).Show();
        }


        private void btnAddCall_Click(object sender, RoutedEventArgs e)
        {
            new CallWindow().Show();
        }

        private IEnumerable<BO.CallInList> FilterCallList()
        {
            return (CallStatus == BO.Enums.CallStatus.None) ?
              s_bl?.Call.GetCallsList() ?? Enumerable.Empty<BO.CallInList>() :
              s_bl.Call.GetCallsList(Enums.CallInListFields.CallStatus, CallStatus, null);
        }
        private void queryVolunteerList()
        {
            CallList = FilterCallList();
        }
        private void callListObserver()
                => queryVolunteerList();

        private void callListWindow_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(callListObserver);

        private void callLisWindow_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(callListObserver);


    }
}
