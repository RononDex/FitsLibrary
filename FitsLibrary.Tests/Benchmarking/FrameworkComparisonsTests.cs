using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using nom.tam.fits;

namespace FitsLibrary.Tests.Benchmarking;

[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net80, launchCount: 5, warmupCount: 5, iterationCount: 5)]
[MemoryDiagnoser]
public class FrameworkComparisonsTests
{
    [Benchmark]
    public void CSharpFITS()
    {
        Fits f = new Fits("/home/cobra/M81_M82_Panel2_Light_Ha_300_secs_2023-02-21T20-13-27_003.fits");
        BasicHDU[] hdus = f.Read();

        for (int i = 0; i < hdus.Length; i += 1)
        {
            hdus[i].Info();
            var data = hdus[i].Kernel;
            var boxed = data as System.Array[];
            for (var x = 0; x < boxed.Length; x++)
            {
                for (var y = 0; y < boxed[x].Length; y++)
                {
                    var val = (short)boxed[x].GetValue(y);
                }
            }
        }
    }

    [Benchmark]
    public async Task FitsLibraryAsync()
    {
        var reader = new FitsDocumentReader<short>();
        var document = await reader.ReadAsync("/home/cobra/M81_M82_Panel2_Light_Ha_300_secs_2023-02-21T20-13-27_003.fits");

        for (var x = 0; x < document.PrimaryHdu.Header.AxisSizes[0]; x++)
        {
            for (var y = 0; y < document.PrimaryHdu.Header.AxisSizes[1]; y++)
            {
                var val = document.PrimaryHdu.Data.GetValueAt(x, y);
            }
        }
    }
}
