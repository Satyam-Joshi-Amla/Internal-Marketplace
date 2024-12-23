﻿using System;
using AmlaMarketPlace.DAL.Service.Services.Profile;
using AmlaMarketPlace.Models.DTO;
using AmlaMarketPlace.Models.ViewModels.Profile;

namespace AmlaMarketPlace.BAL.Agent.Agents.Profile
{
    public class ProfileAgent
    {
        private readonly ProfileService _profileService;
        public ProfileAgent(ProfileService profileService)
        {
            _profileService = profileService;
        }

        public ProfileDetailsViewModel GetUser(int userID)
        {
            UserDTO user = _profileService.GetUser(userID);
            ProfileDetailsViewModel profileDetails = new ProfileDetailsViewModel()
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                IsEmailVerified = user.IsEmailVerified,
                MobileNumber = user.MobileNumber
            };

            return profileDetails;
        }

        public bool UpdateUser(ProfileDetailsViewModel updatedUser)
        {
            UserDTO updatedUserDTO = new UserDTO()
            {
                UserId = updatedUser.UserId,
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                EmailAddress = updatedUser.EmailAddress,
                IsEmailVerified = updatedUser.IsEmailVerified,
                MobileNumber = updatedUser.MobileNumber
            };

            return _profileService.UpdateUser(updatedUserDTO);
        }
    }
}
