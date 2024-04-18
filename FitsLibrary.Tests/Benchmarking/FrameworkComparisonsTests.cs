using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using FitsLibrary.DocumentParts;
using nom.tam.fits;

namespace FitsLibrary.Tests.Benchmarking;

[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net80, launchCount: 5, warmupCount: 5, iterationCount: 5)]
[MemoryDiagnoser]
public class FrameworkComparisonsTests
{
    [Benchmark]
    public void CSharpFITS()
    {
        Fits f = new Fits("/home/cobra/M_101_Light_L_120_secs_2023-05-26T23-00-11_001.fits");
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
        var reader = new FitsDocumentReader();
        var document = await reader.ReadAsync("/home/cobra/M_101_Light_L_120_secs_2023-05-26T23-00-11_001.fits");
        var content = (ImageHeaderDataUnit<float>)document.HeaderDataUnits[0];

        for (var x = 0; x < document.HeaderDataUnits[0].Header.AxisSizes[0]; x++)
        {
            for (var y = 0; y < document.HeaderDataUnits[0].Header.AxisSizes[1]; y++)
            {
                var val = content.Data.GetValueAt(x, y);
            }
        }
    }
}
