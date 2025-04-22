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
        var currentVolunteerAssignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == doVolunteer.Id);
        var totalHandled = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.WasTreated);
        var totalCanceled = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.ManagerCancellation || a.EndType == DO.EndType.SelfCancellation);
        var totalExpired = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.Expired);
        var assignedCallId = currentVolunteerAssignments.FirstOrDefault(a => a?.EndTime == null)?.CallId;
        var currentAssignment = s_dal.Assignment.ReadAll(a => a.VolunteerId == doVolunteer.Id && a.EndTime == null).LastOrDefault();
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
                    CallType = (BO.Enums.CallType)callDetails.CallType,
                    Description = callDetails.Description,
                    FullAddress = callDetails.FullAddress,
                    OpeningTime = callDetails.OpeningTime,
                    MaxFinishTime = (DateTime?)callDetails.MaxFinishTime,
                    StartTime = currentAssignment.StartTime,
                    CallDistance = Tools.CalculateDistance(doVolunteer.Latitude, doVolunteer.Longitude, callDetails.Latitude, callDetails.Longitude, doVolunteer.DistanceTypes),
                    CallStatus = callDetails.CalculateCallStatus()
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
            DistanceType = (BO.Enums.DistanceType)doVolunteer.DistanceTypes,
            MaxDistance = doVolunteer.MaxDistance,
            TotalHandledCalls = totalHandled,
            TotalCanceledCalls = totalCanceled,
            TotalExpiredCalls = totalExpired,
            CallInProgress = callInProgress

        };
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
        //foreach(var a in currentVolunteerAssignments)
        //{
        //    Console.WriteLine(a.EndType);
        //}
        var totalHandled = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.WasTreated);
        var totalCanceled = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.ManagerCancellation || a.EndType == DO.EndType.SelfCancellation);
        var totalExpired = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.Expired);
        var assignedCallId = currentVolunteerAssignments.FirstOrDefault(a => a?.EndTime == null)?.CallId;

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
                ? (BO.Enums.CallType)(s_dal.Call.Read(assignedCallId.Value)?.CallType ?? DO.CallType.Transportation)
                : BO.Enums.CallType.Transportation
        };
    }
    public static List<BO.VolunteerInList> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
        => volunteers.Select(v => ConvertDoVolunteerToBoVolunteerInList(v)).ToList();
    internal static bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        var encryptedPassword = EncryptPassword(enteredPassword);
        return encryptedPassword == storedPassword /*|| enteredPassword==storedPassword*/;
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

    public static void ValidateVolunteer(BO.Volunteer boVolunteer)
    {
        if (boVolunteer.Id <= 0 || !IsValidId(boVolunteer.Id))
            throw new BO.BlInvalidFormatException("Invalid Id number!");

        if (string.IsNullOrWhiteSpace(boVolunteer.FullName) || !boVolunteer.FullName.Contains(" "))
            throw new BO.BlInvalidFormatException("Invalid name!");

        if (!Regex.IsMatch(boVolunteer.CellphoneNumber, @"^\d{10}$"))
            throw new BO.BlInvalidFormatException("Invalid cellphone number!");

        if (!Regex.IsMatch(boVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new BO.BlInvalidFormatException("Invalid email!");
        //var doVolunteer = s_dal.Volunteer.Read(boVolunteer.Id);

        //if (!string.IsNullOrEmpty(boVolunteer.Password))
        //Console.WriteLine(boVolunteer.Password.Length < 8);
        //Console.WriteLine(!Regex.IsMatch(boVolunteer.Password, "[A-Z]"));
        //Console.WriteLine(!Regex.IsMatch(boVolunteer.Password, "[0-9]"));
        //Console.WriteLine(!Regex.IsMatch(boVolunteer.Password, "[!@#$%^&*]"));
        //Console.WriteLine(!VerifyPassword(boVolunteer.Password, doVolunteer.Password!));
        if (!string.IsNullOrEmpty(boVolunteer.Password))
        {
            if (/*!VerifyPassword(boVolunteer.Password, doVolunteer.Password!) ||*/
                (boVolunteer.Password.Length < 8 &&
                 !Regex.IsMatch(boVolunteer.Password, "[A-Z]") &&
                 !Regex.IsMatch(boVolunteer.Password, "[0-9]") &&
                 !Regex.IsMatch(boVolunteer.Password, "[!@#$%^&*]")))
                throw new BO.BlInvalidFormatException("Invalid password!");
        }
        //    else
        //    {
        //        if (boVolunteer.Password.Length < 8 ||
        //                       !Regex.IsMatch(boVolunteer.Password, "[A-Z]") ||
        //                       !Regex.IsMatch(boVolunteer.Password, "[0-9]") ||
        //                       !Regex.IsMatch(boVolunteer.Password, "[!@#$%^&*]"))
        //            throw new BO.BlInvalidFormatException("Invalid password!");

        //    }
        //}328178371

        //var doVolunteer = s_dal.Volunteer.Read(boVolunteer.Id);
        //if ((boVolunteer.Password.Length < 8 ||
        //     !Regex.IsMatch(boVolunteer.Password, "[A-Z]") ||
        //     !Regex.IsMatch(boVolunteer.Password, "[0-9]") ||
        //     !Regex.IsMatch(boVolunteer.Password, "[!@#$%^&*]")) && !VerifyPassword(boVolunteer.Password, doVolunteer.Password!))
        //    throw new BO.BlInvalidFormatException("Invalid password!");


        //if (!string.IsNullOrEmpty(boVolunteer.FullAddress))
        //    throw new BO.BlInvalidFormatException("Invalid address!");

        if (boVolunteer.Role != BO.Enums.Role.Volunteer && boVolunteer.Role != BO.Enums.Role.Manager)
            throw new BO.BlInvalidFormatException("Invalid role!");

        if (boVolunteer.MaxDistance.HasValue && boVolunteer.MaxDistance < 0)
            throw new BO.BlInvalidFormatException("Invalid distance!");

        if (!Enum.IsDefined(typeof(BO.Enums.DistanceType), boVolunteer.DistanceType))
            throw new BO.BlInvalidFormatException("Invalid distance type!");

        if (boVolunteer.TotalHandledCalls < 0)
            throw new BO.BlInvalidFormatException("Invalid sum of handled calls!");

        if (boVolunteer.TotalCanceledCalls < 0)
            throw new BO.BlInvalidFormatException("Invalid sum of canceled calls!");

        if (boVolunteer.TotalExpiredCalls < 0)
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
        if (!password.Any(c => "@#$%^&*!?".Contains(c)))
            return false;
        return true;
    }
}