using System.ComponentModel.DataAnnotations;

namespace BaiTh.Models
{
	public class Product
	{
		public int Id { get; set; }
		[Required, StringLength(100)]
		public string Name { get; set; }
		[Range(0.01, 1000000.00)]
		public decimal Price { get; set; }
		public string Description { get; set; }
		public string? ImageUrl { get; set; }
		public List<ProductImage>? Images { get; set; }
		public int CategoryId { get; set; }
		public Category? Category { get; set; }

		public int? CouponId { get; set; }
		public Coupon? Coupon { get; set; }
        public decimal CalculateDiscountedPrice(decimal discountPercentage)
        {
            return Price - (Price * discountPercentage / 100);
        }
         
    }
}
