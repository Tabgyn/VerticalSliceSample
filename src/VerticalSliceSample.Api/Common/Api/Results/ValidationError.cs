using System.Reflection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;

namespace VerticalSliceSample.Api.Common.Api.Results;

public sealed class ValidationError(string errorMessage)
    : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IContentTypeHttpResult, IValueHttpResult, IValueHttpResult<HttpValidationProblemDetails>
{
    private readonly ValidationProblem _problem = TypedResults.ValidationProblem
        (
            errors: [],
            detail: errorMessage
        );

    public int? StatusCode => _problem.StatusCode;
    public string? ContentType => _problem.ContentType;
    public object? Value => _problem.ProblemDetails;
    HttpValidationProblemDetails? IValueHttpResult<HttpValidationProblemDetails>.Value => _problem.ProblemDetails;

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status400BadRequest, typeof(HttpValidationProblemDetails), ["application/problem+json"]));
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        await _problem.ExecuteAsync(httpContext);
    }
}
