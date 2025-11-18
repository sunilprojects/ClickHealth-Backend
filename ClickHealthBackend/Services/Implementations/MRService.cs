using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;
using ClickHealthBackend.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using BCrypt.Net;

namespace ClickHealthBackend.Services.Implementation
{
    public class MRService : IMRService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMRActivityRepository _mrActivityRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IPatientInviteRepository _patientInviteRepository;
        private readonly IHCPActivityRepository _hcpActivityRepository;
        private readonly IEmailService _emailService;

        public MRService(
            IUserRepository userRepository,
            IMRActivityRepository mrActivityRepository,
            ICampaignRepository campaignRepository,
            IPatientInviteRepository patientInviteRepository,
            IHCPActivityRepository hcpActivityRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _mrActivityRepository = mrActivityRepository;
            _campaignRepository = campaignRepository;
            _patientInviteRepository = patientInviteRepository;
            _hcpActivityRepository = hcpActivityRepository;
            _emailService = emailService;
        }

        // ... (OnboardHCPAsync, LogFieldFeedbackAsync, and GetTerritorySnapshotAsync methods are omitted for brevity, but remain correct) ...

        public async Task<string> OnboardHCPAsync(string mrUserId, string hcpName, string hcpEmail, string hcpPhone, string territory, string specialty)
        {
            var tempPassword = Guid.NewGuid().ToString().Substring(0, 8);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(tempPassword);

            var newUser = new User
            {
                Email = hcpEmail,
                Name = hcpName,
                Password = hashedPassword,
                Phone = hcpPhone,
                Role = UserRole.HCP,
                Territory = territory,
                Specialty = specialty,
                IsActive = true,
                IsApproved = true,
                Status = UserStatus.Approved,
                CreatedAt = DateTime.UtcNow,
                MustResetPassword = true
            };
            await _userRepository.CreateAsync(newUser);

            string subject = "Welcome to ClickHealth - Your Account is Ready";
            string body = $"Dear {hcpName},\n\nYour Temporary Password is: <b>{tempPassword}</b>...";
            await _emailService.SendEmailAsync(newUser.Email, subject, body);

            await _mrActivityRepository.CreateAsync(new MRActivity
            {
                MrUserId = mrUserId,
                MrActivityType = MRActivityType.HCPOnboard,
                HcpUserId = newUser.UserId,
                Territory = territory,
                Timestamp = DateTime.UtcNow,
                Notes = $"HCP Onboarded: {hcpName}"
            });

            return newUser.UserId;
        }

        public async Task<bool> ShareContentWithHCPAsync(string mrUserId, string hcpUserId, string campaignId)
        {
            // FIX: Using the correct, standardized method name: GetByIdAsync
            var campaign = await _campaignRepository.GetByIdAsync(campaignId);

            if (campaign == null || campaign.Status != CampaignStatus.Active)
            {
                return false;
            }

            await _mrActivityRepository.CreateAsync(new MRActivity
            {
                MrUserId = mrUserId,
                MrActivityType = MRActivityType.PackShare,
                HcpUserId = hcpUserId,
                CampaignId = campaignId,
                Timestamp = DateTime.UtcNow,
                Notes = $"Campaign shared: {campaignId}"
            });

            return true;
        }

        public async Task LogFieldFeedbackAsync(string mrUserId, string notes)
        {
            await _mrActivityRepository.CreateAsync(new MRActivity
            {
                MrUserId = mrUserId,
                MrActivityType = MRActivityType.Feedback,
                Timestamp = DateTime.UtcNow,
                Notes = notes
            });
        }

        public async Task<object> GetTerritorySnapshotAsync(string mrUserId)
        {
            // 1. Get MR details for territory filter
            var mr = await _userRepository.GetByIdAsync(mrUserId);
            if (mr == null) return null;

            // 2. Get active HCPs in the MR's territory (using the valid status lookup method)
            var allApprovedUsers = await _userRepository.GetUsersByStatusAsync(UserStatus.Approved);
            var activeHcps = allApprovedUsers
                                .Where(u => u.Role == UserRole.HCP && u.Territory == mr.Territory && u.IsActive)
                                .ToList();

            // 3. Get patient invite count from those HCPs (real-time data)
            var hcpIdsInTerritory = activeHcps.Select(u => u.UserId).ToList();
            var allPatientInvites = await _patientInviteRepository.GetAllAsync();
            var patientInvites = allPatientInvites
                                .Where(i => hcpIdsInTerritory.Contains(i.HcpUserId));

            // 4. Return the aggregated snapshot
            return new
            {
                ActiveHcpCount = activeHcps.Count(),
                PatientInviteCount = patientInvites.Count()
            };
        }
    }
}