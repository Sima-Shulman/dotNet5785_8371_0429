
using System;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using BlApi;

namespace BlTest
{
    /// <summary>
    /// Main user interface entry point for the system.
    /// Provides an interactive console-based navigation through Admin, Call, and Volunteer menus.
    /// Each menu offers operations like view, add, update, and delete for the relevant domain entity.
    /// The main program loop runs until the user explicitly chooses to exit.
    /// All business logic operations are delegated to the IBl instance, ensuring separation of concerns
    /// between the user interface and core functionality.
    /// </summary>
    /// <param name="s_bl">Shared instance of the business logic layer used to access system functionalities.</param>

    internal class Program
    {
        static readonly IBl s_bl = BlApi.Factory.Get();
        /// <summary>
        /// Entry point of the program. Displays the main menu and routes user choices to relevant domain menus.
        /// </summary>
        /// <returns>Runs in a loop until the user chooses to exit the system.</returns>
        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Admin");
                Console.WriteLine("2. Call");
                Console.WriteLine("3. Volunteer");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        HandleAdminMenu();
                        break;
                    case "2":
                        HandleCallMenu();
                        break;
                    case "3":
                        HandleVolunteerMenu();
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        //ADMIN
        /// <summary>
        /// Displays a menu for administrator actions and handles user input to perform system-level operations.
        /// </summary>
        private static void HandleAdminMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Admin Menu:");
                Console.WriteLine("1. Get Clock");
                Console.WriteLine("2. Promote Clock");
                Console.WriteLine("3. Get Risk Time Range");
                Console.WriteLine("4. Set Risk Time Range");
                Console.WriteLine("5. Reset Database");
                Console.WriteLine("6. Initialize Database");
                Console.WriteLine("7. Back to Main Menu");
                Console.Write("Choose an option: ");
                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Console.WriteLine(s_bl.Admin.GetClock());
                        break;
                    case "2":
                        PromoteClock();
                        break;
                    case "3":
                        Console.WriteLine(s_bl.Admin.GetRiskTimeRange());
                        break;
                    case "4":
                        SetRiskTimeRange();
                        break;
                    case "5":
                        s_bl.Admin.ResetDatabase();
                        break;
                    case "6":
                        s_bl.Admin.InitializeDatabase();
                        break;
                    case "7":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        /// <summary>
        /// Promotes the internal system clock by a time unit selected by the user.
        /// </summary>
        private static void PromoteClock()
        {
            Console.WriteLine("Select time unit for promotion:");
            Console.WriteLine("1. Minute");
            Console.WriteLine("2. Hour");
            Console.WriteLine("3. Day");
            Console.WriteLine("4. Month");
            Console.WriteLine("5. Year");
            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();

            BO.Enums.TimeUnit timeUnit = choice switch
            {
                "1" => BO.Enums.TimeUnit.Minute,
                "2" => BO.Enums.TimeUnit.Hour,
                "3" => BO.Enums.TimeUnit.Day,
                "4" => BO.Enums.TimeUnit.Month,
                "5" => BO.Enums.TimeUnit.Year,
                _ => BO.Enums.TimeUnit.None
            };
            if (timeUnit == BO.Enums.TimeUnit.None)
                Console.WriteLine("invalid input, try again.");
            else
                s_bl.Admin.PromoteClock(timeUnit);
        }
        /// <summary>
        /// Sets the risk time range in the system by taking user input in minutes and converting it to a TimeSpan.
        /// </summary>
        private static void SetRiskTimeRange()
        {
            Console.Write("Enter the risk time range in minutes: ");
            if (int.TryParse(Console.ReadLine(), out int minutes))
            {
                TimeSpan riskTimeRange = TimeSpan.FromMinutes(minutes);
                s_bl.Admin.SetRiskTimeRange(riskTimeRange);
                Console.WriteLine($"Risk time range set to {riskTimeRange}.");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }


        //CALL
        /// <summary>
        /// Displays a menu for managing calls and handles user interaction for various call-related operations,
        /// including viewing details, adding, updating, deleting, assigning, and filtering calls.
        /// </summary>
        private static void HandleCallMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Call Menu:");
                Console.WriteLine("1. Get Call Details");
                Console.WriteLine("2. Add Call");
                Console.WriteLine("3. Update Call");
                Console.WriteLine("4. Delete Call");
                Console.WriteLine("5. Get Closed Calls Handled by Volunteer");
                Console.WriteLine("6. Get Open Calls for Volunteer");
                Console.WriteLine("7. Mark Call as Canceled");
                Console.WriteLine("8. Mark Call as Completed");
                Console.WriteLine("9. Get Call Quantities by Status");
                Console.WriteLine("10. Get Calls List");
                Console.WriteLine("11. Select Call for Treatment");
                Console.WriteLine("12. Back to Main Menu");
                Console.Write("Choose an option: ");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        GetCallDetails();
                        break;
                    case "2":
                        AddCall();
                        break;
                    case "3":
                        UpdateCall();
                        break;
                    case "4":
                        DeleteCall();
                        break;
                    case "5":
                        GetClosedCallsHandledByVolunteer();
                        break;
                    case "6":
                        GetOpenCallsForVolunteer();
                        break;
                    case "7":
                        MarkCallCancellation();
                        break;
                    case "8":
                        MarkCallCompletion();
                        break;
                    case "9":
                        GetCallQuantitiesByStatus();
                        break;
                    case "10":
                        GetCallsList();
                        break;
                    case "11":
                        SelectCallForTreatment();
                        break;
                    case "12":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        /// <summary>
        /// Retrieves and displays the details of a specific call based on a user-provided Call ID.
        /// </summary>
        /// <param name="callId">The ID of the call to retrieve, entered by the user via console input.</param>
        /// <returns>Displays the call details if found; otherwise, displays an error message.</returns>
        private static void GetCallDetails()
        {
            Console.Write("Enter Call ID: ");
            if (int.TryParse(Console.ReadLine(), out int callId))
            {
                try
                {
                    BO.Call call = s_bl.Call.GetCallDetails(callId);
                    Console.WriteLine(call);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }
        /// <summary>
        /// Retrieves a list of calls from the system, optionally filtered and sorted based on user input.
        /// </summary>
        /// <param name="fieldInput">An optional filter field name provided by the user (e.g., VolunteerName, Status).</param>
        /// <param name="filterValueInput">An optional value for filtering by status (e.g., Opened, Closed), entered by the user.</param>
        /// <param name="sortFieldInput">An optional field name to sort the results (e.g., Opening_time), entered by the user.</param>
        /// <returns>Displays the list of filtered and/or sorted calls; shows an error message in case of failure.</returns>
        private static void GetCallsList()
        {
            Console.WriteLine("Enter filter field (optional): ");
            string? fieldInput = Console.ReadLine();
            BO.Enums.CallInListFields? fieldFilter = fieldInput != "" ? Enum.TryParse(fieldInput, out BO.Enums.CallInListFields field) ? field : (BO.Enums.CallInListFields?)null : null;

            Console.WriteLine("Enter filter value (optional): ");
            string? filterValueInput = Console.ReadLine();
            // משתנה עזר שאינו Nullable
            BO.Enums.CallStatus tempFilterValue;
            // המרה תוך התחשבות בערך ריק
            BO.Enums.CallStatus? filterValue =
                Enum.TryParse(filterValueInput, out tempFilterValue) ? tempFilterValue : (BO.Enums.CallStatus?)null;

            Console.WriteLine("Enter sort field (optional): ");
            string? sortFieldInput = Console.ReadLine();
            BO.Enums.CallInListFields? sortField = sortFieldInput != "" ? Enum.TryParse(sortFieldInput, out BO.Enums.CallInListFields sort) ? sort : (BO.Enums.CallInListFields?)null : null;

            try
            {
                var calls = s_bl.Call.GetCallsList(fieldFilter, filterValue, sortField);
                foreach (var call in calls)
                {
                    Console.WriteLine(call);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
        }
        //v


        /// <summary>
        /// Assigns a volunteer to a specific call for treatment based on their IDs, as entered by the user.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer to assign to the call.</param>
        /// <param name="callId">The ID of the call to assign to the volunteer.</param>
        /// <returns>Displays a success message if the assignment is successful; otherwise, displays an error message.</returns>
        private static void SelectCallForTreatment()
        {
            Console.Write("Enter Volunteer ID: ");
            if (int.TryParse(Console.ReadLine(), out int volunteerId))
            {
                Console.Write("Enter Call ID: ");
                if (int.TryParse(Console.ReadLine(), out int callId))
                {
                    try
                    {
                        s_bl.Call.SelectCallForTreatment(volunteerId, callId);
                        Console.WriteLine("Call selected for treatment.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Call ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Volunteer ID.");
            }
        }
        /// <summary>
        /// Creates a new call and adds it to the system based on user-provided description, address, and type.
        /// </summary>
        /// <param name="description">The description of the call entered by the user.</param>
        /// <param name="callType">The type of the call (e.g., Transportation, CarAccident).</param>
        /// <param name="address">The full address where the call is located.</param>
        /// <returns>Displays a success message if the call is added; otherwise, displays an error message.</returns>
        private static void AddCall()
        {

            Console.Write("Enter Call Description: ");
            string? description = Console.ReadLine();
            Console.Write("Enter Call type(Transportation, CarAccident, VehicleBreakdown, SearchAndRescue): ");
            BO.Enums.CallType callType = Enum.TryParse(Console.ReadLine(), out BO.Enums.CallType parsedType) ? parsedType : throw new ArgumentException("Invalid call type.");
            Console.Write("Enter Full Address: ");
            string address = Console.ReadLine();

            var newCall = new BO.Call
            {

                Description = description,
                FullAddress = address,
                OpeningTime = s_bl.Admin.GetClock(),
                CallType = callType,
                CallStatus = BO.Enums.CallStatus.Opened
            };
            try
            {
                s_bl.Call.AddCall(newCall);
                Console.WriteLine("Call added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }

        }

        /// <summary>
        /// Updates an existing call's information such as description, address, type, and max finish time based on user input.
        /// </summary>
        /// <param name="callId">The ID of the call to update.</param>
        /// <param name="description">The new description (optional).</param>
        /// <param name="address">The new full address (optional).</param>
        /// <param name="callType">The new type of the call (optional).</param>
        /// <param name="maxFinishTime">The new maximum finish time in hh:mm format (optional).</param>
        /// <returns>Displays a success message if the call is updated; otherwise, displays an error message.</returns>

        private static void UpdateCall()
        {
            Console.Write("Enter Call ID: ");
            int.TryParse(Console.ReadLine(), out int callId);
            Console.Write("Enter New Description (optional) : ");
            string description = Console.ReadLine();
            Console.Write("Enter New Full Address (optional) : ");
            string address = Console.ReadLine();
            Console.Write("Enter Call Type (optional) : ");
            BO.Enums.CallType? callType = Enum.TryParse(Console.ReadLine(), out BO.Enums.CallType parsedType) ? parsedType : (BO.Enums.CallType?)null;
            Console.Write("Enter Max Finish Time (hh:mm , (optional)): ");
            TimeSpan? maxFinishTime = TimeSpan.TryParse(Console.ReadLine(), out TimeSpan parsedTime) ? parsedTime : (TimeSpan?)null;
            try
            {
                var callToUpdate = s_bl.Call.GetCallDetails(callId);
                if (callToUpdate == null)
                    throw new BO.BlDoesNotExistException($"Call with ID{callId} does not exist!");
                var newUpdatedCall = new BO.Call
                {
                    Id = callId,
                    Description = !string.IsNullOrWhiteSpace(description) ? description : callToUpdate.Description,
                    FullAddress = !string.IsNullOrWhiteSpace(address) ? address : callToUpdate.FullAddress,
                    OpeningTime = callToUpdate.OpeningTime,
                    MaxFinishTime = (maxFinishTime.HasValue ? DateTime.Now.Date + maxFinishTime.Value : callToUpdate.MaxFinishTime),
                    CallType = callType ?? callToUpdate.CallType
                };
                s_bl.Call.UpdateCallDetails(newUpdatedCall);
                Console.WriteLine("Call updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a call from the system based on a user-provided Call ID.
        /// </summary>
        /// <param name="callId">The ID of the call to delete.</param>
        /// <returns>Displays a success message if the call is deleted; otherwise, displays an error message.</returns>
        private static void DeleteCall()
        {
            Console.Write("Enter Call ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int callId))
            {
                try
                {
                    s_bl.Call.DeleteCall(callId);
                    Console.WriteLine("Call deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }

        /// <summary>
        /// Retrieves and displays a list of all closed calls that were handled by a specific volunteer.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer whose closed calls are being retrieved.</param>
        /// <returns>Displays the list of closed calls; shows an error message if retrieval fails.</returns>
        private static void GetClosedCallsHandledByVolunteer()
        {
            Console.Write("Enter Volunteer ID: ");
            int volunteerId = int.TryParse(Console.ReadLine(), out int vId) ? vId : throw new BO.BlInvalidFormatException("Invalid format of ID!");
            try
            {
                var closedCalls = s_bl.Call.GetClosedCallsHandledByVolunteer(volunteerId);
                foreach (var call in closedCalls)
                {
                    Console.WriteLine(call);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }

        }

        /// <summary>
        /// Retrieves and displays a list of open calls available for a specific volunteer.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer for whom open calls should be listed.</param>
        /// <returns>Displays the list of open calls; shows a message if none are found or if an error occurs.</returns>
        private static void GetOpenCallsForVolunteer()
        {
            Console.Write("Enter Volunteer ID: ");
            int.TryParse(Console.ReadLine(), out int volunteerId);
            try
            {
                var openCalls = s_bl.Call.GetOpenCallsForVolunteer(volunteerId);
                if (!openCalls.Any())
                    Console.WriteLine("No open calls for this volunteer.");
                else
                    foreach (var call in openCalls)
                    {
                        Console.WriteLine(call);
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves and displays the quantity of calls grouped by their current status.
        /// </summary>
        /// <returns>Displays the quantity of calls per status.</returns>
        static void GetCallQuantitiesByStatus()
        {
            var amounts = s_bl.Call.GetCallQuantitiesByStatus();
            foreach (var amount in amounts)
            {
                Console.WriteLine(amount);
            }
        }

        /// <summary>
        /// Marks a call as canceled by identifying the relevant assignment using the volunteer and assignment IDs.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer requesting the cancellation.</param>
        /// <param name="assignmentId">The ID of the assignment to cancel.</param>
        /// <returns>Displays a success message if the call is canceled; otherwise, displays an error message.</returns>
        private static void MarkCallCancellation()
        {
            Console.Write("Enter Volunteer ID: ");
            int.TryParse(Console.ReadLine(), out int volunteerId);
            Console.Write("Enter Assignment ID: ");
            int.TryParse(Console.ReadLine(), out int assignmentId);

            try
            {

                s_bl.Call.MarkCallCancellation(volunteerId, assignmentId);
                Console.WriteLine("Call marked as canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
        }

        /// <summary>
        /// Marks a call as completed by identifying the relevant assignment using the volunteer and assignment IDs.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer completing the assignment.</param>
        /// <param name="assignmentId">The ID of the assignment to complete.</param>
        /// <returns>Displays a success message if the call is marked as completed; otherwise, displays an error message.</returns>
        private static void MarkCallCompletion()
        {
            Console.Write("Enter Volunteer ID: ");
            int.TryParse(Console.ReadLine(), out int volunteerId);
            Console.Write("Enter Assignment ID: ");
            int.TryParse(Console.ReadLine(), out int assignmentId);

            try
            {
                s_bl.Call.MarkCallCompletion(volunteerId, assignmentId);
                Console.WriteLine("Call marked as completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
        }

        // Volunteer
        /// <summary>
        /// Displays the menu for managing volunteers and handles user selection.
        /// </summary>
        /// <returns>Executes the selected volunteer-related operation or exits to the main menu.</returns>
        private static void HandleVolunteerMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Volunteer Menu:");
                Console.WriteLine("1. Get Volunteer Details");
                Console.WriteLine("2. Get Volunteers List");
                Console.WriteLine("3. Add Volunteer");
                Console.WriteLine("4. Update Volunteer Details");
                Console.WriteLine("5. Delete Volunteer");
                Console.WriteLine("6. Login to System");
                Console.WriteLine("7. Back to Main Menu");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        GetVolunteerDetails();
                        break;
                    case "2":
                        GetVolunteersList();
                        break;
                    case "3":
                        AddVolunteer();
                        break;
                    case "4":
                        UpdateVolunteerDetails();
                        break;
                    case "5":
                        DeleteVolunteer();
                        break;
                    case "6":
                        EnterVolunteerSystem();
                        break;
                    case "7":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        /// <summary>
        /// Retrieves and displays the details of a specific volunteer based on their ID.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer to retrieve.</param>
        /// <returns>Displays the volunteer details or an error message if not found.</returns>
        private static void GetVolunteerDetails()
        {
            Console.Write("Enter Volunteer ID: ");
            if (int.TryParse(Console.ReadLine(), out int volunteerId))
            {
                try
                {
                    BO.Volunteer volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
                    if (volunteer is null)
                        throw new BO.BlDoesNotExistException($"Volunteer with Id {volunteerId} does not exist!");
                    Console.WriteLine(volunteer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }

        /// <summary>
        /// Retrieves and displays a list of volunteers with optional filtering by active status and sorting.
        /// </summary>
        /// <param name="isActiveFilter">Optional filter by volunteer active status.</param>
        /// <param name="sortField">Optional sort field for the volunteer list.</param>
        /// <returns>Displays the list of volunteers or an error message if retrieval fails.</returns>
        private static void GetVolunteersList()
        {
            Console.WriteLine("Enter filter for active volunteers (optional, true/false): ");
            string? isActiveInput = Console.ReadLine();
            bool? isActiveFilter = isActiveInput != "" ? bool.TryParse(isActiveInput, out bool isActive) ? isActive : (bool?)null : null;

            Console.WriteLine("Enter sort field for volunteers (optional): ");
            string? sortFieldInput = Console.ReadLine();
            BO.Enums.VolunteerInListFields? sortField = sortFieldInput != "" ? Enum.TryParse(sortFieldInput, out BO.Enums.VolunteerInListFields field) ? field : (BO.Enums.VolunteerInListFields?)null : null;

            try
            {
                var volunteers = s_bl.Volunteer.GetVolunteersList(isActiveFilter, sortField);
                foreach (var volunteer in volunteers)
                {
                    Console.WriteLine(volunteer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new volunteer to the system based on user-provided input.
        /// </summary>
        /// <param name="newVolunteer">Volunteer details entered by the user.</param>
        /// <returns>Displays a success message if added; otherwise, displays an error message.</returns>
        private static void AddVolunteer()
        {
            Console.WriteLine("Enter Volunteer details:");
            Console.WriteLine("ID:");
            int.TryParse(Console.ReadLine(), out int volunteerId);
            Console.Write("Full Name: ");
            string name = Console.ReadLine();
            Console.WriteLine("Cellphone Number:");
            string cellphone = Console.ReadLine();
            Console.WriteLine("Email:");
            string email = Console.ReadLine();
            Console.WriteLine("Password (optional):");
            string pass = Console.ReadLine();
            Console.Write("Address (optional): ");
            string address = Console.ReadLine();
            Console.WriteLine("Role (Volunteer/Manager):");
            BO.Enums.Role.TryParse(Console.ReadLine(), out BO.Enums.Role role);
            Console.WriteLine("IsActive (true/false):");
            bool.TryParse(Console.ReadLine(), out bool isActive);
            Console.WriteLine("Distance Type (  AerialDistance, WalkingDistance, DrivingDistance):");
            BO.Enums.DistanceType.TryParse(Console.ReadLine(), out BO.Enums.DistanceType distanceType);
            Console.WriteLine("Max Distance (optional):");
            double.TryParse(Console.ReadLine(), out double maxDistance);
            BO.Volunteer newVolunteer = new BO.Volunteer()
            {
                Id = volunteerId,
                FullName = name,
                CellphoneNumber = cellphone,
                Email = email,
                Password = pass,
                FullAddress = address,
                Role = role,
                IsActive = isActive,
                DistanceType = distanceType,
                MaxDistance = maxDistance
            };

            try
            {
                s_bl.Volunteer.AddVolunteer(newVolunteer);
                Console.WriteLine("Volunteer added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the details of an existing volunteer based on user input.
        /// </summary>
        /// <param name="requesterId">The ID of the user requesting the update.</param>
        /// <param name="volunteerId">The ID of the volunteer to update.</param>
        /// <returns>Displays a success message if updated; otherwise, displays an error message.</returns>
        private static void UpdateVolunteerDetails()
        {
            Console.Write("Enter Requester ID: ");
            int requesterId = int.Parse(Console.ReadLine());

            Console.Write("Enter Volunteer ID to Update: ");
            int volunteerId = int.Parse(Console.ReadLine());

            var doVolunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
            if (doVolunteer is null)
                throw new BO.BlDoesNotExistException($"Volunteer with ID {volunteerId} does not exist!");
            Console.Write("Full Name(optional): ");
            string name = Console.ReadLine();

            Console.WriteLine("Cellphone Number(optional):");
            string cellphone = Console.ReadLine();

            Console.WriteLine("Email(optional):");
            string email = Console.ReadLine();

            Console.WriteLine("Password (optional):");
            string pass = Console.ReadLine();

            Console.Write("Address (optional): ");
            string address = Console.ReadLine();

            Console.WriteLine("Role (Volunteer/Manager)(optional):");
            string roleInput = Console.ReadLine()!;
            BO.Enums.Role role = string.IsNullOrEmpty(roleInput) || !Enum.TryParse(roleInput, out BO.Enums.Role rType) ? doVolunteer.Role : rType;

            Console.WriteLine("IsActive (true/false)(optional):");
            string isActiveInput = Console.ReadLine()!;
            bool isActive = string.IsNullOrEmpty(isActiveInput) || !bool.TryParse(isActiveInput, out bool isA) ? doVolunteer.IsActive : isA;

            Console.WriteLine("Distance Type (  AerialDistance, WalkingDistance, DrivingDistance)(optional):");
            string distanceTypeInput = Console.ReadLine()!;
            BO.Enums.DistanceType distanceType = string.IsNullOrEmpty(distanceTypeInput) || !Enum.TryParse(distanceTypeInput, out BO.Enums.DistanceType dType) ? (BO.Enums.DistanceType)doVolunteer.DistanceType : dType;

            Console.WriteLine("Max Distance (optional):");
            string maxDistanceInput = Console.ReadLine()!;
            double? maxDistance = string.IsNullOrEmpty(maxDistanceInput) || !double.TryParse(maxDistanceInput, out double maxDis) ? doVolunteer.MaxDistance : maxDis;

            BO.Volunteer updatedVolunteer = new BO.Volunteer()
            {
                Id = volunteerId,
                FullName = string.IsNullOrEmpty(name) ? doVolunteer.FullName : name,
                CellphoneNumber = string.IsNullOrEmpty(cellphone) ? doVolunteer.CellphoneNumber : cellphone,
                Email = string.IsNullOrEmpty(email) ? doVolunteer.Email : email,
                Password = /*string.IsNullOrEmpty(pass) ? doVolunteer.Password : */pass,
                FullAddress = string.IsNullOrEmpty(address) ? doVolunteer.FullAddress : address,
                Role = (role != BO.Enums.Role.Manager) && (role != BO.Enums.Role.Volunteer) ? doVolunteer.Role : role,
                IsActive = isActive ? doVolunteer.IsActive : isActive,
                DistanceType = (distanceType != BO.Enums.DistanceType.DrivingDistance) && (distanceType != BO.Enums.DistanceType.DrivingDistance) && (distanceType != BO.Enums.DistanceType.AerialDistance) ? doVolunteer.DistanceType : distanceType,
                MaxDistance = maxDistance
            };


            try
            {
                s_bl.Volunteer.UpdateVolunteerDetails(requesterId, updatedVolunteer);
                Console.WriteLine("Volunteer details updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a volunteer from the system based on their ID.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer to delete.</param>
        /// <returns>Displays a success message if deleted; otherwise, displays an error message.</returns>
        private static void DeleteVolunteer()
        {
            try
            {
                Console.Write("Enter Volunteer ID to Delete: ");
                if (int.TryParse(Console.ReadLine(), out int volunteerId))
                    s_bl.Volunteer.DeleteVolunteer(volunteerId);
                Console.WriteLine("Volunteer deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a volunteer into the system using their name and password.
        /// </summary>
        /// <param name="volunteerName">The volunteer's username.</param>
        /// <param name="volunteerPass">The volunteer's password.</param>
        /// <returns>Displays the volunteer's role upon successful login; otherwise, displays an error message.</returns>
        private static void EnterVolunteerSystem()
        {
            try
            {
                Console.Write("Enter Volunteer Name : ");
                string volunteerName = Console.ReadLine() ?? throw new BO.BlNullPropertyException("id can't be null");
                Console.Write("Enter Volunteer Password: ");
                string volunteerPass = Console.ReadLine() ?? throw new BO.BlNullPropertyException("password can't be null");
                var role = s_bl.Volunteer.EnterSystem(volunteerName, volunteerPass);
                Console.WriteLine($"You've logged in successfully! Your role:{role}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
        }
    }
}