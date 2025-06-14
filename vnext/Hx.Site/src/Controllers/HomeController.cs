using System.Diagnostics;

using Hx.Site.Models;

using Microsoft.AspNetCore.Mvc;

namespace Hx.Site.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return this.View();
    }

    public IActionResult Privacy()
    {
        return this.View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
    }
}