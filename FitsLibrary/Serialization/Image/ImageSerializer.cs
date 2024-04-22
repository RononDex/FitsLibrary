using System;
using System.IO.Pipelines;
using System.Numerics;
using System.Threading.Tasks;
using FitsLibrary.DocumentParts;
using FitsLibrary.DocumentParts.ImageData;
using FitsLibrary.Serialization.Image.ImageContentValueSerializer;

namespace FitsLibrary.Serialization.Image;

internal class ImageSerializer<T> : IContentSerializer where T : INumber<T>
{
    private const int CHUNK_SIZE = 2880;
    private const int WRITER_CHUNK_SIZE = CHUNK_SIZE * 2;

    public Task SerializeAsync(PipeWriter writer, ImageDataContent<T> dataContent, DocumentParts.Header header)
    {
        var valueWriter = GetContentValueWriter<T>(header.DataContentType);
        var numberOfBytesPerValue = Math.Abs((int)header.DataContentType / 8);
        var totalWrittenBytes = 0;
        var writtenBytesInBuffer = 0;
        var writerMemorySpan = writer.GetSpan(sizeHint: WRITER_CHUNK_SIZE);
        var dataSpan = dataContent.RawData.Span;

        // Write all the values
        for (var i = 0; i < dataContent.RawData.Length; i++)
        {
            valueWriter.WriteValue(dataSpan[i], writerMemorySpan.Slice(writtenBytesInBuffer, numberOfBytesPerValue));
            writtenBytesInBuffer += numberOfBytesPerValue;
            totalWrittenBytes += numberOfBytesPerValue;

            // Get the next writeable span if needed
            if (writtenBytesInBuffer == WRITER_CHUNK_SIZE || i == dataContent.RawData.Length - 1)
            {
                writer.Advance(writtenBytesInBuffer);
                writerMemorySpan = writer.GetSpan(sizeHint: WRITER_CHUNK_SIZE);
                writtenBytesInBuffer = 0;
            }
        }

        // Fill rest of stream with null values
        var modulo = totalWrittenBytes % CHUNK_SIZE;
        if (modulo != 0)
        {
            var bytesToFill = CHUNK_SIZE - modulo;
            writerMemorySpan = writer.GetSpan(sizeHint: bytesToFill);
            for (var i = 0; i < bytesToFill; i++)
            {
                writerMemorySpan[i] = 0;
            }

            writer.Advance(bytesToFill);
        }

        return writer.FlushAsync().AsTask();
    }

    async Task IContentSerializer.SerializeAsync(PipeWriter writer, DataContent dataContent, DocumentParts.Header header)
    {
        await SerializeAsync(writer, (ImageDataContent<T>)dataContent, header);
    }

    private static IImageContentValueSerializer<TData> GetContentValueWriter<TData>(DataContentType dataContentType) where TData : INumber<TData>
    {
        try
        {
            return dataContentType switch
            {
                DataContentType.DOUBLE => (IImageContentValueSerializer<TData>)new ImageContentValueSerializerDouble(),
                DataContentType.FLOAT => (IImageContentValueSerializer<TData>)new ImageContentValueSerializerFloat(),
                DataContentType.BYTE => (IImageContentValueSerializer<TData>)new ImageContentValueSerilaizerByte(),
                DataContentType.INTEGER => (IImageContentValueSerializer<TData>)new ImageContentValueSerializerInt(),
                DataContentType.LONG => (IImageContentValueSerializer<TData>)new ImageContentValueSerializerLong(),
                DataContentType.SHORT => (IImageContentValueSerializer<TData>)new ImageContentValueSerializerShort(),
                _ => throw new ArgumentException("Tried to serialize unimplemented datatype"),
            };
        }
        catch (InvalidCastException)
        {
            throw new ArgumentException($"Tried to parse a fits document of type {dataContentType} as {typeof(TData).Name}");
        }
    }
}

