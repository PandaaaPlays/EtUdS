namespace EtUdS.Server.Utils;

public class Dates
{
    private static String EST = "America/New_York";

    public static DateTime GetTimeNowEst()
    {
        return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById(EST));
    }
}