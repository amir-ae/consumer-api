using Consumer.API.Client.Base;
using Consumer.API.Client.Resources;
using Consumer.API.Client.Resources.Interfaces;

namespace Consumer.API.Client
{
    public class ConsumerClient : IConsumerClient
    {
        public ConsumerClient(HttpClient client)
        {
            Product = new ProductResource(new BaseClient(client, client.BaseAddress!.ToString()));
            Customer = new CustomerResource(new BaseClient(client, client.BaseAddress!.ToString()));
        }
        public IProductResource Product { get; private set; }
        public ICustomerResource Customer { get; private set; }
    }
}