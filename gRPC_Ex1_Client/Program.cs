using Calculator;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gRPC_Ex1_Client
{
    internal class Program
    {
        private const string Target = "localhost:50051";

        private static async Task Main(string[] args)
        {
            Channel channel = new(Target, ChannelCredentials.Insecure);

            await channel.ConnectAsync().ContinueWith((task) =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    Console.WriteLine("Cliente conectado com sucesso.");
                }
            });

            var client = new CalculatorService.CalculatorServiceClient(channel);

            UnaryExample(client);
            await ServerStreamingExample(client);
            await ClientStreamingExample(client);
            await BiDirectionaltreamingExample(client);

            channel.ShutdownAsync().Wait();

            Console.ReadKey();
        }

        private static async Task BiDirectionaltreamingExample(CalculatorService.CalculatorServiceClient client)
        {
            Console.WriteLine("");
            Console.WriteLine($"#### Bi-Directional Streaming ####");
            Console.WriteLine("");

            var stream = client.FindMaximum();

            var requestTask = RequestFindMaximumAsync(stream);
            var responseTask = ResponseFindMaximumAsync(stream);

            await Task.WhenAll(requestTask, responseTask);
        }

        private static async Task ClientStreamingExample(CalculatorService.CalculatorServiceClient client)
        {
            Console.WriteLine("");
            Console.WriteLine($"#### Client Streaming ####");
            Console.WriteLine("");

            var clientStream = client.ComputeAverage();

            foreach (var item in Enumerable.Range(1, 4))
            {
                await clientStream.RequestStream.WriteAsync(new ComputeAverageRequest
                {
                    Val = item
                });
            }

            await clientStream.RequestStream.CompleteAsync();
            var response = clientStream.ResponseAsync;
            Console.WriteLine($"Resultado: {response.Result}");
        }

        private static async Task RequestFindMaximumAsync(AsyncDuplexStreamingCall<FindMaximumRequest, FindMaximumResponse> stream)
        {
            var values = new List<int> { 1, 5, 3, 6, 2, 20 };

            foreach (var item in values)
            {
                await stream.RequestStream.WriteAsync(new FindMaximumRequest
                {
                    Val = item
                });
            }

            await stream.RequestStream.CompleteAsync();
        }

        private static async Task ResponseFindMaximumAsync(AsyncDuplexStreamingCall<FindMaximumRequest, FindMaximumResponse> stream)
        {
            while (await stream.ResponseStream.MoveNext())
            {
                Console.WriteLine($"Valor máximo: {stream.ResponseStream.Current.Maximum}");
            }
        }

        private static async Task ServerStreamingExample(CalculatorService.CalculatorServiceClient client)
        {
            Console.WriteLine("");
            Console.WriteLine($"#### Server Streaming ####");
            Console.WriteLine("");

            var request = new PrimeNumberRequest { Number = 120 };

            var response = client.PrimeNumberDecomposition(request);

            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.Result.ToString());
            }
        }

        private static void UnaryExample(CalculatorService.CalculatorServiceClient client)
        {
            Console.WriteLine("");
            Console.WriteLine($"#### Unary Streaming ####");
            Console.WriteLine("");

            var request = new SumRequest
            {
                Val1 = 10,
                Val2 = 3
            };

            var response = client.Sum(request);
            Console.WriteLine(response.Result.ToString());
        }
    }
}
