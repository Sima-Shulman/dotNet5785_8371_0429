using BlApi;
using DalApi;
using Helpers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Factory = DalApi.Factory;

namespace Helpers;
/// <summary>
/// Responsible for converting and managing Volunteer objects between BO and DO layers,
/// validating input, password handling, and generating summaries.
/// </summary>

internal static class VolunteerManager
{
    private static IDal s_dal = DalApi.Factory.Get;
    private static IBl s_bl = BlApi.Factory.Get();

    internal static ObserverManager Observers = new(); //stage 5 

    /// <summary>
    /// Converts DO.Volunteer to BO.Volunteer with additional calculated fields.
    /// </summary>
    /// <param name="doVolunteer">The volunteer from the data layer.</param>
    /// <returns>Mapped and enriched business object volunteer.</returns>
    public static BO.Volunteer ConvertDoVolunteerToBoVolunteer(DO.Volunteer doVolunteer)
    {
        lock (AdminManager.BlMutex) //stage 7
        {
            var currentVolunteerAssignments = s_dal.Assignment.ReadAll(a => a?.VolunteerId == doVolunteer.Id);
            var totalHandled = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.WasTreated);
            var totalCanceled = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.ManagerCancellation || a?.EndType == DO.EndType.SelfCancellation);
            var totalExpired = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.Expired);
            var assignedCallId = currentVolunteerAssignments.FirstOrDefault(a => a?.EndTime == null)?.CallId;
            var currentAssignment = s_dal.Assignment.ReadAll(a => a?.VolunteerId == doVolunteer.Id && a.EndTime == null).LastOrDefault();
            BO.CallInProgress? callInProgress = null;
            if (currentAssignment is not null)
            {
                var callDetails = s_dal.Call.Read(currentAssignment.CallId);
                if (callDetails is not null)
                {
                    callInProgress = new BO.CallInProgress
                    {
                        AssignmentId = currentAssignment.Id,
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
                Role = (BO.Enums.Role)doVolunteer!.Role,
                IsActive = doVolunteer.IsActive,
                DistanceType = (BO.Enums.DistanceType)doVolunteer.DistanceTypes,
                MaxDistance = doVolunteer.MaxDistance,
                TotalHandledCalls = totalHandled,
                TotalCanceledCalls = totalCanceled,
                TotalExpiredCalls = totalExpired,
                CallInProgress = callInProgress

            };
        }
    }
    /// <summary>
    /// Converts BO.Volunteer to DO.Volunteer for database use.
    /// </summary>
    /// <param name="boVolunteer">Business object volunteer.</param>
    /// <returns>Data object volunteer.</returns>
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

    /// <summary>
    /// Converts DO.Volunteer to a BO.VolunteerInList for list/summarized display.
    /// </summary>
    /// <param name="doVolunteer">The volunteer from the data layer.</param>
    /// <returns>Summarized volunteer with basic info and current call status.</returns>
    public static BO.VolunteerInList ConvertDoVolunteerToBoVolunteerInList(DO.Volunteer doVolunteer)
    {
        lock (AdminManager.BlMutex) //stage 7
        {
            var currentVolunteerAssignments = s_dal.Assignment.ReadAll(a => a?.VolunteerId == doVolunteer.Id);
            var totalHandled = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.WasTreated);
            var totalCanceled = currentVolunteerAssignments.Count(a => a?.EndType == DO.EndType.ManagerCancellation || a?.EndType == DO.EndType.SelfCancellation);
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
                    : BO.Enums.CallType.None
            };
        }
    }

    /// <summary>
    /// Converts a list of DO.Volunteer to a list of BO.VolunteerInList.
    /// </summary>
    /// <param name="volunteers">Raw volunteer list from data layer.</param>
    /// <returns>List of summarized business object volunteers.</returns>
    public static List<BO.VolunteerInList> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
    => volunteers.Select(v =>
    {
        lock (AdminManager.BlMutex) //stage 7
            return ConvertDoVolunteerToBoVolunteerInList(v);
    }).ToList();


    /// <summary>
    /// Verifies a raw password against a stored (hashed) password.
    /// </summary>
    /// <param name="enteredPassword">User input password.</param>
    /// <param name="storedPassword">Hashed password stored in DB.</param>
    /// <returns>True if passwords match.</returns>
    internal static bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        var encryptedPassword = EncryptPassword(enteredPassword);
        return encryptedPassword == storedPassword;
    }

    /// <summary>
    /// Hashes a password using SHA-256.
    /// </summary>
    /// <param name="password">Raw password.</param>
    /// <returns>Encrypted base64-encoded password.</returns>
    internal static string EncryptPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes!);
    }

    /// <summary>
    /// Generates a strong, random 12-character password.
    /// </summary>
    /// <returns>Random secure password string.</returns>
    internal static string GenerateStrongPassword()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$%^&*";
        return new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Validates a 9-digit Israeli ID using checksum algorithm.
    /// </summary>
    /// <param name="id">ID number to check.</param>
    /// <returns>True if valid.</returns>
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

    /// <summary>
    /// Validates all critical fields of a volunteer object.
    /// Throws BO.BlInvalidFormatException if invalid.
    /// </summary>
    /// <param name="boVolunteer">Volunteer to validate.</param>
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
        if (!string.IsNullOrEmpty(boVolunteer.Password))
        {
            if ((boVolunteer.Password.Length < 8 &&
                 !Regex.IsMatch(boVolunteer.Password, "[A-Z]") &&
                 !Regex.IsMatch(boVolunteer.Password, "[0-9]") &&
                 !Regex.IsMatch(boVolunteer.Password, "[!@#$%^&*]")))
                throw new BO.BlInvalidFormatException("Invalid password!");
        }
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

    /// <summary>
    /// Checks if a password meets minimum strength criteria.
    /// </summary>
    /// <param name="password">Password to check.</param>
    /// <returns>True if strong.</returns>
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


    private static readonly Random s_rand = new();
    private static int s_simulatorCounter = 0;

    internal static void SimulateAssigningCallsToVolunteers()
    {
        Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";

        List<DO.Volunteer?> doVolunteerList;
        lock (AdminManager.BlMutex)
            doVolunteerList = s_dal.Volunteer.ReadAll(v => v?.IsActive == true).ToList();

        foreach (var doVolunteer in doVolunteerList)
        {
            if (doVolunteer == null) continue;
            int volunteerId = doVolunteer.Id;
            DO.Assignment? currentAssignment;
            lock (AdminManager.BlMutex)
                currentAssignment = s_dal.Assignment.ReadAll(a => a?.VolunteerId == volunteerId && a.EndTime == null && a?.StartTime != null).LastOrDefault();

            if (currentAssignment == null)
            {
                if (s_rand.Next(100) < 20)
                {
                    IEnumerable<BO.OpenCallInList> openCalls;

                    openCalls = s_bl.Call.GetOpenCallsForVolunteer(volunteerId);

                    var callsWithCoordinates = openCalls.Where(c => c?.CallDistance != null).ToList();
                    if (callsWithCoordinates.Count > 0)
                    {
                        var selectedCall = callsWithCoordinates[s_rand.Next(callsWithCoordinates.Count)];
                        s_bl.Call.SelectCallForTreatment(doVolunteer.Id, selectedCall.Id);
                    }

                }

            }
            else
            {
                DO.Call? call;
                lock (AdminManager.BlMutex)
                    call = s_dal.Call.Read(currentAssignment.CallId);

                if (call == null) continue;

                double? distance = Tools.CalculateDistance(doVolunteer.Latitude, doVolunteer.Longitude,call.Longitude,call.Latitude , doVolunteer.DistanceTypes);
                if (distance == null) continue;

                TimeSpan timePassed = s_bl.Admin.GetClock() - currentAssignment.StartTime;

                double rawMinutes = distance.Value * 1.5 + s_rand.Next(2, 6);

                // בדיקה אם הערך בתחום סביר (למשל עד 1,000,000 דקות)
                if (rawMinutes <= 0 || rawMinutes > 1_000_000)
                {
                    rawMinutes = 10; // ברירת מחדל אם הערך לא תקין
                }

                TimeSpan minRequiredTime = TimeSpan.FromMinutes(rawMinutes);


                if (timePassed >= minRequiredTime)
                {
                    s_bl.Call.MarkCallCompletion(volunteerId, currentAssignment.Id);
                }
                else if (s_rand.Next(100) < 10)
                {
                    s_bl.Call.MarkCallCancellation(volunteerId, currentAssignment.Id);
                }
            }
        }
    }
}


