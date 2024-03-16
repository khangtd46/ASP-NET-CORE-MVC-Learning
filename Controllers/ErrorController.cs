using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
	[AllowAnonymous]
	public class ErrorController : Controller
	{
		public ILogger<ErrorController> Logger { get; }

		public ErrorController(ILogger<ErrorController> logger)
        {
			Logger = logger;
		}
		//catch URL mà ko có controller và action method
        [Route("Error/{statuscode}")]
		public IActionResult PageError(int statuscode)
		{
			var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
			switch (statuscode)
			{
				case 404:
					ViewBag.ErrorMessage = "404 Error: The resource you are looking for is not found";
					Logger.LogWarning($"Error Path {statusCodeResult.OriginalPath} QS {statusCodeResult.OriginalQueryString}");
					ViewBag.Path = statusCodeResult.OriginalPath;
					ViewBag.QS = statusCodeResult.OriginalQueryString;
					break;
                case 405:
                    ViewBag.ErrorMessage = "405 Error: Method not allowed";
                    Logger.LogWarning($"Error Path {statusCodeResult.OriginalPath} QS {statusCodeResult.OriginalQueryString}");
                    ViewBag.Path = statusCodeResult.OriginalPath;
                    ViewBag.QS = statusCodeResult.OriginalQueryString;
                    break;
            }

			return View("NotFound");
		}

		//Catch khi có exception
		[Route("Error")]
		public IActionResult Error(int statuscode)
		{
			var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
			ViewBag.ExceptionPath = exceptionDetails.Path;
			ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
			ViewBag.StackTrace = exceptionDetails.Error.StackTrace;
			Logger.LogError($"Exception Path: {exceptionDetails.Path} Exception Message {exceptionDetails.Error.Message}");
			return View("Error");
		}
	}
}
