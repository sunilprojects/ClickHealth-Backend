using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ClickHealthBackend.Enums;

namespace ClickHealthBackend.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        // Primary key (MongoDB ObjectId)
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;

        [BsonElement("UserCustomId")]
        public string UserCustomId { get; set; } = null!;

        // --- Basic Info ---
        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("phone")]
        public string Phone { get; set; } = null!;

        [BsonElement("preferredLanguage")]
        public string PreferredLanguage { get; set; } = "English";

        // --- Authentication Info ---
        [BsonElement("password")]
        public string Password { get; set; } = null!; // For manual signup users

        [BsonElement("googleId")]
        public string? GoogleId { get; set; } // For Google login users

        [BsonElement("loginProvider")]
        public string LoginProvider { get; set; } = "LOCAL"; // LOCAL or GOOGLE

        [BsonElement("isApproved")]
        public bool IsApproved { get; set; } = false; // Requires admin approval

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        // --- Role & Permissions ---
        [BsonElement("role")]
        [BsonRepresentation(BsonType.String)]
        public UserRole Role { get; set; } = UserRole.Guest;

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public UserStatus Status { get; set; } = UserStatus.Pending;

        [BsonElement("specialty")]
        public string? Specialty { get; set; } // Only for HCP role

        [BsonElement("territory")]
        public string? Territory { get; set; }

        // --- Security / OTP ---
        [BsonElement("totp")]
        public string? Totp { get; set; }

        [BsonElement("totpGeneratedAt")]
        public DateTime? TotpGeneratedAt { get; set; }

        [BsonElement("mustResetPassword")]
        public bool MustResetPassword { get; set; } = false;

        [BsonElement("lastPasswordChangeAt")]
        public DateTime? LastPasswordChangeAt { get; set; }

        // --- Google profile extras ---
        [BsonElement("profilePicture")]
        public string? ProfilePicture { get; set; }

        [BsonElement("googleAccessToken")]
        public string? GoogleAccessToken { get; set; }

        // --- Audit fields ---
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("lastLoginAt")]
        public DateTime? LastLoginAt { get; set; }

        [BsonElement("createdBy")]
        public string? CreatedBy { get; set; }

        [BsonElement("updatedBy")]
        public string? UpdatedBy { get; set; }
    }
}
