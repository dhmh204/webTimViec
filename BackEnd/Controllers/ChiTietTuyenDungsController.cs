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
    public class ChiTietTuyenDungsController : ControllerBase
    {
        private readonly DbQlcvContext _context;

        public ChiTietTuyenDungsController(DbQlcvContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetChiTietTuyenDungs()
        {
            var tuyenDungs = await (from ct in _context.ChiTietTuyenDungs
                                    join nt in _context.NhaTuyenDungs on ct.IdNhaTuyenDung equals nt.IdNhaTuyenDung
                                    join c in _context.CongTies on nt.IdCongTy equals c.IdCongTy
                                    select new
                                    {
                                        id = ct.IdChiTietTuyenDung,
                                        TieuDeTin = ct.TieuDeTin,
                                        TenCongTy = c.TenCongTy,
                                        LogoUrl = c.LogoUrl,
                                        HanUngTuyen = (ct.HanNopHoSo.ToDateTime(TimeOnly.MinValue) - DateTime.Now.Date).Days,
                                        mucLuongTu = ct.MucLuongTu,
                                        MucLuongToi = ct.MucLuongToi,
                                        DiaDiemLamViecCuThe = ct.DiaDiemLamViecCuThe
                                    }).ToListAsync();

            return Ok(tuyenDungs);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ChiTietTuyenDung>> GetChiTietTuyenDung(int id)
        {
            var chiTietTuyenDung = await _context.ChiTietTuyenDungs.FindAsync(id);

            if (chiTietTuyenDung == null)
            {
                return NotFound();
            }

            return chiTietTuyenDung;
        }



        // GET: api/ChiTietTuyenDungs/5
        [HttpGet("search/{search}")]
        public async Task<ActionResult<IEnumerable<object>>> GetSearchResults(string search)
        {
            // Chuyển đổi từ khóa tìm kiếm thành chữ thường để tìm kiếm không phân biệt hoa thường
            var searchLower = search.ToLower();
            // Kiểm tra xem từ khóa có phải là số (ID) hay không
            bool isNumericSearch = int.TryParse(search, out int searchId);

            var tuyenDungs = await (from ct in _context.ChiTietTuyenDungs
                                    join nt in _context.NhaTuyenDungs on ct.IdNhaTuyenDung equals nt.IdNhaTuyenDung
                                    join c in _context.CongTies on nt.IdCongTy equals c.IdCongTy
                                    where (ct.TieuDeTin.ToLower().Contains(searchLower) ||
                                           c.TenCongTy.ToLower().Contains(searchLower) ||
                                           ct.MucLuongTu.ToString().Contains(searchLower) ||
                                           ct.MucLuongToi.ToString().Contains(searchLower) ||
                                           ct.DiaDiemLamViecCuThe.ToLower().Contains(searchLower) ||
                                           (isNumericSearch && ct.IdChiTietTuyenDung == searchId))

                                    select new
                                    {
                                        TieuDeTin = ct.TieuDeTin,
                                        TenCongTy = c.TenCongTy,
                                        LogoUrl = c.LogoUrl,
                                        MucLuongTu = ct.MucLuongTu,
                                        HanUngTuyen = (ct.HanNopHoSo.ToDateTime(TimeOnly.MinValue) - DateTime.Now.Date).Days,
                                        MucLuongToi = ct.MucLuongToi,
                                        DiaDiemLamViecCuThe = ct.DiaDiemLamViecCuThe,
                                        KinhNghiemLamViec = ct.KinhNghiem,
                                        MoTaCV = ct.MoTaCongViec,
                                        QuyenLoi = ct.QuyenLoiUngVien,
                                        cachthuc = ct.CachThucUngTuyen
                                    }).ToListAsync();

            if (tuyenDungs == null || tuyenDungs.Count == 0)
            {
                return NotFound("No results found.");
            }

            return Ok(tuyenDungs);
        }


        // PUT: api/ChiTietTuyenDungs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChiTietTuyenDung(int id, ChiTietTuyenDung chiTietTuyenDung)
        {
            if (id != chiTietTuyenDung.IdChiTietTuyenDung)
            {
                return BadRequest();
            }

            _context.Entry(chiTietTuyenDung).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChiTietTuyenDungExists(id))
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

        // POST: api/ChiTietTuyenDungs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ChiTietTuyenDung>> PostChiTietTuyenDung(ChiTietTuyenDung chiTietTuyenDung)
        {
            _context.ChiTietTuyenDungs.Add(chiTietTuyenDung);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ChiTietTuyenDungExists(chiTietTuyenDung.IdChiTietTuyenDung))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetChiTietTuyenDung", new { id = chiTietTuyenDung.IdChiTietTuyenDung }, chiTietTuyenDung);
        }

        // DELETE: api/ChiTietTuyenDungs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChiTietTuyenDung(int id)
        {
            var chiTietTuyenDung = await _context.ChiTietTuyenDungs.FindAsync(id);
            if (chiTietTuyenDung == null)
            {
                return NotFound();
            }

            _context.ChiTietTuyenDungs.Remove(chiTietTuyenDung);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ChiTietTuyenDungExists(int id)
        {
            return _context.ChiTietTuyenDungs.Any(e => e.IdChiTietTuyenDung == id);
        }
        [HttpPost("AddContent")]
        public async Task<ActionResult<ChiTietTuyenDung>> AddContent(AddContentRequest request)
        {
            // Generate a random ID and add the offset 16052005
            var random = new Random();
            int generatedId = random.Next(1, 1000000) + 16052004;

            //// Ensure the generated ID is unique
            while (ChiTietTuyenDungExists(generatedId))
            {
                generatedId = random.Next(1, 1000000) + 16052004;
            }

            //// Override IdChiTietTuyenDungs in request with the generated value
            request.IdChiTietTuyenDungs = generatedId;

            // Map AddContentRequest to ChiTietTuyenDung
            var chiTietTuyenDung = new ChiTietTuyenDung
            {
                IdChiTietTuyenDung = request.IdChiTietTuyenDungs, // Use updated IdChiTietTuyenDungs
                TieuDeTin = request.TieuDeTins,
                KinhNghiem = request.KinhNghiems,
                MoTaCongViec = request.MoTaCongViecs,
                YeuCauUngVien = request.YeuCauUngViens,
                QuyenLoiUngVien = request.QuyenLoiUngViens,
                CachThucUngTuyen = request.CachThucUngTuyens,
                LoaiCongViec = request.LoaiCongViecs,
                MucLuongTu = request.MucLuongTus,
                MucLuongToi = request.MucLuongTois,
                LoaiTienTe = request.LoaiTienTes,
                HanNopHoSo = request.HanNopHoSos,
                SoLuongUngTuyen = request.SoLuongUngTuyens,
                DiaDiemLamViecCuThe = request.DiaDiemLamViecCuThes,
                GioiTinh = request.GioiTinhs,
                ThoiGianLamViecTuThu = request.ThoiGianLamViecTuThus,
                ThoiGianLamViecDenThu = request.ThoiGianLamViecDenThus,
                ThoiGianLamViecTuGio = request.ThoiGianLamViecTuGios,
                ThoiGianLamViecDenGio = request.ThoiGianLamViecDenGios,
                HoTenNguoiNhan = request.HoTenNguoiNhans,
                SoDienThoaiNguoiNhan = request.SoDienThoaiNguoiNhans,
                EmailNguoiNhan = request.EmailNguoiNhans,
                IdNhaTuyenDung = request.IdNhaTuyenDungs,
                ViTriChuyenMon = request.ViTriChuyenMons,
                ChiTietDiaDiemLamViec = request.ChiTietDiaDiemLamViecs
            };

            // Add the new object to the database
            _context.ChiTietTuyenDungs.Add(chiTietTuyenDung);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict("Có lỗi xảy ra khi lưu dữ liệu.");
            }

            // Return the created object
            return CreatedAtAction(nameof(GetChiTietTuyenDung), new { id = chiTietTuyenDung.IdChiTietTuyenDung }, chiTietTuyenDung);
        }
        public class AddContentRequest
        {
            public int IdChiTietTuyenDungs { get; set; }

            public string TieuDeTins { get; set; } = null!;

            public string KinhNghiems { get; set; } = null!;

            public string MoTaCongViecs { get; set; } = null!;

            public string YeuCauUngViens { get; set; } = null!;

            public string QuyenLoiUngViens { get; set; } = null!;

            public string CachThucUngTuyens { get; set; } = null!;

            public string LoaiCongViecs { get; set; } = null!;

            public string MucLuongTus { get; set; } = null!;

            public string MucLuongTois { get; set; } = null!;

            public string LoaiTienTes { get; set; } = null!;

            public DateOnly HanNopHoSos { get; set; }

            public string SoLuongUngTuyens { get; set; } = null!;

            public string DiaDiemLamViecCuThes { get; set; } = null!;

            public string GioiTinhs { get; set; } = null!;

            public string ThoiGianLamViecTuThus { get; set; } = null!;

            public string ThoiGianLamViecDenThus { get; set; } = null!;

            public TimeOnly ThoiGianLamViecTuGios { get; set; }

            public TimeOnly ThoiGianLamViecDenGios { get; set; }

            public string HoTenNguoiNhans { get; set; } = null!;

            public string SoDienThoaiNguoiNhans { get; set; } = null!;

            public string EmailNguoiNhans { get; set; } = null!;

            public int IdNhaTuyenDungs { get; set; }

            public int ViTriChuyenMons { get; set; }

            public int ChiTietDiaDiemLamViecs { get; set; }
        }
        [HttpGet("Diadiemlamviec{idCTDC}")]
        public async Task<IActionResult> getCongTyByIdNTD(int idCTDC)
        {

            DiaChiChiTiet dcct = await _context.DiaChiChiTiets
                               .FirstOrDefaultAsync(dc => dc.IdDiaChi == idCTDC);
            String dchi = "";
            if (dcct != null)
            {
                dchi += dcct.MoTaChiTiet;
                var phuongXa = await _context.PhuongXas
                   .FirstOrDefaultAsync(px => px.IdPhuongXa == dcct.IdPhuongXa);
                if (phuongXa != null)
                {
                    dchi += ", " + phuongXa.TenPhuongXa;
                    var quanHuyen = await _context.QuanHuyens
                        .FirstOrDefaultAsync(px => px.IdQuanHuyen == phuongXa.IdQuanHuyen);
                    if (quanHuyen != null)
                    {
                        dchi += ", " + quanHuyen.TenQuanHuyen;
                        var tinh = await _context.ThanhPhos
                        .FirstOrDefaultAsync(px => px.IdThanhPho == quanHuyen.IdThanhPho);
                        if (tinh != null)
                        {
                            dchi += ", " + tinh.TenThanhPho;
                            return Ok(dchi);
                        }
                        return Ok(dchi);
                    }
                    return Ok(dchi);
                }
                return Ok(dchi);

            }

            else
                return NotFound();
        }

    }
}