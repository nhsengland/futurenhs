namespace FutureNHS.Api.DataAccess.Models.Comment
{
    public record UserCommentDetails
    {
        public bool Created { get; init; }
        public bool Liked { get; init; }
    }
}
