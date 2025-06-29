using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;

namespace BlImplementation;
// <summary>
// Implements IVolunteer interface for managing volunteers.
// Includes login, retrieval, updating, and deletion of volunteer data.
// Handles interactions with DAL and throws custom exceptions for errors.
// </summary>

internal class VolunteerImplementation : BlApi.IVolunteer
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    /// <summary>
    /// Allows a volunteer to enter the system by verifying their username and password.
    /// </summary>
    /// <param name="name">The volunteer's full name (username).</param>
    /// <param name="pass">The volunteer's password.</param>
    /// <returns>The role of the volunteer.</returns>
    /// <exception cref="BO.BlUnauthorizedException">Thrown if the username or password is incorrect.</exception>
    public BO.Enums.Role EnterSystem(string name, string pass)
    {
        try
        {
            DO.Volunteer? volunteer;
            lock (AdminManager.BlMutex) //stage 7
                volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v!.FullName == name && VolunteerManager.VerifyPassword(pass, v.Password!));
            return volunteer is null ? throw new BO.BlUnauthorizedException("Invalid username or password") : (BO.Enums.Role)volunteer.Role;

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Error accessing volunteers.", ex);
        }
        catch (BO.BlUnauthorizedException ex)
        {
            throw new BO.BlUnauthorizedException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("An unexpected error occurred.", ex);
        }
    }
    public BO.Enums.Role Login(int id, string pass)//stage 6
    {
        try
        {
            DO.Volunteer? volunteer;
            lock (AdminManager.BlMutex) //stage 7
                volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v!.Id == id && VolunteerManager.VerifyPassword(pass, v.Password!));
            return volunteer is null ? throw new BO.BlUnauthorizedException("Invalid ID or password") : (BO.Enums.Role)volunteer.Role;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Error accessing volunteers.", ex);
        }
        catch (BO.BlUnauthorizedException ex)
        {
            throw new BO.BlUnauthorizedException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("An unexpected error occurred.", ex);
        }
    }
    /// <summary>
    /// Retrieves a list of volunteers with optional filters for activity status and sorting.
    /// </summary>
    /// <param name="isActiveFilter">Optional filter for the volunteer's active status.</param>
    /// <param name="fieldSort">Optional field to sort the volunteers by.</param>
    /// <returns>A sorted list of volunteers matching the filter criteria.</returns>
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActiveFilter = null, BO.Enums.VolunteerInListFields? fieldSort = null)
    {
        try
        {
            List<DO.Volunteer?> volunteers;
            lock (AdminManager.BlMutex) //stage 7
                volunteers = _dal.Volunteer.ReadAll().ToList();
            if (isActiveFilter is not null)
                volunteers = volunteers.Where(v => v?.IsActive == isActiveFilter.Value).ToList();
            var allVolunteersInList = VolunteerManager.GetVolunteerList(volunteers!);
            var sortedVolunteers = fieldSort is not null ? fieldSort switch
            {
                BO.Enums.VolunteerInListFields.FullName => allVolunteersInList.OrderBy(v => v?.FullName).ToList(),
                BO.Enums.VolunteerInListFields.TotalHandledCalls => allVolunteersInList.OrderBy(v => v?.TotalHandledCalls).ToList(),
                BO.Enums.VolunteerInListFields.TotalCanceledCalls => allVolunteersInList.OrderBy(v => v?.TotalCanceledCalls).ToList(),
                BO.Enums.VolunteerInListFields.TotalExpiredCalls => allVolunteersInList.OrderBy(v => v?.TotalExpiredCalls).ToList(),
                BO.Enums.VolunteerInListFields.CallId => allVolunteersInList.OrderBy(v => v?.TotalExpiredCalls).ToList(),
                BO.Enums.VolunteerInListFields.CallType => allVolunteersInList.OrderBy(v => v?.CallType).ToList(),
                _ => allVolunteersInList.OrderBy(v => v?.Id).ToList(),
            } : allVolunteersInList.OrderBy(v => v?.Id).ToList();
            return sortedVolunteers;


        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Error accessing Volunteers.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("An unexpected error occurred.", ex);
        }
    }
    /// <summary>
    /// Retrieves the details of a specific volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to retrieve.</param>
    /// <returns>A BO.Volunteer object containing the volunteer's details.</returns>
    public BO.Volunteer GetVolunteerDetails(int id)
    {
        try
        {
            DO.Volunteer? doVolunteer;
            lock (AdminManager.BlMutex) //stage 7
                doVolunteer = _dal.Volunteer.Read(id);
            if (doVolunteer is not null)
                return Helpers.VolunteerManager.ConvertDoVolunteerToBoVolunteer(doVolunteer);
            else
                return null!;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Error accessing Volunteers.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("An unexpected error occurred.", ex);
        }
    }
    /// <summary>
    /// Updates the details of a specific volunteer.
    /// </summary>
    /// <param name="requesterId">The ID of the volunteer requesting the update.</param>
    /// <param name="boVolunteer">The volunteer object with updated details.</param>
    /// <exception cref="BO.BlUnauthorizedException">Thrown if the requester is not authorized to update the volunteer's details.</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
    /// <exception cref="BO.BlInvalidFormatException">Thrown if the updated details are not in the correct format.</exception>
    public void UpdateVolunteerDetails(int requesterId, BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        try
        {
            DO.Volunteer? requester;
            DO.Volunteer? doVolunteer;
            lock (AdminManager.BlMutex) //stage 7
                requester = _dal.Volunteer.Read(requesterId);
            if (requester is null)
                throw new BO.BlDoesNotExistException($"Volunteer with ID {requesterId} does  not exist! ");
            if (requester.Id != boVolunteer.Id && requester.Role != DO.Role.Manager)
                throw new BO.BlUnauthorizedException("Requester is not authorized!");
            lock (AdminManager.BlMutex) //stage 7
                doVolunteer = _dal.Volunteer.Read(boVolunteer.Id);
            if (doVolunteer is null)
                throw new BO.BlDoesNotExistException($"Volunteer with ID {boVolunteer.Id} does  not exist! ");
            if (!string.IsNullOrEmpty(boVolunteer.Password) && boVolunteer.Password == doVolunteer.Password)
                boVolunteer.Password = null;
            VolunteerManager.ValidateVolunteer(boVolunteer);
            if (!string.IsNullOrEmpty(boVolunteer.Password) && !VolunteerManager.IsPasswordStrong(boVolunteer.Password!))
                throw new BO.BlInvalidFormatException("Password is not strong!");
            if (requester.Role != DO.Role.Manager && requester.Role != (DO.Role)boVolunteer.Role)
                throw new BO.BlUnauthorizedException("Requester is not authorized to change the Role field!");
            if (!string.IsNullOrEmpty(boVolunteer.Password))
            {
                if (!VolunteerManager.VerifyPassword(boVolunteer.Password, doVolunteer.Password!))//if the user wants to update the pass field and he entered a leagal password
                    boVolunteer.Password = VolunteerManager.EncryptPassword(boVolunteer.Password);
            }
            else
                boVolunteer.Password = doVolunteer.Password;//if th password is not meant to be updated.

            DO.Volunteer updatedVolunteer = VolunteerManager.ConvertBoVolunteerToDoVolunteer(boVolunteer);
            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Update(updatedVolunteer);

            VolunteerManager.Observers.NotifyItemUpdated(boVolunteer.Id);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5

            if (!string.IsNullOrEmpty(boVolunteer.FullAddress))
                _ = updateCoordinatesForVolunteerAddressAsync(updatedVolunteer); //stage 7
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.Id} does not exist", ex);
        }
        catch (BO.BlUnauthorizedException ex)
        {
            throw new BO.BlUnauthorizedException(ex.Message, ex);
        }
        catch (BO.BlInvalidFormatException ex)
        {
            throw new BO.BlInvalidFormatException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("An unexpected error occurred.", ex);
        }
    }

    /// <summary>
    /// Deletes a volunteer from the system.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
    /// <exception cref="BO.BlDeletionException">Thrown if the volunteer is currently handling calls and cannot be deleted.</exception>
    public void DeleteVolunteer(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        try
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                var volunteer = _dal.Volunteer.Read(id);
                if (_dal.Assignment.ReadAll(a => a!.VolunteerId == id && a.EndTime is null).Any())
                    throw new BO.BlDeletionException($"Cannot delete volunteer with ID={id} as he is handling calls.");
                _dal.Volunteer.Delete(id);
            }
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5                                                    

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist", ex);
        }
        catch (BO.BlDeletionException ex)
        {
            throw new BO.BlDeletionException($"Cannot delete volunteer with ID={id} as he is handling calls", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("An unexpected error occurred.", ex);
        }
    }
    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="boVolunteer">The volunteer object to be added.</param>
    /// <exception cref="BO.BlAlreadyExistException">Thrown if the volunteer already exists.</exception>
    /// <exception cref="BO.BlInvalidFormatException">Thrown if the volunteer's details are not in the correct format.</exception>
    public void AddVolunteer(BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        try
        {
            Helpers.VolunteerManager.ValidateVolunteer(boVolunteer);
            if (string.IsNullOrEmpty(boVolunteer.Password))
                boVolunteer.Password = VolunteerManager.GenerateStrongPassword();
            boVolunteer.Password = VolunteerManager.EncryptPassword(boVolunteer.Password);
            DO.Volunteer doVolunteer = VolunteerManager.ConvertBoVolunteerToDoVolunteer(boVolunteer);
            lock (AdminManager.BlMutex)//stage 7
                _dal.Volunteer.Create(doVolunteer);
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5

            if (!string.IsNullOrEmpty(boVolunteer.FullAddress))
                _ = updateCoordinatesForVolunteerAddressAsync(doVolunteer); //stage 7


        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
        catch (BO.BlInvalidFormatException ex)
        {
            throw new BO.BlInvalidFormatException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("An unexpected error occurred.", ex);
        }
    }

    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5

    public IEnumerable<VolunteerInList> GetVolunteersFilterList(Enums.CallType? callType)//stage
    {
        try
        {
            IEnumerable<VolunteerInList> volunteers;
            if (callType is null)
                volunteers = GetVolunteersList();
            else
                volunteers = GetVolunteersList().Where(v => v.CallType == callType);
            return volunteers;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Error accessing Volunteers.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("An unexpected error occurred.", ex);
        }

    }
    #endregion Stage 5

    private async Task updateCoordinatesForVolunteerAddressAsync(DO.Volunteer doVolunteer)
    {
        if (doVolunteer.FullAddress is not null)
        {
            var (lat, lon) = await Tools.GetCoordinatesFromAddressAsync(doVolunteer.FullAddress);
            if (lat != default && lon != default) // Fix: Check for default values instead of HasValue  
            {
                doVolunteer = doVolunteer with { Latitude = lat, Longitude = lon };
                lock (AdminManager.BlMutex)
                    _dal.Volunteer.Update(doVolunteer);
                VolunteerManager.Observers.NotifyListUpdated();
                VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.Id);
            }
        }

    }
}

