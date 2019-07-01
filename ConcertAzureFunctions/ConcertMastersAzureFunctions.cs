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
    public static class ConcertMastersAzureFunctions
    {
        private const string Route = "cosmosConcerts";
        private const string DatabaseName = "ConcertManagementDB";
        private const string CollectionName = "ConcertMasters";

#region Azure functions

        [FunctionName("CosmosDB_GetAllConcerts")]
        public static IActionResult GetConcerts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route)]HttpRequest req,
            [CosmosDB(
                DatabaseName,
                CollectionName,
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c order by c._ts desc")]
                IEnumerable<ConcertMasters> concerts)
        {
            return new OkObjectResult(concerts);
        }

        [FunctionName("CosmosDb_CreateConcertMaster")]
        public static async Task<IActionResult> CreateConcertMaster(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route + "/create")]HttpRequest req,
            [CosmosDB(
                DatabaseName,
                CollectionName,
                ConnectionStringSetting = "CosmosDBConnection")]
            IAsyncCollector<object> concertMasters)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ConcertMasters input = JsonConvert.DeserializeObject<ConcertMasters>(requestBody);
          
            await concertMasters.AddAsync(new { input.ConcertNo, input.ConcertDate, input.Description, input.HallId, input.TicketServiceName });
            return new OkObjectResult(input);
        }

        [FunctionName("CosmosDb_GetConcertMasterById")]
        public static IActionResult GetConcertMasterById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route + "/concert/{id}")]HttpRequest req,
            [CosmosDB(
                DatabaseName,
                CollectionName,
                ConnectionStringSetting = "CosmosDBConnection",
                Id = "{id}")]
            ConcertMasters concertMaster, string id)
        {
           

            if (concertMaster == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(concertMaster);
        }

        [FunctionName("CosmosDb_UpdateConcertMaster")]
        public static async Task<IActionResult> UpdateConcertMaster(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = Route + "/update/{id}")]HttpRequest req,
            [CosmosDB(
                ConnectionStringSetting = "CosmosDBConnection")]
                DocumentClient client, string id)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<ConcertMasters>(requestBody);
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName);
            var document = client.CreateDocumentQuery(collectionUri).Where(t => t.Id == id)
                            .AsEnumerable().FirstOrDefault();
            if (document == null)
            {
                return new NotFoundResult();
            }


            if (updated.ConcertNo > 0)
            {
                document.SetPropertyValue("ConcertNo", updated.ConcertNo);
            }
            if (updated.ConcertDate > DateTime.MinValue)
            {
                document.SetPropertyValue("ConcertDate", updated.ConcertDate);
            }

            if (!string.IsNullOrEmpty(updated.Description))
            {
                document.SetPropertyValue("Description", updated.Description);
            }

            if (!string.IsNullOrEmpty(updated.HallId))
            {
                document.SetPropertyValue("HallId", updated.HallId);
            }

            if (!string.IsNullOrEmpty(updated.TicketServiceName))
            {
                document.SetPropertyValue("TicketServiceName", updated.TicketServiceName);
            }
            
            await client.ReplaceDocumentAsync(document);


            // an easier way to deserialize a Document
            ConcertMasters concertMaster = (dynamic)document;

            return new OkObjectResult(concertMaster);
        }

        [FunctionName("CosmosDb_DeleteConcertMaster")]
        public static async Task<IActionResult> DeleteConcertMaster(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Route + "/delete/{id}")]HttpRequest req,
            [CosmosDB(
                ConnectionStringSetting = "CosmosDBConnection")]
                DocumentClient client,string id)
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
