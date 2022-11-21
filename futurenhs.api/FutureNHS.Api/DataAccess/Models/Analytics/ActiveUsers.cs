using MimeDetective.Storage.Xml.v2;

namespace FutureNHS.Api.DataAccess.Models.User;

public record ActiveUsers
{
    public ActiveUsers() {}

    public ActiveUsers(ActiveUsers activeUsers)
    {
        Daily = activeUsers.Daily;
        // Weekly = activeUsers.Weekly;
        // Monthly = activeUsers.Monthly;
    }
    public int Daily { get; init; }
    // public int Weekly { get; init; }
    // public int Monthly { get; init; }
}