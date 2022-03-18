//For x64 Visual Studio command line:  cl.exe /LD NativeTexture.cpp d3d11.lib dxguid.lib user32.lib kernel32.lib gdi32.lib

#include <d3d11.h>
 
static ID3D11Device *device;
static ID3D11DeviceContext *context;
static ID3D11Texture2D *texture;
static ID3D11ShaderResourceView *srv;
 
typedef enum UnityGfxRenderer
{
	kUnityGfxRendererD3D11 = 2,
	kUnityGfxRendererNull = 4,
} UnityGfxRenderer;
 
typedef enum UnityGfxDeviceEventType
{
	kUnityGfxDeviceEventInitialize = 0,
	kUnityGfxDeviceEventShutdown = 1,
	kUnityGfxDeviceEventBeforeReset = 2,
	kUnityGfxDeviceEventAfterReset = 3,
} UnityGfxDeviceEventType;
 
typedef void (__stdcall* IUnityGraphicsDeviceEventCallback)(UnityGfxDeviceEventType eventType);
 
struct UnityInterfaceGUID
{
	UnityInterfaceGUID(unsigned long long high, unsigned long long low) : m_GUIDHigh(high) , m_GUIDLow(low) { }
	unsigned long long m_GUIDHigh;
	unsigned long long m_GUIDLow;
};
 
struct IUnityInterfaces
{
	void* (__stdcall* GetInterface)(UnityInterfaceGUID guid);
	void (__stdcall* RegisterInterface)(UnityInterfaceGUID guid, void *ptr);
	template<typename INTERFACE>
	INTERFACE* Get()
	{
		return static_cast<INTERFACE*>(GetInterface(UnityInterfaceGUID(0xAAB37EF87A87D748ULL, 0xBF76967F07EFB177ULL)));
	}
	void Register(void* ptr)
	{
		RegisterInterface(UnityInterfaceGUID(0xAAB37EF87A87D748ULL, 0xBF76967F07EFB177ULL), ptr);
	}
};
 
struct IUnityGraphics
{
	void (__stdcall* RegisterDeviceEventCallback)(IUnityGraphicsDeviceEventCallback callback);
};
 
struct IUnityGraphicsD3D11
{
	ID3D11Device* (__stdcall * GetDevice)();
};
 
typedef void (__stdcall* UnityRenderingEvent)(int eventId);
typedef void (__stdcall* UnregisterDeviceEventCallback)(IUnityGraphicsDeviceEventCallback callback);
 
static IUnityInterfaces* s_UnityInterfaces;
static UnityGfxRenderer DeviceType = kUnityGfxRendererNull;
	
extern "C"  __declspec(dllexport) ID3D11ShaderResourceView* TextureApply (void *bytes, unsigned int width, unsigned int height)
{
	DeviceType = kUnityGfxRendererD3D11;
	IUnityGraphicsD3D11* d3d = s_UnityInterfaces->Get<IUnityGraphicsD3D11>();
	device = d3d->GetDevice();
	device->GetImmediateContext(&context);
	D3D11_SUBRESOURCE_DATA tsd = {bytes, width * 4, width * height * 4};
	D3D11_TEXTURE2D_DESC tdesc = {width, height, 1, 1, DXGI_FORMAT_B8G8R8A8_UNORM, {1,0}, D3D11_USAGE_DEFAULT, D3D11_BIND_SHADER_RESOURCE, 0, 0};
	device->CreateTexture2D(&tdesc, &tsd, &texture);   
	device->CreateShaderResourceView(texture, 0, &srv);
	return srv;
}

extern "C"  __declspec(dllexport) void TextureRelease ()
{
	texture->Release();
	srv->Release();
	device->Release();
	context->Release();	
}

static void __stdcall OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType)
{
	if (eventType == kUnityGfxDeviceEventShutdown)
	{
		DeviceType = kUnityGfxRendererNull;
	}
}
 
extern "C" void __declspec(dllexport) __stdcall UnityPluginLoad(IUnityInterfaces* unityInterfaces)
{
	s_UnityInterfaces = unityInterfaces;
	IUnityGraphics* s_Graphics = s_UnityInterfaces->Get<IUnityGraphics>();
	s_Graphics->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);
	OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
}
 
extern "C" void __declspec(dllexport) __stdcall UnityPluginUnload()
{
	UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);
}
 