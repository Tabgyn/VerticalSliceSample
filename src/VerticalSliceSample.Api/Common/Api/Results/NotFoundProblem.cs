using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace VerticalSliceSample.Api.Common.Api.Results;

public sealed class NotFoundProblem
    : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IContentTypeHttpResult, IValueHttpResult, IValueHttpResult<ProblemDetails>
{
    private readonly ProblemHttpResult _problem;

    public NotFoundProblem(string errorMessage)
    {
        _problem = TypedResults.Problem
        (
            statusCode: StatusCode,
            title: "Not Found",
            detail: errorMessage
        );
    }

    public int? StatusCode => StatusCodes.Status404NotFound;
    public string? ContentType => _problem.ContentType;
    public object? Value => _problem.ProblemDetails;
    ProblemDetails? IValueHttpResult<ProblemDetails>.Value => _problem.ProblemDetails;

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status404NotFound, typeof(ProblemDetails), ["application/problem+json"]));
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        await _problem.ExecuteAsync(httpContext);
    }
}
