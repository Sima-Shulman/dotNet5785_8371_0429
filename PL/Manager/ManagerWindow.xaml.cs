using PL.Call;
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

namespace PL.Manager
{
    /// <summary>
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation _callStatisticsObserverOperation = null!;
        private static CallListWindow callListWindow = null!;
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerWindow"/> class.
        /// </summary>
        public int Id { get; set; } 
        public ManagerWindow(int id)
        {
            Id = id;
            InitializeComponent();
            LoadCallStatistics();   
        }
        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(ManagerWindow));


        public TimeSpan RiskRange
        {
            get { return (TimeSpan)GetValue(RiskRangeProperty); }
            set { SetValue(RiskRangeProperty, value); }
        }

        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(ManagerWindow));

        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(ManagerWindow), new PropertyMetadata(1));

        public bool IsSimulatorRunning
        {
            get { return (bool)GetValue(IsSimulatorRunningProperty); }
            set { SetValue(IsSimulatorRunningProperty, value); }
        }
        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(ManagerWindow), new PropertyMetadata(false));
        /// <summary>
        /// Handles the Loaded event of the ManagerWindow control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManagerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            RiskRange = s_bl.Admin.GetRiskTimeRange();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
            s_bl.Call.AddObserver(callStatisticsObserver);
        }

        /// <summary>
        /// Handles the Closed event of the ManagerWindow control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManagerWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            s_bl.Call.RemoveObserver(callStatisticsObserver);
            App.Current.Properties["IsManagerLoggedIn"] = false;
        }



        /// <summary>
        /// Handles the click event of the buttons to promote the clock by one minute.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.PromoteClock(BO.Enums.TimeUnit.Minute);
        }

        /// <summary>
        /// Handles the click event of the buttons to promote the clock by one hour.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.PromoteClock(BO.Enums.TimeUnit.Hour);
        }

        /// <summary>
        /// Handles the click event of the buttons to promote the clock by one day.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.PromoteClock(BO.Enums.TimeUnit.Day);
        }

        /// <summary>
        /// Handles the click event of the buttons to promote the clock by one year.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.PromoteClock(BO.Enums.TimeUnit.Year);
        }

        /// <summary>
        /// Handles the click event of the button to update the risk range.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            if (RiskRange.TotalSeconds <= 0)
            {
                MessageBox.Show("Risk range must be a positive TimeSpan.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            s_bl.Admin.SetRiskTimeRange(RiskRange);
        }

        /// <summary>
        /// Handles the click event of the button to handle volunteers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHandleVolunteers_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerListWindow().Show();
        }

        /// <summary>
        /// Handles the click event of the button to handle calls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHandleCalls_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow(Id).Show();
        }

        /// <summary>
        /// Handles the click event of the button to initialize the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInitDB_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.InitializeDatabase();
        }

        /// <summary>
        /// Handles the click event of the button to reset the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetDB_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ResetDatabase();
        }

        /// <summary>
        /// Observes the clock and updates the CurrentTime property accordingly.
        /// </summary>
        private void clockObserver()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentTime = s_bl.Admin.GetClock();
            });
        }

        /// <summary>
        /// Observes the configuration changes and updates the RiskRange property accordingly.
        /// </summary>
        private void configObserver()
        {
            RiskRange = s_bl.Admin.GetRiskTimeRange();
        }




        private void btnToggleSimulator_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSimulatorRunning)
            {
                s_bl.Admin.StartSimulator(Interval);
                IsSimulatorRunning = true;
            }
            else
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
            }
        }


        public int OpenCallsCount
        {
            get { return (int)GetValue(OpenCallsCountProperty); }
            set { SetValue(OpenCallsCountProperty, value); }
        }
        public static readonly DependencyProperty OpenCallsCountProperty =
            DependencyProperty.Register("OpenCallsCount", typeof(int), typeof(ManagerWindow));

        public int InProgressCallsCount
        {
            get { return (int)GetValue(InProgressCallsCountProperty); }
            set { SetValue(InProgressCallsCountProperty, value); }
        }
        public static readonly DependencyProperty InProgressCallsCountProperty =
            DependencyProperty.Register("InProgressCallsCount", typeof(int), typeof(ManagerWindow));

        public int ClosedCallsCount
        {
            get { return (int)GetValue(ClosedCallsCountProperty); }
            set { SetValue(ClosedCallsCountProperty, value); }
        }
        public static readonly DependencyProperty ClosedCallsCountProperty =
            DependencyProperty.Register("ClosedCallsCount", typeof(int), typeof(ManagerWindow));

        public int ExpiredCallsCount
        {
            get { return (int)GetValue(ExpiredCallsCountProperty); }
            set { SetValue(ExpiredCallsCountProperty, value); }
        }
        public static readonly DependencyProperty ExpiredCallsCountProperty =
            DependencyProperty.Register("ExpiredCallsCount", typeof(int), typeof(ManagerWindow));

        public int OpenAtRiskCallsCount
        {
            get { return (int)GetValue(OpenAtRiskCallsCountProperty); }
            set { SetValue(OpenAtRiskCallsCountProperty, value); }
        }
        public static readonly DependencyProperty OpenAtRiskCallsCountProperty =
            DependencyProperty.Register("OpenAtRiskCallsCount", typeof(int), typeof(ManagerWindow));

        public int InProgressAtRiskCallsCount
        {
            get { return (int)GetValue(InProgressAtRiskCallsCountProperty); }
            set { SetValue(InProgressAtRiskCallsCountProperty, value); }
        }
        public static readonly DependencyProperty InProgressAtRiskCallsCountProperty =
            DependencyProperty.Register("InProgressAtRiskCallsCount", typeof(int), typeof(  ManagerWindow));

        private void LoadCallStatistics()
        {
            try
            {
                var counts = s_bl.Call.GetCallQuantitiesByStatus();
                // Array indices correspond to CallStatus enum values:
                // 0: Open, 1: InProgress, 2: Closed, 3: Expired, 4: OpenAtRisk, 5: InProgressAtRisk
                OpenCallsCount = counts.Length > 0 ? counts[0] : 0;
                InProgressCallsCount = counts.Length > 1 ? counts[1] : 0;
                ClosedCallsCount = counts.Length > 2 ? counts[2] : 0;
                ExpiredCallsCount = counts.Length > 3 ? counts[3] : 0;
                OpenAtRiskCallsCount = counts.Length > 4 ? counts[4] : 0;
                InProgressAtRiskCallsCount = counts.Length > 5 ? counts[5] : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading call statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void callStatisticsObserver()
        {
            if (_callStatisticsObserverOperation is null || _callStatisticsObserverOperation.Status == DispatcherOperationStatus.Completed)
                _callStatisticsObserverOperation = Dispatcher.BeginInvoke(() =>
                {
                    LoadCallStatistics();
                });
        }


        private void OpenCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWithFilter(BO.Enums.CallStatus.Opened);
        private void InProgressCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWithFilter(BO.Enums.CallStatus.InTreatment);
        private void ClosedCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWithFilter(BO.Enums.CallStatus.Closed);
        private void ExpiredCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWithFilter(BO.Enums.CallStatus.Expired);
        private void OpenAtRiskCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWithFilter(BO.Enums.CallStatus.OpenedAtRisk);
        private void InProgressAtRiskCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWithFilter(BO.Enums.CallStatus.InTreatmentAtRisk);

        private void OpenCallListWithFilter(BO.Enums.CallStatus status)
        {
            try
            {
                if (callListWindow == null || !callListWindow.IsVisible)
                {
                    callListWindow = new CallListWindow(Id);
                    callListWindow.SelectedStatus = status; // Set the filter
                    callListWindow.Closed += (s, args) => callListWindow = null!;
                    callListWindow.Show();
                }
                else
                {
                    if (callListWindow.WindowState == WindowState.Minimized)
                        callListWindow.WindowState = WindowState.Normal;
                    callListWindow.SelectedStatus = status; // Update the filter
                    callListWindow.Activate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening call list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }





    }
}
