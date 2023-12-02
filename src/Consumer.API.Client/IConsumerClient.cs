using Consumer.API.Client.Resources.Interfaces;

namespace Consumer.API.Client
{
    public interface IConsumerClient
    {
        IProductResource Product { get; }
        ICustomerResource Customer { get; }
    }
}