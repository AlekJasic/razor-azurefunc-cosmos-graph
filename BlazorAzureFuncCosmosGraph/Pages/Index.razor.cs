using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor_CORE.Shared;
using Blazor_CORE.Shared.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Components;

namespace BlazorAzureFuncCosmosGraph.Pages
{
    public class IndexModel : ComponentBase
    {
        protected ConcertMasters[] arrConcertsMaster;
        protected ConcertDetails[] arrConcertsDetail;
        private readonly HttpClient Http = new HttpClient();

        protected ConcertMasters concertM = new ConcertMasters();
        protected ConcertDetails concertD = new ConcertDetails();

        protected Boolean showAddMaster = false;
        protected Boolean showAddDetail = false;

        protected int showDetailStatus = 0;
        protected int sortStatus = 0;
        protected int concertIDs = 0;
        protected string Imageclass = "oi oi-expand-down";
        protected string ImageSortClass = "oi oi-sort-ascending";

        
        protected override async Task OnInitAsync()
        {
            arrConcertsMaster = await Http.GetJsonAsync<ConcertMasters[]>("api/ConcertMasters/");
            concertD = new ConcertDetails();
            concertM = new ConcertMasters();
        }

        //to Add New Concert Master

        protected void AddNewConcertsMaster()
        {

            concertM = new ConcertMasters
            {
                ConcertDate = DateTime.Now
            };

            showAddMaster = true;
            showAddDetail = false;
            Imageclass = "oi oi-expand-down";
            showDetailStatus = 0;
        }

        //Save New or update ConcertsMaster

        protected async Task SaveConcertsMaster()
        {
            if (concertM.ConcertNo == 0)

            //new concert
            {
                await Http.SendJsonAsync(HttpMethod.Post, "/api/ConcertMasters/", concertM);
            }
            //existing concert
            else
            {
                await Http.SendJsonAsync(HttpMethod.Put, "/api/ConcertMasters/" + concertM.Id, concertM);
            }
            concertM = new ConcertMasters();
            arrConcertsMaster = await Http.GetJsonAsync<ConcertMasters[]>("/api/ConcertMasters/");

            showAddMaster = false;

        }

        //Edit Concert Master

        protected async Task EditConcertsMaster(string ConcertNos)
        {
            showAddMaster = true;
            concertM = await Http.GetJsonAsync<ConcertMasters>("/api/ConcertMasters/" + ConcertNos);
        }

        //Delete Concert Master
        protected async Task DeleteConcertsMaster(string ConcertId)
        {
            await Http.DeleteAsync("/api/ConcertMasters/" + ConcertId);

            arrConcertsMaster = await Http.GetJsonAsync<ConcertMasters[]>("/api/ConcertMasters/");          
        }

        //Sorting
        protected async Task ConcertSorting(string SortColumn)
        {
            arrConcertsMaster = await Http.GetJsonAsync<ConcertMasters[]>("/api/ConcertMasters/");
           
            if (sortStatus == 1)
            {
                ImageSortClass = "oi oi-sort-descending";
                sortStatus = 0;

                switch (SortColumn)
                {
                    case "ConcertNo":
                        arrConcertsMaster = arrConcertsMaster.OrderBy(x => x.ConcertNo).ToArray();
                        break;
                    case "HallId":
                        arrConcertsMaster = arrConcertsMaster.OrderBy(x => x.HallId).ToArray();
                        break;

                    case "Description":
                        arrConcertsMaster = arrConcertsMaster.OrderBy(x => x.Description).ToArray();
                        break;
                    case "ConcertDate":
                        arrConcertsMaster = arrConcertsMaster.OrderBy(x => x.ConcertDate).ToArray();
                        break;
                    case "TicketServiceName":
                        arrConcertsMaster = arrConcertsMaster.OrderBy(x => x.TicketServiceName).ToArray();
                        break;
                }
            }
            else
            {
                ImageSortClass = "oi oi-sort-ascending";
                sortStatus = 1;

                switch (SortColumn)
                {
                    case "ConcertNo":
                        arrConcertsMaster = arrConcertsMaster.OrderByDescending(x => x.ConcertNo).ToArray();
                        break;
                    case "HallId":
                        arrConcertsMaster = arrConcertsMaster.OrderByDescending(x => x.HallId).ToArray();
                        break;

                    case "Description":
                        arrConcertsMaster = arrConcertsMaster.OrderByDescending(x => x.Description).ToArray();
                        break;
                    case "ConcertDate":
                        arrConcertsMaster = arrConcertsMaster.OrderByDescending(x => x.ConcertDate).ToArray();
                        break;
                    case "TicketServiceName":
                        arrConcertsMaster = arrConcertsMaster.OrderByDescending(x => x.TicketServiceName).ToArray();
                        break;
                }
            }
        }

        //Filtering
        protected async Task concertFilteringList(String Value, string columnName)
        {
            arrConcertsMaster = await Http.GetJsonAsync<ConcertMasters[]>("/api/ConcertMasters/");

            if (Value.Trim().Length > 0)
            {

                switch (columnName)
                {

                    case "HallId":
                        arrConcertsMaster = arrConcertsMaster.Where(x => x.HallId.StartsWith(Value)).ToArray();
                        break;
                    case "Description":
                        arrConcertsMaster = arrConcertsMaster.Where(x => x.Description.StartsWith(Value)).ToArray();
                        break;
                    case "TicketServiceName":
                        arrConcertsMaster = arrConcertsMaster.Where(x => x.TicketServiceName.StartsWith(Value)).ToArray();
                        break;
                }

            }
            else
            {
                arrConcertsMaster = await Http.GetJsonAsync<ConcertMasters[]>("/api/ConcertMasters/");
            }
        }

        //Detail Grid CRUD

        protected async Task getConcertDetails(int concertID)
        {
            showAddMaster = false;
            showAddDetail = false;
    
            if (concertIDs != concertID)
            {
                Imageclass = "oi oi-collapse-up";
                showDetailStatus = 1;

            }
            else
            {
                if (showDetailStatus == 0)
                {
                    Imageclass = "oi oi-expand-up";
                    showDetailStatus = 1;
                }
                else
                {
                    Imageclass = "oi oi-expand-down";
                    showDetailStatus = 0;
                }

            }
            concertIDs = concertID;
            arrConcertsDetail = await Http.GetJsonAsync<ConcertDetails[]>("/api/ConcertDetails/" + Convert.ToInt32(concertID));

        }
        //to Add New Concert Detail

        protected async Task AddNewConcertDetails(int concertMasterNO)
        {

            concertD = new ConcertDetails();
            concertD.ConcertNo = concertMasterNO;
         
            showAddDetail = true;
            showAddMaster = false;

        }

        //Save New or update Concert detail
        protected async Task SaveConcertDetails()
        {
            if (concertD.ConcertDetailNo == 0)

            {
                await Http.SendJsonAsync(HttpMethod.Post, "/api/ConcertDetails/", concertD);

            }
            else
            {
                await Http.SendJsonAsync(HttpMethod.Put, "/api/ConcertDetails/" + concertD.Id, concertD);
            }

            arrConcertsDetail = await Http.GetJsonAsync<ConcertDetails[]>("/api/ConcertDetails/" + Convert.ToInt32(concertD.ConcertNo));
            concertD = new ConcertDetails();
            showAddDetail = false;
            showAddMaster = false;           
        }

        //Edit Concert detail
        protected async Task EditConcertDetails(string ConcertDetailId)
        {
            concertD = await Http.GetJsonAsync<ConcertDetails>("/api/ConcertDetails/" + ConcertDetailId + "/change");
            showAddDetail = true;
            showAddMaster = false;
        }

        //Delete Concert detail
        protected async Task DeleteConcertDetails(string concertDetailId, int concertMasterNo)
        {

            await Http.DeleteAsync("/api/ConcertDetails/" + concertDetailId);

            //arrConcertsDetail = await Http.GetJsonAsync<ConcertDetails[]>("/api/ConcertDetails/" + Convert.ToInt32(ordVale));
            Imageclass = "oi oi-expand-down";
            showDetailStatus = 0;
            await getConcertDetails(concertMasterNo);         
        }

        protected void hideAddMaster()
        {
            showAddMaster = false;
        }
        protected void hideAddDetail()
        {
            showAddDetail = false;
        }
        protected void hideConcertDetails()
        {
            Imageclass = "oi oi-expand-down";
            showDetailStatus = 0;
        }
    }
}
