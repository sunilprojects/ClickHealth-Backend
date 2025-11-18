
    using Microsoft.Extensions.Options;

    using MongoDB.Driver;

    using ClickHealthBackend.Models;

    using System;

    using System.Collections.Generic;

    using System.Linq;


    using ClickHealth.Server.Models;


    using System.Diagnostics;
 

 
 
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

            public IMongoCollection<AuditLog> AuditLog => Database.GetCollection<AuditLog>("AuditLog");



            // Add rest of your collections

            public IMongoCollection<CampaignAsset> CampaignAsset => Database.GetCollection<CampaignAsset>("CampaignAsset");
            public IMongoCollection<CampaignMetrics> CampaignMetrics => Database.GetCollection<CampaignMetrics>("CampaignMetrics");
            public IMongoCollection<ConsentRecord> ConsentRecord => Database.GetCollection<ConsentRecord>("ConsentRecord");
            public IMongoCollection<ContentApproval> ContentApproval => Database.GetCollection<ContentApproval>("ContentApproval");
            public IMongoCollection<ContentEngagement> ContentEngagement => Database.GetCollection<ContentEngagement>("ContentEngagement");
            public IMongoCollection<HCPActivity> HCPActivity => Database.GetCollection<HCPActivity>("HCPActivity");

            public IMongoCollection<MRActivity> MRActivity => Database.GetCollection<MRActivity>("MRActivity");
            public IMongoCollection<PatientEngagement> PatientEngagement => Database.GetCollection<PatientEngagement>("PatientEngagement");

            public IMongoCollection<Patient> PatientInvite => Database.GetCollection<Patient>("PatientInvite");



            // Add rest of your collections


        }

    }



            // Add rest of your collections



