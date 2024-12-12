using System;
using System.Collections.Generic;

namespace FrontEnd.Models;
public partial class CongTy
{
    public int IdCongTy { get; set; }

    public string LogoUrl { get; set; } = null!;

    public string TenCongTy { get; set; } = null!;

    public string MaSoThue { get; set; } = null!;

    public string WebsiteUrl { get; set; } = null!;

    public string SoLuongNguoiTheoDoi { get; set; } = null!;

    public string QuyMoCongTy { get; set; } = null!;

    public string MoTaCongTy { get; set; } = null!;

    public string Email { get; set; } = null!;

}
