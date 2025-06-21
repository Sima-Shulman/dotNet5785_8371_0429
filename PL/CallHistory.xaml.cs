using BO;
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

namespace PL;

/// <summary>
/// Interaction logic for CallHistory.xaml
/// </summary>
public partial class CallHistory : Window
{

    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public CallHistory(int volunteerId)
    {
        VolunteerId = volunteerId;
        InitializeComponent();
    }


    public List<BO.ClosedCallInList> ClosedCallsList
    {
        get { return (List<BO.ClosedCallInList>)GetValue(ClosedCallsListProperty); }
        set { SetValue(ClosedCallsListProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ClosedCallList.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ClosedCallsListProperty =
        DependencyProperty.Register("ClosedCallList", typeof(List<BO.ClosedCallInList>), typeof(CallHistory), new PropertyMetadata(new List<BO.ClosedCallInList>()));


    public BO.Enums.CallType CallType { get; set; } = BO.Enums.CallType.None;
    public int VolunteerId { get; set; } = 0;

    public BO.CallInList? SelectedCall { get; set; }
    private void comboBoxFilterCallType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        => queryClosedCallsList();

    private IEnumerable<BO.ClosedCallInList> FilterClosedCallsList()
    {
        return (CallType == BO.Enums.CallType.None) ?
          s_bl?.Call.GetClosedCallsHandledByVolunteer(VolunteerId,null,null) ?? Enumerable.Empty<BO.ClosedCallInList>() :
          s_bl.Call.GetClosedCallsHandledByVolunteer(VolunteerId, CallType, null);
    }
    private void queryClosedCallsList()
    {
        ClosedCallsList = FilterClosedCallsList().ToList();
    }
    private void closedCallsListObserver()
            => queryClosedCallsList();

    private void callListWindow_Loaded(object sender, RoutedEventArgs e)
        => s_bl.Call.AddObserver(closedCallsListObserver);

    private void callLisWindow_Closed(object sender, EventArgs e)
        => s_bl.Call.RemoveObserver(closedCallsListObserver);

}