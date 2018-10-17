Compile NativePlugin.cpp with following command (x64):

cl.exe /LD NativePlugin.cpp opengl32.lib

Then copy DLL file to Assets/Plugins and include NativePlugin.cs to Unity scene.

You will see procedural Unity Logo generated from external GLSL fragment shader.
