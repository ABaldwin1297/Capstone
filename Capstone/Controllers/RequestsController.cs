using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Capstone.Models;

namespace Capstone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
          if (_context.Requests == null)
          {
              return NotFound();
          }
            return await _context.Requests.ToListAsync();
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id)
        {
          if (_context.Requests == null)
          {
              return NotFound();
          }
            var request = await _context.Requests
                                        .Include(x => x.User)
                                        .Include(x => x.RequestLines)!
                                           .ThenInclude(x => x.Product)
                                        .SingleOrDefaultAsync(x => x.Id == id);
                                        
                                             

            if (request == null)
            {
                return NotFound();
            }

            return request;
        }


        [HttpGet("Reviews/{id}")]
        public async Task<ActionResult<IEnumerable<Request>>> Reviews(int id) {
              return await _context.Requests
                                          .Where(x => x.Status == "Review" && id != x.Id).ToListAsync();
        }




        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(id))
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


        [HttpPut("Review/{id}")]
        public async Task<IActionResult> ReviewRequest(int id, Request request) {
            request.Status = (request.Total <= 50) ? "Approved" : "Review";
            await _context.SaveChangesAsync();
            return await PutRequest(id, request);
        }


        [HttpPut("Approve/{id}")]
        public async Task<IActionResult> Approve(int id, Request request) {
            if(request.Status == null) {
                return NotFound();
            }
            var prevstatus = request.Status;
            if(prevstatus == "Approved") {
               return Ok();
            }
            if(prevstatus != "Approved") {
                request.Status = "Approved";
            }
            await _context.SaveChangesAsync();
            return await PutRequest(id, request);
        }


        [HttpPut("Reject/{id}")]
        public async Task<IActionResult> Reject(int id, Request request) {
            if (request.Status == null) {
                return NotFound();
            }
            var prevstatus = request.Status;
            if (prevstatus == "Rejected") {
                return Ok();
            }
            if (prevstatus != "Rejected") {
                request.Status = "Rejected";
            }
            await _context.SaveChangesAsync();
            return await PutRequest(id, request);
        }


        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request)
        {
          if (_context.Requests == null)
          {
              return Problem("Entity set 'AppDbContext.Requests'  is null.");
          }
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            if (_context.Requests == null)
            {
                return NotFound();
            }
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RequestExists(int id)
        {
            return (_context.Requests?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
