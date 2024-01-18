using Benjamin.Andre.FeatureMatching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
/*
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});*/

app.MapGet("/", () => "Hello World!");

app.MapPost("/FeatureMatching", async ([FromForm] IFormFileCollection files) =>
{
    if (files.Count != 2)
        return Results.BadRequest();

    using var objectSourceStream = files[0].OpenReadStream();
    using var objectMemoryStream = new MemoryStream();
    objectSourceStream.CopyTo(objectMemoryStream);
    var imageObjectData = objectMemoryStream.ToArray();
    
    var imageSceneDataList = new List<byte[]>();

    for (int i = 1; i < files.Count; i++)
    {
        using var sceneSourceStream = files[i].OpenReadStream();
        using var sceneMemoryStream = new MemoryStream();
        sceneSourceStream.CopyTo(sceneMemoryStream);
        var imageSceneData = sceneMemoryStream.ToArray();
        imageSceneDataList.Add(imageSceneData);
    }

    var detectObjectInScenesResults = await new ObjectDetection().DetectObjectInScenesAsync(imageObjectData, imageSceneDataList);

    // La méthode ci-dessous permet de retourner une image depuis un tableau de bytes, var imageData = new bytes[];
    return Results.File(detectObjectInScenesResults[0].ImageData, "image/png");
}).DisableAntiforgery();

app.Run();