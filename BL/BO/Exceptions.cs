
namespace DO;

[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}


[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}



//namespace BL
//{
//    using System;

//    /// <summary>
//    /// חריגות כלליות בשכבה הלוגית.
//    /// </summary>
//    public class BLException : Exception
//    {
//        public BLException(string message) : base(message) { }
//        public BLException(string message, Exception innerException) : base(message, innerException) { }
//    }

//    /// <summary>
//    /// חריגה במקרה של ניסיון להוספת ישות שכבר קיימת.
//    /// </summary>
//    public class EntityAlreadyExistsException : BLException
//    {
//        public EntityAlreadyExistsException(string entityName, string identifier)
//            : base($"הישות '{entityName}' עם מזהה '{identifier}' כבר קיימת במערכת.") { }
//    }

//    /// <summary>
//    /// חריגה במקרה של ניסיון למחיקת ישות שאינה קיימת.
//    /// </summary>
//    public class EntityNotFoundException : BLException
//    {
//        public EntityNotFoundException(string entityName, string identifier)
//            : base($"הישות '{entityName}' עם מזהה '{identifier}' לא נמצאה במערכת.") { }
//    }

//    /// <summary>
//    /// חריגה במקרה של נתונים לא תקינים.
//    /// </summary>
//    public class InvalidDataException : BLException
//    {
//        public InvalidDataException(string message) : base($"שגיאת נתונים: {message}") { }
//    }

//    /// <summary>
//    /// חריגה במקרה של ניסיון פעולה לא חוקי על הישות.
//    /// </summary>
//    public class IllegalActionException : BLException
//    {
//        public IllegalActionException(string message) : base($"פעולה לא חוקית: {message}") { }
//    }
}

