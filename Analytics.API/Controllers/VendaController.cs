using Analytics.Application.Constants;
using Analytics.Application.DTOs;
using Analytics.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers
{
    [ApiController]
    [Route("vendas")]
    public class VendaController : ControllerBase
    {
        private readonly IMessageBus _messageBus;

        public VendaController(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [HttpPost]
        public async Task<IActionResult> CriarVenda([FromBody] VendaDto venda)
        {
            await _messageBus.PublishAsync(QueueNames.VendasNormal, venda);
            return Accepted(); 
        }
    }
}
