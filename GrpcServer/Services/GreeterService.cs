using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServer;

namespace GrpcServer.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task ChatNotification(
            IAsyncStreamReader<NotificationsRequest> requestStream,
            IServerStreamWriter<NotificationsResponse> responseStream,
            ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var request = requestStream.Current;
                var now = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow);
                var reply = new NotificationsResponse()
                {
                    Message = $"Hi {request.From}!, You have sent Message \"{request.Message}\" to {request.To}",
                    ReceiverAt = now,
                };

                await responseStream.WriteAsync(reply);
            }
        }
            
    }
}
