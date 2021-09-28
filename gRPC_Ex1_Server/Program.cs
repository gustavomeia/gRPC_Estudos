using Calculator;
using Grpc.Core;
using System;
using System.IO;

namespace gRPC_Ex1_Server
{
    internal class Program
    {
        private const int Port = 50051;
        private const string host = "localhost";
        private static void Main(string[] args)
        {
            Server server = null;

            try
            {
                server = new Server
                {
                    Services = { CalculatorService.BindService(new CalculatorServiceImpl()) },
                    Ports = { new ServerPort(host, Port, ServerCredentials.Insecure) }
                };

                server.Start();
                Console.WriteLine($"Server rodando na porta: {Port}");
                Console.ReadKey();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Server falhou na inicialização: {ex.Message}");
                throw;
            }
            finally
            {
                server?.ShutdownAsync().Wait();
            }
        }
    }
}
