using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api_sk1_01efc.Data;
using api_sk1_01efc.Models;
using api_sk1_01efc.ViewModels;

namespace api_sk1_01efc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ArticlesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Articles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListResult<Article>>>> GetArticles(string? search, string? title, string? content, string? order, int page = 0, int size = 10)
        {
            IQueryable<Article> articles = _context.Articles;
            //filtrace
            if (!String.IsNullOrEmpty(title)) 
                articles = articles.Where(x => x.Title.StartsWith(title));
            if (!String.IsNullOrEmpty(content)) 
                articles = articles.Where(x => x.Content.Contains(content));
            if (!String.IsNullOrEmpty(search))
                articles = articles.Where(x => x.Title.Contains(search) || x.Content.Contains(search));
            // řazení
            switch(order)
            {
                case "title": articles = articles.OrderBy(x => x.Title); break;
                case "title_desc": articles = articles.OrderByDescending(x => x.Title); break;
                case "id_desc": articles = articles.OrderByDescending(x => x.ArticleId); break;
                default: articles = articles.OrderBy(x => x.ArticleId); break;
            }
            // paginace
            if (page > 0) articles = articles.Skip((page - 1) * size);
            if (size > 0) articles = articles.Take(size);
            var result = new ListResult<Article>() { Data = await articles.ToListAsync()};
            return Ok(result);
        }

        // GET: api/Articles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            return article;
        }

        // PUT: api/Articles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticle(int id, Article article)
        {
            if (id != article.ArticleId)
            {
                return BadRequest();
            }
            /*
            _context.Entry(article).State = EntityState.Modified;
            */
            _context.Update(article);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Articles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Article>> PostArticle(Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArticle", new { id = article.ArticleId }, article);
        }

        // DELETE: api/Articles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ArticleId == id);
        }

        [HttpGet("{id}/Comments")]
        public async Task<ActionResult<List<Comment>>> GetArticleComments(int id)
        {
            /*
          var article = await _context.Articles.Include(a => a.Comments).FirstOrDefaultAsync(a => a.ArticleId == id);
            if (article == null) {
                return NotFound();
            }
            return article.Comments;
            */
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound("article not found");
            }
            _context.Entry(article).Collection(a => a.Comments).Load();
            return article.Comments.ToList();
        }

        [HttpPost("{id}/Comments")]
        public async Task<ActionResult<Comment>> PostArticleComment(int id, [FromBody] CommentIM input)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound("article not found");
            }
            var comment = new Comment
            {
                Text = input.Text,
                UserId = input.UserId,
                ArticleId = id
            };
            article.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetArticleComment", new { id = comment.CommentId }, comment);
        }
    }
}
