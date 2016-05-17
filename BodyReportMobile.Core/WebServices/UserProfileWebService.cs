using BodyReportMobile.Core.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyReportMobile.Core.WebServices
{
    public static class UserProfileWebService
    {
        public static async Task<string> UploadUserProfilePictureAsync(string filePath)
        {
            string contentType = null;
            string extension = Path.GetExtension(filePath);
            switch(extension)
            {
                case ".png": contentType = "image/png"; break;
                case ".bmp": contentType = "image/bmp"; break;
                case ".jpeg": case ".jpg": contentType = "image/jpeg"; break;
            }
            return await HttpConnector.Instance.UpLoadFileAsync("/Api/UserProfile/UploadProfileImage", filePath, contentType);
        }
    }
}
