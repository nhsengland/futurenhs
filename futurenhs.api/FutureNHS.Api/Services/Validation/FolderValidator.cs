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
                .WithMessage("Enter the folder title");

            RuleFor(model => model.Title)
                .MaximumLength(200)
                .WithMessage("Enter 200 or fewer characters");

            RuleFor(model => model.Description)
                .MaximumLength(4000)
                .WithMessage("Enter 4000 or fewer characters");

            RuleFor(model => model)
                .MustAsync(IsFolderUnique)
                .OverridePropertyName(nameof(FolderDto.Title))
                .WithMessage("Enter a unique folder title");                
        }

        private async Task<bool> IsFolderUnique(FolderDto folder, CancellationToken cancellationToken)
        {
            return await _folderCommand.IsFolderUniqueAsync(folder.Title, folder.Id, folder.ParentFolder, folder.GroupId, cancellationToken);
        }
    }
}
