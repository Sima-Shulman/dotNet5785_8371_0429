namespace BlApi;


/// <summary>
/// Factory class responsible for creating an instance of the Business Logic layer.
/// </summary>
public static class Factory
{
    /// <summary>
    /// A function that returns an instance of the BL implementation.
    /// </summary>
    /// <returns>An object that implements IBl</returns>
    public static IBl Get() => new BlImplementation.Bl();
}

