namespace FutureNHS.Api.Services.Interfaces
{
    public interface IEtagService
    {
        byte[] GetIfMatch();
        byte[] GetIfNoneMatch();
    }
}
