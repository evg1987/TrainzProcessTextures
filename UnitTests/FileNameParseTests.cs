using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessTextures;

namespace UnitTests
{
    [TestClass]
    public class FileNameParseTests
    {
        [TestMethod]
        public void ParseTest()
        {
            ParsedTextureName parsedName1 = ParsedTextureName.Parse("map1_albedo.png");
            Assert.AreEqual("map1", parsedName1.Name);
            Assert.AreEqual(TextureTypes.Albedo, parsedName1.TextureType);
            Assert.AreEqual("png", parsedName1.Extension);

            ParsedTextureName parsedName2 = ParsedTextureName.Parse("map3_parameter.png");
            Assert.AreEqual("map3", parsedName2.Name);
            Assert.AreEqual(TextureTypes.Parameter, parsedName2.TextureType);
            Assert.AreEqual("png", parsedName2.Extension);

            ParsedTextureName parsedName3 = ParsedTextureName.Parse("Map2_Normal.PNG");
            Assert.AreEqual("map2", parsedName3.Name);
            Assert.AreEqual(TextureTypes.Normal, parsedName3.TextureType);
            Assert.AreEqual("png", parsedName3.Extension);

            ParsedTextureName parsedName4 = ParsedTextureName.Parse("map9_abc.txt");
            Assert.AreEqual("map9_abc", parsedName4.Name);
            Assert.AreEqual(TextureTypes.Unknown, parsedName4.TextureType);
            Assert.AreEqual("txt", parsedName4.Extension);
        }

        [TestMethod]
        public void ToStringTest()
        {
            var parsedName1 = new ParsedTextureName
            {
                Name = "map1",
                TextureType = TextureTypes.Albedo,
                Extension = "png"
            };
            Assert.AreEqual("map1_albedo.png", parsedName1.ToString());

            var parsedName2 = new ParsedTextureName
            {
                Name = "map2",
                TextureType = TextureTypes.Parameter,
                Extension = "png"
            };
            Assert.AreEqual("map2_parameter.png", parsedName2.ToString());

            var parsedName3 = new ParsedTextureName
            {
                Name = "map3",
                TextureType = TextureTypes.Normal,
                Extension = "png"
            };
            Assert.AreEqual("map3_normal.png", parsedName3.ToString());
        }

        [TestMethod]
        public void NameTest1()
        {
            var parsedName = ParsedTextureName.Parse("map1.png");
            Assert.AreEqual("map1", parsedName.Name);
            Assert.AreEqual(TextureTypes.Unknown, parsedName.TextureType);
            Assert.AreEqual("png", parsedName.Extension);
        }

        [TestMethod]
        public void NameTest2()
        {
            var parsedName = ParsedTextureName.Parse("map1_abc.png");
            Assert.AreEqual("map1_abc", parsedName.Name);
            Assert.AreEqual(TextureTypes.Unknown, parsedName.TextureType);
            Assert.AreEqual("png", parsedName.Extension);
        }
    }
}
