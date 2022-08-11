namespace OWP.PreMatchNotification.Shared.Services;

public interface DateTimeHelper
{
    DateTime GetCurrentDate();
}

public class DateTimeHelperImp : DateTimeHelper
{
    public DateTime GetCurrentDate()
        => DateTime.Today;
}