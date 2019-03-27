using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.File;
using ServerlessDuropubokusu.Models;

namespace ServerlessDuropubokusu
{
    public static class Mapper
    {
        public static FileModel ToFileModel(this CloudFile file)
        {
            return new FileModel
            {
                Name = file.Name,
                Uri = file.Uri
            };
        }

        public static List<FileModel> ToFileModel(this IEnumerable<IListFileItem> list)
        {
            var result = new List<FileModel>();

            foreach (var listFileItem in list)
            {
                var file = (CloudFile) listFileItem;
                result.Add(file.ToFileModel());
            }

            return result;
        }

        public static ShareModel ToShareModel(this CloudFileShare share, string uri)
        {
            return new ShareModel
            {
                Name = share.Name,
                Uri = new Uri(uri + $"/{share.Name}")
            };
        }

        public static List<ShareModel> ToShareModel(this IEnumerable<CloudFileShare> list, string uri)
        {
            return list.Select(share => share.ToShareModel(uri)).ToList();
        }
    }
}
