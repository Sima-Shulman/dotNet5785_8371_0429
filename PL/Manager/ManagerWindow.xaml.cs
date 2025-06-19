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
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));


        public TimeSpan RiskRange
        {
            get { return (TimeSpan)GetValue(RiskRangeProperty); }
            set { SetValue(RiskRangeProperty, value); }
        }

        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow));

        private void ManagerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize the current time and risk range
            CurrentTime = s_bl.Admin.GetClock();
            RiskRange = s_bl.Admin.GetRiskTimeRange();
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }
        private void ManagerWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }
        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.PromoteClock(BO.Enums.TimeUnit.Minute);
        }
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.PromoteClock(BO.Enums.TimeUnit.Hour);
        }
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.PromoteClock(BO.Enums.TimeUnit.Day);
        }
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.PromoteClock(BO.Enums.TimeUnit.Year);
        }
        private void btnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskTimeRange(RiskRange);
        }
        private void btnHandleVolunteers_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerListWindow().Show();
        }
        private void btnHandleCalls_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow().Show();
        }
        private void btnInitDB_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.InitializeDatabase();
        }
        private void btnResetDB_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ResetDatabase();
        }
        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }
        private void configObserver()
        {
            RiskRange = s_bl.Admin.GetRiskTimeRange();
        }
    }
}
