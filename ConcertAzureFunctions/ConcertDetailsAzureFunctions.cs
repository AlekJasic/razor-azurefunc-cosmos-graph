using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;
using System.Collections.Generic;
using Blazor_CORE.Shared.Models;
using Microsoft.Azure.Documents.Client;
using System.Linq;

namespace ConcertAzureFunctions
{
    public static class ConcertDetailsAzureFunctions
    {
        private const string Route = "cosmosDetails";
        private const string DatabaseName = "ConcertManagementDB";
        private const string CollectionName = "ConcertDetails";

        #region Azure functions

        [FunctionName("CosmosDB_GetAllConcertsDetails")]
        public static IActionResult GetConcertsDetails(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route)]HttpRequest req,
            [CosmosDB(
                DatabaseName,
                CollectionName,
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c order by c._ts desc")]
                IEnumerable<ConcertDetails> concertDetails)
        {
            return new OkObjectResult(concertDetails);
        }

        [FunctionName("CosmosDb_CreateConcertDetail")]
        public static async Task<IActionResult> CreateConcertDetail(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route + "/create")]HttpRequest req,
            [CosmosDB(
                DatabaseName,
                CollectionName,
                ConnectionStringSetting = "CosmosDBConnection")]
            IAsyncCollector<object> concertDetails)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ConcertDetails input = JsonConvert.DeserializeObject<ConcertDetails>(requestBody);

            await concertDetails.AddAsync(new { input.ConcertDetailNo, input.ConcertNo, input.ArtistName, input.Notes, input.Quantity, input.Price });
            return new OkObjectResult(input);
        }

        [FunctionName("CosmosDb_GetConcertDetailById")]
        public static IActionResult GetConcertDetailById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route + "/concertDetail/{id}")]HttpRequest req,
            [CosmosDB(
                DatabaseName,
                CollectionName,
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{id}")]
            ConcertDetails concertDetail, string id)
        {


            if (concertDetail == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(concertDetail);
        }

        [FunctionName("CosmosDb_UpdateConcertDetail")]
        public static async Task<IActionResult> UpdateConcertDetail(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = Route + "/update/{id}")]HttpRequest req,
            [CosmosDB(
                ConnectionStringSetting = "CosmosDBConnection")]
                DocumentClient client, string id)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<ConcertDetails>(requestBody);
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName);
            var document = client.CreateDocumentQuery(collectionUri).Where(t => t.Id == id)
                            .AsEnumerable().FirstOrDefault();
            if (document == null)
            {
                return new NotFoundResult();
            }

            if (updated.ConcertDetailNo > 0)
            {
                document.SetPropertyValue("ConcertDetailNo", updated.ConcertDetailNo);
            }
            if (updated.ConcertNo > 0)
            {
                document.SetPropertyValue("ConcertNo", updated.ConcertNo);
            }
            if (!string.IsNullOrEmpty(updated.ArtistName))
            {
                document.SetPropertyValue("ArtistName", updated.ArtistName);
            }

            if (!string.IsNullOrEmpty(updated.Notes))
            {
                document.SetPropertyValue("Notes", updated.Notes);
            }

            if (updated.Quantity > 0)
            {
                document.SetPropertyValue("Quantity", updated.Quantity);
            }

            if (updated.Price > 0)
            {
                document.SetPropertyValue("Price", updated.Price);
            }

            await client.ReplaceDocumentAsync(document);


            // an easier way to deserialize a Document
            ConcertDetails concertDetail = (dynamic)document;

            return new OkObjectResult(concertDetail);
        }

        [FunctionName("CosmosDb_DeleteConcertDetail")]
        public static async Task<IActionResult> DeleteConcertDetail(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Route + "/delete/{id}")]HttpRequest req,
            [CosmosDB(
                ConnectionStringSetting = "CosmosDBConnection")]
                DocumentClient client, string id)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName);
            var document = client.CreateDocumentQuery(collectionUri).Where(t => t.Id == id)
                    .AsEnumerable().FirstOrDefault();
            if (document == null)
            {
                return new NotFoundResult();
            }
            await client.DeleteDocumentAsync(document.SelfLink);
            return new OkResult();
        }



        #endregion
    }
}
