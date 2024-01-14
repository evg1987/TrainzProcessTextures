using System.Diagnostics;
using System.Reflection;
using ImageMagick;
using ProcessTextures.Properties;

namespace ProcessTextures
{
    public class Program
    {
        class WorkerThreadArgs
        {
            public readonly List<TextureBundle> TextureBundles;
            public readonly Parameters Parameters;

            public WorkerThreadArgs(Parameters parameters)
            {
                TextureBundles = new List<TextureBundle>();
                Parameters = parameters;
            }
        }

        // result codes
        const int RESULT_OK = 0;
        const int RESULT_BadArguments = 1;
        const int RESULT_MissingInputDirectory = 2;

        // entry point
        static int Main(string[] args)
        {
            //AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
            //{
            //    string dllName = e.Name.Contains(',') 
            //        ? e.Name.Substring(0, e.Name.IndexOf(',')) 
            //        : e.Name.Replace(".dll", "");

            //    dllName = dllName.Replace(".", "_");

            //    Console.WriteLine($"Assembly request: {dllName}");

            //    if (dllName.EndsWith("_resources"))
            //    {
            //        return null;
            //    }
            //    else
            //    {
            //        return Assembly.Load(Resources.Magick_NET_Q16_AnyCPU);
            //    }
            //};

            // parse command line arguments
            Parameters parameters;
            if (args.Length > 0)
            {
                if (args[0] == "-h" || args[0] == "--help")
                {
                    // print help and exit
                    Console.WriteLine(Resources.Help);
                    return RESULT_OK;
                }
                else
                {
                    // parse paramters from command line arguments
                    try
                    {
                        parameters = new Parameters(args);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERROR: Bad command line arguments ({ex.Message})");
                        return RESULT_BadArguments;
                    }
                }
            }
            else
            {
                // use default paramters
                parameters = new Parameters();
            }

            // check if input directory exists
            if (!Directory.Exists(parameters.InputDirectory))
            {
                Console.WriteLine($"ERROR: Missing input directory ({parameters.InputDirectory})");
                return RESULT_MissingInputDirectory;
            }
            
            MagickNET.Initialize();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // add root directory and subdirectories
            var directories = new List<string>();
            directories.Add(parameters.InputDirectory);
            directories.AddRange(Directory.GetDirectories(parameters.InputDirectory, "*", SearchOption.AllDirectories));

            // process each directory
            foreach (string directory in directories)
            {
                // set parameters for current directory
                Parameters parametersCurrent = Parameters.Copy(parameters);
                parametersCurrent.InputDirectory = directory;
                if (parameters.InputDirectory == parameters.OutputDirectory)
                {
                    parametersCurrent.OutputDirectory = directory;
                }
                else
                {
                    // relative path
                    string subdir = Path.GetRelativePath(parameters.InputDirectory, directory);
                    if (subdir != ".")
                    {
                        parametersCurrent.OutputDirectory = Path.Combine(parametersCurrent.OutputDirectory, subdir);
                    }
                }

                ProcessDirectory(directory, parametersCurrent);
            }

            // done
            Console.WriteLine($"DONE Elapsed time: {stopwatch.Elapsed.TotalSeconds:0.0000} s");
            return RESULT_OK;
        }

        /// <summary>
        /// Modify all textures in directory
        /// </summary>
        static void ProcessDirectory(string directory, Parameters parameters)
        {
            // read file names from input directory and create texture bundles
            TextureBundle[] textureBundles = TextureBundle.Create(Directory.GetFiles(parameters.InputDirectory, "*.tga", SearchOption.TopDirectoryOnly));

            if (textureBundles.Length == 0)
            {
                return;
            }

            Console.WriteLine($"Number of files: {textureBundles.Length}");
            Console.WriteLine($"Input directory: {parameters.InputDirectory}");
            Console.WriteLine($"Output directory: {parameters.OutputDirectory}");
            //Console.WriteLine($"Generate PSD: {parameters.GeneratePSD}"); // not supported in this version

            // start worker threads
            var threads = new List<Thread>();
            {
                int numThreads = Environment.ProcessorCount;
                int chunkSize = Math.Max(1, textureBundles.Length / numThreads);
                int chunkSizeFirst = chunkSize + (Math.Max(0, textureBundles.Length - chunkSize * numThreads));
                int chunkIndex = 0;

                for (int i = 0; i < numThreads; i++)
                {
                    // assign texture bundles to each worker thread
                    var workerArgs = new WorkerThreadArgs(parameters);
                    int chunkSizeCurrent = i == 0 ? chunkSizeFirst : chunkSize;
                    for (int j = 0; j < chunkSizeCurrent && chunkIndex < textureBundles.Length; j++, chunkIndex++)
                    {
                        workerArgs.TextureBundles.Add(textureBundles[chunkIndex]);
                    }

                    // start worker thread
                    if (workerArgs.TextureBundles.Count > 0)
                    {
                        Thread thread = new Thread(ProcessTextures);
                        threads.Add(thread);
                        thread.Start(workerArgs);
                    }
                }
            }

            // wait for job done
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        /// <summary>
        /// Parametrized thread start (it only need for start thread with parameters)
        /// </summary>
        /// <param name="args">WorkerThreadArgs</param>
        /// <exception cref="ArgumentException"></exception>
        static void ProcessTextures(object? args)
        {
            if (args is WorkerThreadArgs)
            {
                var workerThreadArgs = (WorkerThreadArgs)args;
                foreach (TextureBundle textureBundle in workerThreadArgs.TextureBundles)
                {
                    if (textureBundle.Albedo.IsValid())
                    {
                        ProcessTexture(textureBundle, workerThreadArgs.Parameters);
                    }
                    else
                    {
                        ConvertToPNG(textureBundle, workerThreadArgs.Parameters);
                    }
                }
            }
            else
            {
                throw new ArgumentException(nameof(args));
            }
        }

        /// <summary>
        /// Convert to PNG and save
        /// </summary>
        static bool ConvertToPNG(TextureBundle textureBundle, Parameters parameters)
        {
            string fullPath = Path.Combine(parameters.InputDirectory, textureBundle.Other.ToString());
            
            // ensure file exists
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"ERROR: File not found: {fullPath}");
                return false;
            }

            var outputFileName = new ParsedTextureName
            {
                Name = textureBundle.Other.Name,
                TextureType = TextureTypes.Unknown,
                Extension = "png"
            };

            string outputPath = Path.Combine(parameters.OutputDirectory, outputFileName.ToString());
            using (var imageAlbedo = new MagickImage(fullPath))
            {
                imageAlbedo.AutoOrient();
                imageAlbedo.Write(outputPath, MagickFormat.Png32);
                return true;
            }
        }

        /// <summary>
        /// Modify textures
        /// </summary>
        static bool ProcessTexture(TextureBundle textureBundle, Parameters parameters)
        {
            string fullPathAlbedo = Path.Combine(parameters.InputDirectory, textureBundle.Albedo.ToString());
            string fullPathParameter = Path.Combine(parameters.InputDirectory, textureBundle.Parameter.ToString());
            string fullPathNormal = Path.Combine(parameters.InputDirectory, textureBundle.Normal.ToString());

            // ensure all neccessary files exist
            {
                bool result = true;

                // albedo
                if (!File.Exists(fullPathAlbedo))
                {
                    result = false;
                    Console.WriteLine($"ERROR: File not found: {fullPathAlbedo}");
                }

                // parameter
                if (!File.Exists(fullPathParameter))
                {
                    result = false;
                    Console.WriteLine($"ERROR: File not found: {fullPathParameter}");
                }

                // normal

                if (!result)
                {
                    return false;
                }
            }

            // check image sizes
            var infoAlbedo = new MagickImageInfo(fullPathAlbedo);
            var infoParameter = new MagickImageInfo(fullPathParameter);
            if (infoAlbedo.Width != infoParameter.Width || infoAlbedo.Height != infoParameter.Height)
            {
                Console.WriteLine($"ERROR: Image size mismatch. Ensure that albedo and parameter images have same size ({textureBundle.Albedo}, {textureBundle.Parameter})");
                return false;
            }

            // create files names and prepare output directory
            string outputPathAlbedo, outputPathParameter, outputPathNormal;
            {
                // create file names
                outputPathAlbedo = Path.Combine(parameters.OutputDirectory, new ParsedTextureName(textureBundle.Albedo.Name, TextureTypes.Albedo, "png").ToString());
                outputPathParameter = Path.Combine(parameters.OutputDirectory, new ParsedTextureName(textureBundle.Parameter.Name, TextureTypes.Parameter, "png").ToString());
                outputPathNormal = Path.Combine(parameters.OutputDirectory, new ParsedTextureName(textureBundle.Normal.Name, TextureTypes.Normal, "png").ToString());
                
                // create output directory
                if (!Directory.Exists(parameters.OutputDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(parameters.OutputDirectory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERROR: Unable to create output directory: {parameters.OutputDirectory} ({ex.Message})");
                        return false;
                    }
                }
            }

            // process images
            using (var imageAlbedo = new MagickImage(fullPathAlbedo))
            using (var imageParameter = new MagickImage(fullPathParameter))
            using (var imageNormal = new MagickImage(fullPathNormal))
            {
                Console.WriteLine($"PROCESSING {textureBundle.Albedo.Name}");

                if (ProcessImages(imageAlbedo, imageParameter))
                {
                    imageAlbedo.AutoOrient();
                    imageAlbedo.Write(outputPathAlbedo, MagickFormat.Png32);

                    imageParameter.AutoOrient();
                    imageParameter.Write(outputPathParameter, MagickFormat.Png32);

                    imageNormal.AutoOrient();
                    imageNormal.Write(outputPathNormal, MagickFormat.Png32);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Modify images
        /// </summary>
        static private bool ProcessImages(MagickImage imageAlbedo, MagickImage imageParameter)
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
