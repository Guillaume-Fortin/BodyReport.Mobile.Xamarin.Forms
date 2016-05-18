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
            }
            catch(Exception except)
            {
                ILogger.Instance.Error("Unable to init AppTools", except);
            }
        }

        public async Task CachingImageAsync<T>(T bindingObject, string urlImage, string localImagePath, CachingImageResultHandler<T> cachingImageResultEvent) where T : class
        {
            var cachingImageResult = new CachingImageResult<T>() { BindingObject = bindingObject, ImagePath = localImagePath };

            if (bindingObject != null && !string.IsNullOrWhiteSpace(urlImage) && !string.IsNullOrWhiteSpace(localImagePath))
            {
                try
                {
                    var fileManager = Resolver.Resolve<IFileManager>();
                    if (fileManager.FileExist(localImagePath))
                        cachingImageResult.ImageLoaded = true;
                    else
                    {
                        if (await HttpConnector.Instance.DownloadFileAsync(urlImage, localImagePath, true))
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
