using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using HomeFileHosting.Models;

namespace HomeFileHosting.Controllers
{
    public class FileController : Controller
    {

        private ApplicationDbContext applicationDb = new ApplicationDbContext();

        // GET: File
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Directory
        [Authorize]
        public ActionResult Directory(string user)
        {
            //show all public files
            IEnumerable<FileInformation> publicFiles = applicationDb.Files.ToList().FindAll((FileInformation a) => {
                return !a.Private;
            });
            //
            ViewBag.PublicFiles = publicFiles;

            //show all owner files
            IEnumerable<FileInformation> ownerFiles = applicationDb.Files.ToList().FindAll((FileInformation a) => {
                return a.Owner == User.Identity.Name;
            });
            //
            ViewBag.OwnerFiles = ownerFiles;
            return View();
        }

        //
        //GET: /Account/EditFile
        [Authorize]
        [HttpGet]
        public ActionResult EditFile(int id)
        {
            FileInformation file = applicationDb.Files.Find(id);
            ViewBag.EditFile = file;
            return View(new FileInformationEditModel { Name = file.Name, Private = file.Private });
        }
        //
        //POST: /Account/EditFile
        [Authorize]
        [HttpPost]
        public ActionResult EditFile(FileInformationEditModel file, int id)
        {
            if (!ModelState.IsValid)
            {
                return View(file);
            }

            var willBeChanged = applicationDb.Files.Find(id);
            if (willBeChanged.Name != file.Name)
                willBeChanged.Name = file.Name + "." + willBeChanged.Format;
            willBeChanged.Private = file.Private;
            applicationDb.SaveChanges();
            return RedirectToAction("Directory");
        }

        //
        //Get: /Account/UploadFile

        [Authorize]
        public ActionResult UploadFile()
        {
            return View();
        }
        //
        // POST: /Account/UploadFile
        [Authorize]
        [HttpPost]
        public ActionResult UploadFile(FileInformationViewModel fileModel)
        {
            if (!ModelState.IsValid)
            {
                return View(fileModel);
            }

            DirectoryInfo dirInfo = new DirectoryInfo(Server.MapPath("~/UserFiles/" + User.Identity.Name));
            dirInfo.Create();
            if (dirInfo.GetFiles(fileModel.File.FileName).Length == 0)//перевірямо чи існує вже такий файл
            {
                string fullFileName = dirInfo.FullName + "\\" + fileModel.File.FileName;
                fileModel.File.SaveAs(fullFileName);
                string[] format = fileModel.File.FileName.Split('.');
                FileInformation fi = new FileInformation { Name = fileModel.File.FileName, Owner = User.Identity.Name, Format = format[format.Length - 1], FullName = fullFileName, Private = fileModel.Private, DateUpload = DateTime.Now };
                applicationDb.Files.Add(fi);
                applicationDb.SaveChanges();
            }
            else
            {
                return new HttpStatusCodeResult(400);
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult DeleteFile(int id)
        {
            FileInformation delFile = applicationDb.Files.Find(id);
            applicationDb.Files.Remove(delFile);
            FileInfo fileOnServer = new FileInfo(delFile.FullName);
            fileOnServer.Delete();

            applicationDb.SaveChanges();
            return RedirectToAction("Directory");
        }
    }
}