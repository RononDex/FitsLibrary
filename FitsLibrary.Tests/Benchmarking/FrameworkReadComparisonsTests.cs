using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using nom.tam.fits;

namespace FitsLibrary.Tests.Benchmarking;

[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net80, launchCount: 5, warmupCount: 5, iterationCount: 5)]
[MemoryDiagnoser]
public class FrameworkReadComparisonsTests
{
    const string FITS_FILE = "/home/cobra/M_101_Light_L_120_secs_2023-05-26T23-00-11_001.fits";

    [Benchmark]
    public void CSharpFITSRead()
    {
        Fits f = new Fits(FITS_FILE);
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
    public async Task FitsLibraryAsyncRead()
    {
        var reader = new FitsDocumentReader<short>();
        var document = await reader.ReadAsync(FITS_FILE);

        for (var x = 0; x < document.PrimaryHdu.Header.AxisSizes[0]; x++)
        {
            for (var y = 0; y < document.PrimaryHdu.Header.AxisSizes[1]; y++)
            {
                var val = document.PrimaryHdu.Data.GetValueAt(x, y);
            }
        }
    }
}
