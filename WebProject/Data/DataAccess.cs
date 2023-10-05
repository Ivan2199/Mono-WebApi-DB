using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using WebProject.Models;

namespace WebProject.Data
{
    public class DataAccess
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["ABCD"].ToString();

        public void InitializeDatabase()
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;

                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS Vehicle (" +
                        "Id UUID PRIMARY KEY," +
                        "VehicleType VARCHAR(255)," +
                        "VehicleBrand VARCHAR(255)," +
                        "YearOfProduction INT," +
                        "TopSpeed INT," +
                        "VehicleMileage INT," +
                        "VehicleOwner VARCHAR(255))";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Vehicle> GetVehicles(string vehicleType = null)
        {
            var vehicles = new List<Vehicle>();

            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;

                    if (vehicleType != null)
                    {
                        cmd.Parameters.AddWithValue("@type", vehicleType);
                        cmd.CommandText = "SELECT v.*, h.* FROM Vehicle v " +
                                          "LEFT JOIN VehicleServiceHistory h ON v.Id = h.VehicleId " +
                                          "WHERE v.VehicleType = @type";
                    }
                    else
                    {
                        cmd.CommandText = "SELECT v.*, h.* FROM Vehicle v " +
                                          "LEFT JOIN VehicleServiceHistory h ON v.Id = h.VehicleId";
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var vehicle = MapVehicleFromReader(reader);
                            vehicle.VehicleServiceHistory = MapServiceHistoryFromReader(reader);
                            vehicles.Add(vehicle);
                        }
                    }
                }
            }

            return vehicles;
        }

        public Vehicle GetVehicleById(Guid id)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;

                    cmd.CommandText = "SELECT * FROM Vehicle WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapVehicleFromReader(reader);
                        }
                    }
                }
            }

            return null;
        }

        public void AddVehicle(Vehicle vehicle)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;

                    cmd.CommandText = "INSERT INTO Vehicle (Id, VehicleType, VehicleBrand, YearOfProduction, TopSpeed, VehicleMileage, VehicleOwner)" +
                                      "VALUES (@id, @type, @brand, @year, @speed, @mileage, @owner)";
                    cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
                    AddVehicleParameters(cmd, vehicle);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateVehicle(Guid id, Vehicle updatedVehicle)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;

                    cmd.CommandText = "UPDATE Vehicle SET VehicleType = @type, VehicleBrand = @brand, " +
                                      "YearOfProduction = @year, TopSpeed = @speed, VehicleMileage = @mileage, " +
                                      "VehicleOwner = @owner WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    AddVehicleParameters(cmd, updatedVehicle);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteVehicle(Guid id)
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = con;

                    cmd.CommandText = "DELETE FROM Vehicle WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void AddVehicleParameters(NpgsqlCommand cmd, Vehicle vehicle)
        {
            cmd.Parameters.AddWithValue("@type", vehicle.VehicleType);
            cmd.Parameters.AddWithValue("@brand", vehicle.VehicleBrand);
            cmd.Parameters.AddWithValue("@year", vehicle.YearOfProduction);
            cmd.Parameters.AddWithValue("@speed", vehicle.TopSpeed);
            cmd.Parameters.AddWithValue("@mileage", vehicle.VehicleMileage);
            cmd.Parameters.AddWithValue("@owner", vehicle.VehicleOwner);
        }

        private Vehicle MapVehicleFromReader(NpgsqlDataReader reader)
        {
            DataAccessVehicleServiceHistory dataAccessVehicleServiceHistory = new DataAccessVehicleServiceHistory();
            return new Vehicle
            {
                Id = Guid.Parse(reader["Id"].ToString()),
                VehicleType = Convert.ToString(reader["VehicleType"]),
                VehicleBrand = Convert.ToString(reader["VehicleBrand"]),
                YearOfProduction = Convert.ToInt32(reader["YearOfProduction"]),
                TopSpeed = Convert.ToInt32(reader["TopSpeed"]),
                VehicleMileage = Convert.ToInt32(reader["VehicleMileage"]),
                VehicleOwner = Convert.ToString(reader["VehicleOwner"]),
                VehicleServiceHistory = MapServiceHistoryFromReader(reader)
            };
        }

        private VehicleServiceHistory MapVehicleServiceHistoryFromReader(NpgsqlDataReader reader)
        {
            DataAccess _dataAccess = new DataAccess();
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

        private List<VehicleServiceHistory> MapServiceHistoryFromReader(NpgsqlDataReader reader)
        {
            var serviceHistories = new List<VehicleServiceHistory>();

            while (reader.Read())
            {
                var serviceHistory = MapVehicleServiceHistoryFromReader(reader);
                serviceHistories.Add(serviceHistory);
            }

            return serviceHistories;
        }
    }
}
