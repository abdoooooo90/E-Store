using Microsoft.AspNetCore.Mvc;

namespace E_LapShop.Controllers
{
    public class FurniController : Controller
    {
        private readonly ILogger<FurniController> _logger;

        public FurniController(ILogger<FurniController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Shop()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Blog()
        {
            return View();
        } 

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Cart()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }

        public IActionResult ThankYou()
        {
            return View();
        }

        public IActionResult ProductDetails(int id)
        {
            // For demo purposes, creating a sample product based on ID
            var product = new
            {
                Id = id,
                Name = id == 1 ? "Nordic Chair" : id == 2 ? "Kruzo Aero Chair" : "Ergonomic Chair",
                Price = id == 1 ? 50.00m : id == 2 ? 78.00m : 43.00m,
                Description = "Donec facilisis quam ut purus rutrum lobortis. Donec vitae odio quis nisl dapibus malesuada. Nullam ac aliquet velit. Aliquam vulputate velit imperdiet dolor tempor tristique. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas.",
                LongDescription = "Far far away, behind the word mountains, far from the countries Vokalia and Consonantia, there live the blind texts. Separated they live in Bookmarksgrove right at the coast of the Semantics, a large language ocean. A small river named Duden flows by their place and supplies it with the necessary regelialia. It is a paradisematic country, in which roasted parts of sentences fly into your mouth.",
                ImageUrl = $"~/furni/images/product-{id}.png",
                Category = "Chairs",
                InStock = true,
                Rating = 4.5,
                Reviews = 24
            };

            return View(product);
        }
    }
}
