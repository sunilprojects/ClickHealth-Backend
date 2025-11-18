using ClickHealth.Server.Models;
using ClickHealthBackend.Enums;
using ClickHealthBackend.Models;
using ClickHealthBackend.Repositories.Interfaces;
using ClickHealthBackend.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClickHealthBackend.Services.Implementation

{

    public class PatientService : IPatientService

    {

        private readonly IPatientInviteRepository _patientInviteRepository;

        private readonly IContentRepository _contentRepository;

        private readonly IConsentRecordRepository _consentRecordRepository;

        private readonly IPatientEngagementRepository _patientEngagementRepository;

        public PatientService(

            IPatientInviteRepository patientInviteRepository,

            IContentRepository contentRepository,

            IConsentRecordRepository consentRecordRepository,

            IPatientEngagementRepository patientEngagementRepository)

        {

            _patientInviteRepository = patientInviteRepository;

            _contentRepository = contentRepository;

            _consentRecordRepository = consentRecordRepository;

            _patientEngagementRepository = patientEngagementRepository;

        }

        // Retrieve content by secure invite code

        public async Task<Content> GetContentByInviteCodeAsync(string inviteCode)

        {

            var invite = await _patientInviteRepository.GetByInviteCodeAsync(inviteCode);

            if (invite == null || !invite.IsActive || invite.ExpiresAt < DateTime.UtcNow)

                return null;

            return await _contentRepository.GetByIdAsync(invite.ContentId);

        }

        // Record patient consent including IP address for audit/compliance

        public async Task<bool> RecordPatientConsentAsync(string inviteCode, string userIpAddress)

        {

            var invite = await _patientInviteRepository.GetByInviteCodeAsync(inviteCode);

            if (invite == null) return false;

            var consentRecord = new ConsentRecord

            {

                UserId = invite.HcpUserId,   // HCP responsible for the invite

                UserType = "Patient",

                ConsentType = "DPDP",

                IsGranted = true,

                GrantedAt = DateTime.UtcNow,

                UserIpAddress = userIpAddress // Store the IP for compliance logging

            };

            await _consentRecordRepository.CreateAsync(consentRecord);

            return true;

        }

        // Log engagement metrics for a patient

        public async Task LogContentEngagementAsync(string inviteCode, string engagementType, int durationSeconds, string city, string language)

        {

            var invite = await _patientInviteRepository.GetByInviteCodeAsync(inviteCode);

            if (invite == null) return;

            if (!Enum.TryParse(engagementType, true, out EngagementType parsedType))

                parsedType = EngagementType.View;

            var engagement = new PatientEngagement

            {

                InviteCode = inviteCode,

                ContentId = invite.ContentId,

                CampaignId = invite.CampaignId,

                ViewedAt = DateTime.UtcNow,

                ConsentGiven = true,

                DurationSeconds = durationSeconds,

                City = city,

                Language = language,

                EngagementType = parsedType

            };

            await _patientEngagementRepository.CreateAsync(engagement);

        }

        // Log quiz completion with responses safely converted to BSON

        public async Task<bool> LogQuizCompletionAsync(string inviteCode, string contentId, Dictionary<string, object> quizResponses)

        {

            var invite = await _patientInviteRepository.GetByInviteCodeAsync(inviteCode);

            if (invite == null) return false;

            var bsonQuizResponse = BsonSerializer.Deserialize<BsonDocument>(JsonSerializer.Serialize(quizResponses));

            var engagement = new PatientEngagement

            {

                InviteCode = inviteCode,

                ContentId = contentId,

                CampaignId = invite.CampaignId,

                CompletedAt = DateTime.UtcNow,

                ConsentGiven = true,

                QuizResponse = bsonQuizResponse,

                EngagementType = EngagementType.Complete

            };

            await _patientEngagementRepository.CreateAsync(engagement);

            return true;

        }

    }

}

