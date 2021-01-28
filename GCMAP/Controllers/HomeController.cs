using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GCMAP.Models;
using Microsoft.EntityFrameworkCore;
using GCMAP.ViewModels;
using GCMAP.Services;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace GCMAP.Controllers
{
    public class HomeController : Controller
    {
        private readonly GCMAPContext db;
        private readonly IWebHostEnvironment _appEnvironment;

        //Ижектируем контекст и сервис окружения пользователя
        public HomeController(GCMAPContext context, IWebHostEnvironment appEnvironment)
        {
            db = context;
            _appEnvironment = appEnvironment;
        }

        //Поиск актуальной карты
        public async Task<IActionResult> Index()
        {
            //Выбираем наиболее актуальную из таблицы карт
            Map map = await db.Maps.OrderByDescending(m => m.Id).Include(m=>m.Photo).FirstOrDefaultAsync();
            //Страница может быть отображена только если карта существует, иначе редиректим на новостной дайджест
            if (map != null)
            {
                return View(map);
            }
            return RedirectToAction("News");
        }

        //Новостной дайджест
        public async Task<IActionResult> News(int page = 1)
        {
            //Выбираем новости в зависимости от параметров пагинации 
            //(можно посмотреть PagesTagHelper)
            int pageSize = 6;
            IQueryable<News> news = db.News.Include(n => n.Photo).OrderByDescending(i => i.Id);
            var count = await news.CountAsync();
            var items = await news.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            NewsViewModel model = new NewsViewModel
            {
                News = items,
                PageViewModel = pageViewModel
            };

            return View(model);
        }

        //Представления
        public IActionResult Requests() => View();

        public IActionResult Join() => View();

        public IActionResult JoinTeam() => View();

        public IActionResult Failure() => View();

        //Обрабатываем запрос от формы на подключение
        [HttpPost]
        public async Task<IActionResult> Join(ConnectionViewModel model)
        {
            if (ModelState.IsValid)
            {
                Connection connection = new Connection()
                {
                    NickName = model.NickName,
                    StationName = model.StationName,
                    X = model.X,
                    Z = model.Z,
                    Contact = model.Contact,
                    Date = DateTime.Today,
                    Accepted = false
                };

                await db.AddAsync(connection);
                await db.SaveChangesAsync();

                return View("OK");
            }
            return View(model);
        }

        //Обрабатываем запрос от формы на присоединение к команде
        [HttpPost]
        public async Task<IActionResult> JoinTeam(RequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                Request request = new Request()
                {
                    NickName = model.NickName,
                    Description = model.Description,
                    Contact = model.Contact,
                    Date = DateTime.Today,
                    Accepted = false,
                    RequestType = Data.RequestType.JoinTeam
                };

                await db.AddAsync(request);
                await db.SaveChangesAsync();

                return View("OK");
            }
            return View(model);
        }

        //Обрабатываем запрос от формы на подачу жалобы
        [HttpPost]
        public async Task<IActionResult> Failure(RequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                Request request = new Request()
                {
                    NickName = model.NickName,
                    Description = model.Description,
                    Contact = model.Contact,
                    Date = DateTime.Today,
                    Accepted = false,
                    RequestType = Data.RequestType.Failure
                };

                //Сжимаем фотографию, если такая имеется
                if (model.Photo != null)
                {
                    //Создаем экземпляр класса компрессора (Services/ImageCompessor)
                    ImageCompessor compessor = new ImageCompessor();

                    string path = "/Files/" + model.Photo.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await compessor.CompressAsync(model.Photo.OpenReadStream(), fileStream, 80); //80 - качество
                    }

                    //Добавляем данные о фото в таблицу БД
                    Photo photo = new Photo { Name = model.Photo.FileName, Path = path };
                    request.Photo = photo;

                    await db.Photos.AddAsync(photo);
                }

                await db.AddAsync(request);
                await db.SaveChangesAsync();

                return View("OK");
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
