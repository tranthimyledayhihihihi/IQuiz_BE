using System.Web.Mvc;
using WEBB.Models.Account;
using System;
using System.Linq;
using System.Web.Security;         // FormsAuthentication
using System.Security.Claims;       // ClaimTypes
using System.IdentityModel.Tokens.Jwt; // JwtSecurityTokenHandler
// Dòng using Microsoft.Web.Http.Internal; đã được XÓA (vì nó lỗi)

namespace WEBB.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // GET: /Account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            // Xóa Cookie
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        // === HÀM CẦU NỐI (BRIDGE) JWT --> COOKIE ===
        [HttpPost]
        [AllowAnonymous]
        public JsonResult SetAuthCookie(string token)
        {
            try
            {
                // 1. Giải mã Token để lấy Quyền (Roles)
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                // 1. Lấy Role và UserName từ Token
                var roleClaim = jsonToken.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.Role);

                string userRole = roleClaim?.Value ?? "User"; // "Admin" hoặc "User"

                string userName = jsonToken.Claims
                    .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value ?? "User";

                // 2. TẠO COOKIE Forms Authentication
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    1,
                    userName,
                    DateTime.Now,
                    DateTime.Now.AddHours(24),
                    false, // (Không "Remember me")
                    userRole // Lưu Quyền (Roles) vào Cookie
                );

                // 3. Gửi Cookie về trình duyệt
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new System.Web.HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                authCookie.Expires = authTicket.Expiration;

                Response.Cookies.Add(authCookie);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Lỗi khi giải mã token (ví dụ: thiếu package, token hết hạn)
                return Json(new { success = false, message = "Lỗi giải mã token: " + ex.Message });
            }
        }
    }
}