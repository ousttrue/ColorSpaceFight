using NUnit.Framework;
using System;
using System.Linq;
using UnityEngine;



namespace ColorSpaceFight
{
    public class ColorSpaceTests
    {

        [Test]
        public void ColorSpaceTest()
        {
            {
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);
                var desc = default(D3D11_TEXTURE2D_DESC);
                TextureHelper.GetTextureDesc(texture.GetNativeTexturePtr(), ref desc);
                Assert.AreEqual(DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, desc.Format);
            }
            {
                var texture = new Texture2D(2, 2);
                var desc = default(D3D11_TEXTURE2D_DESC);
                TextureHelper.GetTextureDesc(texture.GetNativeTexturePtr(), ref desc);
                Assert.AreEqual(DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, desc.Format);
            }
            {
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false, false);
                var desc = default(D3D11_TEXTURE2D_DESC);
                TextureHelper.GetTextureDesc(texture.GetNativeTexturePtr(), ref desc);
                Assert.AreEqual(DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, desc.Format);
            }


            {
                var texture = new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32);
                texture.Create();
                var desc = default(D3D11_TEXTURE2D_DESC);
                TextureHelper.GetTextureDesc(texture.GetNativeTexturePtr(), ref desc);
                Assert.AreEqual(DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_TYPELESS, desc.Format);
            }
            {
                var texture = new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                texture.Create();
                var desc = default(D3D11_TEXTURE2D_DESC);
                TextureHelper.GetTextureDesc(texture.GetNativeTexturePtr(), ref desc);
                Assert.AreEqual(DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_TYPELESS, desc.Format);
            }
            {
                var texture = new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
                texture.Create();
                var desc = default(D3D11_TEXTURE2D_DESC);
                TextureHelper.GetTextureDesc(texture.GetNativeTexturePtr(), ref desc);
                Assert.AreEqual(DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_TYPELESS, desc.Format);
            }
            {
                var texture = new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
                texture.Create();
                var desc = default(D3D11_TEXTURE2D_DESC);
                TextureHelper.GetTextureDesc(texture.GetNativeTexturePtr(), ref desc);
                Assert.AreEqual(DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_TYPELESS, desc.Format);
            }
        }

        static Byte Read(RenderTexture dst)
        {
            var read = new Texture2D(2, 2, TextureFormat.RGBA32, false, true);
            read.SetPixels32(new Color32[]
            {
                Color.black,
                Color.black,
                Color.black,
                Color.black,
            });
            read.Apply();

            //RenderTexture.active = dst;
            read.ReadPixels(new Rect(0, 0, 2, 2), 0, 0);
            read.Apply();
            RenderTexture.active = null;
            /*
            var pixels = read.GetPixels32();
            return pixels[0].r;
            */
            return read.GetRawTextureData()[1];
        }

        static Byte Blit(Texture2D src, RenderTexture dst, Material material, bool? setSRGBWrite = null)
        {
            var restore = GL.sRGBWrite;
            if (setSRGBWrite.HasValue)
            {
                //Debug.LogFormat("Gl.sRGBWrite = {0}", setSRGBWrite);
                GL.sRGBWrite = setSRGBWrite.Value;
            }
            Graphics.Blit(src, dst, material);
            GL.sRGBWrite = restore;

            return Read(dst);
        }

        static Byte Blit(Byte value, Texture2D src, RenderTexture dst, bool? setSRGBWrite = null)
        {
            var values = Enumerable.Repeat(value, src.width * src.height * 4).ToArray();
            src.LoadRawTextureData(values);
            src.Apply();

            var restore = GL.sRGBWrite;
            if (setSRGBWrite.HasValue)
            {
                //Debug.LogFormat("Gl.sRGBWrite = {0}", setSRGBWrite);
                GL.sRGBWrite = setSRGBWrite.Value;
            }
            Graphics.Blit(src, dst);
            GL.sRGBWrite = restore;

            return Read(dst);
        }


        [Test]
        public void DefaultValueTest()
        {
            Assert.False(GL.sRGBWrite);

            var texture = new Texture2D(2, 2);
            Assert.AreEqual(TextureFormat.RGBA32, texture.format);
            Assert.AreEqual(2, texture.mipmapCount);

            var rt = new RenderTexture(2, 2, 0);
            Assert.AreEqual(RenderTextureFormat.ARGB32, rt.format);
            Assert.AreEqual(false, rt.sRGB);
        }

        [Test]
        public void BlitWithoutMaterialTest()
        {
            {
                var pixel = Blit(127,
                    new Texture2D(2, 2, TextureFormat.RGBA32, false, false),
                    new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB),
                    true);
                Assert.AreEqual(127, pixel);
            }

            {
                var pixel = Blit(127,
                    new Texture2D(2, 2, TextureFormat.RGBA32, false, false),
                    new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB),
                    false);
                Assert.AreEqual(127, pixel);
            }
        }

        [Test]
        public void BlitWithMaterialTest()
        {
            var shader = Resources.Load<Shader>("Write_05");
            var material = new Material(shader);

            {
                var pixel = Blit(new Texture2D(2, 2),
                    new RenderTexture(2, 2, 0),
                    material,
                    true);
                Assert.AreEqual(127, pixel);
            }

            {
                var pixel = Blit(new Texture2D(2, 2),
                    new RenderTexture(2, 2, 0),
                    material,
                    false);
                Assert.AreEqual(127, pixel);
            }

            {
                var pixel = Blit(new Texture2D(2, 2, TextureFormat.RGBA32, false, false),
                    new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB),
                    material,
                    true);
                Assert.AreEqual(127, pixel);
            }

            {
                var pixel = Blit(new Texture2D(2, 2, TextureFormat.RGBA32, false, false),
                    new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB),
                    material,
                    false);
                Assert.AreEqual(127, pixel);
            }

            {
                var pixel = Blit(new Texture2D(2, 2, TextureFormat.RGBA32, false, false),
                    new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear),
                    material,
                    false);
                Assert.AreEqual(127, pixel);
            }
        }
    }
}
