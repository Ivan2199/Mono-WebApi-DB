// VehicleController.cs
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebProject.Data;
using WebProject.Models;
using Npgsql;

namespace WebProject.Controllers
{
    public class VehicleController : ApiController
    {
        private readonly DataAccess _dataAccess;

        public VehicleController()
        {
            _dataAccess = new DataAccess();
            _dataAccess.InitializeDatabase();
        }

        [HttpGet]
        public HttpResponseMessage Get(string vehicleType = null)
        {
            try
            {
                var vehicles = _dataAccess.GetVehicles(vehicleType);

                if (vehicles.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "List is empty");
                }

                return Request.CreateResponse(HttpStatusCode.OK, vehicles);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage Get(Guid id)
        {
            try
            {
                var vehicle = _dataAccess.GetVehicleById(id);

                if (vehicle == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No Vehicle with that Id");
                }

                return Request.CreateResponse(HttpStatusCode.OK, vehicle);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody] Vehicle vehicle)
        {
            try
            {
                if (vehicle == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "No data has been entered");
                }

                _dataAccess.AddVehicle(vehicle);

                return Request.CreateResponse(HttpStatusCode.Created, "Data has been entered successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public HttpResponseMessage Put(Guid id, [FromBody] Vehicle updatedVehicle)
        {
            try
            {
                var vehicleCurrent = _dataAccess.GetVehicleById(id);

                if (vehicleCurrent == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No vehicle with that id was found");
                }

                _dataAccess.UpdateVehicle(id, updatedVehicle);

                return Request.CreateResponse(HttpStatusCode.OK, "Data has been updated successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        public HttpResponseMessage Delete(Guid id)
        {
            try
            {
                var vehicle = _dataAccess.GetVehicleById(id);

                if (vehicle == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No vehicle with that id was found");
                }

                _dataAccess.DeleteVehicle(id);

                return Request.CreateResponse(HttpStatusCode.OK, "Vehicle has been deleted successfully");
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
