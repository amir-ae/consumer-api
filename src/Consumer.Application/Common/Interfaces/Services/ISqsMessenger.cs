using Amazon.SQS.Model;

namespace Consumer.Application.Common.Interfaces.Services;

public interface ISqsMessenger
{
    Task<SendMessageResponse> SendMessageAsync<T>(T message);
}