using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using VerticalSliceSample.Api.Authentication.Services;
using VerticalSliceSample.Api.Common.Api;
using VerticalSliceSample.Api.Common.Api.Extensions;
using VerticalSliceSample.Api.Common.Api.Results;
using VerticalSliceSample.Api.Database;
using VerticalSliceSample.Api.Database.Entities;

namespace VerticalSliceSample.Api.Authentication.Endpoints;

public class Signup : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/signup", Handle)
        .WithName("Signup")
        .WithTags("Autenticacao")
        .WithSummary("Creates a new user account")
        .WithRequestValidation<Request>();

    public record Request(string Username, string Password, string Name);
    public record Response(string Token);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    private static async Task<Results<Ok<Response>, ValidationError>> Handle(
        Request request,
        ApplicationDbContext database,
        IJwtService jwtService,
        IPasswordHasherService hasher,
        CancellationToken cancellationToken)
    {
        var isUsernameTaken = await database.Users
            .AnyAsync(x => x.Username == request.Username, cancellationToken);

        if (isUsernameTaken)
        {
            return new ValidationError("Username is already taken");
        }

        var passwordHash = hasher.HashPassword(request.Password);
        var user = new User
        {
            Username = request.Username,
            Password = passwordHash,
            DisplayName = request.Name
        };

        await database.Users.AddAsync(user, cancellationToken);
        await database.SaveChangesAsync(cancellationToken);

        var token = jwtService.GenerateAccessToken(user);
        var response = new Response(token);
        return TypedResults.Ok(response);
    }
}
