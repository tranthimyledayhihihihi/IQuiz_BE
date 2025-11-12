using System;
using System.ComponentModel.DataAnnotations;

namespace WEBB.Models.Admin
{
    // Dùng để gửi yêu cầu Báo cáo (gửi tới API /api/baocao/export)
    public class BaoCaoRequestModel
    {
        [Required(ErrorMessage = "Vui lòng chọn loại báo cáo")]
        [Display(Name = "Loại báo cáo")]
        public string LoaiBaoCao { get; set; }

        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? NgayBatDau { get; set; }

        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? NgayKetThuc { get; set; }
    }
}