using imagein_api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using imagein_api.Helpers;

namespace imagein_api.Controllers
{

[Route("api/[controller]")]
[ApiController]
public class ImageController : ControllerBase
{
    private readonly ImaggaHelper _imaggaHelper;

    public ImageController()
    {
        _imaggaHelper = new ImaggaHelper();
    }

    [HttpPost]
    [Route("UploadFile")]
    public async Task<Response> UploadFile([FromForm] FileModel fileModel)
    {
        var response = new Response();

        if (fileModel?.file == null || fileModel.file.Length == 0)
        {
            response.StatusCode = 400;
            response.ErrorMessage = "No file uploaded.";
            return response;
        }

        try
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string filePath = Path.Combine(uploadsFolder, fileModel.fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileModel.file.CopyToAsync(stream);
            }

            string uploadId = await _imaggaHelper.UploadImageAsync(filePath);
            var tags = await _imaggaHelper.GetImageTagsAsync(uploadId);

            response.StatusCode = 200;
            response.ErrorMessage = "Image uploaded and analyzed successfully.";
            response.Data = tags;
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            response.ErrorMessage = "An error occurred: " + ex.Message;
        }

        return response;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("pong 🏓");
    }
    }
}