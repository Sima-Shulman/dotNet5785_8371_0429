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
using System.Windows.Threading;
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

        /// <summary>
        /// Handles the selection change event of the Call Type filter combo box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxFilterCallType_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => queryCallList();

        /// <summary>
        /// Handles the selection change event of the Call Status filter combo box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxFilterCallStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
             => queryCallList();



        /// <summary>
        /// Handles the click event of the Delete Call button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Handles the click event of the Unassign Call button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUnassignCall_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall is BO.CallInList call)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to unassign {call.CallId}?", "Unassign Call", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                try
                {
                    if (result == MessageBoxResult.Yes)
                    {
                        var volunteer = s_bl.Volunteer.GetVolunteersList()
                            .FirstOrDefault(v => v.FullName == SelectedCall.LastVolunteerName && v.CallId is not null);////לבדוק כי יכול להיות כמה עם אותו שם
                        if (volunteer != null)
                        {
                            s_bl.Call.MarkCallCancellation(volunteer.Id, call.AssignmentId!.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        /// <summary>
        /// Handles the double-click event on the DataGrid to open the CallWindow for the selected call.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            new CallWindow(SelectedCall!.CallId).Show();
        }


        /// <summary>
        /// Handles the click event of the Add Call button to open a new CallWindow for adding a call.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddCall_Click(object sender, RoutedEventArgs e)
        {
            new CallWindow().Show();
        }

        /// <summary>
        /// Filters the call list based on the selected CallStatus and CallType.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<BO.CallInList> FilterCallList()
        {
            var allCalls = s_bl?.Call.GetCallsList() ?? Enumerable.Empty<BO.CallInList>();

            var filteredCalls = allCalls;

            if (CallStatus != BO.Enums.CallStatus.None)
                filteredCalls = filteredCalls.Where(c => c.CallStatus == CallStatus);

            if (CallType != BO.Enums.CallType.None)
                filteredCalls = filteredCalls.Where(c => c.CallType == CallType);

            return filteredCalls;
        }

        /// <summary>
        /// Queries the call list and updates the CallList property with the filtered results.
        /// </summary>
        private void queryCallList()
        {
            CallList = FilterCallList();
        }

        /// <summary>
        /// Registers the observer for call list updates and queries the initial call list.
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
        /// Handles the Loaded event of the CallListWindow to register the call list observer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void callListWindow_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(callListObserver);


        /// <summary>
        /// Handles the Closed event of the CallListWindow to remove the call list observer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void callLisWindow_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(callListObserver);


    }
}
