using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Benjamin.Andre.FeatureMatching.Tests;

public class FeatureMatchingUnitTest
{
    [Fact]
    public async Task ObjectShouldBeDetectedCorrectly()
    {
        var executingPath = GetExecutingPath();
        var imageScenesData = new List<byte[]>();
        foreach (var imagePath in Directory.EnumerateFiles(Path.Combine(executingPath, "Scenes")))
        {
            var imageBytes = await File.ReadAllBytesAsync(imagePath);
            imageScenesData.Add(imageBytes);
        }

        var objectImageData = await File.ReadAllBytesAsync(Path.Combine(executingPath, "Andre-Benjamin-object.jpg"));

        var detectObjectInScenesResults =
            await new ObjectDetection().DetectObjectInScenesAsync(objectImageData, imageScenesData);

        Assert.Equal("[{\"X\":1473,\"Y\":1205},{\"X\":572,\"Y\":1690},{\"X\":1035,\"Y\":2360},{\"X\":1835,\"Y\":1821}]",
            JsonSerializer.Serialize(detectObjectInScenesResults[0].Points));

        Assert.Equal("[{\"X\":1990,\"Y\":831},{\"X\":1287,\"Y\":1746},{\"X\":2002,\"Y\":2315},{\"X\":2730,\"Y\":1315}]",
            JsonSerializer.Serialize(detectObjectInScenesResults[1].Points));
    }

    private static string GetExecutingPath()
        {
            var executingAssemblyPath =
                Assembly.GetExecutingAssembly().Location;
            var executingPath =
                Path.GetDirectoryName(executingAssemblyPath);
            return executingPath;
        }
}
