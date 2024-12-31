using System.Net;

namespace SharedDatabase.Application.Common.Exceptions;

public class UnauthorizedException(string message) : CustomException(message, null, HttpStatusCode.Unauthorized);
