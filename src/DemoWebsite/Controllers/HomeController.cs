using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace DemoWebsite.Controllers
{
    /// <summary>
    /// Home page controller
    /// </summary>
    public class HomeController : Controller
    {
        private IHostingEnvironment hostingEnv;

        /// <summary>
        /// Construct function
        /// </summary>
        /// <param name="env"></param>
        public HomeController(IHostingEnvironment env)
        {
            hostingEnv = env;
        }

        /// <summary>
        /// home page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// upload file
        /// </summary>
        /// <returns></returns>
        public IActionResult Uploading()
        {
            return View();
        }

        /// <summary>
        /// Get the file list from httprequest
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Uploading(IList<IFormFile> files)
        {
            var djiaf = Request.Form.Files;
            long size = 0L;
            foreach (var file in files)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition)
                    .FileName
                    .Trim('"');
                fileName = hostingEnv.WebRootPath + $@"\{fileName}";
                size += file.Length;
                using(FileStream fs = System.IO.File.Create(fileName))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            ViewBag.Message = $"{files.Count} file(s)/{size} bytes upload successfully!";
            return View();
        }

        /// <summary>
        /// Upload file use ajax
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadFilesAjax()
        {
            long size = 0;
            var files = Request.Form.Files;
            foreach (var file in files)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                fileName = hostingEnv.WebRootPath + $@"\{fileName}";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(fileName))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            var message=$"{files.Count} file(s)/{size} bytes upload successfully!";
            return Json(message);
        }
    }
}
