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
    public class HoSoDaNopsController : ControllerBase
    {
        private readonly DbQlcvContext _context;

        public HoSoDaNopsController(DbQlcvContext context)
        {
            _context = context;
        }

        // GET: api/HoSoDaNops
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoSoDaNop>>> GetHoSoDaNops()
        {
            return await _context.HoSoDaNops.ToListAsync();
        }

        // GET: api/HoSoDaNops/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HoSoDaNop>> GetHoSoDaNop(int id)
        {
            var hoSoDaNop = await _context.HoSoDaNops.FindAsync(id);

            if (hoSoDaNop == null)
            {
                return NotFound();
            }

            return hoSoDaNop;
        }

        // PUT: api/HoSoDaNops/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHoSoDaNop(int id, HoSoDaNop hoSoDaNop)
        {
            if (id != hoSoDaNop.IdHoSoDaNop)
            {
                return BadRequest();
            }

            _context.Entry(hoSoDaNop).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HoSoDaNopExists(id))
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

        // POST: api/HoSoDaNops
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HoSoDaNop>> PostHoSoDaNop(HoSoDaNopDAO hsDAO)
        {
            // Kiểm tra dữ liệu đầu vào (validation cơ bản)
            if (hsDAO == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }
            int idHS = GenerateUniqueId();

            // Chuyển đổi dữ liệu từ DTO (Data Transfer Object) sang Entity
            var hs = new HoSoDaNop
            {
                IdHoSoDaNop = idHS,
                ThoiGianNop = hsDAO.thoiGianNop,
                TrangThai = hsDAO.trangThai,
                IdChiTietTuyenDung = hsDAO.idChiTietTuyenDung,
                IdUngVien = hsDAO.idUngVien,

            };  

            try
            {
                _context.HoSoDaNops.Add(hs);
                await _context.SaveChangesAsync();


                return CreatedAtAction("GetHoSoDaNop", new { id = hs.IdHoSoDaNop }, hs);
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

        // DELETE: api/HoSoDaNops/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoSoDaNop(int id)
        {
            var hoSoDaNop = await _context.HoSoDaNops.FindAsync(id);
            if (hoSoDaNop == null)
            {
                return NotFound();
            }

            _context.HoSoDaNops.Remove(hoSoDaNop);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HoSoDaNopExists(int id)
        {
            return _context.HoSoDaNops.Any(e => e.IdHoSoDaNop == id);
        }
        //Update trạng thái cv
        [HttpPost("UpdateTrangThai")]
        public async Task<IActionResult> UpdateTrangThai(int idHoSo, string trangThai)
        {
            try
            {
                var hoSo = await _context.HoSoDaNops.FirstOrDefaultAsync(h => h.IdHoSoDaNop == idHoSo);
                if (hoSo == null)
                {
                    return NotFound(new { Success = false, Message = "Không tìm thấy hồ sơ." });
                }


                hoSo.TrangThai = trangThai;
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Cập nhật trạng thái thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Lỗi server: {ex.Message}" });
            }
        }

        // DemCVUngTuyenMoi
        [HttpGet("DemCVUngTuyenMoi")]
        public IActionResult GetPendingCVCount()
        {
            int pendingCVCount = _context.HoSoDaNops.Count(cv => cv.TrangThai == "Chờ duyệt");
            return Ok(new { PendingCVCount = pendingCVCount });
        }

        // DemCVDaTiepNhan
        [HttpGet("DemCVDaTiepNhan")]
        public IActionResult GetApprovedCVCount()
        {
            int approvedCVCount = _context.HoSoDaNops.Count(cv => cv.TrangThai == "Đã duyệt");
            return Ok(new { ApprovedCVCount = approvedCVCount });
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


        [HttpGet("UngVien/{idUV}")]
        public async Task<IActionResult> GetAllApplyJobs(int idUV)
        {
            var applyJobs = await _context.HoSoDaNops
                .Where(l => l.IdUngVien == idUV)
                .ToListAsync();
            var cv = await _context.HoSoCvs
                .FirstOrDefaultAsync(c => c.IdUngVien == idUV);
            if (!applyJobs.Any())
            {
                return NotFound("Ứng viên chưa nộp tin tuyển dụng nào.");
            }

            var result = new List<object>();

            foreach (var job in applyJobs)
            {
                //chi tiêt tuyen dung
                var jobDetail = await _context.ChiTietTuyenDungs
                    .FirstOrDefaultAsync(c => c.IdChiTietTuyenDung == job.IdChiTietTuyenDung);


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
                    tieuDeTin = jobDetail.TieuDeTin,
                    tenCongTy = company.TenCongTy,
                    thoiGianNop = job.ThoiGianNop,
                    diaDiemLamViec = jobDetail.DiaDiemLamViecCuThe,
                    mucLuongTu = jobDetail.MucLuongTu,
                    mucLuongToi = jobDetail.MucLuongToi,
                    idCongTy = company.IdCongTy,
                    logoUrl = company.LogoUrl,
                    trangThai = job.TrangThai,
                    cvUrl = cv.FileUrl
                });
            }

            return Ok(result);
        }
    }
}
