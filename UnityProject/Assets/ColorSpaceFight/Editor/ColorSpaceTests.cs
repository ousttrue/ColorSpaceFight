using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
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
                Assert.AreEqual(DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB, desc.Format);
            }
            {
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false, false);
                var desc = default(D3D11_TEXTURE2D_DESC);
                TextureHelper.GetTextureDesc(texture.GetNativeTexturePtr(), ref desc);
                Assert.AreEqual(DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB, desc.Format);
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

        static Byte Read(RenderTexture dst, bool linear = false, string saveName=null)
        {
            var read = new Texture2D(dst.width, dst.height, TextureFormat.RGBA32, false, linear);
            /*
            read.SetPixels32(new Color32[]
            {
                Color.black,
                Color.black,
                Color.black,
                Color.black,
            });
            read.Apply();
            */

            //RenderTexture.active = dst;
            Assert.AreSame(RenderTexture.active, dst);
            read.ReadPixels(new Rect(0, 0, dst.width, dst.height), 0, 0);
            read.Apply();
            RenderTexture.active = null;
            /*
            var pixels = read.GetPixels32();
            return pixels[0].r;
            */

            var png = read.EncodeToPNG();
            if (!string.IsNullOrEmpty(saveName))
            {
                //AssetDatabase.CreateAsset(read, saveName);
                File.WriteAllBytes(saveName, read.EncodeToPNG());
            }
            else {
                UnityEngine.Object.DestroyImmediate(read);
            }

            var pixel = default(Color32);
            if (TextureHelper.GetPngPixel(png, png.Length, 0, 0, ref pixel))
            {
                return pixel.g;
            }

            //return read.GetRawTextureData()[1];

            throw new Exception();
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
            Assert.AreSame(RenderTexture.active, dst);
            GL.sRGBWrite = restore;
            UnityEngine.GameObject.DestroyImmediate(src);

            var b= Read(dst);
            UnityEngine.GameObject.DestroyImmediate(dst);

            return b;
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
            Assert.AreSame(RenderTexture.active, dst);
            GL.sRGBWrite = restore;
            UnityEngine.GameObject.DestroyImmediate(src);

            var b = Read(dst);
            UnityEngine.GameObject.DestroyImmediate(dst);

            return b;
        }

        struct ColorSpaceScope : IDisposable
        {
            bool m_sRGBWrite;

            public ColorSpaceScope(RenderTextureReadWrite colorSpace)
            {
                m_sRGBWrite = GL.sRGBWrite;
                switch (colorSpace)
                {
                    case RenderTextureReadWrite.Linear:
                        GL.sRGBWrite = false;
                        break;

                    case RenderTextureReadWrite.sRGB:
                    default:
                        GL.sRGBWrite = true;
                        break;
                }
            }
            public ColorSpaceScope(bool sRGBWrite)
            {
                m_sRGBWrite = GL.sRGBWrite;
                GL.sRGBWrite = sRGBWrite;
            }

            public void Dispose()
            {
                GL.sRGBWrite = m_sRGBWrite;
            }
        }

        [Test]
        public void TextureProperty()
        {
            Assert.False(GL.sRGBWrite);

            var texture = new Texture2D(2, 2);
            Assert.AreEqual(TextureFormat.RGBA32, texture.format);
            Assert.AreEqual(2, texture.mipmapCount);

            {
                var rt = new RenderTexture(2, 2, 0);
                Assert.AreEqual(RenderTextureFormat.ARGB32, rt.format);
                if(PlayerSettings.colorSpace == ColorSpace.Linear)
                {
                    Assert.True(rt.sRGB);
                }
                else
                {
                    Assert.False(rt.sRGB);
                }
            }
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
                Assert.AreEqual(54, pixel);
            }
        }

        static Byte CopySRGBWrite(Texture2D src, bool isSRGB, string saveName=null)
        {
            var renderTexture = new RenderTexture(src.width, src.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            Assert.True(renderTexture.sRGB);

            var restore = GL.sRGBWrite;
            GL.sRGBWrite = isSRGB;
            {
                Graphics.Blit(src, renderTexture);
                Assert.AreSame(RenderTexture.active, renderTexture);
            }
            GL.sRGBWrite = restore;
            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(src)))
            {
                UnityEngine.GameObject.DestroyImmediate(src);
            }

            var b = Read(renderTexture, isSRGB, saveName);
            UnityEngine.GameObject.DestroyImmediate(renderTexture);

            return b;
        }

        [Test]
        public void BlitWithoutMaterialTest2()
        {
            {
                var src = new Texture2D(2, 2, TextureFormat.ARGB32, false, false);
                /*
                src.SetPixels32(new[] {
                    new Color32(127, 127, 127, 255),
                    new Color32(127, 127, 127, 255),
                    new Color32(127, 127, 127, 255),
                    new Color32(127, 127, 127, 255),
                });
                */
                src.LoadRawTextureData(Enumerable.Repeat<Byte>(127, src.width * src.height * 4).ToArray());
                src.Apply();

                var b = CopySRGBWrite(src, true);
                Assert.AreEqual(127, b);
            }

            {
                var src = new Texture2D(2, 2, TextureFormat.ARGB32, false, false);
                /*
                src.SetPixels32(new[] {
                    new Color32(127, 127, 127, 255),
                    new Color32(127, 127, 127, 255),
                    new Color32(127, 127, 127, 255),
                    new Color32(127, 127, 127, 255),
                });
                */

                src.LoadRawTextureData(Enumerable.Repeat<Byte>(127, src.width * src.height * 4).ToArray());
                src.Apply();

                var b = CopySRGBWrite(src, false);
                Assert.AreEqual(54, b);
            }

            {
                var src = Resources.Load<Texture2D>("gray127");
                var b = CopySRGBWrite(src, true, "Assets/gray127true.png");
                Assert.AreEqual(127, b);
            }

            {
                var src = Resources.Load<Texture2D>("gray127");
                var b = CopySRGBWrite(src, false, "Assets/gray127false.png");
                Assert.AreEqual(54, b);
            }
        }

        [Test]
        public void BlitWithMaterialTest()
        {
            var shader = Resources.Load<Shader>("Write_05");
            var material = new Material(shader);

            {
                var pixel = Blit(new Texture2D(2, 2, TextureFormat.ARGB32, false, false),
                    new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB),
                    material,
                    true);
                Assert.AreEqual(188, pixel);
            }

            {
                var pixel = Blit(new Texture2D(2, 2, TextureFormat.ARGB32, false, false),
                    new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear),
                    material,
                    false);
                Assert.AreEqual(127, pixel);
            }

            {
                var pixel = Blit(new Texture2D(2, 2, TextureFormat.ARGB32, false, true),
                    new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB),
                    material,
                    true);
                Assert.AreEqual(188, pixel);
            }

            {
                var pixel = Blit(new Texture2D(2, 2, TextureFormat.ARGB32, false, true),
                    new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear),
                    material,
                    false);
                Assert.AreEqual(127, pixel);
            }
        }
    }
}
