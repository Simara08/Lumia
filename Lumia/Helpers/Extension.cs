        using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lumia.Helpers
{
    public static class Extension
    {
       public static bool FileSize(this IFormFile file,int kb)
        {
            return file.Length/1024<=200;
        }
        public static bool FileType(this IFormFile file,string type)
        {
            return file.ContentType.Contains(type);
        }

        public async static Task<string>  SaveFileAsync(this IFormFile file,string root,params string[] folders)
        {
            var filename = Guid.NewGuid().ToString() + file.FileName;
            var respath = Path.Combine(Helper.GetPath(root, folders), filename);
            using (FileStream stream =new FileStream(respath,FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filename;
        }

    }
}

