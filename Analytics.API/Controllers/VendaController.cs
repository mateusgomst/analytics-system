using Analytics.Application.DTOs;
using Analytics.Infrastructure.Messaging.RabbitMQ.Config; // Adicionado
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("vendas")]
public class VendaController : ControllerBase
{
    // O controller agora tem uma dependência da nossa classe de conexão.
    private readonly RabbitMQConnection _rabbitMQConnection;

    // O .NET vai injetar automaticamente a instância singleton de RabbitMQConnection que registramos no Program.cs.
    public VendaController(RabbitMQConnection rabbitMQConnection)
    {
        _rabbitMQConnection = rabbitMQConnection;
    }

    [HttpPost("new")] 
    public async Task<IActionResult> CreateVenda([FromBody] VendaDto vendaDto)
    {
        // A lógica de conexão foi removida daqui.
        // Agora, simplesmente usamos a conexão que foi injetada.
        // CORRIGIDO: Adicionado 'await' para aguardar a criação da conexão assíncrona.
        using var connection = await _rabbitMQConnection.CreateConnection();
        using var channel = await connection.CreateChannelAsync();

        // A declaração da fila e a publicação da mensagem permanecem iguais.
        await channel.QueueDeclareAsync(queue: "fila_vendas",
                                       durable: false,
                                       exclusive: false,
                                       autoDelete: false,
                                       arguments: null);

        var json = JsonSerializer.Serialize(vendaDto);
        var body = Encoding.UTF8.GetBytes(json);

        // O BasicProperties foi removido pois não era necessário para esta publicação simples.
        await channel.BasicPublishAsync(exchange: "",
                                       routingKey: "fila_vendas",
                                       body: body);

        return Ok(new { status = "Venda recebida e enviada para processamento" });
    }
}
