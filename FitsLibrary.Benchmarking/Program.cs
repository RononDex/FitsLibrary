using BenchmarkDotNet.Running;
using FitsLibrary.Tests.SampleFiles;

namespace FitsLibrary.Benchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            _ = BenchmarkSwitcher.FromAssembly(typeof(SampleFilesTests).Assembly).Run(args);
        }
    }
}
