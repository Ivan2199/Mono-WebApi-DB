using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebProject.Data;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class VehicleServiceHistoryController : ApiController
    {
        private readonly DataAccessVehicleServiceHistory _dataAccessVehicleServiceHistory;

        public VehicleServiceHistoryController()
        {
            _dataAccessVehicleServiceHistory = new DataAccessVehicleServiceHistory();
            _dataAccessVehicleServiceHistory.InitializeDatabase();
        }

        // GET api/VehicleServiceHistory
        public HttpResponseMessage Get()
        {
            var vehicleServiceHistories = _dataAccessVehicleServiceHistory.GetVehicleHistoryServices();

            if (vehicleServiceHistories.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
            }

            return Request.CreateResponse(HttpStatusCode.OK, vehicleServiceHistories);
        }

        // GET api/VehicleServiceHistory/5
        public HttpResponseMessage Get(Guid id)
        {
            try
            {
                var vehicleServiceHistory = _dataAccessVehicleServiceHistory.GetVehicleServiceHistoryById(id);

                if (vehicleServiceHistory == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No Vehicle Service History with that Id");
                }

                return Request.CreateResponse(HttpStatusCode.OK, vehicleServiceHistory);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST api/VehicleServiceHistory
        public HttpResponseMessage Post([FromBody] VehicleServiceHistory vehicleServiceHistory)
        {
            try
            {
                _dataAccessVehicleServiceHistory.AddVehicleServiceHistory(vehicleServiceHistory);

                var response = Request.CreateResponse(HttpStatusCode.Created, vehicleServiceHistory);
                response.Headers.Location = new Uri(Request.RequestUri, $"/api/VehicleServiceHistory/{vehicleServiceHistory.Id}");
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // PUT api/VehicleServiceHistory/5
        public HttpResponseMessage Put(Guid id, [FromBody] VehicleServiceHistory vehicleServiceHistory)
        {
            try
            {
                var existingVehicleServiceHistory = _dataAccessVehicleServiceHistory.GetVehicleServiceHistoryById(id);

                if (existingVehicleServiceHistory == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Vehicle Service History not found");
                }

                _dataAccessVehicleServiceHistory.UpdateVehicleServiceHistory(vehicleServiceHistory);

                return Request.CreateResponse(HttpStatusCode.OK, vehicleServiceHistory);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE api/VehicleServiceHistory/5
        public HttpResponseMessage Delete(Guid id)
        {
            try
            {
                var vehicleServiceHistory = _dataAccessVehicleServiceHistory.GetVehicleServiceHistoryById(id);

                if (vehicleServiceHistory == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Vehicle Service History not found");
                }

                _dataAccessVehicleServiceHistory.DeleteVehicleServiceHistory(id);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
