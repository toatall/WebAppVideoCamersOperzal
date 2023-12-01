using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebAppVideoCamersOperzal.Models;
using WebAppVideoCamersOperzal.Models.Entities;


namespace WebAppVideoCamersOperzal.Controllers
{
    [Authorize]
    public class OrganizationController : Controller
    {
        /// <summary>
        /// Контекст БД
        /// </summary>
        private readonly ApplicationContext _applicationContext;
        /// <summary>
        /// Сервис аввторизации
        /// </summary>
        private readonly AuthorizeService _authorizeService;
        /// <summary>
        /// Конфигурация приложения
        /// </summary>
        private readonly IConfiguration _configuration;


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="context"></param>
        public OrganizationController(ApplicationContext context, IConfiguration configuration, 
            AuthorizeService authorizeService)
        {
            _applicationContext = context;
            _configuration = configuration;
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
        /// Создание - форма
        /// </summary>
        /// <returns></returns>
        // GET: OrganizationController/Create
        [Authorize("Admin")]
        [SaveVisit]
        public IActionResult Create()
        {
            var org = new Organization();
            return View(org);
        }

        /// <summary>
        /// Создание - сохранение
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        // POST: OrganizationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        [SaveVisit]
        public IActionResult Create(Organization organization)
        {
            organization.dateCreate = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            CheckQnique(organization.code);
            if (ModelState.IsValid)
            {
                _applicationContext.Organizations.Add(organization);
                _applicationContext.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(organization);
        }

        /// <summary>
        /// Редактирование - форма
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: OrganizationController/Edit/5
        [Authorize("Admin")]
        [SaveVisit]
        public IActionResult Edit(string id)
        {
            Organization organization = _applicationContext.Organizations.Where(t => t.code == id).FirstOrDefault();
            if (organization != null)
            {
                return View(organization);
            }
            return NotFound();
        }

        /// <summary>
        /// Редактирование - сохранение
        /// </summary>
        /// <param name="id"></param>
        /// <param name="org"></param>
        /// <returns></returns>
        // POST: OrganizationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        [SaveVisit]
        public IActionResult Edit(string id, Organization org)
        {
            Organization organization = _applicationContext.Organizations.Where(t => t.code == id).FirstOrDefault();
            if (id != org.code)
            {
                CheckQnique(org.code);
            }

            if (ModelState.IsValid)
            {
                if (id != org.code)
                {
                    _applicationContext.Entry(organization).State = EntityState.Deleted;
                    _applicationContext.SaveChanges();
                    org.dateCreate = organization.dateCreate;
                    _applicationContext.Add(org);
                    _applicationContext.SaveChanges();
                }
                else
                {
                    org.dateCreate = organization.dateCreate;
                    _applicationContext.Entry(organization).CurrentValues.SetValues(org);
                    _applicationContext.SaveChanges();
                }
                return RedirectToAction("Index", "Home");
            }
            return View(org);
        }

        /// <summary>
        /// Запрос на удаление организации
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet, Route("Organization/Delete/{id}")]
        [Authorize("Admin")]
        [SaveVisit]
        public IActionResult DeleteGet(string id)
        {
            Organization organization = _applicationContext
                .Organizations
                .Include(e => e.camers)
                .Where(t => t.code == id)
                .FirstOrDefault();
            if (organization == null)
            {
                return NotFound();
            }
            return View("Delete", organization);
        }

        /// <summary>
        /// Удаление организации
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: OrganizationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Admin")]
        [SaveVisit]
        public IActionResult Delete(string id)
        {
            Organization organization = _applicationContext.Organizations.Where(t => t.code == id).FirstOrDefault();
            if (organization != null)
            {
                _applicationContext.Organizations.Remove(organization);
                _applicationContext.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return NotFound();
        }

        /// <summary>
        /// Вывод камер по указанному налоговому органу
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SaveVisit]
        public IActionResult View(string id)
        {
            ViewBag.GetUrlFromCameraInterval = _configuration["GetUrlFromCameraInterval"];
            Organization organization = _applicationContext.Organizations
                .Include(e => e.camers)
                .Where(t => t.code == id)
                .FirstOrDefault();
            if (organization == null)
            {
                return NotFound();
            }           
            return View(organization);
        }

        /// <summary>
        /// Проверка уникальности записи
        /// </summary>
        /// <param name="code"></param>
        private void CheckQnique(string code)
        {
            var query = from t in _applicationContext.Organizations
                         where t.code == code
                         select 1;
            if (query.Count() > 0)
            {
                ModelState.AddModelError("code", $"Код {code} уже используется!");
            }
        }
        
    }
}
