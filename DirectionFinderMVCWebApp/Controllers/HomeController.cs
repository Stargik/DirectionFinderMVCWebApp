using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DirectionFinderMVCWebApp.Models;
using DirectionFinderMVCWebApp.Services;
using System;

namespace DirectionFinderMVCWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DirectionFinderService directionFinderService;
    private static List<Locator> locators = new List<Locator>
    {
        new Locator
        {
            X = 8,
            Y = 6,
            Angle = 42
        },
        new Locator
        {
            X = -4,
            Y = 5,
            Angle = 158
        },
        new Locator
        {
            X = 1,
            Y = -3,
            Angle = 248
        },
        new Locator
        {
            X = -1,
            Y = -5,
            Angle = 260
        },
        new Locator
        {
            X = 0,
            Y = 14,
            Angle = 320
        },
        new Locator
        {
            X = 4,
            Y = 4,
            Angle = 100
        } 
    };
    private static Point teorPoint;
    private static Point factPoint;

    public HomeController(ILogger<HomeController> logger, DirectionFinderService directionFinderService)
    {
        _logger = logger;
        this.directionFinderService = directionFinderService;
        var coefs = directionFinderService.GetCoefs(locators);
        teorPoint = directionFinderService.GetTargetPoint(coefs);
    }

    public IActionResult Index()
    {
        var coefsA = directionFinderService.GetCoefsA(locators);
        var coefsB = directionFinderService.GetCoefsB(locators);
        var coefs = directionFinderService.GetCoefs(locators);
        ViewBag.Locators = locators;
        return View(coefs);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet("/api/Locators")]
    public async Task<JsonResult> Locators()
    {
        return new JsonResult(locators);
    }

    [HttpGet("/api/Locators/TargetPoint")]
    public async Task<JsonResult> TargetPoint()
    {
        var coefs = directionFinderService.GetCoefs(locators);
        var point = directionFinderService.GetTargetPoint(coefs);
        return new JsonResult(point);
    }

    [HttpGet("/api/Locators/TargetPointDivergence")]
    public async Task<JsonResult> TargetPointDivergence()
    {
        var coefs = directionFinderService.GetCoefs(locators);
        var point = directionFinderService.GetTargetPoint(coefs);
        teorPoint = point;
        double div = 0;
        for (int i = 0; i < coefs.Count; i++)
        {
            div += Math.Pow((locators[i].Y - Math.Tan(locators[i].Angle * Math.PI / 180) * locators[i].X - point.Y + Math.Tan(locators[i].Angle * Math.PI / 180) * point.X), 2);
        }
        div = Math.Sqrt(div);
        return new JsonResult(div);
    }

    [HttpPost("/api/Locators/SetFactPoint")]
    public async Task<JsonResult> SetFactPoint([FromBody] Point point)
    {
        factPoint = point;
        double distance = Math.Sqrt(Math.Pow(factPoint.X - teorPoint.X, 2) + Math.Pow(factPoint.Y - teorPoint.Y, 2));
        return new JsonResult(distance);
    }

    [HttpPost("/api/Locators/Setlocators")]
    public async Task Setlocators([FromBody] List<Locator> newlocators)
    {
        locators = newlocators;
        Console.WriteLine(locators);
    }

    [HttpPost("/api/Locators/RemoveLocators")]
    public async Task RemoveLocators([FromBody] FilterModel filter)
    {
        List<Locator> newLocators = new List<Locator>();
        for (int i = 0; i < locators.Count; i++)
        {
            if (Math.Abs((locators[i].Y - Math.Tan(locators[i].Angle * Math.PI / 180) * locators[i].X - factPoint.Y + Math.Tan(locators[i].Angle * Math.PI / 180) * factPoint.X)) <= filter.Eps)
            {
                newLocators.Add(locators[i]);
            }
        }
        locators = newLocators;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

