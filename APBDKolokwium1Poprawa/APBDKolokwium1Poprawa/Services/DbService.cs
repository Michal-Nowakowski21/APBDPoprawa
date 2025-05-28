using APBDKolokwium1Poprawa.ModelsDTOs;
using Microsoft.Data.SqlClient;

namespace APBDKolokwium1Poprawa.Services;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;

    public DbService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<List<ClientRentalDTO>> getClientsRentals(int clientId)
    {
        string connectionString = _configuration.GetConnectionString("Default");
        string command = "select c.VIN, co.Name, m.Name, cr.DateFrom, cr.DateTo, cr.TotalPrice from cars c join colors co on c.ID = co.ID join models m on co.ID = m.ID join car_rentals cr on c.ID = cr.CarID where cr.ClientID = @id  ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@id", clientId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var rentals = new List<ClientRentalDTO>();
                    while (await reader.ReadAsync())
                    {
                        rentals.Add(new ClientRentalDTO()
                        {
                            Vin = reader.GetString(0),
                            Color = reader.GetString(1),
                            Model = reader.GetString(2),
                            DateFrom = reader.GetDateTime(3),
                            DateTo = reader.GetDateTime(4),
                            TotalPrice = reader.GetInt32(5),
                        });
                            
                        



                    }
                    return rentals;

                }
            }
        }
         
    }
    public async Task<ClientsDTO> getClients(int clientId)
    { 
        var rentals = await getClientsRentals(clientId);
       string connectionString = _configuration.GetConnectionString("Default");
        
        string command = "select c.ID, c.FirstName, c.LastName, c.Address from clients c where c.ID = @id";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@id", clientId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var client = new ClientsDTO()
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Address = reader.GetString(3),
                            Rentals = rentals
                        };

                        return client;
                    };

                    
                }
            }
        }
        return null;  
    }

    public async Task<bool> CarExist(int carId)
    {
        string connectionString = _configuration.GetConnectionString("Default");
        string command = "select c.ID from cars c where ID = @id";

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@id", carId);
                var reader = await cmd.ExecuteReaderAsync();
                if (reader == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
        }
    }

    public async Task<int > createClient(ClientPostDTO client)
    {
        string connectionString = _configuration.GetConnectionString("Default");
        string command =
            "insert into clients(FirstName, LastName, Address) values (@firstName, @lastName, @address);SELECT CAST(SCOPE_IDENTITY() as int);";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@firstName", client.Client.FirstName);
                cmd.Parameters.AddWithValue("@lastName", client.Client.LastName);
                cmd.Parameters.AddWithValue("@address", client.Client.Address);
                var id = (int)await cmd.ExecuteScalarAsync();
                return id;            
            }
        }
    }

    public async Task<int> getCarPrice(int carId)
    {
        string connectionString = _configuration.GetConnectionString("Default");
        string command = "select c.PricePerDay from cars c where c.ID = @id ";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@id", carId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        return reader.GetInt32(0);
                    }
                }

                return 0;
            }
        }
    }
    

    public async Task createRental(ClientPostDTO client)
    {
        var id  = await createClient(client);
        string connectionString = _configuration.GetConnectionString("Default");
        string command =
            "insert into car_rentals(ClientID, CarID,DateFrom, DateTo, TotalPrice) values (@clientID, @carID, @dateFrom, @dateTo, @totalPrice)";
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            var totalPrice = await getCarPrice(client.CarID);
            totalPrice = totalPrice * (client.DateTo.DayOfYear-client.DateFrom.DayOfYear);
            await conn.OpenAsync();
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.Parameters.AddWithValue("@clientID",id);
                cmd.Parameters.AddWithValue("@carID", client.CarID);
                cmd.Parameters.AddWithValue("@dateFrom", client.DateFrom);
                cmd.Parameters.AddWithValue("@dateTo", client.DateTo);
                cmd.Parameters.AddWithValue("@totalPrice", totalPrice);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
   
    
