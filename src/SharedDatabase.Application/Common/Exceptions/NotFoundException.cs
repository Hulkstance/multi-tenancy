using System.Net;

namespace SharedDatabase.Application.Common.Exceptions;

public class NotFoundException(string message = "Not Found") : CustomException(message, null, HttpStatusCode.NotFound);
