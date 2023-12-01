using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using WebAppVideoCamersOperzal.Models.Entities;

namespace WebAppVideoCamersOperzal.Models
{
    public class SaveVisitAttribute: ActionFilterAttribute, IAuthorizationFilter
    {
        /// <summary>
        /// Контекст БД
        /// </summary>
        private ApplicationContext _applicationContext;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="context"></param>
        public SaveVisitAttribute()
        {
            //_applicationContext = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _applicationContext = context.HttpContext
                .RequestServices
                .GetService(typeof(ApplicationContext)) as ApplicationContext;            
        }

        /// <summary>
        /// Код, который должен быть выполнен перед выполнением действия
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        { 
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Код, который должен быть выполнен после выполнением действия
        /// Сохранения посещения
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            try
            {
                if (context.HttpContext.User.Identity.IsAuthenticated)
                {
                    Visit visit = new Visit()
                    {
                        Username = context.HttpContext.User.Identity.Name,
                        Url = context.HttpContext.Request.Path.ToString(),
                        Method = context.HttpContext.Request.Method.ToString(),
                        UserAgent = context.HttpContext.Request.Headers["User-Agent"].ToString(),
                        DateCreate = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
                    };
                    _applicationContext.Add(visit);
                    _applicationContext.SaveChanges();                    
                }
            }
            catch { }            
        }
    }
}
