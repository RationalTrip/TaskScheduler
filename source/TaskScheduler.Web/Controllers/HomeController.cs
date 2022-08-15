using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskScheduler.Web
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        public const string CONTROLLER_ROUTE = "home";

        public const string INDEX_ROUTE = "index";
        public const string ERROR_PAGE_ROUTE = "error";

        [Route(INDEX_ROUTE)]
        [Route("~/", Name = "default")]
        public IActionResult Index() => View();

        [Route(ERROR_PAGE_ROUTE + "/{message?}")]
        //[Route(ERROR_PAGE_ROUTE)]
        public IActionResult ErrorPage(string message = null) => View((object)message?.Replace('_', ' '));
    }
}
