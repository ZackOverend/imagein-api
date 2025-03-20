using imagein_api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace imagein_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpPost]
        [Route("UploadFile")]
        public  Response UploadFile([FromForm] FileModel fileModel)
        {
            Response response = new Response();

            if (fileModel == null || fileModel.file == null || fileModel.file.Length == 0)
            {
                response.StatusCode = 400;
                response.ErrorMessage = "No File Uploaded.";
            }
            try
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                // Ensure the directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string filePath = Path.Combine(uploadsFolder, fileModel.fileName);

                using (Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    fileModel.file.CopyTo(stream);
                }

                response.StatusCode = 200;
                response.ErrorMessage = "Image created successfully";
            }
            catch (Exception ex)
            {
                response.StatusCode = 100;
                response.ErrorMessage = "Some error occured" + ex.Message;
            }
            return response;
        }
    }
}