namespace FutureNHS.Api.DataAccess.Models.User;

public record ActiveUsers
{
    public ActiveUsers() {}

    public ActiveUsers(ActiveUsers activeUsers)
    {
        Daily = activeUsers.Daily;
        Weekly = activeUsers.Weekly;
        Monthly = activeUsers.Monthly;
    }
    public uint Daily { get; init; }
    public uint Weekly { get; init; }
    public uint Monthly { get; init; }
}