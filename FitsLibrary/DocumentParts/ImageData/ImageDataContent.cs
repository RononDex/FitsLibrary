using System;
using System.Numerics;

namespace FitsLibrary.DocumentParts.ImageData;

public class ImageDataContent<T> : DataContent where T : INumber<T>
{
    private int[] AxisIndexFactors { get; }
    private int[] AxisSizes { get; }
    private int[] PreCalcedAxisBounds { get; }

    /// <summary>
    /// The main data content of the image block
    /// </summary>
    public Memory<T> RawData { get; private set; }

    public ImageDataContent(int[] axisSizes, Memory<T> rawData)
    {
        this.AxisSizes = axisSizes;
        this.RawData = rawData;

        this.AxisIndexFactors = new int[this.AxisSizes.Length];
        this.PreCalcedAxisBounds = new int[this.AxisSizes.Length];
        this.AxisIndexFactors[0] = 1;
        this.PreCalcedAxisBounds[0] = this.AxisSizes[0];

        for (var i = 1; i < this.AxisIndexFactors.Length; i++)
        {
            this.AxisIndexFactors[i] = this.AxisIndexFactors[i - 1] * this.AxisSizes[i];
            this.PreCalcedAxisBounds[i] = this.PreCalcedAxisBounds[i - 1] * this.AxisSizes[i];
        }
    }

    /// <summary>
    /// Returns the value at the given coordinates as a byte
    /// </summary>
    /// <param name="coordinates">coordinates inside the multi dimensional array</param>
    public T GetValueAt(params int[] coordinates)
    {
        // TODO: Maybe move to different data structure for faster access code
        // var index = 0;
        // return this.RawData.Span[GetIndexByCoordinates(coordinates)];
        var slice = this.RawData;
        for (var i = coordinates.Length - 1; i > 0; i--)
        {
            slice = slice.Slice(coordinates[i] * this.AxisIndexFactors[i], this.PreCalcedAxisBounds[i - 1]);
        }
        return slice.Span[coordinates[0]];
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
