#pragma once

#ifdef BUILD_TEXTUREHELPER
#define EXPORT_TEXTUREHELPER extern "C" __declspec(dllexport)
#else
#define EXPORT_TEXTUREHELPER extern "C" __declspec(dllimport)
#endif

struct D3D11_TEXTURE2D_DESC;
struct ID3D11Texture2D;
EXPORT_TEXTUREHELPER void GetTextureDesc(ID3D11Texture2D *pTexture, D3D11_TEXTURE2D_DESC *desc);
