using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using extract_slip_api.Models;

namespace extract_slip_api.Controllers;

[Route("/")]
[ApiController]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Index()
    {
        return View();
    }
}
