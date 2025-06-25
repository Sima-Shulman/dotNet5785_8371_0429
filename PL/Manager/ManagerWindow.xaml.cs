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

namespace PL.Manager
{
    /// <summary>
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerWindow"/> class.
        /// </summary>
        public ManagerWindow()
        {
            InitializeComponent();
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
            new CallListWindow().Show();
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
            CurrentTime = s_bl.Admin.GetClock();
        }

        /// <summary>
        /// Observes the configuration changes and updates the RiskRange property accordingly.
        /// </summary>
        private void configObserver()
        {
            RiskRange = s_bl.Admin.GetRiskTimeRange();
        }
    }
}
