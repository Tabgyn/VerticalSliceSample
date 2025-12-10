using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using VerticalSliceSample.Api.Authentication.Services;
using VerticalSliceSample.Api.Common.Api;
using VerticalSliceSample.Api.Common.Api.Extensions;
using VerticalSliceSample.Api.Database;

namespace VerticalSliceSample.Api.Authentication.Endpoints;

public class Login : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/login", Handle)
        .WithName("Login")
        .WithSummary("Logs in a user")
        .WithRequestValidation<Request>();

    public record Request(string Username, string Password);
    public record Response(string Token);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    private static async Task<Results<Ok<Response>, UnauthorizedHttpResult>> Handle(
        Request request,
        ApplicationDbContext database,
        IJwtService jwtService,
        IPasswordHasherService hasher,
        CancellationToken cancellationToken)
    {
        var user = await database.Users.SingleOrDefaultAsync(x => x.Username == request.Username, cancellationToken);

        if (user is null || !hasher.VerifyPassword(user.Password, request.Password))
        {
            return TypedResults.Unauthorized();
        }

        var token = jwtService.GenerateAccessToken(user);
        var response = new Response(token);
        return TypedResults.Ok(response);
    }
}
