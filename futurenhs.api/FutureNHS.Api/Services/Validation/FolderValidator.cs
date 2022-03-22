using FluentValidation;
using FutureNHS.Api.DataAccess.Database.Write.Interfaces;
using FutureNHS.Api.DataAccess.DTOs;

namespace FutureNHS.Api.Services.Validation
{
    public sealed class FolderValidator : AbstractValidator<FolderDto>
    {
        private readonly IFolderCommand _folderCommand;

        public FolderValidator(IFolderCommand folderCommand)
        {
            _folderCommand = folderCommand ?? throw new ArgumentNullException(nameof(folderCommand));

            RuleFor(model => model)
                .MustAsync(IsFolderUnique)
                .OverridePropertyName(nameof(FolderDto.Title))
                .WithMessage("Enter a unique folder title");                
        }

        private async Task<bool> IsFolderUnique(FolderDto folder, CancellationToken cancellationToken)
        {
            return await _folderCommand.IsFolderUnique(folder.Title, folder.ParentFolder, folder.GroupId, cancellationToken);
        }
    }
}
