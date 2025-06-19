namespace BO;

public class Enums
{
    /// <summary>
    /// Enum for the volunteer's role.
    /// </summary>
    public enum Role
    {
        Manager,
        Volunteer,
        None
    }

    /// <summary>
    /// Enum for the distance type for the max distance for the volunteer to receive a call.
    /// </summary>
    public enum DistanceType
    {
        AerialDistance,
        WalkingDistance,
        DrivingDistance,
        None
    }

    /// <summary>
    /// Enum for the call type.
    /// </summary>
    public enum CallType
    {
        Transportation,
        CarAccident,
        VehicleBreakdown,
        SearchAndRescue,
        None
    }

    /// <summary>
    /// Enum for the type of ending of the call.
    /// </summary>
    public enum EndType
    {
        WasTreated,
        SelfCancellation,
        ManagerCancellation,
        Expired
    }

    /// <summary>
    /// Enum for the status of the call.
    /// </summary>
    public enum CallStatus
    {
        None,
        InTreatment,
        InTreatmentAtRisk,
        Opened,
        Closed,
        Expired,
        OpenedAtRisk
    }

    /// <summary>
    /// Enum representing the fields of a volunteer in list view.
    /// </summary>
    public enum VolunteerInListFields
    {
        Id,
        FullName,
        TotalHandledCalls,
        TotalCanceledCalls,
        TotalExpiredCalls,
        CallId,
        CallType,
    }
    /// <summary>
    /// Enum representing the fields of a call in list view.
    /// </summary>
    public enum CallInListFields
    {
        CallId,
        CallType,
        OpeningTime,
        TimeLeft,
        LastVolunteerName,
        TotalTime,
        CallStatus,
        TotalAssignments,
    }
    /// <summary>
    /// Enum representing the fields of an open call in list view.
    /// </summary>
    public enum OpenCallInListFields
    {
        Id,
        CallType,
        Description,
        FullAddress,
        StartTime,
        MaxFinishTime,
        CallDistance,
    }
    /// <summary>
    /// Enum representing the fields of a closed call in list view.
    /// </summary>
    public enum ClosedCallInListFields
    {
        Id,
        CallType,
        FullAddress,
        OpeningTime,
        StartTime,
        EndTime,
        EndType,
    }
    /// <summary>
    /// Enum representing time units for durations or intervals.
    /// </summary>
    public enum TimeUnit
    {
        Minute,
        Hour,
        Day,
        Month,
        Year,
        None
    }
}

