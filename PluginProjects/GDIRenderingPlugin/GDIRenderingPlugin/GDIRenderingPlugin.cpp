// This is the main DLL file.

#include "stdafx.h"

#include "GDIRenderingPlugin.h"

#include <math.h>
#include <stdio.h>
#include <Windows.h>
#include <WinUser.h>


// --------------------------------------------------------------------------
// Include headers for the graphics APIs we support

#if SUPPORT_D3D9
#include <d3d9.h>
#endif
#if SUPPORT_D3D11
#include <d3d11.h>
#endif
#if SUPPORT_OPENGL
#if UNITY_WIN
#include <gl/GL.h>
#else
#include <OpenGL/OpenGL.h>
#endif
#endif


// Prints a string
static void DebugLog(const char* str)
{
#if UNITY_WIN
	OutputDebugStringA(str);
#else
	printf("%s", str);
#endif
}


// COM-like Release macro
#ifndef SAFE_RELEASE
#define SAFE_RELEASE(a) if (a) { a->Release(); a = NULL; }
#endif



// SetTimeFromUnity, an example function we export which is called by one of the scripts.

static float g_Time;

extern "C" void EXPORT_API SetTimeFromUnity(float t) { g_Time = t; }




// SetTextureFromUnity, an example function we export which is called by one of the scripts.

static void* g_TexturePointer;

extern "C" void EXPORT_API SetTextureFromUnity(void* texturePtr)
{
	// A script calls this at initialization time; just remember the texture pointer here.
	// Will update texture pixels each frame from the plugin rendering event (texture update
	// needs to happen on the rendering thread).
	g_TexturePointer = texturePtr;
}




// --------------------------------------------------------------------------
// UnitySetGraphicsDevice

static int g_DeviceType = -1;


// Actual setup/teardown functions defined below
#if SUPPORT_D3D9
static void SetGraphicsDeviceD3D9(IDirect3DDevice9* device, GfxDeviceEventType eventType);
#endif
#if SUPPORT_D3D11
static void SetGraphicsDeviceD3D11(ID3D11Device* device, GfxDeviceEventType eventType);
#endif


extern "C" void EXPORT_API UnitySetGraphicsDevice(void* device, int deviceType, int eventType)
{
	// Set device type to -1, i.e. "not recognized by our plugin"
	g_DeviceType = -1;

#if SUPPORT_D3D9
	// D3D9 device, remember device pointer and device type.
	// The pointer we get is IDirect3DDevice9.
	if (deviceType == kGfxRendererD3D9)
	{
		DebugLog("Set D3D9 graphics device\n");
		g_DeviceType = deviceType;
		SetGraphicsDeviceD3D9((IDirect3DDevice9*)device, (GfxDeviceEventType)eventType);
	}
#endif

#if SUPPORT_D3D11
	// D3D11 device, remember device pointer and device type.
	// The pointer we get is ID3D11Device.
	if (deviceType == kGfxRendererD3D11)
	{
		DebugLog("Set D3D11 graphics device\n");
		g_DeviceType = deviceType;
		SetGraphicsDeviceD3D11((ID3D11Device*)device, (GfxDeviceEventType)eventType);
	}
#endif

#if SUPPORT_OPENGL
	// If we've got an OpenGL device, remember device type. There's no OpenGL
	// "device pointer" to remember since OpenGL always operates on a currently set
	// global context.
	if (deviceType == kGfxRendererOpenGL)
	{
		DebugLog("Set OpenGL graphics device\n");
		g_DeviceType = deviceType;
	}
#endif
}



extern "C" void EXPORT_API UnityRenderEvent(int eventID)
{
	// Unknown graphics device type? Do nothing.
	if (g_DeviceType == -1)
		return;


}