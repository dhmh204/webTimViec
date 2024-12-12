namespace BackEnd.Models
{
    public class HoSoDaNopDAO
    {

        public DateTime thoiGianNop { get; set; }

        public string trangThai { get; set; } = null!;

        public int idChiTietTuyenDung { get; set; }

        public int idUngVien { get; set; }
    }
}
