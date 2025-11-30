using fs_2025_a_api_demo_002.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace fs_2025_a_api_demo_002.Controllers
{
    [ApiVersion("1.0")]
    public class StationsV1Controller : StationsControllerBase
    {
        public StationsV1Controller(JsonDataService data, IMemoryCache cache)
            : base(data, cache) { }
    }

}
