namespace Analytics.Domain.Entities;

public class Venda
{
    public Guid Id { get; set; }
    public decimal Valor { get; set; }
    public DateTime Data { get; set; }
    public string Produto { get; set; } = string.Empty;
}
