using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HomeFileHosting.Models;
using System.Data.Entity;

namespace HomeFileHosting.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext applicationDb = new ApplicationDbContext();
        public ActionResult Index()
        {
            //show all public files
            IEnumerable<FileInformation> files = applicationDb.Files.ToList().FindAll((FileInformation a)=> {
                return !a.Private;
            });
            ViewBag.PublicFiles = files;
            
            return View();
        }
        public FileResult GetFile(int id)
        {
            FileInformation file = applicationDb.Files.Find(id);
            if (User.Identity.IsAuthenticated)
            {
                if (!file.Private || User.Identity.Name == file.Owner)
                    return File(file.FullName, file.Format, file.Name);
                else
                    return File("", null, null);//new HttpNotFoundResult();//HttpStatusCodeResult(404);
            }
            else
            {
                if (!file.Private)
                    return File(file.FullName, file.Format, file.Name);
                else
                    return File("", null, null);
            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "It is practical project on summer holiday.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "asmalchevskyi@gmail.com";

            return View();
        }
    }
}