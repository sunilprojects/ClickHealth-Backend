
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ClickHealthBackend.Models
{
    public class Patient
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]   // <-- FIX HERE
        public string Id { get; set; }

        public string PatientCustomId { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Specialty { get; set; }
        public string HcpId { get; set; }
        public string ShareChannel { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string Condition { get; set; }
        public string Language { get; set; }
    }

}