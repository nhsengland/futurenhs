namespace MvcForum.Web.ViewModels.ExtensionMethods
{
    using Core.Models.Entities;
    using Member;

    /// <summary>
    /// Extension methods for the member edit view model
    /// </summary>
    public static partial class MemberFrontEndEditViewModelExtensions
    {
        /// <summary>
        /// Creates a MembershipUser from the view model
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static MembershipUser ToMembershipUser(this MemberFrontEndEditViewModel viewModel, MembershipUser user)
        {
            user.Id = viewModel.Id;
            user.FirstName = viewModel.FirstName;
            user.Surname = viewModel.Surname ?? null;
            var surnameInitial = string.IsNullOrEmpty(user.Surname) ? string.Empty : user.Surname[0].ToString();
            user.Initials = $"{user.FirstName[0]}{surnameInitial}".ToUpper();


            user.HasAgreedToTermsAndConditions = viewModel.HasAgreedToTermsAndConditions;
            user.Pronouns = viewModel.Pronouns;

            return user;
        }

        /// <summary>
        ///     Creates view model
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static MemberFrontEndEditViewModel PopulateMemberViewModel(this MembershipUser user)
        {
            var viewModel = new MemberFrontEndEditViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                Surname = user.Surname,
                Initials = user.Initials,
                Email = user.Email,
                Pronouns = user.Pronouns,
            };
            return viewModel;
        }
    }
}