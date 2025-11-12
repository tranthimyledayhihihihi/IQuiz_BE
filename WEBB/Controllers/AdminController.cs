using System.Web.Mvc;

namespace WEBB.Controllers
{
    /**
     * BỘ LỌC BẢO MẬT:
     * Dòng [Authorize(Roles = "Admin")] này là "người gác cổng".
     * Nó sẽ tự động kiểm tra Cookie (hoặc Token) của người dùng.
     * Nếu người dùng CHƯA đăng nhập, nó sẽ đá họ về trang /Account/Login.
     * Nếu đã đăng nhập NHƯNG không có quyền "Admin", nó sẽ báo lỗi 403 Forbidden.
     */
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // GET: /Admin/Index
        // Trả về trang Dashboard của Admin
        // View sẽ nằm ở: /Views/Admin/Index.cshtml
        public ActionResult Index()
        {
            // Trang này sẽ dùng JS gọi API /api/admin/dashboard
            return View();
        }

        // GET: /Admin/Users
        // Trả về trang Quản lý Người dùng
        // View sẽ nằm ở: /Views/Admin/Users.cshtml
        public ActionResult Users()
        {
            // Trang này sẽ dùng JS gọi API /api/QLNguoiDung
            return View();
        }

        // GET: /Admin/Topics
        // Trả về trang Quản lý Chủ đề
        // View sẽ nằm ở: /Views/Admin/Topics.cshtml
        public ActionResult Topics()
        {
            // Trang này sẽ dùng JS gọi API /api/QLChuDe
            return View();
        }

        // GET: /Admin/Questions
        // Trả về trang Quản lý Câu hỏi
        // View sẽ nằm ở: /Views/Admin/Questions.cshtml
        public ActionResult Questions()
        {
            // Trang này sẽ dùng JS gọi API /api/QLCauHoi
            return View();
        }

        // GET: /Admin/Reports
        // Trả về trang Báo cáo
        // View sẽ nằm ở: /Views/Admin/Reports.cshtml
        public ActionResult Reports()
        {
            // Trang này sẽ dùng JS gọi API /api/baocao/export
            return View();
        }
    }
}