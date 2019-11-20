using System.IO;

namespace DFC.Composite.Paths.IntegrationTests.Extensions
{
    public static class StringExtensions
    {
        public static Stream AsStream(this string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
