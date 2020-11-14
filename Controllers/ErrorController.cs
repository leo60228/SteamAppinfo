using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;

namespace SteamAppinfo.Controllers {
    [ApiController]
    public class ErrorController : ControllerBase {
        [Route("/error")]
        public IActionResult Error() {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var error = context.Error as WebException;
            if (error != null) {
                var type = error.GetBaseException().GetType().FullName;
                string typeUri;
                if (type.StartsWith("System.") || type.StartsWith("Microsoft.")) {
                    typeUri = $"https://docs.microsoft.com/en-us/dotnet/api/{type}";
                } else {
                    typeUri = $"exception:{type}";
                }
                return Problem(detail: error.Message, type: typeUri);
            } else {
                return Problem();
            }
        }
    }
}
