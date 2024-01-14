using ImageMagick;

namespace ProcessTextures.ImageProcessors
{
    internal class ImageMagickProcessor : IImageProcessor
    {
        public void Initialize()
        {
            MagickNET.Initialize();
        }

        public bool CheckImageSizes(params string[] sourceFiles)
        {
            var info0 = new MagickImageInfo(sourceFiles[0]);
            for (int i = 1; i < sourceFiles.Length; i++)
            {
                var info = new MagickImageInfo(sourceFiles[i]);
                if (info.Width != info0.Width || info.Height != info0.Height)
                {
                    return false;
                }
            }

            return true;
        }

        public bool ConvertToPng(string sourceFilePath, string outputFilePath)
        {
            try
            {
                using (var image = new MagickImage(sourceFilePath))
                {
                    image.AutoOrient();
                    image.Write(outputFilePath, MagickFormat.Png32);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Unable to convert file to png: {sourceFilePath} to {outputFilePath} ({ex.Message})");
                return false;
            }
        }

        public bool ProcessImages(string inAlbedoFileName, string inParameterFileName, string inNormalFileName, string outAlbedoFileName, string outParameterFileName, string outNormalFileName)
        {
            // process images
            using (var imageAlbedo = new MagickImage(inAlbedoFileName))
            using (var imageParameter = new MagickImage(inParameterFileName))
            using (var imageNormal = new MagickImage(inNormalFileName))
            {
                Console.WriteLine($"PROCESSING {Path.GetFileName(inAlbedoFileName)}");

                if (ProcessImages(imageAlbedo, imageParameter))
                {
                    imageAlbedo.AutoOrient();
                    imageAlbedo.Write(outAlbedoFileName, MagickFormat.Png32);

                    imageParameter.AutoOrient();
                    imageParameter.Write(outParameterFileName, MagickFormat.Png32);

                    imageNormal.AutoOrient();
                    imageNormal.Write(outNormalFileName, MagickFormat.Png32);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool ProcessImages(MagickImage imageAlbedo, MagickImage imageParameter)
        {
            imageAlbedo.BackgroundColor = MagickColors.Black;
            imageAlbedo.Sharpen();

            // multiply by parameter's blue channel (AO)
            using (var alpha = (MagickImage)imageAlbedo.Separate(Channels.Alpha).First())
            using (var blue = (MagickImage)imageParameter.Separate(Channels.Blue).First())
            {
                if (!imageAlbedo.IsOpaque)
                {
                    blue.Composite(alpha, CompositeOperator.Multiply, Channels.RGB);
                }

                blue.BackgroundColor = MagickColors.Black;
                imageAlbedo.Composite(blue, CompositeOperator.Multiply, Channels.RGB);

                return true;
            }
        }
    }
}
