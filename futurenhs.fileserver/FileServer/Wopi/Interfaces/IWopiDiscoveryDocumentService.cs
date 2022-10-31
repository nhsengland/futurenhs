namespace FileServer.Wopi.Interfaces;

public interface IWopiDiscoveryDocumentService
{
    Task<IWopiDiscoveryDocument> GetAsync(CancellationToken cancellationToken);
}