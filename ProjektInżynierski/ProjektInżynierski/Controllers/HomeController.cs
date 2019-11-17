using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjektInżynierski.Models;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace ProjektInżynierski.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHosting;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHosting)
        {
            _webHosting = webHosting;
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Decipher(IFormFile encryptedFile, IFormFile originalFile)
        {
            if (encryptedFile != null && originalFile != null)
            {
                var fileNameOriginal = Path.Combine(_webHosting.WebRootPath,"images", Path.GetFileName(originalFile.FileName));
                var fileNameEncrypted = Path.Combine(_webHosting.WebRootPath,"images", Path.GetFileName(encryptedFile.FileName));
                using (FileStream saveFileStream = new FileStream(fileNameOriginal, FileMode.Create))
                {
                    originalFile.CopyTo(saveFileStream);
                }

                using (FileStream saveFileStream = new FileStream(fileNameEncrypted, FileMode.Create))
                {
                    encryptedFile.CopyTo(saveFileStream);
                }

                Bitmap convertedImageOriginal = ConvertToBitmap(fileNameOriginal);
                Bitmap convertedImageEncrypted = ConvertToBitmap(fileNameEncrypted);
                string decryptedMessage = SteganographyHelper.DecryptText(convertedImageOriginal, convertedImageEncrypted);
                convertedImageOriginal.Dispose();
                convertedImageEncrypted.Dispose();

                ViewData["decryptedMessage"] = decryptedMessage;
            }


            return View();
        }

        public IActionResult Cipher(string messageText, IFormFile pic)
        {
            if (pic != null && messageText != null)
            {

                // path to save the image file
                var fileName = Path.Combine(_webHosting.WebRootPath, Path.GetFileName(pic.FileName));
                using (FileStream saveFileStream = new FileStream(fileName, FileMode.Create))
                {
                    pic.CopyTo(saveFileStream);
                }

                Bitmap convertedImage = ConvertToBitmap(fileName);
                Bitmap encryptedImage = SteganographyHelper.EncryptText(messageText, convertedImage);
                convertedImage.Dispose();

                fileName = Path.Combine(_webHosting.WebRootPath,"images", "yourEncryptedImage.png");
                encryptedImage.Save(fileName, System.Drawing.Imaging.ImageFormat.Png);

            }

            
            return View();
        }



        public IActionResult Download()
        {

            string filename = "yourEncryptedImage.png";
            var file = Path.Combine(_webHosting.WebRootPath,"images", filename);
            return File(System.IO.File.ReadAllBytes(file), "image/png", filename);
        }

        public Bitmap ConvertToBitmap(string fileName)
        {
            Bitmap bitmap;
            using (Stream bmpStream = System.IO.File.Open(fileName, System.IO.FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);

                bitmap = new Bitmap(image);

            }
            return bitmap;
        }

        


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

  
      

    }
}
