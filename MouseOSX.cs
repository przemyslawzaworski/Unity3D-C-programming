using UnityEngine;
using System.Runtime.InteropServices;
using System;

// WinAPI SetCursorPos and GetCursorPos - equivalents for Unity Mac
public class MouseOSX : MonoBehaviour
{
    // Mac calculates global screen coordinates from top left corner of screen
    #if (UNITY_EDITOR && UNITY_EDITOR_OSX) || (!UNITY_EDITOR && UNITY_STANDALONE_OSX)

        [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static extern int CGWarpMouseCursorPosition(CGPoint point);

        [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static extern IntPtr CGEventCreate(IntPtr source);

        [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static extern CGPoint CGEventGetLocation(IntPtr evt);

        [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        public static extern void CFRelease(IntPtr cf);

        public struct CGPoint
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        Vector2 GetCursorPos ()
        {
            IntPtr ptr = CGEventCreate(IntPtr.Zero);
            CGPoint loc = CGEventGetLocation(ptr);
            CFRelease(ptr);
            return new Vector2((float)loc.X, (float)loc.Y);
        }

        void SetCursorPos(float x, float y)
        {
            CGPoint point = new CGPoint() {X = x, Y = y};
            CGWarpMouseCursorPosition(point);
        }

    #endif

    void Update()
    {
        if (Time.time < 12.0f) //test: mouse circular movement through 12 seconds
        {
			#if (UNITY_EDITOR && UNITY_EDITOR_OSX) || (!UNITY_EDITOR && UNITY_STANDALONE_OSX)
				SetCursorPos((Mathf.Sin(Time.time) * 0.5f + 0.5f) * 500.0f, (Mathf.Cos(Time.time) * 0.5f + 0.5f) * 500.0f);
				Debug.Log(GetCursorPos());
			#endif
        }
    }
}