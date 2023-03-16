using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using FitsLibrary.Tests.SampleFiles;

namespace FitsLibrary.Benchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ManualConfig()
                .WithOptions(ConfigOptions.DisableOptimizationsValidator)
                .AddValidator(JitOptimizationsValidator.DontFailOnError)
                .AddLogger(ConsoleLogger.Default)
                .AddColumnProvider(DefaultColumnProviders.Instance);
            _ = BenchmarkSwitcher.FromAssembly(typeof(SampleFilesTests).Assembly).Run(args, config);
        }
    }
}
