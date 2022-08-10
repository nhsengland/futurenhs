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
            RuleFor(model => model.Name)
                .NotEmpty()
                .WithMessage("Enter the folder name");

            RuleFor(model => model.Name)
                .MaximumLength(200)
                .WithMessage("Enter 200 or fewer characters");

            RuleFor(model => model.Description)
                .MaximumLength(4000)
                .WithMessage("Enter 4000 or fewer characters");

            RuleFor(model => model)
                .MustAsync(IsFolderUnique)
                .OverridePropertyName(nameof(FolderDto.Name))
                .WithMessage("Enter a unique folder name");                
        }

        private async Task<bool> IsFolderUnique(FolderDto folder, CancellationToken cancellationToken)
        {
            return await _folderCommand.IsFolderUniqueAsync(folder.Name, folder.Id, folder.ParentFolder, folder.GroupId, cancellationToken);
        }
    }
}
