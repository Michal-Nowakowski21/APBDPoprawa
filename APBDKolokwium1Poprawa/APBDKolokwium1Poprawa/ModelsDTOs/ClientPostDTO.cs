namespace APBDKolokwium1Poprawa.ModelsDTOs;

public class ClientPostDTO
{
    public ClientPost Client { get; set; }
    public int CarID { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}

public class ClientPost
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
}