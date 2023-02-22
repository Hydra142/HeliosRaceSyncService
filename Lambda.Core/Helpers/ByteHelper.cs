namespace Lambda.Core.Helpers;

public static class ByteHelper
{
    public static byte[] ToByteArray<T>(T source)
    {
        using var memoryStream = new MemoryStream();

        var stringifiedSource = source?.ToString();
        if (stringifiedSource != null)
        {
            using var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(stringifiedSource);
        }

        return memoryStream.ToArray();
    }

    public static T ToObject<T>(byte[] source)
    {
        using var memoryStream = new MemoryStream(source);
        using var binaryReader = new BinaryReader(memoryStream);

        var stringifiedSource = binaryReader.ReadString();

        return (T)Convert.ChangeType(stringifiedSource, typeof(T));
    }
}
