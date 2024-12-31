using System.Net;

namespace SharedDatabase.Application.Common.Exceptions;

public class CustomException(string message, List<string>? errors = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : Exception(message)
{
    public List<string>? ErrorMessages { get; } = errors;

    public HttpStatusCode StatusCode { get; } = statusCode;
}
