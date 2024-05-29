namespace BaiTh.Payment.Momo.Config
{
    public class MomoConfig // Đổi tên Momocofig thành MomoConfig để phù hợp với quy ước đặt tên
    {
        public static string ConfigName => "Momo";
        public static string PartnerCode { get; set; } = string.Empty;
        public static string ReturnUrl { get; set; } = string.Empty;
        public static string IpnUrl { get; set; } = string.Empty;
        public static string AccessKey { get; set; } = string.Empty; // Sửa chính tả từ "AcessKey" thành "AccessKey"
        public static string SecretKey { get; set; } = string.Empty; // Sửa chính tả từ "SceretKey" thành "SecretKey"
        public static string PaymentUrl { get; set; } = string.Empty;
    }
}
