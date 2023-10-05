using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using WebProject.Models;

namespace WebProject.Data
{
    public class DataAccessVehicleServiceHistory
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["ABCD"].ToString();
        private readonly DataAccess _dataAccess;

        public DataAccessVehicleServiceHistory() { }

        public DataAccessVehicleServiceHistory(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public void InitializeDatabase()
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;

                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS VehicleServiceHistory (" +
                        "Id SERIAL PRIMARY KEY," +
                        "VehicleId UUID," +
                        "ServiceDate DATE," +
                        "ServiceDescription VARCHAR(255)," +
                        "ServiceCost DECIMAL," +
                        "FOREIGN KEY (VehicleId) REFERENCES Vehicle(Id))";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<VehicleServiceHistory> GetVehicleHistoryServices()
        {
            List<VehicleServiceHistory> vehicleServiceHistories = new List<VehicleServiceHistory>();

            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;

                    cmd.CommandText = "SELECT * FROM VehicleServiceHistory";

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var vehicleServiceHistory = MapVehicleServiceHistoryFromReader(reader);
                            vehicleServiceHistories.Add(vehicleServiceHistory);
                        }
                    }
                }
            }

            return vehicleServiceHistories;
        }

        public VehicleServiceHistory GetVehicleServiceHistoryById(Guid id)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;

                    cmd.CommandText = "SELECT * FROM VehicleServiceHistory WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapVehicleServiceHistoryFromReader(reader);
                        }
                    }
                }
            }

            return null;
        }

        public void AddVehicleServiceHistory(VehicleServiceHistory vehicleServiceHistory)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;

                    cmd.CommandText = "INSERT INTO VehicleServiceHistory (VehicleId, ServiceDate, ServiceDescription, ServiceCost)" +
                                      "VALUES (@vehicleId, @serviceDate, @serviceDescription, @serviceCost)";
                    AddVehicleServiceHistoryParameters(cmd, vehicleServiceHistory);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateVehicleServiceHistory(VehicleServiceHistory vehicleServiceHistory)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;

                    cmd.CommandText = "UPDATE VehicleServiceHistory SET ServiceDate = @serviceDate, " +
                                      "ServiceDescription = @serviceDescription, ServiceCost = @serviceCost " +
                                      "WHERE Id = @id";
                    AddVehicleServiceHistoryParameters(cmd, vehicleServiceHistory);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteVehicleServiceHistory(Guid id)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;

                    cmd.CommandText = "DELETE FROM VehicleServiceHistory WHERE Id = @Id";
                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private VehicleServiceHistory MapVehicleServiceHistoryFromReader(NpgsqlDataReader reader)
        {
            return new VehicleServiceHistory
            {
                Id = Guid.Parse(reader["VehicleId"].ToString()),
                VehicleId = Guid.Parse(reader["VehicleId"].ToString()),
                ServiceDate = Convert.ToDateTime(reader["ServiceDate"]).Date,
                ServiceDescription = Convert.ToString(reader["ServiceDescription"]),
                ServiceCost = Convert.ToDecimal(reader["ServiceCost"]),
                Vehicle = _dataAccess.GetVehicleById(Guid.Parse(reader["VehicleId"].ToString()))
            };
        }

        private void AddVehicleServiceHistoryParameters(NpgsqlCommand cmd, VehicleServiceHistory vehicleServiceHistory)
        {
            cmd.Parameters.AddWithValue("@vehicleId", vehicleServiceHistory.VehicleId);
            cmd.Parameters.AddWithValue("@serviceDate", vehicleServiceHistory.ServiceDate);
            cmd.Parameters.AddWithValue("@serviceDescription", vehicleServiceHistory.ServiceDescription);
            cmd.Parameters.AddWithValue("@serviceCost", vehicleServiceHistory.ServiceCost);
        }
    }
}
