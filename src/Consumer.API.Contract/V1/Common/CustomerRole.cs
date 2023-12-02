using System.Text.Json.Serialization;

namespace Consumer.API.Contract.V1.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CustomerRole
{
    Владелец = 1,
    Owner = 1,
    Дилер = 8,
    Dealer = 8
}