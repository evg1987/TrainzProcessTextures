using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace ProcessTextures
{
    public struct ParsedTextureName
    {
        static readonly Regex regex1 = new Regex(@"(\w[^_]{1,})_(\w{1,}).(\w{1,3})$");
        static readonly Regex regex2 = new Regex(@"(\w{1,}).(\w{1,3})$");

        public string Name;
        public TextureTypes TextureType;
        public string Extension;

        public ParsedTextureName(string Name, TextureTypes TextureType, string Extension)
        {
            this.Name = Name;
            this.TextureType = TextureType;
            this.Extension = Extension;
        }

        public override string ToString()
        {
            string textureTypeString = TextureType == TextureTypes.Unknown
                ? ""
                : "_" + TextureType.ToString().ToLowerInvariant();
            
            return $"{Name}{textureTypeString}.{Extension}";
        }

        /// <summary>
        /// Parse file name
        /// </summary>
        /// <param name="FileName">File name (e.g map1_albedo.tga)</param>
        /// <exception cref="FormatException"/>
        public static ParsedTextureName Parse(string FileName)
        {
            // name like map1_albedo.tga
            string FileNameShort = Path.GetFileName(FileName).ToLowerInvariant();
            string[] parts1 = regex1.Split(FileNameShort);
            if (parts1.Length == 5)
            {
                TextureTypes textureType = TextureTypes.Unknown;
                switch (parts1[2])
                {
                    case "albedo":
                        textureType = TextureTypes.Albedo;
                        break;
                    case "parameter":
                        textureType = TextureTypes.Parameter;
                        break;
                    case "normal":
                        textureType = TextureTypes.Normal;
                        break;
                }

                return new ParsedTextureName
                {
                    Name = textureType != TextureTypes.Unknown ? parts1[1] : parts1[1] + "_" + parts1[2],
                    TextureType = textureType,
                    Extension = parts1[3]
                };
            }
            else
            {
                // name like parameters.tga
                string[] parts2 = regex2.Split(FileNameShort);
                if (parts2.Length == 4)
                {
                    return new ParsedTextureName
                    {
                        Name = parts2[1],
                        TextureType = TextureTypes.Unknown,
                        Extension = parts2[2]
                    };
                }
                else
                {
                    throw new FormatException(Path.GetFileName(FileName));
                }
            }
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Extension);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return base.Equals(obj);
        }

        public static bool operator ==(ParsedTextureName lhs, ParsedTextureName rhs)
        {
            return lhs.Name == rhs.Name &&
                   lhs.TextureType == rhs.TextureType &&
                   lhs.Extension == rhs.Extension;
        }

        public static bool operator !=(ParsedTextureName lhs, ParsedTextureName rhs)
        {
            return !(lhs == rhs);
        }
    }
}
