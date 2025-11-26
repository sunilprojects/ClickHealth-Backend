
    using Microsoft.Extensions.Options;

    using MongoDB.Driver;

    using ClickHealthBackend.Models;
using ClickHealth.Server.Models;


namespace ClickHealthBackend.Data

    {

        public class MongoDbContext

        {

            public IMongoDatabase Database { get; }

            public MongoDbContext(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)

            {

                var dbName = settings.Value.DatabaseName;

                if (string.IsNullOrEmpty(dbName))

                    throw new ArgumentNullException(nameof(dbName), "MongoDB database name is missing.");

                Database = mongoClient.GetDatabase(dbName);

            }

            public IMongoCollection<User> Users => Database.GetCollection<User>("Users");

            public IMongoCollection<Campaign> Campaigns => Database.GetCollection<Campaign>("Campaigns");

            public IMongoCollection<Content> Contents => Database.GetCollection<Content>("Contents");




            // Add rest of your collections

            public IMongoCollection<CampaignAsset> CampaignAsset => Database.GetCollection<CampaignAsset>("CampaignAsset");
            public IMongoCollection<CampaignMetrics> CampaignMetrics => Database.GetCollection<CampaignMetrics>("CampaignMetrics");
            public IMongoCollection<ContentApproval> ContentApproval => Database.GetCollection<ContentApproval>("ContentApproval");
            public IMongoCollection<ContentEngagement> ContentEngagement => Database.GetCollection<ContentEngagement>("ContentEngagement");

            public IMongoCollection<MRActivity> MRActivity => Database.GetCollection<MRActivity>("MRActivity");
            public IMongoCollection<PatientEngagement> PatientEngagement => Database.GetCollection<PatientEngagement>("PatientEngagement");

            public IMongoCollection<Patient> PatientInvite => Database.GetCollection<Patient>("PatientInvite");
        public IMongoCollection<HCP> HCPs => Database.GetCollection<HCP>("HCPs");
        public IMongoCollection<PatientInvite> PatientInvites => Database.GetCollection<PatientInvite>("PatientInvites");


        // Add rest of your collections
        public IMongoCollection<Content> Content => Database.GetCollection<Content>("contents");

        public IMongoCollection<ContentAsset> ContentAssets => Database.GetCollection<ContentAsset>("contentAssets");

        public IMongoCollection<ConsentRecord>? ConsentRecord { get; internal set; }
    }

}





