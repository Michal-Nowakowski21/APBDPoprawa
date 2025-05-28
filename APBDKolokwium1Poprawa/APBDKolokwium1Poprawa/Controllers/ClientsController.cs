using APBDKolokwium1Poprawa.ModelsDTOs;
using APBDKolokwium1Poprawa.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBDKolokwium1Poprawa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IDbService _dbservice;

        public ClientsController(IDbService idbservice)
        {
            _dbservice = idbservice;
        }
        [HttpGet("{clientId}")]
        public async Task<IActionResult> Get(int clientId )
        {
            try
            {
                var client = await _dbservice.getClients(clientId);
                return Ok(client);
            }
            catch
            {
                return NotFound();
            }
         
            
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ClientPostDTO client)
        {
            var carExist = await _dbservice.CarExist(client.CarID);

            if (carExist == false)
            {
                return NotFound();
            }
            await _dbservice.createRental(client);
            return Ok();
        }
    }
}
