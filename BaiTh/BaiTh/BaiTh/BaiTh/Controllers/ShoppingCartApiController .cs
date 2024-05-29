using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace BaiTh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartApiController : ControllerBase
    {
        private static Dictionary<int, int> _cart = new Dictionary<int, int>();

        [HttpGet]
        public ActionResult<IEnumerable<KeyValuePair<int, int>>> GetCart()
        {
            return _cart.ToList();
        }

        [HttpPost("add/{productId}")]
        public IActionResult AddToCart(int productId)
        {
            if (!_cart.ContainsKey(productId))
            {
                _cart.Add(productId, 1);
            }
            else
            {
                _cart[productId]++;
            }
            return Ok();
        }

        [HttpDelete("remove/{productId}")]
        public IActionResult RemoveFromCart(int productId)
        {
            if (_cart.ContainsKey(productId))
            {
                if (_cart[productId] > 1)
                {
                    _cart[productId]--;
                }
                else
                {
                    _cart.Remove(productId);
                }
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("clear")]
        public IActionResult ClearCart()
        {
            _cart.Clear();
            return Ok();
        }
    }
}
