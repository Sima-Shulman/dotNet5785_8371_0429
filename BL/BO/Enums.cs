namespace BO;

public class Enums
{
    /// <summary>
    /// Enum for the volunteer's role.
    /// </summary>
    public enum Role
    {
        Manager,
        Volunteer
    }

    /// <summary>
    /// Enum for the distance type for the max distance for the volunteer to receive a call.
    /// </summary>
    public enum DistanceType
    {
        AerialDistance,
        WalkingDistance,
        DrivingDistance
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
        InTreatment,
        InTreatmentAtRisk,
        Opened,
        Closed,
        Expired,
        OpenedAtRisk
    }

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

