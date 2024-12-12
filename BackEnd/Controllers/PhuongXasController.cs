﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd.Models;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhuongXasController : ControllerBase
    {
        private readonly DbQlcvContext _context;

        public PhuongXasController(DbQlcvContext context)
        {
            _context = context;
        }

        // GET: api/PhuongXas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhuongXa>>> GetPhuongXas()
        {
            return await _context.PhuongXas.ToListAsync();
        }

        // GET: api/PhuongXas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhuongXa>> GetPhuongXa(int id)
        {
            var phuongXa = await _context.PhuongXas.FindAsync(id);

            if (phuongXa == null)
            {
                return NotFound();
            }

            return phuongXa;
        }

        // PUT: api/PhuongXas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhuongXa(int id, PhuongXa phuongXa)
        {
            if (id != phuongXa.IdPhuongXa)
            {
                return BadRequest();
            }

            _context.Entry(phuongXa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhuongXaExists(id))
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

        // POST: api/PhuongXas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PhuongXa>> PostPhuongXa(PhuongXa phuongXa)
        {
            _context.PhuongXas.Add(phuongXa);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PhuongXaExists(phuongXa.IdPhuongXa))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPhuongXa", new { id = phuongXa.IdPhuongXa }, phuongXa);
        }

        // DELETE: api/PhuongXas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhuongXa(int id)
        {
            var phuongXa = await _context.PhuongXas.FindAsync(id);
            if (phuongXa == null)
            {
                return NotFound();
            }

            _context.PhuongXas.Remove(phuongXa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhuongXaExists(int id)
        {
            return _context.PhuongXas.Any(e => e.IdPhuongXa == id);
        }
    }
}
