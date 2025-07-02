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
        public int AdminId { get; set; }
        public CallListWindow(int id)
        {
            AdminId = id;
            InitializeComponent();
        }
        public BO.Enums.CallType CallType { get; set; } = BO.Enums.CallType.None;

        //public BO.Enums.CallStatus CallStatus { get; set; } = BO.Enums.CallStatus.None;

        public BO.CallInList? SelectedCall { get; set; }

        public BO.Enums.CallStatus? SelectedStatus
        {
            get => (BO.Enums.CallStatus?)GetValue(SelectedStatusProperty);
            set => SetValue(SelectedStatusProperty, value);
        }
        public static readonly DependencyProperty SelectedStatusProperty =
            DependencyProperty.Register("SelectedStatus", typeof(BO.Enums.CallStatus?), typeof(CallListWindow), new PropertyMetadata(BO.Enums.CallStatus.None, OnFilterChanged));
        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(PL.Call.CallListWindow), new PropertyMetadata(null));

        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CallListWindow window)
                window.queryCallList();
        }
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
                        if (AdminId != default)
                        {
                            s_bl.Call.MarkCallCancellation(AdminId, call.AssignmentId!.Value);
                            queryCallList();// Refresh the call list after unassigning because the observers do not observe assignments.
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
            try
            {
                var allCalls = s_bl?.Call.GetCallsList() ?? Enumerable.Empty<BO.CallInList>();

                var filteredCalls = allCalls;

                if (SelectedStatus != BO.Enums.CallStatus.None)
                    filteredCalls = filteredCalls.Where(c => c.CallStatus == SelectedStatus);

                if (CallType != BO.Enums.CallType.None)
                    filteredCalls = filteredCalls.Where(c => c.CallType == CallType);

                return filteredCalls;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return Enumerable.Empty<BO.CallInList>();
            }
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
        {
            s_bl.Call.AddObserver(callListObserver);
            s_bl.Admin.AddConfigObserver(callListObserver); // Registering for configuration changes
            s_bl.Admin.AddClockObserver(callListObserver); // Registering for clock changes
        }


        /// <summary>
        /// Handles the Closed event of the CallListWindow to remove the call list observer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void callLisWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(callListObserver);
            s_bl.Admin.RemoveConfigObserver(callListObserver);
            s_bl.Admin.RemoveClockObserver(callListObserver);

        }


    }
}
