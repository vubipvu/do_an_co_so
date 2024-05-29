using Microsoft.AspNetCore.Mvc;

namespace BaiTh.Controllers
{
    public class NewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
