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
    public class NhaTuyenDungsController : ControllerBase
    {
        private readonly DbQlcvContext _context;

        public NhaTuyenDungsController(DbQlcvContext context)
        {
            _context = context;
        }

        // GET: api/NhaTuyenDungs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NhaTuyenDung>>> GetNhaTuyenDungs()
        {
            return await _context.NhaTuyenDungs.ToListAsync();
        }

        // GET: api/NhaTuyenDungs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NhaTuyenDung>> GetNhaTuyenDung(int id)
        {
            var nhaTuyenDung = await _context.NhaTuyenDungs.FindAsync(id);

            if (nhaTuyenDung == null)
            {
                return NotFound();
            }

            return nhaTuyenDung;
        }

        // PUT: api/NhaTuyenDungs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNhaTuyenDung(int id, NhaTuyenDung nhaTuyenDung)
        {
            if (id != nhaTuyenDung.IdNhaTuyenDung)
            {
                return BadRequest();
            }

            _context.Entry(nhaTuyenDung).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NhaTuyenDungExists(id))
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

        // POST: api/NhaTuyenDungs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754


        // DELETE: api/NhaTuyenDungs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNhaTuyenDung(int id)
        {
            var nhaTuyenDung = await _context.NhaTuyenDungs.FindAsync(id);
            if (nhaTuyenDung == null)
            {
                return NotFound();
            }

            _context.NhaTuyenDungs.Remove(nhaTuyenDung);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NhaTuyenDungExists(int id)
        {
            return _context.NhaTuyenDungs.Any(e => e.IdNhaTuyenDung == id);
        }


        //[HttpPut("changepassword/{id}")]
        //public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        //{
        //    var nhaTuyenDung = await _context.NhaTuyenDungs.FindAsync(id);

        //    if (nhaTuyenDung == null)
        //    {
        //        return NotFound(new { message = "Nhà tuyển dụng không tồn tại." });
        //    }

        //    // Kiểm tra mật khẩu hiện tại
        //    if (nhaTuyenDung.MatKhau != request.CurrentPassword)
        //    {
        //        return BadRequest(new { message = "Mật khẩu hiện tại không chính xác." });
        //    }

        //    // Kiểm tra mật khẩu mới và mật khẩu xác nhận
        //    if (request.NewPassword != request.ConfirmPassword)
        //    {
        //        return BadRequest(new { message = "Mật khẩu mới và mật khẩu xác nhận không khớp." });
        //    }

        //    // Cập nhật mật khẩu mới
        //    nhaTuyenDung.MatKhau = request.NewPassword;

        //    _context.Entry(nhaTuyenDung).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi cập nhật mật khẩu." });
        //    }

        //    return Ok(new { message = "Mật khẩu đã được thay đổi thành công." });
        //}

        //public class ChangePasswordRequest
        //{
        //    public string CurrentPassword { get; set; }
        //    public string NewPassword { get; set; }
        //    public string ConfirmPassword { get; set; }
        //}



        [HttpPost]
        public async Task<ActionResult<NhaTuyenDung>> PostNhaTuyenDung(NhaTuyenDungDTO nhaTuyenDungDto)
        {
            // Tạo đối tượng NhaTuyenDung từ DTO
            var nhaTuyenDung = new NhaTuyenDung
            {
                IdNhaTuyenDung = GenerateUniqueId(),
                Email = nhaTuyenDungDto.Email,
                SoDienThoai = nhaTuyenDungDto.SoDienThoai,
                MatKhau = nhaTuyenDungDto.MatKhau,
                AnhHoSoUrl = nhaTuyenDungDto.AnhHoSoUrl,
                HoTen = nhaTuyenDungDto.HoTen,
                GioiTinh = nhaTuyenDungDto.GioiTinh,
                IdCongTy = nhaTuyenDungDto.IdCongTy
            };

            _context.NhaTuyenDungs.Add(nhaTuyenDung);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {

            }

            return CreatedAtAction("GetNhaTuyenDung", new { id = nhaTuyenDung.IdNhaTuyenDung }, nhaTuyenDung);
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