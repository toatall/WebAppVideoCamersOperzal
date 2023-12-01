using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebAppVideoCamersOperzal.Models;
using Microsoft.EntityFrameworkCore;
using WebAppVideoCamersOperzal.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAppVideoCamersOperzal.Controllers
{
    
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _applicationContext;
        private readonly AuthorizeService _authorizeService;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="applicationContext"></param>
        public HomeController(ILogger<HomeController> logger, ApplicationContext applicationContext, AuthorizeService authorizeService)
        {
            _logger = logger;
            _applicationContext = applicationContext;
            _authorizeService = authorizeService;
            
        }

        /// <summary>
        /// Переопределение метода, который запускается перед выполнением каждого действия
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.IsAdmin = _authorizeService.IsAdmin(context.HttpContext);
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Главная страница
        /// </summary>
        /// <returns></returns>    
        [SaveVisit]
        public IActionResult Index()
        {
            IEnumerable<Organization> organizations = _applicationContext.Organizations
                .Include(e => e.camers)
                .Where(t => t.dateEnd == null)
                .OrderBy(e => e.sort)
                .ThenBy(e => e.code);
                        
            return View(organizations);
        }

        /// <summary>
        /// Статус камер
        /// </summary>
        /// <returns></returns>
        [SaveVisit]
        public IActionResult Status()
        {
            var organizations = _applicationContext.Organizations
                .Include(e => e.camers)
                .Where(t => t.dateEnd == null)
                .OrderBy(e => e.sort)
                .ThenBy(e => e.code);
            return View(organizations);
        }        

        /// <summary>
        /// Ошибка
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [SaveVisit]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
     
    }
}
