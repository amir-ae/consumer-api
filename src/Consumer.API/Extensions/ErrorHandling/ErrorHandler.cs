using ErrorOr;
using Serilog;

namespace Consumer.API.Extensions.ErrorHandling;

public class ErrorHandler : IErrorHandler
{
    public IResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return Results.Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }

    public IResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => error.Code == "ClientClosedRequest" ? 499 : StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        if (!string.IsNullOrEmpty(error.Description))
        {
            Log.Error(error.Description);
        }
        
        return TypedResults.Problem(statusCode: statusCode, detail: error.Description);
    }

    private IResult ValidationProblem(List<Error> errors)
    {
        var dict = new Dictionary<string, string[]>();

        foreach (var error in errors)
        {
            dict.Add(error.Code, new [] { error.Description });
            Log.Error(error.Description);
        }

        return TypedResults.ValidationProblem(errors: dict);
    }
}