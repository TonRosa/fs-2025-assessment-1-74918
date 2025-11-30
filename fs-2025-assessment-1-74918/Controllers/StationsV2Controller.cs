using fs_2025_a_api_demo_002.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace fs_2025_a_api_demo_002.Controllers
{
    [ApiVersion("2.0")]
    public class StationsV2Controller : StationsControllerBase
    {
        public StationsV2Controller(CosmosDataService data, IMemoryCache cache)
            : base(data, cache) { }
    }

}
