using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for MainVolunteer.xaml
    /// </summary>
    public partial class MainVolunteer : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.Volunteer? CurrentVolunteer
        {
            get => (BO.Volunteer?)GetValue(CurrentVolunteerProperty);
            set => SetValue(CurrentVolunteerProperty, value);
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(MainVolunteer), new PropertyMetadata(null));

        private BO.CallInProgress? _currentCall;
        public BO.CallInProgress? CurrentCall
        {
            get => _currentCall;
            set
            {
                _currentCall = value;
                OnPropertyChanged(nameof(CurrentCall));
            }
        }





        public MainVolunteer(int id)
        {

            InitializeComponent();

            try
            {
                CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                //CurrentCall = s_bl.Volunteer.GetVolunteerDetails(id).CallInProgress;
                var volunteerDetails = s_bl.Volunteer.GetVolunteerDetails(id);
                CurrentVolunteer = volunteerDetails;
                if (volunteerDetails.CallInProgress != null &&
                     volunteerDetails.CallInProgress.CallStatus != BO.Enums.CallStatus.Expired)
                {
                    CurrentCall = volunteerDetails.CallInProgress;
                }
                else
                {
                    CurrentCall = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer!.Id, CurrentVolunteer);
                MessageBox.Show("פרטי המתנדב עודכנו בהצלחה");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בעדכון: " + ex.Message);
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
                s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, volunteerObserver);
            //צריך לעקוב פה אחרי שינויים ב assignments
            // כי בישות של DO.Volunteer אין שדה שמתייחס לזה אם יש קריאה בטיפולו או לא
            //אז מעקב אחרי המתנדב עצמו לא יעזור לי כדי לדעת אם הוא לקח קריאה לטיפול.
        }

        private void volunteerWindow_Closed(object sender, EventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, volunteerObserver);
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
           new CallHistory(CurrentVolunteer!.Id).ShowDialog();
        }


        private void btnChooseCall_Click(object sender, RoutedEventArgs e)
        {
            var chooseCallWindow = new ChooseCallWindow(CurrentVolunteer!.Id);
            //צריך לעקוב פה אחרי שינויים ב assignments
            // כי בישות של DO.Volunteer אין שדה שמתייחס לזה אם יש קריאה בטיפולו או לא
            //אז מעקב אחרי המתנדב עצמו לא יעזור לי כדי לדעת אם הוא לקח קריאה לטיפול.
            chooseCallWindow.Closed += (s, args) => RefreshCallDetails();
            chooseCallWindow.ShowDialog();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        //צריך לעקוב פה אחרי שינויים ב assignments
        // כי בישות של DO.Volunteer אין שדה שמתייחס לזה אם יש קריאה בטיפולו או לא
        //אז מעקב אחרי המתנדב עצמו לא יעזור לי כדי לדעת אם הוא לקח קריאה לטיפול.
        private void RefreshCallDetails()
        {
            var id = CurrentVolunteer!.Id;
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
            CurrentCall = CurrentVolunteer.CallInProgress;
            OnPropertyChanged(nameof(CurrentVolunteer));
            OnPropertyChanged(nameof(CurrentCall));
        }


        private void btnEndCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // אותה הבעיה של האסימנט אידי
                //s_bl.Call.MarkCallCompletion();
                //CurrentCall = null;
                MessageBox.Show("הטיפול בקריאה הסתיים.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בסיום טיפול: " + ex.Message);
            }
        }

        private void btnCancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // אותה הבעיה של האסימנט אידי
                //s_bl.Call.MarkCallCancellation();
                //CurrentCall = null;
                MessageBox.Show("הטיפול בקריאה בוטל.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בביטול טיפול: " + ex.Message);
            }
        }

    }
}

