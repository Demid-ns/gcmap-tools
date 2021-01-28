using GCMAP.Areas.Identity.Data;
using GCMAP.Models;
using GCMAP.Services;
using GCMAP.ViewModels;
using ImageMagick;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GCMAP.Controllers
{
    //Доступ к контроллеру есть только у указанных ролей
    [Authorize(Roles = "admin, moderator, root")]
    public class ModeratorController : Controller
    {
        private readonly GCMAPContext db;
        private readonly UserManager<GCMAPUser> _userManager;
        private readonly IWebHostEnvironment _appEnvironment;

        //Инжектируем контекст и Identity сервисы
        public ModeratorController(GCMAPContext context, UserManager<GCMAPUser> userManager, IWebHostEnvironment appEnvironment)
        {
            db = context;
            _userManager = userManager;
            _appEnvironment = appEnvironment;
        }

        //Стартовая страница модерации
        [Route("/Moderator")]
        [Route("/Moderator/Start")]
        public ActionResult Start() => View();

        //Модерирование списка новостей
        public async Task<IActionResult> News(int page = 1)
        {
            //Выбираем новости согласно параметров пагинации
            int pageSize = 8;
            IQueryable<News> news = db.News.Include(n => n.Photo).OrderByDescending(n => n.Id); ;
            var count = await news.CountAsync();
            var items = await news.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            ModerateNewsViewModel model = new ModerateNewsViewModel
            {
                News = items,
                PageViewModel = pageViewModel
            };

            return View(model);
        }

        //Создание новости
        public ActionResult CreateNews() => View();

        //Обрабатываем сабмит формы создания новости
        [HttpPost]
        public async Task<IActionResult> CreateNews(CreateNewsViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Создаем экземпляр компрессора, чтобы сжать фотографию для новости 
                ImageCompessor compessor = new ImageCompessor();
                ClaimsPrincipal currentUser = this.User;
                GCMAPUser user = await _userManager.GetUserAsync(currentUser);

                //Сохраняем переданную фотографию
                string path = "/Files/" + model.Photo.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await compessor.CompressAsync(model.Photo.OpenReadStream(), fileStream, 40); //40 - качество
                }

                //Создаем объект фотографии для последующего добавления в БД
                Photo photo = new Photo { Name = model.Photo.FileName, Path = path };

                News news = new News()
                {
                    Author = user.NickName,
                    Date = DateTime.Today,
                    Theme = model.Theme,
                    Description = model.Description,
                    Photo = photo
                };

                await db.Photos.AddAsync(photo);
                await db.News.AddAsync(news);
                await db.SaveChangesAsync();

                return RedirectToAction("News");
            }
            else
            {
                return View(model);
            }
        }

        //Изменяем новость
        public async Task<IActionResult> EditNews(int? id)
        {
            if (id != null)
            {
                //Находим новость по айди
                News news = await db.News.FindAsync(id);
                if (news != null)
                {
                    EditNewsViewModel model = new EditNewsViewModel
                    {
                        Id = news.Id,
                        Theme = news.Theme,
                        Description = news.Description
                    };
                    return View(model);
                }
            }
            return NotFound();
        }

        //Обрабатываем форму на изменение новости
        [HttpPost]
        public async Task<IActionResult> EditNews(EditNewsViewModel model)
        {
            if (ModelState.IsValid)
            {
                News news = await db.News.FindAsync(model.Id);

                if (news != null)
                {
                    news.Theme = model.Theme;
                    news.Description = model.Description;

                    if (model.Photo != null)
                    {
                        //Создаем экземпляр компрессора, чтобы сжать фотографию для новости 
                        //фото может и не передаваться с формой
                        ImageCompessor compessor = new ImageCompessor();

                        string path = "/Files/" + model.Photo.FileName;
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await compessor.CompressAsync(model.Photo.OpenReadStream(), fileStream, 40); //40 - качество
                        }

                        //Создаем объект фотографии для последующего добавления в БД
                        Photo photo = new Photo { Name = model.Photo.FileName, Path = path };
                        news.Photo = photo;
                    }

                    await db.SaveChangesAsync();
                    return RedirectToAction("News");
                }
            }
            return View(model);
        }

        //Удаляем новость по id
        [HttpPost]
        public async Task<IActionResult> DeleteNews(int? id)
        {
            if (id != null)
            {
                News news = await db.News.FindAsync(id);
                if (news != null)
                {
                    db.News.Remove(news);
                    await db.SaveChangesAsync();
                    return RedirectToAction("News");
                }
            }
            return NotFound();
        }

        //Возврщаем подключения
        public async Task<IActionResult> Connections(int page = 1)
        {
            //Выбираем подключения, согласно параметров пагинации
            int pageSize = 8;
            IQueryable<Connection> connections = db.Connections.Where(c => !c.Accepted).OrderByDescending(c => c.Id);
            var count = await connections.CountAsync();
            var items = await connections.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            ModerateConnectionsViewModel model = new ModerateConnectionsViewModel()
            {
                Connections = connections,
                PageViewModel = pageViewModel
            };

            return View(model);
        }

        //Подтверждаем подключение по id
        public async Task<IActionResult> AcceptConnection(int? id)
        {
            if (id != null)
            {
                Connection connection = await db.Connections.FindAsync(id);
                if (connection != null)
                {
                    AcceptConnectionViewModel model = new AcceptConnectionViewModel
                    {
                        Id = connection.Id,
                        NickName = connection.NickName,
                        StationName = connection.StationName,
                        X = connection.X,
                        Z = connection.Z,
                        Contact = connection.Contact
                    };
                    return View(model);
                }
            }
            return NotFound();
        }

        //Обрабатываем post-запрос на одобрение подключения
        [HttpPost]
        public async Task<IActionResult> AcceptConnection(AcceptConnectionViewModel model)
        {
            if (ModelState.IsValid)
            {
                Connection connection = await db.Connections.FindAsync(model.Id);
                if (connection != null)
                {
                    connection.Accepted = true;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Connections");
                }
            }
            return View(model);
        }

        //Возвращаем одобренные подключения
        public async Task<IActionResult> AcceptedConnections(int page = 1)
        {
            //Выбираем одобренные подключения, согласно параметров пагинации
            int pageSize = 8;
            IQueryable<Connection> connections = db.Connections.Where(c => c.Accepted).OrderByDescending(c => c.Id);
            var count = await connections.CountAsync();
            var items = await connections.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            ModerateConnectionsViewModel model = new ModerateConnectionsViewModel()
            {
                Connections = connections,
                PageViewModel = pageViewModel
            };

            return View(model);
        }

        //Удаляем подключение по выбранному id
        [HttpPost]
        public async Task<IActionResult> DeleteConnection(int? id)
        {
            if (id != null)
            {
                Connection connection = await db.Connections.FindAsync(id);
                if (connection != null)
                {
                    db.Connections.Remove(connection);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Connections");
                }
            }
            return NotFound();
        }

        //Получаем список заявок
        public async Task<IActionResult> Requests(int page = 1)
        {
            //Выбираем заявки, согласно параметров пагинации
            int pageSize = 8;
            IQueryable<Request> requests = db.Requests.Where(r => !r.Accepted).OrderByDescending(r=>r.Id);
            var count = await requests.CountAsync();
            var items = await requests.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            ModerateRequestsViewModel model = new ModerateRequestsViewModel()
            {
                Requests = requests,
                PageViewModel = pageViewModel
            };

            return View(model);
        }

        //Получаем одобренные заявки
        public async Task<IActionResult> AcceptedRequests(int page = 1)
        {
            //Выбираем заявки, согласно параметров пагинации
            int pageSize = 8;
            IQueryable<Request> requests = db.Requests.Where(r => r.Accepted).OrderByDescending(r => r.Id);
            var count = await requests.CountAsync();
            var items = await requests.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            ModerateRequestsViewModel model = new ModerateRequestsViewModel()
            {
                Requests = requests,
                PageViewModel = pageViewModel
            };

            return View(model);
        }

        //Обрабатываем запрос на удаление заявки
        [HttpPost]
        public async Task<IActionResult> DeleteRequest(int? id)
        {
            if (id != null)
            {
                Request request = await db.Requests.FindAsync(id);
                if (request != null)
                {
                    db.Requests.Remove(request);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Requests");
                }
            }
            return NotFound();
        }

        //Одобряем заявку
        public async Task<IActionResult> AcceptRequest(int? id)
        {
            if (id != null)
            {
                Request request = await db.Requests.Include(r => r.Photo).FirstOrDefaultAsync(r => r.Id == id);
                if (request != null)
                {
                    AcceptRequestViewModel model = new AcceptRequestViewModel
                    {
                        Id = request.Id,
                        NickName = request.NickName,
                        Description = request.Description,
                        Contact = request.Contact
                    };
                    if (request.Photo != null)
                    {
                        model.Photo = request.Photo;
                    }
                    return View(model);
                }
            }
            return NotFound();
        }

        //Обрабатываем сабмит формы при подтверждении заявки
        [HttpPost]
        public async Task<IActionResult> AcceptRequest(AcceptRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                Request request = await db.Requests.FindAsync(model.Id);
                if (request != null)
                {
                    request.Accepted = true;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Requests");
                }
            }
            return View(model);
        }

        //Возвращаем представление для актуализации карты
        public IActionResult UploadMap() => View();

        [HttpPost]
        public async Task<IActionResult> UploadMap(UploadMapViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Создаем экземпляр компрессора, чтобы сжать фотографию для карты
                ImageCompessor compessor = new ImageCompessor();
                ClaimsPrincipal currentUser = this.User;
                GCMAPUser user = await _userManager.GetUserAsync(currentUser);
                Map map = new Map()
                {
                    Date = DateTime.Today,
                    Uploader = user.NickName
                };

                //Изменяем название файла, чтобы на сервере они хранились в удобном виде
                string path = "/Files/" + $"metro{DateTime.Now.ToString("ddMMyyyy_hh_mm")}.png";
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await compessor.CompressAsync(model.Photo.OpenReadStream(), fileStream, 70); //40 - качество
                }
                Photo photo = new Photo { Name = model.Photo.FileName, Path = path };
                map.Photo = photo;

                await db.AddAsync(map);
                await db.SaveChangesAsync();

                return RedirectToAction("Start");
            }
            return View(model);
        }
    }
}
