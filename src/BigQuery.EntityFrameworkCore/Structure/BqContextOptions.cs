using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigQuery.EntityFrameworkCore
{
    public class BqContextOptions
    {
        public BqContextOptions()
        {
        }

        public BqContextOptions(string projectId, string googleCredentialJson)
        {
            ProjectId = projectId;
            GoogleCredential = GoogleCredential.FromJson(googleCredentialJson);
        }

        public string ProjectId { get; set; }
        public GoogleCredential GoogleCredential { get; set; }
    }
}
