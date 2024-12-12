using System;
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
    public class LuuTinTuyenDungsController : ControllerBase
    {
        private readonly DbQlcvContext _context;

        public LuuTinTuyenDungsController(DbQlcvContext context)
        {
            _context = context;
        }

        // GET: api/LuuTinTuyenDungs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LuuTinTuyenDung>>> GetLuuTinTuyenDungs()
        {
            return await _context.LuuTinTuyenDungs.ToListAsync();
        }

        // GET: api/LuuTinTuyenDungs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LuuTinTuyenDung>> GetLuuTinTuyenDung(int id)
        {
            var luuTinTuyenDung = await _context.LuuTinTuyenDungs.FindAsync(id);

            if (luuTinTuyenDung == null)
            {
                return NotFound();
            }

            return luuTinTuyenDung;
        }

        // PUT: api/LuuTinTuyenDungs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLuuTinTuyenDung(int id, LuuTinTuyenDung luuTinTuyenDung)
        {
            if (id != luuTinTuyenDung.IdLuuTinTuyenDung)
            {
                return BadRequest();
            }

            _context.Entry(luuTinTuyenDung).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LuuTinTuyenDungExists(id))
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

        // POST: api/LuuTinTuyenDungs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Route("api/[controller]")]
        [HttpPost]
        public async Task<ActionResult<LuuTinTuyenDung>> PostHoSoDaNop(LuuTinTuyenDungDAO hsDAO)
        {
            // Kiểm tra dữ liệu đầu vào (validation cơ bản)
            if (hsDAO == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }
            int idHS = GenerateUniqueId();

            // Chuyển đổi dữ liệu từ DTO (Data Transfer Object) sang Entity
            var hs = new LuuTinTuyenDung
            {
                IdLuuTinTuyenDung = idHS,
                ThoiGianLuuTin = hsDAO.ThoiGianLuuTin,
                IdChiTietTuyenDung = hsDAO.IdChiTietTuyenDung,
                IdUngVien = hsDAO.IdUngVien,

            };

            try
            {
                _context.LuuTinTuyenDungs.Add(hs);
                await _context.SaveChangesAsync();


                return CreatedAtAction("GetHoSoDaNop", new { id = hs.IdLuuTinTuyenDung }, hs);
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Lỗi khi lưu vào cơ sở dữ liệu: {dbEx.Message}");
                Console.WriteLine($"StackTrace: {dbEx.StackTrace}");
                return StatusCode(500, $"Lỗi: {dbEx.InnerException?.Message ?? dbEx.Message}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi không mong muốn: {ex.Message}");

                return StatusCode(500, "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại sau.");
            }
        }

        // DELETE: api/LuuTinTuyenDungs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLuuTinTuyenDung(int id)
        {
            var luuTinTuyenDung = await _context.LuuTinTuyenDungs.FindAsync(id);
            if (luuTinTuyenDung == null)
            {
                return NotFound();
            }

            _context.LuuTinTuyenDungs.Remove(luuTinTuyenDung);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LuuTinTuyenDungExists(int id)
        {
            return _context.LuuTinTuyenDungs.Any(e => e.IdLuuTinTuyenDung == id);
        }
        [HttpGet("ungvien/{idUngVien}/congty")]
        public async Task<IActionResult> GetAllSavedJobsByCandidateId(int idUngVien)
        {
            var savedJobs = await _context.LuuTinTuyenDungs
                .Where(l => l.IdUngVien == idUngVien)
                .ToListAsync();

            if (!savedJobs.Any())
            {
                return NotFound("Ứng viên chưa lưu tin tuyển dụng nào.");
            }

            var result = new List<object>();

            foreach (var savedJob in savedJobs)
            {
                var jobDetail = await _context.ChiTietTuyenDungs
                    .FirstOrDefaultAsync(c => c.IdChiTietTuyenDung == savedJob.IdChiTietTuyenDung);

                if (jobDetail == null) continue;

                var recruiter = await _context.NhaTuyenDungs
                    .FirstOrDefaultAsync(n => n.IdNhaTuyenDung == jobDetail.IdNhaTuyenDung);

                if (recruiter == null) continue;

                var company = await _context.CongTies
                    .FirstOrDefaultAsync(c => c.IdCongTy == recruiter.IdCongTy);

                if (company == null) continue;

                // Thêm vào kết quả
                result.Add(new
                {
                    idChiTietTuyenDung = jobDetail.IdChiTietTuyenDung,
                    idLuuTin = savedJob.IdLuuTinTuyenDung,
                    tieuDeTin = jobDetail.TieuDeTin,
                    tenCongTy = company.TenCongTy,
                    thoiGianLuuTin = savedJob.ThoiGianLuuTin,
                    diaDiemLamViec = jobDetail.DiaDiemLamViecCuThe,
                    mucLuongTu = jobDetail.MucLuongTu,
                    mucLuongToi = jobDetail.MucLuongToi,
                    hanNopHoSo = jobDetail.HanNopHoSo,
                    idCongTy = company.IdCongTy,
                    logoUrl = company.LogoUrl
                });
            }

            return Ok(result);
        }
        private static object lockObject = new object();
        public static int GenerateUniqueId()
        {
            lock (lockObject)
            {
                long milliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                return (int)(milliseconds % int.MaxValue);
            }
        }
    }
}
