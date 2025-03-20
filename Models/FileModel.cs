using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace imagein_api.Models
{
    public class FileModel
    {
        public string fileName {get; set;}
        public IFormFile file {get; set;}
    }
}
