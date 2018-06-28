using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace BlogDataService.Controllers
{
    [Produces("application/json")]
    [Route("api/Posts")]
    public class PostsController : Controller
    {
        private readonly IReliableStateManager stateManager;

        public PostsController(IReliableStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            CancellationToken ct = new CancellationToken();

            IReliableDictionary<string, string> posts = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, string>>("posts");

            return this.Json(new string[] { "value1", "value2" });
        }
    }
}
