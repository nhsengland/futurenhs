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
            RuleFor(model => model.Title)
                .NotEmpty()
                .WithMessage("Enter a folder title");

            RuleFor(model => model.Title)
                .MaximumLength(200)
                .WithMessage("Folder title must be 200 characters or fewer");

            RuleFor(model => model.Description)
                .MaximumLength(4000)
                .WithMessage("Folder description must be 4000 characters or fewer");

            RuleFor(model => model)
                .MustAsync(IsFolderUnique)
                .OverridePropertyName(nameof(FolderDto.Title))
                .WithMessage("Enter a unique folder title");                
        }

        private async Task<bool> IsFolderUnique(FolderDto folder, CancellationToken cancellationToken)
        {
            return await _folderCommand.IsFolderUniqueAsync(folder.Title, folder.ParentFolder, folder.GroupId, cancellationToken);
        }
    }
}
