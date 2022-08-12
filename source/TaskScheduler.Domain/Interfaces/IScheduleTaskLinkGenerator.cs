namespace TaskScheduler.Domain
{
    public interface IScheduleTaskLinkGenerator
    {
        string GenerateLink(string userInfo, string scheduleTaskInfo, int iteration);
    }
}
