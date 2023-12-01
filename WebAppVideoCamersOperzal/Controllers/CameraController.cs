using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebAppVideoCamersOperzal.Models;
using WebAppVideoCamersOperzal.Models.Entities;

namespace WebAppVideoCamersOperzal.Controllers
{
    [Authorize()]
    public class CameraController : Controller
    {

        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Контекст БД
        /// </summary>
        private readonly ApplicationContext _applicationContext;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="context"></param>
        public CameraController(ApplicationContext context, IMemoryCache memoryCache)
        {
            _applicationContext = context;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Создание камеры - форма
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        // GET: CameraController/Create
        [Route("Camera/Create/{code}")]
        [Authorize("Admin")]
        [SaveVisit]
        public IActionResult Create(string code)
        {
            ViewBag.code = code;
            Organization organization = FindOrganization(code);
            if (organization == null)
            {
                return NotFound();
            }
            VideoCamera camera = new VideoCamera();
            camera.organization = organization;
            return View(camera);
        }

        /// <summary>
        /// Создание камеры - сохранение
        /// </summary>
        /// <param name="code"></param>
        /// <param name="videoCamera"></param>
        /// <returns></returns>
        // POST: CameraController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Camera/Create/{code}")]
        [Authorize("Admin")]
        [SaveVisit]
        public IActionResult Create(string code, VideoCamera videoCamera)
        {
            ViewBag.code = code;
            Organization organization = FindOrganization(code);
            if (organization == null)
            {
                return NotFound();
            }
            videoCamera.organization = organization;
            videoCamera.dateCreate = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            if (ModelState.IsValid)
            {
                _applicationContext.VideoCameras.Add(videoCamera);
                _applicationContext.SaveChanges();
                return RedirectToAction("View", "Organization", new { id = code });
            }
            return View(videoCamera);
        }

        /// <summary>
        /// Редактирование камеры - форма
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: CameraController/Edit/5
        [Route("Camera/Edit/{id}")]
        [Authorize("Admin")]
        [SaveVisit]
        public IActionResult Edit(int id)
        {
            VideoCamera videoCamera = FindVideoCamera(id);
            if (videoCamera == null)
            {
                return NotFound();
            }
            ViewBag.code = videoCamera.orgCode;
            return View(videoCamera);
        }

        /// <summary>
        /// Редактирование камеры - сохранение
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videoCamera"></param>
        /// <returns></returns>
        // POST: CameraController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Camera/Edit/{id}")]
        [Authorize("Admin")]
        [SaveVisit]
        public IActionResult Edit(int id, VideoCamera videoCamera)
        {
            VideoCamera camera = FindVideoCamera(id);
            if (camera == null)
            {
                return NotFound();
            }
            ViewBag.code = camera.orgCode;
            if (ModelState.IsValid)
            {
                videoCamera.dateCreate = camera.dateCreate;
                videoCamera.orgCode = camera.orgCode;
                _applicationContext.Entry(camera).CurrentValues.SetValues(videoCamera);
                _applicationContext.SaveChanges();
                return RedirectToAction("View", "Organization", new { id = videoCamera.orgCode });
            }
            return View(videoCamera);

        }

        /// <summary>
        /// Запрос на удаление камеры
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Camera/Delete/{id}")]
        [Authorize("Admin")]
        [SaveVisit]
        public IActionResult DeleteGet(int id)
        {
            VideoCamera camera = FindVideoCamera(id);
            if (camera == null)
            {
                return NotFound();
            }
            return View("Delete", camera);
        }

        /// <summary>
        /// Удаление камеры
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // POST: CameraController/Delete/5
        [HttpPost, ValidateAntiForgeryToken]
        [Route("Camera/Delete/{id}")]
        [Authorize("Admin")]
        [SaveVisit]
        public IActionResult Delete(int id)
        {
            VideoCamera camera = FindVideoCamera(id);
            if (camera == null)
            {
                return NotFound();
            }
            _applicationContext.Remove(camera);
            _applicationContext.SaveChanges();
            return RedirectToAction("View", "Organization", new { id = camera.orgCode });
        }       

        /// <summary>
        /// Получение изображения по ссылке
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Camera/CameraUrl/{id}")]        
        public async Task<IActionResult> CameraUrl(int id)
        {            
            VideoCamera videoCamera = _applicationContext.VideoCameras
                                    .Where(e => e.id == id)
                                    .FirstOrDefault();
            if (videoCamera == null)
            {
                return NotFound();
            }
            if (videoCamera.disabled)
            {
                return StatusCode(222, videoCamera.disabledMessage);
            }

            try {

                HttpClient client = new HttpClient();
                if (!string.IsNullOrEmpty(videoCamera.user))
                {
                    string authHeaderValue = $"{videoCamera.user}:{videoCamera.password}";
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authHeaderValue)));
                }

                HttpResponseMessage response = await client.GetAsync(videoCamera.url);
                if (response.IsSuccessStatusCode)
                {
                    Stream imageStream = await response.Content.ReadAsStreamAsync();
                    return File(imageStream, "image/jpeg");
                }
                else
                {
                    return StatusCode(599);
                }
            } 
            catch
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Проверка доступности камеры
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Status(int id)
        {
            VideoCamera camera = FindVideoCamera(id);
            if (camera == null)
            {
                return NotFound();
            }
            CameraStatus cameraStatus = new CameraStatus(camera.url, camera.user, camera.password);
            bool status = await cameraStatus.Check();
            return Json(new { status });
        }

        /// <summary>
        /// Поиск камеры в БД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private VideoCamera FindVideoCamera(int id)
        {
            return _applicationContext.VideoCameras
                .Where(t => t.id == id)
                .Include(e => e.organization)
                .FirstOrDefault();
        }

        /// <summary>
        /// Поиск организации в БД
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private Organization FindOrganization(string code)
        {
            return _applicationContext.Organizations.Where(t => t.code == code).FirstOrDefault();
        }

    }
}
