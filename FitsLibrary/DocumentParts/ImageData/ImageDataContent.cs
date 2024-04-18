
using System;
using System.Numerics;

namespace FitsLibrary.DocumentParts.ImageData;

public class ImageDataContent<T> : DataContent where T : INumber<T>
{
    private int[] AxisIndexFactors { get; }
    private int[] AxisSizes { get; }

    /// <summary>
    /// The main data content of the image block
    /// </summary>
    public Memory<T> RawData { get; private set; }

    public ImageDataContent(int[] axisSizes, Memory<T> rawData)
    {
        this.AxisSizes = axisSizes;
        this.RawData = rawData;

        this.AxisIndexFactors = new int[this.AxisSizes.Length];
        this.AxisIndexFactors[0] = 1;

        for (var i = 1; i < this.AxisIndexFactors.Length; i++)
        {
            this.AxisIndexFactors[i] = this.AxisIndexFactors[i - 1] * this.AxisSizes[i];
        }
    }

    /// <summary>
    /// Returns the value at the given coordinates as a byte
    /// </summary>
    /// <param name="coordinates">coordinates inside the multi dimensional array</param>
    public T GetValueAt(params int[] coordinates)
    {
        // TODO: Maybe move to different data structure for faster access code
        var index = GetIndexByCoordinates(coordinates);
        return this.RawData.Span[index];
    }

    private int GetIndexByCoordinates(params int[] coordinates)
    {
        var index = 0;
        for (var i = 0; i < coordinates.Length; i++)
        {
            index += coordinates[i] * this.AxisIndexFactors[i];
        }

        return index;
    }
}
