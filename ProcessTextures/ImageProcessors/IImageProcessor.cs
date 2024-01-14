namespace ProcessTextures.ImageProcessors
{
    internal interface IImageProcessor
    {
        void Initialize();

        bool CheckImageSizes(params string[] sourceFiles);

        bool ConvertToPng(string sourceFilePath, string outputFilePath);

        bool ProcessImages(string inAlbedoFileName, string inParameterFileName, string inNormalFileName, string outAlbedoFileName, string outParameterFileName, string outNormalFileName);
    }
}
