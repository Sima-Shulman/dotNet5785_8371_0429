using Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic;

namespace DalTest;

internal class Program
{
    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    private static ICall? s_dalCall = new CallImplementation(); //stage 1
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 1
    static void Main(string[] args)
    {
        try
        {
            bool exit = false;
            while (!exit)
            {
                ShowMainMenu();
                int choice = GetUserChoice();
                switch (choice)
                {
                    case 0:
                        exit = true;
                        break;
                    case 1:
                        ShowCallMenu();
                        break;
                    case 2:
                        ShowVolunteerMenu();
                        break;
                    case 3:
                        ShowAssignmentMenu();
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    case 6:
                        ShowConfigMenu();
                        break;
                    case 7:
                        break;
                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    private static void ShowMainMenu()
    {
        Console.Clear();
        Console.WriteLine("Main Menu:");
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. Call Menu");
        Console.WriteLine("2. Volunteer Menu");
        Console.WriteLine("3. Assignment Menu");
        Console.WriteLine("4. Initializing the data");//
        Console.WriteLine("5. Displaying all data in the database");//
        Console.WriteLine("6. Configuration Menu");
        Console.WriteLine("7. Database reset and configuration data reset");//
        Console.Write("Please choose an option: ");
    }

    private static int GetUserChoice()
    {
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice))
        {
            Console.WriteLine("Invalid input. Please enter a valid number.");
        }
        return choice;
    }

    private static void ShowCallMenu()
    {
        Console.Clear();
        Console.WriteLine("Call Menu:");
        Console.WriteLine("0. Back to Main Menu");
        Console.WriteLine("1. Create Call");
        Console.WriteLine("2. Read Call by ID");
        Console.WriteLine("3. Read All Calls");
        Console.WriteLine("4. Update Call");
        Console.WriteLine("5. Delete Call");
        Console.WriteLine("6. Delete All Calls");
        Console.Write("Please choose an option: ");
        int choice = GetUserChoice();

        switch (choice)
        {
            case 0:
                return;
            case 1:
                CreateCall();
                break;
            case 2:
                ReadCallById();
                break;
            case 3:
                ReadAllCalls();
                break;
            case 4:
                UpdateCall();
                break;
            case 5:
                DeleteCall();
                break;
            case 6:
                DeleteAllCalls();
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }

    private static void CreateCall()
    {
        Console.Clear();

        // קריאה לסוג ה-call עם בדיקת תקינות
        Console.Write("Enter Call Type (transportation, car_accident, vehicle_breakdown, search_and_rescue): ");
        if (!Enum.TryParse(Console.ReadLine()!, out DO.CallType call_type))
        {
            throw new FormatException("Call Type is invalid!");
        }

        // קריאה לתיאור ה-call
        Console.Write("Enter Call Description: ");
        string verbal_description = Console.ReadLine()!;

        // קריאה לכתובת ה-call
        Console.Write("Enter Full Address: ");
        string full_address = Console.ReadLine()!;

        // קריאה לקואורדינטות (Latitude ו-Longitude) עם TryParse
        Console.Write("Enter Latitude: ");
        if (!double.TryParse(Console.ReadLine(), out double latitude))
        {
            throw new FormatException("Latitude is invalid!");
        }

        Console.Write("Enter Longitude: ");
        if (!double.TryParse(Console.ReadLine(), out double longitude))
        {
            throw new FormatException("Longitude is invalid!");
        }

        // יצירת ה-call החדש
        Call newCall = new Call
        {
            Call_type = call_type,                      // המרה תקינה של Enum
            Verbal_description = verbal_description,    // שמירת תיאור
            Full_address = full_address,                // שמירת כתובת
            Latitude = latitude,                        // שמירת Latitude
            Longitude = longitude,                      // שמירת Longitude
        };

        // יצירת ה-call ב-DAL
        s_dalCall?.Create(newCall);
        Console.WriteLine("Call created successfully.");
    }

    private static void ReadCallById()
    {
        Console.Clear();
        Console.Write("Enter Call ID to read: ");
        int id = int.Parse(Console.ReadLine()!);
        var call = s_dalCall?.Read(id);
        if (call != null)
        {
            Console.WriteLine($"Call ID: {call.Id}, Call Type: {call.Call_type}, " +
                $"Description: {call.Verbal_description ?? "N/A"}, Full Address: {call.Full_address}, " +
                $"Latitude: {call.Latitude}, Longitude: {call.Longitude}, " +
                $"Opening Time: {call.Opening_time}, Max Finish Time: {call.Max_finish_time}");
        }
        else
        {
            Console.WriteLine("Call not found.");
        }
    }

    private static void ReadAllCalls()
    {
        Console.Clear();
        var calls = s_dalCall?.ReadAll();
        if (calls != null)
        {
            foreach (var call in calls)
            {
                Console.WriteLine($"Call ID: {call.Id}, Call Type: {call.Call_type}, " +
                    $"Description: {call.Verbal_description ?? "N/A"}, Full Address: {call.Full_address}, " +
                    $"Latitude: {call.Latitude}, Longitude: {call.Longitude}, " +
                    $"Opening Time: {call.Opening_time}, Max Finish Time: {call.Max_finish_time}");
            }
        }
        else
        {
            Console.WriteLine("No calls found.");
        }
    }

    private static void UpdateCall()
    {
        Console.Clear();

        // קריאה למזהה ה-call
        Console.Write("Enter Call ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
            throw new FormatException("Call ID is invalid!");

        // קריאה לסוג ה-call
        Console.Write("Enter Call Type (transportation, car_accident, vehicle_breakdown, search_and_rescue): ");
        if (!Enum.TryParse(Console.ReadLine(), out DO.CallType call_type))
            throw new FormatException("Call Type is invalid!");

        // קריאה לשדות האחרים
        Console.Write("Enter Call Description: ");
        string verbal_description = Console.ReadLine();

        Console.Write("Enter Full Address: ");
        string full_address = Console.ReadLine();

        Console.Write("Enter Latitude: ");
        if (!double.TryParse(Console.ReadLine(), out double latitude))
            throw new FormatException("Latitude is invalid!");

        Console.Write("Enter Longitude: ");
        if (!double.TryParse(Console.ReadLine(), out double longitude))
            throw new FormatException("Longitude is invalid!");

        // קריאת ה-call לפי מזהה לעדכון
        var call = s_dalCall?.Read(id);
        if (call != null)
        {
            // יצירת אובייקט Call חדש עם ערכים חדשים אם יש, אחרת נשאיר את הערכים הקיימים
            Call newCall = new Call
            {
                Call_type = call_type != null ? call_type : call.Call_type,
                Verbal_description = string.IsNullOrEmpty(verbal_description) ? call.Verbal_description : verbal_description,
                Full_address = string.IsNullOrEmpty(full_address) ? call.Full_address : full_address,
                Latitude = latitude != 0 ? latitude : call.Latitude,
                Longitude = longitude != 0 ? longitude : call.Longitude,
            };

            s_dalCall?.Update(newCall);  // עדכון ה-call ב-DAL
            Console.WriteLine("Call updated successfully.");
        }
        else
        {
            Console.WriteLine("Call not found.");
        }
    }

    private static void DeleteCall()
    {
        Console.Clear();
        Console.Write("Enter Call ID to delete: ");
        int id = int.Parse(Console.ReadLine());
        s_dalCall?.Delete(id);
        Console.WriteLine("Call deleted successfully.");
    }

    private static void DeleteAllCalls()
    {
        s_dalCall?.DeleteAll();
        Console.WriteLine("All calls deleted successfully.");
    }

    private static void ShowVolunteerMenu()
    {
        Console.Clear();
        Console.WriteLine("Volunteer Menu:");
        Console.WriteLine("0. Back to Main Menu");
        Console.WriteLine("1. Create Volunteer");
        Console.WriteLine("2. Read Volunteer by ID");
        Console.WriteLine("3. Read All Volunteers");
        Console.WriteLine("4. Update Volunteer");
        Console.WriteLine("5. Delete Volunteer");
        Console.WriteLine("6. Delete All Volunteers");
        Console.Write("Please choose an option: ");
        int choice = GetUserChoice();

        switch (choice)
        {
            case 0:
                return;
            case 1:
                CreateVolunteer();
                break;
            case 2:
                ReadVolunteerById();
                break;
            case 3:
                ReadAllVolunteers();
                break;
            case 4:
                UpdateVolunteer();
                break;
            case 5:
                DeleteVolunteer();
                break;
            case 6:
                DeleteAllVolunteers();
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }

    private static void CreateVolunteer()
    {
        Console.Clear();
        Console.Write("Enter Volunteer ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            throw new FormatException("ID is invalid!");
        }

        // קריאה למספר טלפון נייד עם בדיקת תקינות
        Console.Write("Enter Cellphone Number: ");
        string cellphoneNumber = Console.ReadLine();
        if (string.IsNullOrEmpty(cellphoneNumber) || cellphoneNumber.Length < 10)
        {
            throw new FormatException("Cellphone number is invalid!");
        }

        // קריאה לשם מלא
        Console.Write("Enter Full Name: ");
        string fullName = Console.ReadLine();
        if (string.IsNullOrEmpty(fullName))
        {
            throw new FormatException("Full Name is required!");
        }

        // קריאה לכתובת דואר אלקטרוני
        Console.Write("Enter Email: ");
        string email = Console.ReadLine();
        if (!email.Contains("@"))
        {
            throw new FormatException("Email is invalid!");
        }
        // קריאה לכתובת (אפשר להשאיר ריק)
        Console.Write("Enter Full Address (optional): ");
        string? fullAddress = Console.ReadLine();

        // קריאה לקואורדינטות (Latitude ו-Longitude) עם TryParse
        Console.Write("Enter Latitude (optional): ");
        double? latitude = null;
        string latitudeInput = Console.ReadLine()!;
        if (!double.TryParse(latitudeInput, out double parsedLatitude))
        {
            throw new FormatException("Latitude is invalid!");
        }
        latitude = parsedLatitude;

        Console.Write("Enter Longitude (optional): ");
        double? longitude = null;
        string longitudeInput = Console.ReadLine()!;
        if (!double.TryParse(longitudeInput, out double parsedLongitude))
        {
            throw new FormatException("Longitude is invalid!");
        }
        longitude = parsedLongitude;

        // קריאה לתפקיד (Role) עם בדיקת תקינות
        Console.Write("Enter Role (manager/Volunteer): ");
        if (!Enum.TryParse(Console.ReadLine(), out Role role))
        {
            throw new FormatException("Role is invalid!");
        }

        // קריאה למצב (IsActive)
        Console.Write("Is Active (true/false): ");
        if (!bool.TryParse(Console.ReadLine(), out bool isActive))
        {
            throw new FormatException("IsActive is invalid!");
        }

        // קריאה לסוג מרחק (DistanceTypes) עם בדיקת תקינות
        Console.Write("Enter Distance Type (aerial_distance,walking_distance,  driving_distance): ");
        if (!Enum.TryParse(Console.ReadLine(), out DistanceTypes distanceType))
        {
            throw new FormatException("DistanceType is invalid!");
        }

        // קריאה למרחק מקסימלי (MaxDistance)
        Console.Write("Enter Max Distance (optional): ");
        double? maxDistance = null;
        string maxDistanceInput = Console.ReadLine()!;
        if (!double.TryParse(maxDistanceInput, out double parsedMaxDistance))
        {
            throw new FormatException("Max Distance is invalid!");
        }
        maxDistance = parsedMaxDistance;

        // קריאה לסיסמא (Password) (ניתן להשאיר ריק או להזין)
        Console.Write("Enter Password (optional): ");
        string? password = Console.ReadLine();
        Volunteer newVolunteer;
        // יצירת המתנדב החדש
        if (password == null)
        {
            newVolunteer = new Volunteer
            {
                Id = id,
                FullName = fullName,
                CellphoneNumber = cellphoneNumber,
                Email = email,
                FullAddress = fullAddress,
                Latitude = latitude,
                Longitude = longitude,
                Role = role,
                IsActive = isActive,
                DistanceTypes = distanceType,
                MaxDistance = maxDistance,
            };
        }
        else
        {
            newVolunteer = new Volunteer
            {
                Id = id,
                FullName = fullName,
                CellphoneNumber = cellphoneNumber,
                Email = email,
                FullAddress = fullAddress,
                Latitude = latitude,
                Longitude = longitude,
                Role = role,
                IsActive = isActive,
                DistanceTypes = distanceType,
                MaxDistance = maxDistance,
                Password = password,
            };

        }

        s_dalVolunteer?.Create(newVolunteer);
        Console.WriteLine("Volunteer created successfully.");
    }

    private static void ReadVolunteerById()
    {
        Console.Clear();
        Console.Write("Enter Volunteer ID to read: ");
        int id = int.Parse(Console.ReadLine()!);
        var volunteer = s_dalVolunteer?.Read(id);
        if (volunteer != null)
        {
            Console.WriteLine($"Volunteer ID: {volunteer.Id}, Full Name: {volunteer.FullName}, Cellphone: {volunteer.CellphoneNumber}, Email: {volunteer.Email}, " +
                $"Full Address: {volunteer.FullAddress}, Latitude: {volunteer.Latitude}, Longitude: {volunteer.Longitude}, " +
                $"Role: {volunteer.Role}, Active: {volunteer.IsActive}, Distance Type: {volunteer.DistanceTypes}, " +
                $"Max Distance: {volunteer.MaxDistance}, Password: {volunteer.Password}");
        }
        else
        {
            Console.WriteLine("Volunteer not found.");
        }
    }

    private static void ReadAllVolunteers()
    {
        Console.Clear();
        var volunteers = s_dalVolunteer?.ReadAll();
        if (volunteers != null)
        {
            int i = 1;
            foreach (var volunteer in volunteers)
            {
                Console.WriteLine($"volunteer{i++}");
                Console.WriteLine($"Volunteer ID: {volunteer.Id}, Full Name: {volunteer.FullName}, Cellphone: {volunteer.CellphoneNumber}, Email: {volunteer.Email}, " +
               $"Full Address: {volunteer.FullAddress}, Latitude: {volunteer.Latitude}, Longitude: {volunteer.Longitude}, " +
               $"Role: {volunteer.Role}, Active: {volunteer.IsActive}, Distance Type: {volunteer.DistanceTypes}, " +
               $"Max Distance: {volunteer.MaxDistance}, Password: {volunteer.Password}");
            }
        }
        else
        {
            Console.WriteLine("No volunteers found.");
        }
    }

    private static void UpdateVolunteer()
    {
        Console.Clear();
        Console.Write("Enter Volunteer ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
            throw new FormatException("Volunteer ID is invalid!");

        Console.Write("Enter Volunteer Type distanceTypes(aerial_distance,walking_distance,driving_distance) to update: ");
        if (!Enum.TryParse(Console.ReadLine(), out DO.DistanceTypes distanceTypes))
            throw new FormatException("Volunteer Type is invalid!");

        Console.Write("Enter Volunteer Type Role(manager/Volunteer) to update: ");
        if (!Enum.TryParse(Console.ReadLine(), out DO.Role role))
            throw new FormatException("Volunteer Type is invalid!");

        Console.Write("Enter Volunteer Full name to update: ");
        string fullName = Console.ReadLine()!;

        Console.Write("Enter Cellphone number to update: ");
        string cellphoneNumber = Console.ReadLine()!;

        Console.Write("Enter Volunteer Email to update: ");
        string email = Console.ReadLine()!;

        Console.Write("Enter Volunteer FullAddress to update: ");
        string fullAddress = Console.ReadLine()!;

        Console.Write("Enter Volunteer Password to update: ");
        string password = Console.ReadLine()!;


        Console.Write("Enter Latitude to update: ");
        if (!double.TryParse(Console.ReadLine(), out double latitude))
            throw new FormatException("Latitude is invalid!");

        Console.Write("Enter Longitude to update: ");
        if (!double.TryParse(Console.ReadLine(), out double longitude))
            throw new FormatException("Longitude is invalid!");

        Console.Write("Enter Max distance to update: ");
        if (!double.TryParse(Console.ReadLine(), out double maxDistance))
            throw new FormatException("Max distance is invalid!");

        Console.Write("Enter Active to update: ");
        if (!bool.TryParse(Console.ReadLine(), out bool isActive))
        {
            throw new FormatException("IsActive is invalid!");
        }


        var volunteer = s_dalVolunteer?.Read(id);
        if (volunteer != null)
        {
            Volunteer newVolunteer = new Volunteer
            {
                Id = id == null ? id : volunteer.Id,
                FullName = string.IsNullOrEmpty(fullName) ? volunteer.FullName : fullName,
                CellphoneNumber = string.IsNullOrEmpty(cellphoneNumber) ? volunteer.CellphoneNumber : cellphoneNumber,
                Email = string.IsNullOrEmpty(email) ? volunteer.Email : email,
                FullAddress = string.IsNullOrEmpty(fullAddress) ? volunteer.FullAddress : fullAddress,
                Latitude = latitude != 0 ? latitude : volunteer.Latitude,
                Longitude = longitude != 0 ? longitude : volunteer.Longitude,
                Role = role == null ? volunteer.Role : role,
                IsActive = isActive ? volunteer.IsActive : isActive,
                DistanceTypes = distanceTypes == null ? volunteer.DistanceTypes : distanceTypes,
                MaxDistance = maxDistance != 0 ? maxDistance : volunteer.MaxDistance,
                Password = string.IsNullOrEmpty(password) ? volunteer.Password : password,
            };

            s_dalVolunteer?.Update(newVolunteer);
            Console.WriteLine("Volunteer updated successfully.");
        }
        else
        {
            Console.WriteLine("Volunteer not found.");
        }
    }

    private static void DeleteVolunteer()
    {
        Console.Clear();
        Console.Write("Enter Volunteer ID to delete: ");
        int id = int.Parse(Console.ReadLine());
        s_dalVolunteer?.Delete(id);
        Console.WriteLine("Volunteer deleted successfully.");
    }

    private static void DeleteAllVolunteers()
    {
        s_dalVolunteer?.DeleteAll();
        Console.WriteLine("All volunteers deleted successfully.");
    }

    private static void ShowAssignmentMenu()
    {
        Console.Clear();
        Console.WriteLine("Assignment Menu:");
        Console.WriteLine("0. Back to Main Menu");
        Console.WriteLine("1. Create Assignment");
        Console.WriteLine("2. Read Assignment by ID");
        Console.WriteLine("3. Read All Assignments");
        Console.WriteLine("4. Update Assignment");
        Console.WriteLine("5. Delete Assignment");
        Console.WriteLine("6. Delete All Assignments");
        Console.Write("Please choose an option: ");
        int choice = GetUserChoice();

        switch (choice)
        {
            case 0:
                return;
            case 1:
                CreateAssignment();
                break;
            case 2:
                ReadAssignmentById();
                break;
            case 3:
                ReadAllAssignments();
                break;
            case 4:
                UpdateAssignment();
                break;
            case 5:
                DeleteAssignment();
                break;
            case 6:
                DeleteAllAssignments();
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }

    private static void ShowConfigMenu()
    {
        Console.Clear();
        Console.WriteLine("Configuration Menu:");
        Console.WriteLine("0. Back to Main Menu");
        Console.WriteLine("1. Advance system clock by one minute");
        Console.WriteLine("2. Advance system clock by one hour");
        Console.WriteLine("3. Advance system clock by one month");
        Console.WriteLine("4. Advance system clock by one year");
        Console.WriteLine("5. Show current system clock value");
        Console.WriteLine("6. Set a new value for any configuration variable");
        Console.WriteLine("7. Show current value for any configuration variable");
        Console.WriteLine("8. Reset values ​​for all configuration variables");
        Console.Write("Please choose an option: ");
        int choice = GetUserChoice();
        switch (choice)
        {
            case 0:
                return;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                ResetConfig();
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }

    //private static void ShowCurrentConfig()
    //{
    //    Console.Clear();
    //    Console.WriteLine($"Current Clock: {s_dalConfig?.Clock}");
    //}

    private static void ResetConfig()
    {
        Console.Clear();
        s_dalConfig?.Reset();
        Console.WriteLine("Configuration has been reset.");
    }
}