using DalApi;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get;
    public static BO.Volunteer ConvertDoVolunteerToBoVolunteer(DO.Volunteer doVolunteer)
    {
        try
        {
            var currentVolunteerAssignments = s_dal.Assignment.ReadAll(a => a?.VolunteerId == doVolunteer.Id);
            var totalHandled = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.was_treated);
            var totalCanceled = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.manager_cancellation || a.EndType == DO.EndType.self_cancellation);
            var totalExpired = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.expired);
            var assignedCallId = currentVolunteerAssignments.FirstOrDefault(a => a?.End_time == null)?.CallId;
            var currentAssignment = s_dal.Assignment.ReadAll(a => a.VolunteerId == doVolunteer.Id && a.End_time == null).FirstOrDefault();
            BO.CallInProgress? callInProgress = null;
            if (currentAssignment is not null)
            {
                var callDetails = s_dal.Call.Read(currentAssignment.CallId);
                if (callDetails is not null)
                {
                    callInProgress = new BO.CallInProgress
                    {
                        Id = currentAssignment.Id,
                        CallId = currentAssignment.CallId,
                        CallType = (BO.Enums.CallType)callDetails.Call_type,
                        Verbal_description = callDetails.Verbal_description,
                        FullAddress = callDetails.Full_address,
                        Opening_time = callDetails.Opening_time,
                        Max_finish_time = (DateTime)callDetails.Max_finish_time,
                        Start_time = currentAssignment.Start_time,
                        CallDistance = Tools.CalculateDistance(doVolunteer.Latitude, doVolunteer.Longitude, callDetails.Latitude, callDetails.Longitude),
                        CallStatus = CallManager.CalculateCallStatus(callDetails)
                    };
                }
            }
            return new BO.Volunteer
            {
                Id = doVolunteer.Id,
                FullName = doVolunteer.FullName,
                CellphoneNumber = doVolunteer.CellphoneNumber,
                Email = doVolunteer.Email,
                Password = doVolunteer.Password,
                FullAddress = doVolunteer.FullAddress,
                Latitude = doVolunteer?.Latitude,
                Longitude = doVolunteer?.Longitude,
                Role = (BO.Enums.Role)doVolunteer.Role,
                IsActive = doVolunteer.IsActive,
                DistanceType = (BO.Enums.DistanceTypes)doVolunteer.DistanceTypes,
                MaxDistance = doVolunteer.MaxDistance,
                TotalHandledCalls = totalHandled,
                TotalCanceledCalls = totalCanceled,
                TotalExpiredCalls = totalExpired,


            };
        }
        catch (DO.DalDoesNotExistException ex)
        {
            return null;
        }
    }

    public static DO.Volunteer ConvertBoVolunteerToDoVolunteer(BO.Volunteer boVolunteer)
    {
        return new DO.Volunteer(
    boVolunteer.Id,
    boVolunteer.FullName,
    boVolunteer.CellphoneNumber,
    boVolunteer.Email,
    boVolunteer.FullAddress,
    boVolunteer.Latitude,
    boVolunteer.Longitude,
    (DO.Role)boVolunteer.Role,
    boVolunteer.IsActive,
    (DO.DistanceTypes)boVolunteer.DistanceType,
    boVolunteer.MaxDistance,
    boVolunteer.Password
    );
    }

    public static BO.VolunteerInList ConvertDoVolunteerToBoVolunteerInList(DO.Volunteer doVolunteer)
    {
        var currentVolunteerAssignments = s_dal.Assignment.ReadAll(a => a?.VolunteerId == doVolunteer.Id);

        var totalHandled = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.was_treated);
        var totalCanceled = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.manager_cancellation || a.EndType == DO.EndType.self_cancellation);
        var totalExpired = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.expired);
        var assignedCallId = currentVolunteerAssignments.FirstOrDefault(a => a?.End_time == null)?.CallId;

        return new BO.VolunteerInList
        {
            Id = doVolunteer.Id,
            FullName = doVolunteer.FullName,
            IsActive = doVolunteer.IsActive,
            TotalHandledCalls = totalHandled,
            TotalCanceledCalls = totalCanceled,
            TotalExpiredCalls = totalExpired,
            CallId = assignedCallId,
            CallType = assignedCallId is not null
                ? (BO.Enums.CallType)(s_dal.Call.Read(assignedCallId.Value)?.Call_type ?? DO.CallType.transportation)
                : BO.Enums.CallType.transportation
        };
    }
    public static List<BO.VolunteerInList> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
        => volunteers.Select(v => ConvertDoVolunteerToBoVolunteerInList(v)).ToList();
    internal static bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        var encryptedPassword = EncryptPassword(enteredPassword);
        return encryptedPassword == storedPassword;
    }
    internal static string EncryptPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes!);
    }

    internal static string GenerateStrongPassword()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$%^&*";
        return new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static bool IsValidId(int id)
    {
        string idStr = id.ToString().PadLeft(9, '0');
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int digit = idStr[i] - '0';
            int weight = (i % 2 == 0) ? 1 : 2;
            int product = digit * weight;

            sum += (product > 9) ? product - 9 : product;
        }
        return sum % 10 == 0;
    }

    public static void ValidateVolunteer(BO.Volunteer volunteer)
    {
        if (volunteer.Id <= 0 || !IsValidId(volunteer.Id))
            throw new BO.BlInvalidFormatException("Invalid Id number!");

        if (string.IsNullOrWhiteSpace(volunteer.FullName) || !volunteer.FullName.Contains(" "))
            throw new BO.BlInvalidFormatException("Invalid name!");

        if (!Regex.IsMatch(volunteer.CellphoneNumber, @"^\d{10}$"))
            throw new BO.BlInvalidFormatException("Invalid cellphone number!");

        if (!Regex.IsMatch(volunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new BO.BlInvalidFormatException("Invalid email!");

        if ((volunteer.Password.Length < 8 ||
             !Regex.IsMatch(volunteer.Password, "[A-Z]") ||
             !Regex.IsMatch(volunteer.Password, "[0-9]") ||
             !Regex.IsMatch(volunteer.Password, "[!@#$%^&*]")))
            throw new BO.BlInvalidFormatException("Invalid password!");

        if (!string.IsNullOrEmpty(volunteer.FullAddress))
            throw new BO.BlInvalidFormatException("Invalid address!");

        if (volunteer.Role != BO.Enums.Role.volunteer || volunteer.Role != BO.Enums.Role.manager)
            throw new BO.BlInvalidFormatException("Invalid role!");

        if (volunteer.MaxDistance.HasValue && volunteer.MaxDistance <= 0)
            throw new BO.BlInvalidFormatException("Invalid distance!");

        if (!Enum.IsDefined(typeof(BO.Enums.DistanceTypes), volunteer.DistanceType))
            throw new BO.BlInvalidFormatException("Invalid distance type!");

        if (volunteer.TotalHandledCalls < 0)
            throw new BO.BlInvalidFormatException("Invalid sum of handled calls!");

        if (volunteer.TotalCanceledCalls < 0)
            throw new BO.BlInvalidFormatException("Invalid sum of canceled calls!");

        if (volunteer.TotalExpiredCalls < 0)
            throw new BO.BlInvalidFormatException("Invalid sum of expired calls!");
    }
    internal static bool IsPasswordStrong(string password)
    {
        if (password.Length < 8)
            return false;
        if (!password.Any(char.IsUpper))
            return false;
        if (!password.Any(char.IsLower))
            return false;
        if (!password.Any(char.IsDigit))
            return false;
        if (!password.Any(c => "@#$%^&*".Contains(c)))
            return false;
        return true;
    }
}