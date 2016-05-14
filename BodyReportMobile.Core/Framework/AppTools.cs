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
    public class CachingImageResult
    {
        public bool ImageLoaded { get; set; } = false;
        public int IdImage { get; set; } = 0;
        public string ImagePath { get; set; }
    }

    public delegate void CachingImageResultHandler(CachingImageResult result);

    public class AppTools
    {
        private static AppTools _instance = null;

        /// <summary>
        /// ./bodyexercises/
        /// </summary>
        public static string ImagesDirectory { get; set; }

        /// <summary>
        /// ./images/bodyexercises/
        /// </summary>
        public static string BodyExercisesImagesDirectory { get; set; }

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

                ImagesDirectory = Path.Combine(documentPath, "images");
                if(!fileManager.DirectoryExist(ImagesDirectory))
                    fileManager.CreateDirectory(ImagesDirectory);

                BodyExercisesImagesDirectory = Path.Combine(ImagesDirectory, "bodyexercises");
                if (!fileManager.DirectoryExist(BodyExercisesImagesDirectory))
                    fileManager.CreateDirectory(BodyExercisesImagesDirectory);
            }
            catch(Exception except)
            {
                ILogger.Instance.Error("Unable to init AppTools", except);
            }
        }

        public async Task CachingImage(int idImage, string urlImage, string localImagePath, CachingImageResultHandler cachingImageResultEvent)
        {
            var cachingImageResult = new CachingImageResult() { IdImage = idImage, ImagePath = localImagePath };

            if (idImage != 0 && !string.IsNullOrWhiteSpace(urlImage) && !string.IsNullOrWhiteSpace(localImagePath))
            {
                try
                {
                    var fileManager = Resolver.Resolve<IFileManager>();
                    if (fileManager.FileExist(localImagePath))
                        cachingImageResult.ImageLoaded = true;
                    else
                    {
                        if (await HttpConnector.Instance.DownloadFile(urlImage, localImagePath, true))
                            cachingImageResult.ImageLoaded = true;
                    }
                    cachingImageResultEvent?.Invoke(cachingImageResult);
                }
                catch (Exception except)
                {
                    ILogger.Instance.Error("Unable caching image", except);
                }
            }
        }
    }
}
