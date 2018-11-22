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
    }
}
