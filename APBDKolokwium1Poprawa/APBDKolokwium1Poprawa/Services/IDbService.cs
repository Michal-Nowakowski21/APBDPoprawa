using APBDKolokwium1Poprawa.ModelsDTOs;

namespace APBDKolokwium1Poprawa.Services;

public interface IDbService
{

    
    Task <ClientsDTO> getClients(int clientId);
    Task <bool> CarExist(int carId);
    Task <int >createClient(ClientPostDTO client);
    Task createRental(ClientPostDTO client);
    Task<int> getCarPrice(int carId);
}