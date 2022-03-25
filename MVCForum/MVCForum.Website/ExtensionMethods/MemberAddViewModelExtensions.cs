namespace MvcForum.Web.ViewModels.ExtensionMethods
{
    using Core.Constants;
    using Core.ExtensionMethods;
    using Core.Models.Entities;
    using Core.Models.Enums;
    using Member;

    public static partial class MemberAddViewModelExtensions
    {
        /// <summary>
        /// Converts a add view model to a membershipuser
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static MembershipUser ToMembershipUser(this MemberAddViewModel viewModel)
        {
            var surnameInitial = string.IsNullOrEmpty(viewModel.Surname) ? string.Empty : viewModel.Surname[0].ToString();

            var userToSave = new MembershipUser
            {
                UserName = viewModel.Email,
                FirstName = viewModel.FirstName,
                Surname = viewModel.Surname,
                Initials = string.Format("{0}{1}", viewModel.FirstName[0], surnameInitial).ToUpper(),
                Email = viewModel.Email,
                Password = viewModel.Password,
                IsApproved = viewModel.IsApproved
            };


            return userToSave;
        }
    }
}