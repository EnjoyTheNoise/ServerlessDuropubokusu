using System;
using Microsoft.WindowsAzure.Storage;

namespace ServerlessDuropubokusu.Models
{
    public class FileModel : INode
    {
        public Uri Uri { get; set; }
        public string Name { get; set; }
        public string Sas { get; set; }

        public void GenerateSas(CloudStorageAccount storage)
        {
            var constraints = new SharedAccessAccountPolicy
            {
                Protocols = SharedAccessProtocol.HttpsOrHttp,
                Services = SharedAccessAccountServices.File,
                SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5),
                SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(1),
                Permissions = SharedAccessAccountPermissions.Read | SharedAccessAccountPermissions.List,
                ResourceTypes = SharedAccessAccountResourceTypes.Service | SharedAccessAccountResourceTypes.Container | SharedAccessAccountResourceTypes.Object
            };

            Sas = storage.GetSharedAccessSignature(constraints);
        }
    }
}
