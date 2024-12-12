using FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using FrontEnd.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Text;
namespace FrontEnd.Models;


public class ChiTietViecLam : Controller
{
    private readonly HttpClient _httpClient;
    private readonly string apiUrl = "https://localhost:7208/api/ChiTietTuyenDungs/search";
    public ChiTietViecLam(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<IActionResult> Index(int idChiTietTuyenDung)
    {
        HttpContext.Session.SetInt32("idCtvl", idChiTietTuyenDung);
        String url = $"https://localhost:7208/api/ChiTietTuyenDungs/{idChiTietTuyenDung}";
        TempData["url"] = url;
        ChiTietTuyenDung ctvl = await getChiTietViecLam(url);
        ViewBag.cttd = ctvl;
        String url2 = $"https://localhost:7208/api/CongTies/CongTy{ctvl.IdNhaTuyenDung}";
        CongTy cty = await getCongTy(url2);
        ViewBag.cty = cty;
        String url3 = $"https://localhost:7208/api/NhomNghes/ViTriChuyenMon/{ctvl.ViTriChuyenMon}";
        NhomNghe nhomNghe = await getNhomNghe(url3);
        ViewBag.nghe = nhomNghe;
        String url4 = $"https://localhost:7208/api/ChiTietTuyenDungs/Diadiemlamviec{ctvl.ChiTietDiaDiemLamViec}";
        String dch = await getDiaChi(url4);
        ViewBag.dchi = dch;

        return View();
    }
    private async Task<List<Dictionary<string, object>>> SearchJobsFromApi(string searchTerm)
    {
        // Gọi API với từ khóa tìm kiếm
        var response = await _httpClient.GetStringAsync($"{apiUrl}/{searchTerm}");
        var jobs = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(response);
        return jobs;
    }



    public async Task<ChiTietTuyenDung> getChiTietViecLam(string url)
    {
        {
            try
            {
                string jsonResponse = await _httpClient.GetStringAsync(url);
                return JsonConvert.DeserializeObject<ChiTietTuyenDung>(jsonResponse);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return null;
            }
        }
    }

    public async Task<CongTy> getCongTy(String url)
    {
        try
        {
            string jsonResponse = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<CongTy>(jsonResponse);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching data: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error: {ex.Message}");
            return null;
        }
    }

    public async Task<NhomNghe> getNhomNghe(String url)
    {
        try
        {
            string jsonResponse = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<NhomNghe>(jsonResponse);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching data: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error: {ex.Message}");
            return null;
        }
    }

    public async Task<String> getDiaChi(String url)
    {
        try
        {
            string jsonResponse = await _httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<String>(jsonResponse);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching data: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error: {ex.Message}");
            return null;
        }
    }

    public async Task<IActionResult> UngTuyen()
    {
        // Đăng ký nhà tuyển dụng
        var newUngTuyen = new
        {
            thoiGianNop = DateTime.UtcNow,
            trangThai = "Chờ duyệt",
            idChiTietTuyenDung = HttpContext.Session.GetInt32("idCtvl"),
            idUngVien = HttpContext.Session.GetInt32("Id")
        };

        using (var client = new HttpClient())
        {
            try
            {
                var json = JsonConvert.SerializeObject(newUngTuyen);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                // dua base path vao config
                string urlPost = "https://localhost:7208/api/HoSoDaNops";
                var response = await client.PostAsync(urlPost, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["successDKi"] = "Đăng ký thành công!";
                    return RedirectToAction("Index", "ThongBaoUngTuyenThanhCong"); ;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["errorDKi"] = $"Đăng ký thất bại: {error}";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["errorDKi"] = $"Lỗi hệ thống khi đăng ký: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }
    }
    //api/LuuTinTuyenDungs
    public async Task<IActionResult> LuuTinTuyenDung()
    {
        // Đăng ký nhà tuyển dụng
        var newTinTD = new
        {
            thoiGianLuuTin = DateTime.UtcNow,
            idChiTietTuyenDung = HttpContext.Session.GetInt32("idCtvl"),
            idUngVien = HttpContext.Session.GetInt32("Id")
        };

        using (var client = new HttpClient())
        {
            try
            {
                var json = JsonConvert.SerializeObject(newTinTD);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                // dua base path vao config
                string urlPost = "https://localhost:7208/api/LuuTinTuyenDungs/api/LuuTinTuyenDungs";
                var response = await client.PostAsync(urlPost, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["successDKi"] = "Đăng ký thành công!";
                    return RedirectToAction("Index", "ChiTietViecLam"); ;
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["errorDKi"] = $"Đăng ký thất bại: {error}";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["errorDKi"] = $"Lỗi hệ thống khi đăng ký: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}