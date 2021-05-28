using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityShell.Quickstart.LogEvents
{
    public class LogEvents : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
