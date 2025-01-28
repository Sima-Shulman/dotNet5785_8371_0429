namespace BO;

public class Enums
{
    /// <summary>
    /// Enum for the volunteer's role.
    /// </summary>
    public enum Role
    {
        manager,
        volunteer
    }

    /// <summary>
    /// Enum for the distance type for the max distance for the volunteer to receive a call.
    /// </summary>
    public enum DistanceTypes
    {
        aerial_distance,
        walking_distance,
        driving_distance
    }

    /// <summary>
    /// Enum for the call type.
    /// </summary>
    public enum CallType
    {
        transportation,
        car_accident,
        vehicle_breakdown,
        search_and_rescue,
        none
    }

    /// <summary>
    /// Enum for the type of ending of the call.
    /// </summary>
    public enum EndType
    {
        was_treated,
        self_cancellation,
        manager_cancellation,
        expired
    }

    /// <summary>
    /// Enum for the status of the call.
    /// </summary>
    public enum CallStatus
    {
        is_treated,
        treated_at_risk,
        opened,
        closed,
        expired,
        opened_at_risk
    }

    public enum VolunteerFields
    {
        Id,
        FullName,
        CellphoneNumber,
        Email,
        Password,
        FullAddress,
        Latitude,
        Longitude,
        Role,
        IsActive,
        DistanceTypes,
        MaxDistance,
        TotalHandledCalls,
        TotalCanceledCalls,
        TotalExpiredCalls,
        CallInProgress,
    }



    public enum CallFields
    {
        Status,
        VolunteerId,
        CallType,
        Priority,
        OpenDate,
        CloseDate,
        MaxFinishDate,
        Address,
        Longitude,
        Latitude,
        CallDescription
    }



    public enum TimeUnit
    {
        Minute,
        Hour,
        Day,
        Month,
        Year
    }
}

