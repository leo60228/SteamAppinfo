using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SteamKit2;

namespace SteamAppinfo.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ProductInfoController : ControllerBase {
        private readonly ILogger<ProductInfoController> _logger;

        public ProductInfoController(ILogger<ProductInfoController> logger) {
            _logger = logger;
        }

        [HttpGet("{app}")]
        public async Task<SteamApps.PICSProductInfoCallback.PICSProductInfo> Get(uint app, [FromServices] IGetProductInfo steam) {
            _logger.LogInformation($"Got request for {app}");
            return await steam.ProductInfo(app);
        }
    }
}
