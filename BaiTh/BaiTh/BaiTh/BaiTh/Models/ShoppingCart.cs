namespace BaiTh.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId ==
           item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                Items.Add(item);
            }
        }
        public void RemoveItem(int productId)
        {
            Items.RemoveAll(i => i.ProductId == productId);
        }
        // Các phương thức khác...
        public void UpdateQuantity(int producId, int quantity)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == producId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }
    }
}
