namespace DalTest;

using Dal;
using DalApi;
using DO;
using System.Security.Cryptography.X509Certificates;
/// <summary>
/// A class that initializes the DB lists with some data for the test class.
/// </summary>
public static class Initialization
{
    //private static ICall? s_dalCall; //stage 1
    //private static IAssignment? s_dalAssignment; //stage 1
    //private static IVolunteer? s_dalVolunteer; //stage 1
    //private static IConfig? s_dalConfig; //stage 1
    private static IDal? s_dal; //stage 2
    private static readonly Random s_rand = new();
    /// <summary>
    /// To fill up the calls list in the DataSource with calls.
    /// </summary>
    private static void CreateCalls()
    {
        CallType[] callsTypes = {
            CallType.SearchAndRescue,
            CallType.Transportation,
            CallType.CarAccident,
            CallType.VehicleBreakdown,
            CallType.VehicleBreakdown,
            CallType.SearchAndRescue,
            CallType.Transportation,
            CallType.CarAccident,
            CallType.VehicleBreakdown,
            CallType.VehicleBreakdown,
            CallType.SearchAndRescue,
            CallType.Transportation,
            CallType.CarAccident,
            CallType.VehicleBreakdown,
            CallType.VehicleBreakdown,
            CallType.SearchAndRescue,
            CallType.Transportation,
            CallType.CarAccident,
            CallType.VehicleBreakdown,
            CallType.VehicleBreakdown,
            CallType.SearchAndRescue,
            CallType.Transportation,
            CallType.CarAccident,
            CallType.VehicleBreakdown,
            CallType.VehicleBreakdown,
            CallType.SearchAndRescue,
            CallType.Transportation,
            CallType.CarAccident,
            CallType.VehicleBreakdown,
            CallType.VehicleBreakdown,
            CallType.SearchAndRescue,
            CallType.Transportation,
            CallType.CarAccident,
            CallType.VehicleBreakdown,
            CallType.VehicleBreakdown,
            CallType.SearchAndRescue,
            CallType.Transportation,
            CallType.CarAccident,
            CallType.VehicleBreakdown,
            CallType.VehicleBreakdown,
            CallType.SearchAndRescue,
            CallType.Transportation,
            CallType.CarAccident,
            CallType.VehicleBreakdown,
            CallType.VehicleBreakdown,
            CallType.SearchAndRescue,
            CallType.Transportation,
            CallType.CarAccident,
            CallType.VehicleBreakdown
        };
        string[] verbalDescriptions = {
            "A child is lost in a park and needs immediate rescue.",
            "An elderly person needs to be driven to a hospital for an urgent check-up.",
            "Two cars collided on a highway, blocking the road.",
            "A car broke down on the highway due to a flat tire.",
            "A vehicle's door is locked, and the driver is stranded on the highway.",
            "A hiker got lost in the forest and requires search and rescue.",
            "A patient needs to be transported to the hospital for surgery.",
            "Two cars are involved in a crash on a busy intersection.",
            "A car has overheated and stopped on the side of the highway.",
            "A vehicle has a mechanical issue, leaving the driver stuck on the road.",
            "A climber fell and requires immediate medical evacuation.",
            "A van is transporting an injured person to the hospital.",
            "A collision between two trucks caused traffic chaos on the freeway.",
            "A car broke down because of engine failure on the highway.",
            "A car’s battery is dead, and it’s blocking traffic on a busy road.",
            "A rescue team is needed to help a stranded motorist on a remote road.",
            "An urgent transfer of a patient from a clinic to the hospital is required.",
            "A serious accident involving three vehicles on a main road needs attention.",
            "A vehicle is stranded on the highway after a tire exploded.",
            "A car's electrical system failed, leaving it stuck in the middle of the road.",
            "A family’s car broke down while traveling and needs roadside assistance.",
            "A person is trapped in a wrecked car and needs rescue services.",
            "A car was involved in a pile-up on the freeway and needs to be towed.",
            "A car's engine broke down on a remote highway and is causing a traffic jam.",
            "A truck's cargo shifted, blocking multiple lanes on the highway.",
            "A mother and child need to be driven to a hospital due to an emergency.",
            "A bicycle rider was hit by a car and needs urgent transportation to the ER.",
            "A car broke down in the middle of a busy intersection.",
            "A vehicle is stuck in a ditch after a heavy rainstorm.",
            "An elderly person is lost in a public building and needs assistance.",
            "A car’s engine overheated while traveling through the mountains.",
            "An accident occurred at a railway crossing, blocking the intersection.",
            "A person fell and injured themselves, requiring immediate medical help.",
            "A vehicle with mechanical problems is blocking the entrance to a tunnel.",
            "A car broke down during a long road trip, leaving the driver stranded.",
            "A group of tourists got lost in the wilderness and needs a rescue operation.",
            "A truck carrying hazardous materials crashed on the highway.",
            "A broken-down car is causing a major traffic jam on a busy street.",
            "An ambulance needs to transport a patient with critical injuries.",
            "A car was involved in a hit-and-run accident and needs to be towed.",
            "A stranded truck requires roadside assistance on a remote mountain road.",
            "A vehicle is stuck on the side of a cliff and needs emergency help.",
            "A police car was involved in a collision and requires immediate assistance.",
            "A driver needs to be rescued from a flooded area during a storm.",
            "A person requires immediate transport to a specialized hospital for treatment.",
            "A vehicle’s brakes failed, leading to an accident on a downhill road.",
            "A passenger’s health deteriorated during a trip and needs urgent medical attention.",
            "A vehicle is stuck in deep snow and needs a rescue operation.",
            "A motorcyclist is injured and requires immediate medical transport.",
            "A vehicle was involved in a major crash and is blocking multiple lanes"
        };
        string[] addressesInIsrael = {
            "1 Jaffa Street, Jerusalem",
            "5 Herzl Boulevard, Tel Aviv",
            "12 HaMeginim Street, Haifa",
            "22 Beit David Street, Petah Tikva",
            "13 Ben-Gurion Boulevard, Ashdod",
            "34 King George Street, Jerusalem",
            "10 Herzl Street, Rishon Lezion",
            "4 Moshe Dayan Boulevard, Rehovot",
            "27 Bethlehem Road, Jerusalem",
            "45 David Remez Street, Kfar Saba",
            "33 Jaffa Street, Nazareth",
            "2 Mea Shearim Street, Jerusalem",
            "16 Shmuel Hanavi Street, Haifa",
            "29 Arlozorov Street, Tel Aviv",
            "8 Chaim Weizmann Boulevard, Beersheba",
            "20 Yosef Maimon Street, Acre",
            "56 Allenby Street, Tel Aviv",
            "18 King George Street, Haifa",
            "9 Golomb Street, Eilat",
            "13 Jerusalem Boulevard, Ashkelon",
            "31 Herzog Street, Netanya",
            "17 Gaza Street, Ashdod",
            "11 Revolution Boulevard, Petah Tikva",
            "50 Keren Hayesod Street, Tiberias",
            "5 Ha'arba'a Street, Tel Aviv",
            "34 Chaim Ozer Street, Herzliya",
            "23 Achuza Street, Herzliya",
            "15 Duani Boulevard, Ramat Gan",
            "41 Nordau Street, Bnei Brak",
            "55 Herzl Boulevard, Holon",
            "30 Salah A-Din Street, Nazareth",
            "14 Hebron Road, Jerusalem",
            "7 Ben-Zvi Boulevard, Bat Yam",
            "6 Aba Hushi Street, Netanya",
            "19 HaSolalim Street, Kiryat Shmona",
            "21 Sheinkin Street, Tel Aviv",
            "8 Shivtei Israel Street, Haifa",
            "10 Knesset Street, Jerusalem",
            "12 Yehuda Halevi Street, Ramat Gan",
            "4 Eshkol Boulevard, Ashdod",
            "8 Shmuel Hanavi Street, Tel Aviv",
            "23 Melche Israel Street, Petah Tikva",
            "33 Aben-Gurion Street, Rehovot",
            "4 French Hill Street, Jerusalem",
            "10 IDF Street, Ashkelon",
            "2 Yitzhak Navon Street, Rishon Lezion",
            "19 King David Street, Jerusalem",
            "8 Tel-High Street, Netanya",
            "27 Golomb Street, Haifa",
            "19 Moshe Dayan Boulevard, Herzliya",
            "11 HaTsoorafim Street, Beersheba",
            "28 Daniel Street, Ramat Hasharon",
            "3 Gan Ha'ir Street, Tel Aviv",
            "10 HaRakavet Street, Hadera",
            "25 Yirmiyahu Street, Ashkelon",
            "5 Hana Senesh Street, Givat Shmuel",
            "18 Yechiel Street, Beersheba"
        };
        double[] callsLatitudes = {
            31.7683, 32.0853, 32.8170, 32.0730, 33.1303, 31.7683, 31.9632, 31.8970, 31.7612, 32.0924,
            32.7116, 31.7656, 32.7992, 32.0782, 31.2583, 32.9257, 32.0648, 32.1093, 29.5623, 31.8479,
            32.3217, 31.7630, 32.0620, 32.1125, 32.0541, 32.0870, 32.2244, 32.1097, 31.7944, 32.1092,
            32.0594, 32.0605, 32.0975, 32.2537, 31.7712, 32.1098, 31.7680, 31.9535, 31.7741, 32.0767,
            31.9533, 32.2207, 32.0609, 31.9106, 31.7364, 31.9156, 32.0052, 31.8521, 31.7945, 32.1570
        };
        double[] callsLongitudes = {
            35.2137, 34.7818, 35.0365, 34.8707, 34.7671, 35.2137, 34.8022, 34.8110, 35.2271, 34.7957,
            35.2793, 35.2132, 34.9837, 34.7684, 34.8114, 35.0720, 34.7883, 34.7821, 34.9519, 34.6574,
            34.7432, 34.7755, 34.8150, 34.8052, 34.8872, 34.8883, 34.8725, 34.7934, 34.7522, 34.8090,
            34.8017, 34.7875, 34.7784, 34.7854, 34.7989, 34.8371, 34.8915, 34.7785, 34.8124, 34.7972,
            34.9076, 34.7915, 34.7761, 34.8527, 34.8183, 34.7928, 34.8034, 34.7569, 34.9075, 34.8063
        };
        DateTime systemTime = s_dal!.Config.Clock;
        DateTime callOpeningTime;
        DateTime? callMaxFinishTime;
        Random rand = new Random();
        int i = 0;
        foreach (CallType c_type in callsTypes)
        {
            int randomNegativeSeconds = rand.Next(1, 1000); // הגרלת מספר רנדומלי בין 1 ל-1000
            callOpeningTime = systemTime.AddMinutes(-randomNegativeSeconds); // הפחתה מהזמן הראשי
            int randomPositiveNumber = rand.Next(1, 100000); // הגרלת מספר רנדומלי בין 1 ל-1000
            callMaxFinishTime = (rand.NextDouble() > 0.5) ? callOpeningTime.AddMinutes(randomPositiveNumber) : null;
            s_dal!.Call.Create(new Call(c_type, verbalDescriptions[i], addressesInIsrael[i], callsLatitudes[i], callsLongitudes[i++], callOpeningTime, callMaxFinishTime));
        }

    }
    /// <summary>
    /// To fill up the assignment list in the DataSource with assignments.
    /// </summary>
    private static void CreateAssignments()
    {
        Random rand = new Random();
        var calls = s_dal!.Call.ReadAll().ToList();
        var volunteers = s_dal!.Volunteer.ReadAll().ToList();
        var callsWithAssignment = calls.Skip((int)(calls.Count * 0.2)).ToList();//some of the calls will not be assigned.
        foreach (Call? call in callsWithAssignment)
        {
            Volunteer randomVolunteer;
            if (volunteers.Count > 0)
            {
                randomVolunteer = volunteers[rand.Next(volunteers.Count)]!;
                if (randomVolunteer == null || randomVolunteer.Id == 0)
                    throw new Exception("Selected volunteer is invalid or has ID 0.");
            }
            else
            {
                throw new Exception("No volunteers available.");
            }
            TimeSpan duration;
            if (call!.MaxFinishTime.HasValue)
                duration = call.MaxFinishTime.Value - call.OpeningTime;
            else
                duration = TimeSpan.Zero;

            double randomMinutes = rand.NextDouble() * duration.TotalMinutes;
            DateTime randomStartTime = call.OpeningTime.AddMinutes(randomMinutes);

            DateTime? randomEndTime;

            if (!call.MaxFinishTime.HasValue)
            {
                // MaxFinishTime == null → 50% סיכוי לקבל זמן רנדומלי לפני Config.Clock
                if (rand.NextDouble() < 0.5)
                {
                    double maxMinutes = (s_dal.Config.Clock - randomStartTime).TotalMinutes;
                    if (maxMinutes > 0)
                    {
                        double endMinutes = rand.NextDouble() * maxMinutes;
                        randomEndTime = randomStartTime.AddMinutes(endMinutes);
                    }
                    else
                    {
                        randomEndTime = randomStartTime; // fallback
                    }
                }
                else
                {
                    randomEndTime = null;
                }
            }
            else if (call.MaxFinishTime.Value < s_dal.Config.Clock)
            {
                // תוקף פג → קבע את הזמן הנוכחי
                randomEndTime = s_dal.Config.Clock;
            }
            else // MaxFinishTime > Config.Clock
            {
                // 50% מהפעמים זמן רנדומלי לפני Config.Clock ו־MaxFinishTime, אחרת null
                if (rand.NextDouble() < 0.5)
                {
                    DateTime maxAllowed = call.MaxFinishTime.Value < s_dal.Config.Clock
                        ? call.MaxFinishTime.Value
                        : s_dal.Config.Clock;

                    double maxMinutes = (maxAllowed - randomStartTime).TotalMinutes;
                    if (maxMinutes > 0)
                    {
                        double endMinutes = rand.NextDouble() * maxMinutes;
                        randomEndTime = randomStartTime.AddMinutes(endMinutes);
                    }
                    else
                    {
                        randomEndTime = randomStartTime; // fallback
                    }
                }
                else
                {
                    randomEndTime = null;
                }
            }

            EndType? endType = null;
            if (randomEndTime == null)
                endType = null;
            else if (call.MaxFinishTime.HasValue && call.MaxFinishTime < s_dal.Config.Clock)
            {
                endType = EndType.Expired;
            }
            else
            {
                double randomPercentage = rand.NextDouble();
                if (randomPercentage < 0.33)
                    endType = EndType.WasTreated;
                else if (randomPercentage < 0.66)
                    endType = EndType.SelfCancellation;
                else
                    endType = EndType.ManagerCancellation;
            }
            s_dal!.Assignment.Create(new Assignment
            {
                CallId = call.Id,
                VolunteerId = randomVolunteer.Id,
                StartTime = randomStartTime,
                EndTime = randomEndTime,
                EndType = endType
            });
        }
    }
    /// <summary>
    /// To fill up the volunteer list in the DataSource with volunteers.
    /// </summary>
    private static void CreateVolunteers()
    {
        Random rand = new Random();

        string[] fullNames = {
            "Yitzhak Cohen", "Menachem Levy", "Shmuel Goldstein", "David Ben-David", "Yosef Shapiro",
            "Aryeh Rosen", "Chaim Mizrahi", "Shlomo Klein", "Avraham Amsalem", "Yoel Peretz",
            "Eliahu Carmel", "Moshe Shtern", "Yigal Binyamini", "Reuven Bar", "Daniel Shmuel",
            "Binyamin Levi", "Yaakov Avrahami", "Nataniel Ben Shimon", "Efraim Shani", "Shimon Tzukrel"
        };
        string[] addresses = {
            "12 Keren Hayesod Street, Tel Aviv", "3 Allenby Street, Haifa", "45 Yehuda Halevi Street, Jerusalem", "22 Jaffa Street, Netanya", "8 Shlomo Ben Yosef Street, Rishon Lezion",
            "5 Bograshov Street, Tel Aviv", "77 Herzl Boulevard, Holon", "12 Moshe Dayan Boulevard, Rehovot", "18 Shmuel Hanavi Street, Haifa", "14 Be'er Sheva Boulevard, Ashdod",
            "34 Neve Shaanan Street, Beer Sheva", "7 David Remez Street, Petah Tikva", "5 Ben Gurion Boulevard, Tel Aviv", "22 Kfar Shalem Street, Jaffa", "3 Ze'ev Jabotinsky Street, Holon",
            "9 Weizmann Street, Tel Aviv", "16 David Ben Gurion Avenue, Haifa", "11 Yigal Alon Street, Herzliya", "24 Ibn Gabirol Street, Tel Aviv", "14 King George Street, Jerusalem"
        };
        double[] latitudes = { 32.0777, 32.8170, 31.7683, 32.3210, 31.9463, 32.0840, 32.0496, 31.8991, 32.7992, 31.8479,
            31.2383, 32.1093, 32.0853, 32.0700, 32.1497, 31.8780, 32.0853, 32.0614, 32.0911, 31.7683 };
        double[] longitudes = { 34.7846, 34.9862, 35.2137, 34.8530, 34.7815, 34.7671, 34.8123, 34.8110, 35.2271, 34.7812,
            34.7707, 34.8722, 34.7818, 34.7665, 34.7881, 34.7933, 34.7660, 34.8075, 34.7833, 35.2137 };
        //create 19 volunteers.
        int i = 0;
        foreach (string name in fullNames)
        {
            s_dal!.Volunteer.Create(new Volunteer() with
            {
                Id = GenerateValidIsraeliId(rand),
                FullName = name,
                CellphoneNumber = $"0{rand.Next(100000000, 999999999)}",
                Email = name.Replace(" ", "").ToLower() + "@gmail.com",
                FullAddress = addresses[i],
                Latitude = latitudes[i],
                Longitude = latitudes[i++],
                Role = Role.Volunteer,
                IsActive = true,
                DistanceTypes = (DistanceTypes)rand.Next(Enum.GetValues(typeof(DistanceTypes)).Length),
                MaxDistance = rand.Next(0, 100000)
            });
        }
        //create 1 manager.
        s_dal!.Volunteer.Create(new Volunteer() with
        {
            Id = GenerateValidIsraeliId(rand),
            FullName = fullNames[19],
            CellphoneNumber = $"0{rand.Next(100000000, 999999999)}",
            Email = fullNames[19].Replace(" ", "").ToLower() + "@gmail.com",
            FullAddress = addresses[19],
            Latitude = latitudes[19],
            Longitude = latitudes[19],
            Role = Role.Manager,
            IsActive = true,
            DistanceTypes = (DistanceTypes)rand.Next(Enum.GetValues(typeof(DistanceTypes)).Length),
            MaxDistance = rand.Next(0, 100000)
        });
    }
    //Generate random valid ID numbers.
    private static int GenerateValidIsraeliId(Random rand)
    {
        int idWithoutCheckDigit = rand.Next(10000000, 99999999); // מספר בן 8 ספרות בלבד
        int checkDigit = CalculateIsraeliIdCheckDigit(idWithoutCheckDigit);
        int finalId = unchecked(idWithoutCheckDigit * 10 + checkDigit); // לוודא שהתוצאה לא חורגת

        return finalId >= 0 ? finalId : -finalId; // לוודא שהתוצאה חיובית
    }
    private static int CalculateIsraeliIdCheckDigit(int idWithoutCheckDigit)
    {
        int sum = 0;
        for (int i = 0; i < 8; i++)
        {
            int digit = (idWithoutCheckDigit / (int)Math.Pow(10, 7 - i)) % 10;
            int weighted = digit * (i % 2 == 0 ? 1 : 2);
            sum += (weighted > 9) ? weighted - 9 : weighted;
        }
        return (10 - (sum % 10)) % 10;
    }
    /// <summary>
    /// A function that calls all the functions of initializing the DataSource and is called in the main function in the test program for to actually initialize the 'BD'.
    /// </summary>
    /// <param name="dalCall"> an ICall object which holds all the CRUD functions and is used for initialize the calls list.</param>
    /// <param name="dalAssignment">an IAssignment object which holds all the CRUD functions and is used for initialize the assignments list.</param>
    /// <param name="dalVolunteer">an IVolunteer object which holds all the CRUD functions and is used for initialize the volunteers list.</param>
    /// <param name="dalConfig">an IConfig object which is used for reset all the system's variables</param>
    /// <exception cref="NullReferenceException"></exception>
    public static void Do() //stage 4
    {
        //s_dalCall = dalCall ?? throw new NullReferenceException("DAL can not be null!");
        //s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL can not be null!");
        //s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL can not be null!");
        //s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL can not be null!");
        //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); stage 2
        s_dal = Factory.Get;
        Console.WriteLine("Reset Configuration values and List values...");
        //s_dalConfig.Reset(); //stage 1
        //s_dalCall.DeleteAll(); //stage 1
        //Console.WriteLine("Reset Configuration values and List values...");
        //s_dalConfig.Reset(); //stage 1
        //s_dalAssignment.DeleteAll(); //stage 
        //Console.WriteLine("Reset Configuration values and List values...");
        //s_dalConfig.Reset(); //stage 1
        //s_dalVolunteer.DeleteAll(); //stage 1
        s_dal.ResetDB();//stage 2
        Console.WriteLine("Initializing Calls list ...");
        CreateCalls();
        Console.WriteLine("Initializing Volunteers list ...");
        CreateVolunteers();
        Console.WriteLine("Initializing Assignments list ...");
        CreateAssignments();

    }
}