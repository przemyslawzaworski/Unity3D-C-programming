// For x64 Visual Studio command line:  cl.exe /LD NativePlugin.cpp opengl32.lib
#include <windows.h>
#include <GL/gl.h>

typedef GLuint(APIENTRY *PFNGLCREATEPROGRAMPROC) ();
typedef GLuint(APIENTRY *PFNGLCREATESHADERPROC) (GLenum t);
typedef void(APIENTRY *PFNGLSHADERSOURCEPROC) (GLuint s, GLsizei c, const char*const*string, const GLint* i);
typedef void(APIENTRY *PFNGLCOMPILESHADERPROC) (GLuint s);
typedef void(APIENTRY *PFNGLATTACHSHADERPROC) (GLuint p, GLuint s);
typedef void(APIENTRY *PFNGLLINKPROGRAMPROC) (GLuint p);
typedef void(APIENTRY *PFNGLUSEPROGRAMPROC) (GLuint p);
typedef void(APIENTRY *PFNGLGENBUFFERSPROC) (GLsizei n, GLuint *b);
typedef void(APIENTRY *PFNGLBINDBUFFERPROC) (GLenum t, GLuint b);
typedef void(APIENTRY *PFNGLBUFFERDATAPROC) (GLenum t, ptrdiff_t s, const GLvoid *d, GLenum u);
typedef void(APIENTRY *PFNGLBINDVERTEXARRAYPROC) (GLuint a);
typedef void(APIENTRY *PFNGLENABLEVERTEXATTRIBARRAYPROC) (GLuint i);
typedef void(APIENTRY *PFNGLVERTEXATTRIBPOINTERPROC) (GLuint i, GLint s, GLenum t, GLboolean n, GLsizei k, const void *p);
typedef void(APIENTRY *PFNGLGENVERTEXARRAYSPROC) (GLsizei n, GLuint *a);
typedef void(APIENTRY *PFNGLDELETEVERTEXARRAYSPROC) (GLsizei n, const GLuint *a);

unsigned int PS, VertexArrayID, VertexBuffer;
static const GLfloat vertices[] = {-1.0f,-1.0f,0.0f,1.0f,-1.0f,0.0f,-1.0f,1.0f,0.0f,1.0f,-1.0f,0.0f,1.0f,1.0f,0.0f,-1.0f,1.0f,0.0f};

static const char* VertexShader = \
	"#version 430\n"
	"layout (location=0) in vec3 position;"
	"void main()"
	"{"
		"gl_Position=vec4(position,1.0);"
	"}";

static const char* FragmentShader = \
	"#version 430\n"
	"layout (location=0) out vec4 color;"
	"void main()"
	"{"
		"vec2 resolution = vec2(1280,720);" 
		"vec2 u = gl_FragCoord.xy;"  
		"u.y = abs( u += u-resolution.xy ).y;"   
		"u.y = abs( u -= vec2(1.23,0.71)* max(0.0, dot(u/=resolution.y,vec2(1.23,0.71)))).y;"  
		"vec4 k = 1.0-(.3*resolution.y* clamp(abs(u - u.x - .9) - .1,u - .53, max(u - .07, abs(u.x + .43) - .43)).yyyy);"
		"color = max(vec4(.13,.17,.22,1.),k);"
	"}";

int MakeShader(const char* VS, const char* FS)
{
	int p = ((PFNGLCREATEPROGRAMPROC)wglGetProcAddress("glCreateProgram"))();
	int s1 = ((PFNGLCREATESHADERPROC)wglGetProcAddress("glCreateShader"))(0x8B31);	
	int s2 = ((PFNGLCREATESHADERPROC)wglGetProcAddress("glCreateShader"))(0x8B30);	
	((PFNGLSHADERSOURCEPROC)wglGetProcAddress("glShaderSource"))(s1,1,&VS,0);
	((PFNGLSHADERSOURCEPROC)wglGetProcAddress("glShaderSource"))(s2,1,&FS,0);	
	((PFNGLCOMPILESHADERPROC)wglGetProcAddress("glCompileShader"))(s1);
	((PFNGLCOMPILESHADERPROC)wglGetProcAddress("glCompileShader"))(s2);	
	((PFNGLATTACHSHADERPROC)wglGetProcAddress("glAttachShader"))(p,s1);
	((PFNGLATTACHSHADERPROC)wglGetProcAddress("glAttachShader"))(p,s2);	
	((PFNGLLINKPROGRAMPROC)wglGetProcAddress("glLinkProgram"))(p);
	return p;
}

void Rendering()
{
	glDisable(GL_CULL_FACE);
	glDisable(GL_BLEND);
	glDepthFunc(GL_LEQUAL);
	glEnable(GL_DEPTH_TEST);
	glDepthMask(GL_FALSE);
	((PFNGLUSEPROGRAMPROC)wglGetProcAddress("glUseProgram"))(PS);
	((PFNGLGENVERTEXARRAYSPROC)wglGetProcAddress("glGenVertexArrays")) (1, &VertexArrayID);		
	((PFNGLBINDVERTEXARRAYPROC)wglGetProcAddress("glBindVertexArray")) (VertexArrayID);	
	((PFNGLGENBUFFERSPROC)wglGetProcAddress("glGenBuffers"))(1, &VertexBuffer);
	((PFNGLBINDBUFFERPROC)wglGetProcAddress("glBindBuffer"))(0x8892, VertexBuffer);
	((PFNGLBUFFERDATAPROC)wglGetProcAddress("glBufferData"))(0x8892, sizeof(vertices), vertices, 0x88E4);		
	((PFNGLENABLEVERTEXATTRIBARRAYPROC)wglGetProcAddress("glEnableVertexAttribArray"))(0);
	((PFNGLBINDBUFFERPROC)wglGetProcAddress("glBindBuffer"))(0x8892, VertexBuffer);
	((PFNGLVERTEXATTRIBPOINTERPROC)wglGetProcAddress("glVertexAttribPointer"))(0,3, GL_FLOAT, GL_FALSE, 0,(void*)0 );
	glDrawArrays(GL_TRIANGLES, 0, 6);
	((PFNGLDELETEVERTEXARRAYSPROC)wglGetProcAddress("glDeleteVertexArrays"))(1, &VertexArrayID);
}

typedef enum UnityGfxRenderer
{
	kUnityGfxRendererNull = 4, 
	kUnityGfxRendererOpenGLCore = 17, 
} UnityGfxRenderer;

typedef enum UnityGfxDeviceEventType
{
	kUnityGfxDeviceEventInitialize = 0,
	kUnityGfxDeviceEventShutdown = 1,
	kUnityGfxDeviceEventBeforeReset = 2,
	kUnityGfxDeviceEventAfterReset = 3,
} UnityGfxDeviceEventType;
	
struct UnityInterfaceGUID
{
	UnityInterfaceGUID(unsigned long long high, unsigned long long low) : m_GUIDHigh(high) , m_GUIDLow(low) { }
	unsigned long long m_GUIDHigh;
	unsigned long long m_GUIDLow;
};

struct IUnityInterface {};
typedef void (__stdcall * IUnityGraphicsDeviceEventCallback)(UnityGfxDeviceEventType eventType);

struct IUnityInterfaces
{
	IUnityInterface* (__stdcall* GetInterface)(UnityInterfaceGUID guid);
	void(__stdcall* RegisterInterface)(UnityInterfaceGUID guid, IUnityInterface * ptr);
	template<typename INTERFACE>
	INTERFACE* Get()
	{
		return static_cast<INTERFACE*>(GetInterface(UnityInterfaceGUID(0x7CBA0A9CA4DDB544ULL, 0x8C5AD4926EB17B11ULL)));
	}
	void Register(IUnityInterface* ptr)
	{
		RegisterInterface(UnityInterfaceGUID(0x7CBA0A9CA4DDB544ULL, 0x8C5AD4926EB17B11ULL), ptr);
	}
};

struct IUnityGraphics : IUnityInterface
{
	void(__stdcall* RegisterDeviceEventCallback)(IUnityGraphicsDeviceEventCallback callback);
};

typedef void (__stdcall* UnityRenderingEvent)(int eventId);
typedef void(__stdcall* UnregisterDeviceEventCallback)(IUnityGraphicsDeviceEventCallback callback);
static UnityGfxRenderer DeviceType = kUnityGfxRendererNull;

static void __stdcall OnGraphicsDeviceEvent(UnityGfxDeviceEventType eventType)
{
	if (eventType == kUnityGfxDeviceEventInitialize)
	{
		DeviceType = kUnityGfxRendererOpenGLCore;
		PS = MakeShader(VertexShader,FragmentShader);
	}
	if (eventType == kUnityGfxDeviceEventShutdown)
	{
		DeviceType = kUnityGfxRendererNull;
	}
}

static void __stdcall OnRenderEvent(int eventID)
{
	Rendering();
}

extern "C" void	__declspec(dllexport) __stdcall UnityPluginLoad(IUnityInterfaces* unityInterfaces)
{
	IUnityInterfaces* s_UnityInterfaces = unityInterfaces;
	IUnityGraphics* s_Graphics = s_UnityInterfaces->Get<IUnityGraphics>();
	s_Graphics->RegisterDeviceEventCallback(OnGraphicsDeviceEvent);
	OnGraphicsDeviceEvent(kUnityGfxDeviceEventInitialize);
}

extern "C" void __declspec(dllexport) __stdcall UnityPluginUnload()
{
	UnregisterDeviceEventCallback(OnGraphicsDeviceEvent);	
}

extern "C" UnityRenderingEvent __declspec(dllexport) __stdcall Execute()
{
	return OnRenderEvent;
}