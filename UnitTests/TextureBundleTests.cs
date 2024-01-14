using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessTextures;

namespace UnitTests
{
    [TestClass]
    public class TextureBundleTests
    {
        [TestMethod]
        public void CreateFromFileListTest()
        {
            var files = new List<string>();
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map1_albedo.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map1_parameter.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map1_normal.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map2_albedo.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map2_parameter.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map2_normal.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map3_albedo.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map3_parameter.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map3_normal.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map4_albedo.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map4_parameter.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map4_normal.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map5_albedo.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map5_parameter.tga");
            files.Add(@"g:\Trainz\Content\editing\kuid 998317 100055 UP AC4460CW 6896-6942\c449w_body\map5_normal.tga");

            files.Shuffle();

            TextureBundle[] bundles = TextureBundle.Create(files);
            Assert.AreEqual(5, bundles.Length);

            TextureBundle bundle1 = new TextureBundle
            {
                Albedo = new ParsedTextureName { Name = "map1", TextureType = TextureTypes.Albedo, Extension = "tga" },
                Normal = new ParsedTextureName { Name = "map1", TextureType = TextureTypes.Normal, Extension = "tga" },
                Parameter = new ParsedTextureName { Name = "map1", TextureType = TextureTypes.Parameter, Extension = "tga" }
            };

            TextureBundle bundle2 = new TextureBundle
            {
                Albedo = new ParsedTextureName { Name = "map2", TextureType = TextureTypes.Albedo, Extension = "tga" },
                Normal = new ParsedTextureName { Name = "map2", TextureType = TextureTypes.Normal, Extension = "tga" },
                Parameter = new ParsedTextureName { Name = "map2", TextureType = TextureTypes.Parameter, Extension = "tga" }
            };

            TextureBundle bundle3 = new TextureBundle
            {
                Albedo = new ParsedTextureName { Name = "map3", TextureType = TextureTypes.Albedo, Extension = "tga" },
                Normal = new ParsedTextureName { Name = "map3", TextureType = TextureTypes.Normal, Extension = "tga" },
                Parameter = new ParsedTextureName { Name = "map3", TextureType = TextureTypes.Parameter, Extension = "tga" }
            };

            TextureBundle bundle4 = new TextureBundle
            {
                Albedo = new ParsedTextureName { Name = "map4", TextureType = TextureTypes.Albedo, Extension = "tga" },
                Normal = new ParsedTextureName { Name = "map4", TextureType = TextureTypes.Normal, Extension = "tga" },
                Parameter = new ParsedTextureName { Name = "map4", TextureType = TextureTypes.Parameter, Extension = "tga" }
            };

            TextureBundle bundle5 = new TextureBundle
            {
                Albedo = new ParsedTextureName { Name = "map5", TextureType = TextureTypes.Albedo, Extension = "tga" },
                Normal = new ParsedTextureName { Name = "map5", TextureType = TextureTypes.Normal, Extension = "tga" },
                Parameter = new ParsedTextureName { Name = "map5", TextureType = TextureTypes.Parameter, Extension = "tga" }
            };

            Assert.IsTrue(bundles.Contains(bundle1));
            Assert.IsTrue(bundles.Contains(bundle2));
            Assert.IsTrue(bundles.Contains(bundle3));
            Assert.IsTrue(bundles.Contains(bundle4));
            Assert.IsTrue(bundles.Contains(bundle5));
        }
    }
}
