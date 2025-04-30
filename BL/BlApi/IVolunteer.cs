using BO;

namespace BlApi
{
    public interface IVolunteer: IObservable //stage 5 הרחבת ממשק
    {
        /// <summary>
        /// Interface for Volunteer-related functionalities, such as managing volunteers and accessing their details.
        /// </summary>
        BO.Enums.Role EnterSystem(string name, string pass);
        IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.Enums.VolunteerInListFields? fieldFilter = null);
        BO.Volunteer GetVolunteerDetails(int id);
        void UpdateVolunteerDetails(int id, BO.Volunteer volunteer);
        void DeleteVolunteer(int id);
        void AddVolunteer(BO.Volunteer volunteer);
        IEnumerable<BO.VolunteerInList> GetVolunteersFilterList ( Enums.CallType? callType);
    }
}
