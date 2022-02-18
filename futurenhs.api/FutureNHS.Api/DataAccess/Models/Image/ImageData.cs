using FutureNHS.Api.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace FutureNHS.Api.DataAccess.Models
{
    public record ImageData
    {
        private readonly IOptions<AzureImageBlobStorageConfiguration> _options;

        public ImageData() { }

        public ImageData(ImageData image, IOptions<AzureImageBlobStorageConfiguration> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            Id = image.Id;
            Height = image.Height;
            Width = image.Width;
            FileName = image.FileName;
            MediaType = image.MediaType;
            SourceUri = $"{_options.Value.PrimaryServiceUrl}/{_options.Value.ContainerName}";
        }

        public Guid Id { get; init; }

        public string? Source => $@"{SourceUri}/{FileName}";

        public int Height { get; init; }

        public int Width { get; init; }

        public string FileName { get; init; }

        public string MediaType { get; init; }

        [JsonIgnore]
        public string SourceUri { get; init; }
    }
}
