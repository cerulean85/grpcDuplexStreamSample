using Grpc.Net.Client;
using GrpcServer;


namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5037");
            var client = new Greeter.GreeterClient(channel);

            using var call = client.ChatNotification();

            var responseReaderTask = Task.Run(async Task () =>
            {
                while (await call.ResponseStream.MoveNext(CancellationToken.None))
                {
                    var note = call.ResponseStream.Current;
                    Console.WriteLine($"{note.Message}, received at {note.ReceiverAt}");
                }
            });

            foreach (var msg in Enumerable.Range(1, 1000))
            {
                var request = new NotificationsRequest()
                {
                    Message = $"Hello {msg}",
                    From = "Mom",
                    To = msg.ToString()
                };

                Console.WriteLine($"Send: {msg}");
                await call.RequestStream.WriteAsync(request);
            }

            await call.RequestStream.CompleteAsync();
            await responseReaderTask;

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}