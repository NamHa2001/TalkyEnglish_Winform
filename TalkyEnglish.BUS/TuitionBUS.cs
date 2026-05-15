using System;
using System.Collections.Generic;
using TalkyEnglish.DAL;
using TalkyEnglish.DTO;

namespace TalkyEnglish.BUS
{
    public class TuitionBUS
    {
        private TuitionDAL _tuitionDAL = new TuitionDAL();

        // Lấy toàn bộ danh sách học phí để đổ lên Grid
        public List<TuitionDTO> GetAllTuitions()
        {
            return _tuitionDAL.GetTuitionList();
        }

        // Xử lý logic trước khi gọi DAL xác nhận thu tiền
        public string ProcessPayment(int enrollmentId, decimal amount)
        {
            // Tầng BUS sẽ làm nhiệm vụ gác cổng, kiểm tra dữ liệu đầu vào
            if (enrollmentId <= 0)
            {
                return "Lỗi: Không xác định được lượt đăng ký!";
            }
            if (amount <= 0)
            {
                return "Lỗi: Số tiền học phí không hợp lệ!";
            }

            // Gọi xuống DAL để thực hiện Transaction
            bool isSuccess = _tuitionDAL.ConfirmPayment(enrollmentId, amount);

            if (isSuccess)
                return "OK"; // Trả về chữ OK nếu thành công để GUI biết đường thông báo
            else
                return "Lỗi: Giao dịch thất bại, vui lòng kiểm tra lại kết nối!";
        }
    }
}