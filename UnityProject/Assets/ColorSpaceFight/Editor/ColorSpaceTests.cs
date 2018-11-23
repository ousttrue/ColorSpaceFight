using NUnit.Framework;
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

        static Color32 Read(RenderTexture dst)
        {
            var read = new Texture2D(2, 2, TextureFormat.ARGB32, false, true);
            read.SetPixels32(new Color32[]
            {
                Color.black,
                Color.black,
                Color.black,
                Color.black,
            });
            read.Apply();

            RenderTexture.active = dst;
            read.ReadPixels(new Rect(0, 0, 2, 2), 0, 0);
            read.Apply();
            RenderTexture.active = null;
            var pixels = read.GetPixels32();
            return pixels[0];
        }

        static Color32 Blit(Texture2D src, RenderTexture dst, Material material, bool? setSRGBWrite = null)
        {
            if (setSRGBWrite.HasValue)
            {
                GL.sRGBWrite = setSRGBWrite.Value;
            }

            if (material != null)
            {
                Graphics.Blit(src, dst, material);
            }
            else
            {
                Graphics.Blit(src, dst);
            }

            return Read(dst);
        }

        [Test]
        public void BlitTest()
        {
            var shader = Resources.Load<Shader>("Write_05");
            var material = new Material(shader);

            Assert.False(GL.sRGBWrite);

            {
                var pixel = Blit(new Texture2D(2, 2),
                    new RenderTexture(2, 2, 0),
                    material,
                    true);
                Assert.AreEqual(127, pixel.r);
            }

            {
                var pixel = Blit(new Texture2D(2, 2),
                    new RenderTexture(2, 2, 0),
                    material,
                    false);
                Assert.AreEqual(127, pixel.r);
            }

            {
                var pixel = Blit(new Texture2D(2, 2, TextureFormat.ARGB32, false, false),
                    new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB),
                    material,
                    true);
                Assert.AreEqual(127, pixel.r);
            }

            {
                var pixel = Blit(new Texture2D(2, 2, TextureFormat.ARGB32, false, false),
                    new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB),
                    material,
                    false);
                Assert.AreEqual(127, pixel.r);
            }

            {
                var pixel = Blit(new Texture2D(2, 2, TextureFormat.ARGB32, false, false),
                    new RenderTexture(2, 2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear),
                    material,
                    false);
                Assert.AreEqual(127, pixel.r);
            }
        }
    }
}
