﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Models.ForTableBased;
using TableBasedStrategy;

namespace multitenancy_db.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableBlogPostsController : ControllerBase
    {
        private readonly TableBasedContext _db;
        public TableBlogPostsController(TableBasedContext db)
        {
            _db = db;
        }

        //Note: tenant id can be pass in а different way
        //For instance, it could be a part of the token payload
        //In this case, it was placed as a query argument for simplifying example.
        //Check TenantResolver.cs out, and everything will become more clear. 


        [HttpGet]
        public ActionResult Blog(int page, int size, [FromQuery]  string tenantId)
        {
            var blogs = _db.Blogs
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
            return Ok(new { data = blogs });
        }

        [HttpPost]
        public ActionResult AddBlog([FromBody] BlogTable blog, [FromQuery] string tenantId)
        {
            blog.SetTenant(tenantId);
            _db.Blogs.Add(blog);
            _db.SaveChanges();
            return Ok();
        }

        [HttpGet("{blogId}/posts")]
        public ActionResult GetPosts([FromRoute] int blogId, [FromQuery]  string tenantId)
        {
            var posts = _db.Blogs
                .FirstOrDefault(p => p.Id == blogId)
                ?.Posts
                .ToList();
            return Ok(new { data = posts });
        }

        [HttpPost("{blogId}/posts")]
        public ActionResult AddPost([FromBody] PostTable post, [FromRoute] int blogId, [FromQuery] string tenantId)
        {
            var blog = _db.Blogs
                .FirstOrDefault(p => p.Id == blogId);
            blog?.Posts.Add(post);
            _db.SaveChanges();
            return Ok();
        }
    }
}