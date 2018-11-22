#include "TextureHelper.h"
#include <d3d11.h>

EXPORT_TEXTUREHELPER void GetTextureDesc(ID3D11Texture2D *pTexture, D3D11_TEXTURE2D_DESC *desc)
{
    if (!pTexture)
    {
        return;
    }
    if (!desc)
    {
        return;
    }
    pTexture->GetDesc(desc);
}
