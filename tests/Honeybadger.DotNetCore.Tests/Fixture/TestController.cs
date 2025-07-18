using System;
using Microsoft.AspNetCore.Mvc;

namespace Honeybadger.DotNetCore.Tests.Fixture;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    // GET: /Debug/
    [HttpGet("Debug")]
    public string Debug()
    {
        throw new Exception("Hello from .Net Core Web App!");
    }
}