using Analytics.Application.DTOs;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("vendas")]
public class VendaController : ControllerBase
{
    [HttpPost("new")] 
    public async Task<IActionResult> CreateVenda([FromBody] VendaDto vendaDto)
    {
        // Use "analytics-rabbitmq" se a aplicação estiver em container
        // Use "localhost" se a aplicação estiver rodando localmente
        var factory = new ConnectionFactory() 
        { 
            HostName = "analytics-rabbitmq", // ou "localhost" se app local
            Port = 5672,
            UserName = "admin",
            Password = "admin"
        };
        
        // Corrigido: usando await para método assíncrono
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        // Corrigido: usando await para método assíncrono
        await channel.QueueDeclareAsync(queue: "fila_vendas",
                                       durable: false,
                                       exclusive: false,
                                       autoDelete: false,
                                       arguments: null);

        var json = JsonSerializer.Serialize(vendaDto);
        var body = Encoding.UTF8.GetBytes(json);

        // Corrigido: usando BasicProperties explicitamente
        var properties = new BasicProperties();
        await channel.BasicPublishAsync(exchange: "",
                                       routingKey: "fila_vendas",
                                       mandatory: false,
                                       basicProperties: properties,
                                       body: body);

        return Ok(new { status = "Venda recebida e enviada para processamento" });
    }
}