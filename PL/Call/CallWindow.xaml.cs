using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using BO;
using static BO.Enums;

namespace PL.Call;

/// <summary>
/// Interaction logic for CallWindow.xaml
/// </summary>
public partial class CallWindow : Window, INotifyPropertyChanged
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    private string _buttonText;
    public string ButtonText
    {
        get => _buttonText;
        set
        {
            _buttonText = value;
            OnPropertyChanged(nameof(ButtonText));
        }
    }

    /// <summary>
    /// Constructor for CallWindow.
    /// </summary>
    /// <param name="callId"></param>
    public CallWindow(int callId = 0)
    {
        _buttonText = string.Empty;
        PropertyChanged = delegate { };

        ButtonText = callId == 0 ? "Add" : "Update";
        InitializeComponent();

        try
        {
            if (callId != 0)
            {
                CurrentCall = s_bl.Call.GetCallDetails(callId);
            }
            else
            {
                CurrentCall = new BO.Call()
                {
                    Id = 0,
                    Description = string.Empty,
                    FullAddress = string.Empty,
                    Latitude = 0.0,
                    Longitude = 0.0,
                    OpeningTime = DateTime.Now,
                    MaxFinishTime = null,
                    CallType = CallType.None,
                    CallStatus = CallStatus.None
                };
            }
            if (CurrentCall == null)
            {
                MessageBox.Show("Call not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }

    public BO.Call CurrentCall
    {
        get => (BO.Call)GetValue(CurrentCallProperty);
        set => SetValue(CurrentCallProperty, value);
    }

    public static readonly DependencyProperty CurrentCallProperty =
        DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow),
            new PropertyMetadata(null, OnCurrentCallChanged));


    /// <summary>
    /// Callback for when the CurrentCall property changes.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void OnCurrentCallChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = d as CallWindow;
        if (window == null) return;

        window.OnPropertyChanged(nameof(IsEditableAllFields));
        window.OnPropertyChanged(nameof(IsEditableOnlyMaxFinish));
        window.OnPropertyChanged(nameof(IsEditable));
    }

    /// <summary>
    /// Determines if all fields of the call are editable based on the call status or if the button text is "Add".
    /// </summary>
    public bool IsEditableAllFields =>
        CurrentCall?.CallStatus == CallStatus.Opened ||
        CurrentCall?.CallStatus == CallStatus.OpenedAtRisk || _buttonText == "Add";

    /// <summary>
    /// Determines if only the Max Finish Time field is editable based on the call status.
    /// </summary>
    public bool IsEditableOnlyMaxFinish =>
        CurrentCall?.CallStatus == CallStatus.InTreatment ||
        CurrentCall?.CallStatus == CallStatus.InTreatmentAtRisk || IsEditableAllFields;

    /// <summary>
    /// Determines if the call is editable based on the statuses that allow editing all fields or only the Max Finish Time field.
    /// </summary>
    public bool IsEditable => IsEditableAllFields || IsEditableOnlyMaxFinish;


    /// <summary>
    /// Handles the click event for the Update button.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentCall == null)
            return;

        if (string.IsNullOrWhiteSpace(CurrentCall.Description))
        {
            MessageBox.Show("Description is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (IsEditableAllFields && string.IsNullOrWhiteSpace(CurrentCall.FullAddress))
        {
            MessageBox.Show("Address is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (CurrentCall.MaxFinishTime != null)
        {
            if (CurrentCall.MaxFinishTime <= CurrentCall.OpeningTime)
            {
                MessageBox.Show("Max Finish Time must be after Opening Time.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CurrentCall.MaxFinishTime < DateTime.Now)
            {
                MessageBox.Show("Max Finish Time cannot be in the past.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        if (!Enum.IsDefined(typeof(CallType), CurrentCall.CallType) || CurrentCall.CallType == CallType.None)
        {
            MessageBox.Show("Please select a valid Call Type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (ButtonText == "Add")
        {
            try
            {
                s_bl.Call.AddCall(CurrentCall!);
                MessageBox.Show("Call added successfully");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        else if (ButtonText == "Update")
        {
            try
            {
                s_bl.Call.UpdateCallDetails(CurrentCall);
                MessageBox.Show("Call updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



    }
}
