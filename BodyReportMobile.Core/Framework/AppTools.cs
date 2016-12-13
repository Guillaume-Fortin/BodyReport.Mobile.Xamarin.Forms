using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XLabs.Ioc;

namespace BodyReportMobile.Core.Framework
{
    public class CachingImageResult<T> where T : class
    {
        public bool ImageLoaded { get; set; } = false;
        public T BindingObject { get; set; } = null;
        public string ImagePath { get; set; }
    }

    public delegate void CachingImageResultHandler<T>(CachingImageResult<T> result) where T : class;

    public class AppTools
    {
        private static AppTools _instance = null;

        /// <summary>
        /// ./temp/
        /// </summary>
        public static string TempDirectory { get; set; }

        /// <summary>
        /// ./images/
        /// </summary>
        public static string ImagesDirectory { get; set; }

        /// <summary>
        /// ./images/bodyexercises/
        /// </summary>
        public static string BodyExercisesImagesDirectory { get; set; }

        /// <summary>
        /// ./userprofil/
        /// </summary>
        public static string UserProfilLocalDirectory { get; set; }

        private AppTools()
        {
        }

        public static AppTools Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppTools();
                }
                return _instance;
            }
        }

        public void Init()
        {
            try
            {
                var fileManager = Resolver.Resolve<IFileManager>();
                string documentPath = fileManager.GetDocumentPath();

                TempDirectory = Path.Combine(documentPath, "temp");
                if (!fileManager.DirectoryExist(TempDirectory))
                    fileManager.CreateDirectory(TempDirectory);

                ImagesDirectory = Path.Combine(documentPath, "images");
                if(!fileManager.DirectoryExist(ImagesDirectory))
                    fileManager.CreateDirectory(ImagesDirectory);

                BodyExercisesImagesDirectory = Path.Combine(ImagesDirectory, "bodyexercises");
                if (!fileManager.DirectoryExist(BodyExercisesImagesDirectory))
                    fileManager.CreateDirectory(BodyExercisesImagesDirectory);
                
                UserProfilLocalDirectory = Path.Combine(documentPath, "userprofil");
                if (!fileManager.DirectoryExist(UserProfilLocalDirectory))
                    fileManager.CreateDirectory(UserProfilLocalDirectory);
            }
            catch(Exception except)
            {
                ILogger.Instance.Error("Unable to init AppTools", except);
            }
        }

        public string GetUserImageLocalPath(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;
            return Path.Combine(UserProfilLocalDirectory, userId + ".png");
        }

        public async Task CachingImageAsync<T>(T bindingObject, string urlImage, string localImagePath, CachingImageResultHandler<T> cachingImageResultEvent) where T : class
        {
            var cachingImageResult = new CachingImageResult<T>() { BindingObject = bindingObject, ImagePath = localImagePath };

            if (bindingObject != null && !string.IsNullOrWhiteSpace(urlImage) && !string.IsNullOrWhiteSpace(localImagePath))
            {
				var fileManager = Resolver.Resolve<IFileManager> ();
                try
                {
					if (fileManager.FileExist(localImagePath) && fileManager.FileLength(localImagePath) > 0)
                        cachingImageResult.ImageLoaded = true;
                    else
                    {
						if (await HttpConnector.Instance.DownloadFileAsync (urlImage, localImagePath, true))
						{
							if (fileManager.FileExist (localImagePath) && fileManager.FileLength (localImagePath) == 0)
								fileManager.DeleteFile (localImagePath);
							else
								cachingImageResult.ImageLoaded = true;
						}
                    }
                    cachingImageResultEvent?.Invoke(cachingImageResult);
                }
                catch (Exception except)
                {
					if (fileManager.FileExist (localImagePath) && fileManager.FileLength (localImagePath) == 0)
						fileManager.DeleteFile (localImagePath);
                    ILogger.Instance.Error("Unable caching image", except);
                }
            }
        }
    }
}
