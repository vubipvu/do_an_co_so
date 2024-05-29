using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace BaiTh.Models
{
	public class Coupon
	{
		public int? Id { get; set; }
		[Required, StringLength(50)]
		public string Code { get; set; }
		[Required]
		public decimal DiscountPercentage { get; set; }
		public DateTime ValidFrom { get; set; }
		public DateTime ValidUntil { get; set; }

        public bool IsActive { get; set; }
        public List<Product>? Products { get; set; }
        // Thêm các thuộc tính khác tùy theo nhu cầu
    }
}
