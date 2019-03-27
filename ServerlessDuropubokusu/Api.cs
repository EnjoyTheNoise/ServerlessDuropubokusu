using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using ServerlessDuropubokusu.HtmlRendering;
using ServerlessDuropubokusu.Models;

namespace ServerlessDuropubokusu
{
    [StorageAccount("ThisIsNotSecureAtAll")]
    public static class Api
    {
        private static CloudStorageAccount Storage => CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("ThisIsNotSecureAtAll"));

        [FunctionName("GetShares")]
        public static async Task<HttpResponseMessage> GetShares(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",  Route = "share")] HttpRequest req,
            ILogger log)
        {
            var client = GetClient();
            var shares = await client.ListSharesSegmentedAsync(new FileContinuationToken());

            var shareModels = shares.Results.ToShareModel(req.GetDisplayUrl());

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(GetHtmlContent(shareModels, false));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }

        [FunctionName("GetShareContent")]
        public static async Task<HttpResponseMessage> GetShareContent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "share/{name}")]
            HttpRequest req, string name, ILogger log)
        {
            var client = GetClient();

            var contents = client.GetShareReference(name);

            var allFiles = contents.GetRootDirectoryReference();
            var segmentResult = await allFiles.ListFilesAndDirectoriesSegmentedAsync(default(FileContinuationToken));

            var fileModels = segmentResult.Results.ToFileModel();
            fileModels.GenerateSasForFiles();

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(GetHtmlContent(fileModels));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }

        private static void GenerateSasForFiles(this IEnumerable<FileModel> files)
        {
            foreach (var file in files)
            {
                file.GenerateSas(Storage);
            }
        }

        private static CloudFileClient GetClient()
        {
            return Storage.CreateCloudFileClient();
        }

        private static string GetHtmlContent(IEnumerable<INode> listOfNodes, bool isSasNeeded = true)
        {
            var renderer = new HtmlRenderer();
            renderer.CreateContent();

            listOfNodes = listOfNodes.ToList();
            foreach (var node in listOfNodes)
            {
                renderer.AddNode(node, isSasNeeded);
            }

            return renderer.Build();
        }
    }
}
