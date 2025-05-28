namespace APBDKolokwium1Poprawa.ModelsDTOs;

public class ClientsDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public List<ClientRentalDTO> Rentals { get; set; }
}