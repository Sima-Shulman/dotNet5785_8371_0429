using DalApi;

namespace Helpers;
/// <summary>
/// Provides static access to assignment-related operations using the data layer.
/// </summary>
internal static class AssignmentManager
{
    private static IDal s_dal = Factory.Get;

}
