using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class ucAnnounceItem : UserControl
    {
        /// <summary>
        /// Hàm nạp dữ liệu thực từ DTO vào các nhãn hiển thị trên giao diện
        /// </summary>
        /// <param name="data">Đối tượng thông báo lấy từ Database</param>
       
        public ucAnnounceItem()
        {
            InitializeComponent();
            ButtonEffectHelper.RemoveGrayEffect(this);
        }

        public void SetData(AnnouncementsDTO data)
        {
            if (data == null) return;

            // 1. Hiển thị tiêu đề và nội dung (Dùng Trim để xóa khoảng trắng thừa từ SQL)
            lblTitle.Text = data.Title?.Trim() ?? "Không có tiêu đề";
            lblContent.Text = data.Content?.Trim() ?? "Nội dung đang được cập nhật...";

            // 2. Định dạng ngày giờ chi tiết hơn (Gồm cả giờ phút để học viên dễ theo dõi)
            // Giả sử thuộc tính trong DTO của bạn là PublishDate hoặc CreatedAt
            lblDate.Text = data.PublishDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";

            // 3. Mẹo nhỏ: Tự động chỉnh độ cao Label nội dung nếu văn bản quá dài
            // (Đảm bảo Label của bạn đã bật thuộc tính AutoSize = true)
        }
    }
}
