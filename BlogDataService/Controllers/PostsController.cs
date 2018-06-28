using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Requests;
using Common.Models;
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

            IReliableDictionary<string, Post> posts = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, Post>>("posts");

            using (ITransaction tx = this.stateManager.CreateTransaction())
            {
                Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<string, Post>> list = await posts.CreateEnumerableAsync(tx);

                Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<string, Post>> enumerator = list.GetAsyncEnumerator();

                List<KeyValuePair<string, Post>> result = new List<KeyValuePair<string, Post>>();

                while (await enumerator.MoveNextAsync(ct))
                {
                    result.Add(enumerator.Current);
                }

                return this.Json(result);
            }
        }

        // POST: api/Posts
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]PostsRequest request)
        {
            Post post = new Post();
            post.message = request.message;
            post.userName = request.userName;
            IReliableDictionary<string, Post> votesDictionary = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, Post>>("posts");

            using (ITransaction tx = this.stateManager.CreateTransaction())
            {
                await votesDictionary.AddAsync(tx, post.id.ToString(), post);
                await tx.CommitAsync();
            }

            return new OkResult();
        }
    }
}
