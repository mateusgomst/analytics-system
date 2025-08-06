namespace Analytics.Domain.Entities;

public class AlertRule
{
    // | Campo       | Tipo sugerido          | Descrição                                                                                           |
    // | ----------- | ---------------------- | --------------------------------------------------------------------------------------------------- |
    // | `Id`        | `Guid`                 | Identificador único da regra                                                                        |
    // | `Name`      | `string`               | Nome descritivo da regra de alerta                                                                  |
    // | `Condition` | `string` (DSL ou JSON) | Regra condicional (ex: "total\_vendas > 1000") — pode ser uma mini-DSL ou JSON com operador e valor |
    // | `Channels`  | `List<string>`         | Canais de notificação (ex: \["email", "slack", "webhook"])                                          |

    public AlertRule() { }
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Condition { get; set; }
    public List<string> Channels { get; set; }
    
}