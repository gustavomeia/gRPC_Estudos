using Calculator;
using Grpc.Core;
using System;
using System.Threading.Tasks;
using static Calculator.CalculatorService;

namespace gRPC_Ex1_Server
{
    internal class CalculatorServiceImpl : CalculatorServiceBase
    {
        public override Task<SumResponse> Sum(SumRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Request recebido: {request.Val1} + {request.Val2}");

            return Task.FromResult(new SumResponse
            {
                Result = request.Val1 + request.Val2
            });
        }

        public override async Task PrimeNumberDecomposition(
            PrimeNumberRequest request,
            IServerStreamWriter<PrimeNumberResponse> responseStream,
            ServerCallContext context)
        {
            Console.WriteLine($"Request recebido: {request.Number}");

            var number = request.Number;
            var divisor = 2;

            while (number > 1)
            {
                if (number % divisor == 0)
                {
                    number /= divisor;
                    Console.WriteLine($"Enviando resposta: {divisor}");
                    await responseStream.WriteAsync(new PrimeNumberResponse { Result = divisor });
                }
                else
                {
                    divisor++;
                }
            }
        }

        public override async Task<ComputeAverageResponse> ComputeAverage(
            IAsyncStreamReader<ComputeAverageRequest> requestStream,
            ServerCallContext context)
        {
            var result = 0.0;
            var index = 0;

            while (await requestStream.MoveNext())
            {
                Console.WriteLine($"Request recebido: {requestStream.Current.Val}");
                index++;
                result += requestStream.Current.Val;
            }

            result /= index;

            return new ComputeAverageResponse { Result = result };
        }

        public override async Task FindMaximum(
            IAsyncStreamReader<FindMaximumRequest> requestStream,
            IServerStreamWriter<FindMaximumResponse> responseStream,
            ServerCallContext context)
        {
            var maximum = int.MinValue;

            while (await requestStream.MoveNext())
            {
                Console.WriteLine($"Request recebido: {requestStream.Current.Val}");

                if (maximum < requestStream.Current.Val)
                {
                    maximum = requestStream.Current.Val;

                    Console.WriteLine($"Enviando response: {maximum}");
                    await responseStream.WriteAsync(new FindMaximumResponse { Maximum = maximum });
                }
            }
        }
    }
}
