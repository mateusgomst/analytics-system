using System.Data;
using System.Net.Http.Headers;

namespace Analytics.Domain.Entities;

public class Venda
{
    public Venda(decimal valor, string produto)
    {
        this.Produto = produto;
        this.Valor = valor;
        this.Data = DateTime.UtcNow;
    }
    public Guid Id { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public string Produto { get; set; } = string.Empty;
}
