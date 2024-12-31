using System.Net;

namespace SharedDatabase.Application.Common.Exceptions;

public class ForbiddenException(string message) : CustomException(message, null, HttpStatusCode.Forbidden);
