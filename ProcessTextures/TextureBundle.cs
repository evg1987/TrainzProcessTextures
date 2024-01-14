using System.Diagnostics;

namespace ProcessTextures
{
    public struct TextureBundle
    {
        public ParsedTextureName Albedo;
        public ParsedTextureName Parameter;
        public ParsedTextureName Normal;
        public ParsedTextureName Other;

        public bool IsValid()
        {
            return (Albedo.IsValid() && Normal.IsValid() && Parameter.IsValid()) || Other.IsValid();
        }

        /// <summary>
        /// Create texture bundles from file name list
        /// </summary>
        /// <param name="FileNames">List of file names</param>
        public static TextureBundle[] Create(IEnumerable<string> FileNames)
        {
            return Create(FileNames.ToArray());
        }

        public override string ToString()
        {
            return IsValid() ? Albedo.Name : "[Invalid]";
        }

        /// <summary>
        /// Create texture bundles from file name list
        /// </summary>
        /// <param name="FileNames">List of file names</param>
        public static TextureBundle[] Create(string[] FileNames)
        {
            var result = new List<TextureBundle>(20);

            // parse file names
            var parsedNames = new List<ParsedTextureName>(FileNames.Length);
            foreach (string FileName in FileNames)
            {
                try
                {
                    ParsedTextureName parsedName = ParsedTextureName.Parse(FileName);
                    if (parsedName.IsValid())
                    {
                        parsedNames.Add(parsedName);
                    }
                }
                catch (FormatException)
                {
                    Debug.WriteLine($"WARNING: Cannot parse file name: {Path.GetFileName(FileName)}");
                }
            }

            while (parsedNames.Count > 0)
            {
                List<ParsedTextureName> otherTextures = parsedNames.Where(n => n.Name == parsedNames[0].Name)
                           .Where(n => n.Extension == parsedNames[0].Extension)
                           .Where(n => n.TextureType != parsedNames[0].TextureType)
                           .ToList();

                otherTextures.Add(parsedNames[0]);

                var textureBundle = new TextureBundle();
                foreach (ParsedTextureName parsedName in otherTextures)
                {
                    switch (parsedName.TextureType)
                    {
                        case TextureTypes.Albedo:
                            textureBundle.Albedo = parsedName;
                            break;
                        case TextureTypes.Parameter:
                            textureBundle.Parameter = parsedName;
                            break;
                        case TextureTypes.Normal:
                            textureBundle.Normal = parsedName;
                            break;
                        default:
                            textureBundle.Other = parsedName;
                            break;
                    }

                    parsedNames.RemoveAll(n => n == parsedName);
                }

                if (textureBundle.IsValid())
                {
                    result.Add(textureBundle);
                }
            }

            return result.ToArray();
        }
    }
}
