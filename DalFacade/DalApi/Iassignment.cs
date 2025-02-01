namespace DalApi;
using DO;
using System;

/// <summary>
/// Interface for the Assignment class, with the CRUD functions.
/// </summary>
public interface IAssignment : ICrud<Assignment> {
    object Where(Func<object, bool> value);

    start
}

