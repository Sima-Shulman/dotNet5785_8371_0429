namespace Dal;

/// <summary>
/// The configuration class. Holds the system's variables.
/// </summary>
internal static class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_calls_xml = "calls.xml";
    internal const string s_assignments_xml = "assignments.xml";
    internal const string s_volunteers_xml = "volunteers.xml";
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallID");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallID", value);
    }
    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }
    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }
    internal static TimeSpan RiskRange
    {
        get => XMLTools.GetConfigSpanVal(s_data_config_xml, "RiskRange");
        set => XMLTools.SetConfigSpanVal(s_data_config_xml, "RiskRange", value);
    }
    /// <summary>
    /// Reset all the Config fields.
    /// </summary>
    internal static void Reset()
    {
        NextCallId = 1000;
        NextAssignmentId = 1;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
    }
}
