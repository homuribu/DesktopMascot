using Live2D.Cubism.Core;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Windows;
//using System.Windows.Automation;

// 定義部
static class NativeMethods
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWINFO
    {
        public int cbSize;
        public RECT rcWindow;
        public RECT rcClient;
        public int dwStyle;
        public int dwExStyle;
        public int dwWindowStatus;
        public uint cxWindowBorders;
        public uint cyWindowBorders;
        public short atomWindowType;
        public short wCreatorVersion;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);
    public static WINDOWINFO MyGetWindowInfo(IntPtr hWnd, out int retCode)
    {
        var wi = new WINDOWINFO();
        wi.cbSize = Marshal.SizeOf(wi);
        retCode = NativeMethods.GetWindowInfo(hWnd, ref wi);
        return wi;
    }

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hwnd, out RECT rect);

    [DllImport("LibUniWinC")]
    public static extern IntPtr GetWindowHandle();

    public static Vector2 GetFocusWindowPosition()
    {
        IntPtr hwnd = GetForegroundWindow();
        if(hwnd != IntPtr.Zero)
        {
            return GetWindowPosition(hwnd);
        }
        return new Vector2(0, 0);
    }
    public static Vector2 GetMyWindowPosition()
    {
        IntPtr hwnd = GetWindowHandle();
        if(hwnd != IntPtr.Zero)
        {
            return GetWindowPosition(hwnd);
        }
        return new Vector2(0, 0);
    }

    public static Vector2 GetWindowPosition(IntPtr hwnd)
    {
        if (hwnd != IntPtr.Zero)
        {
            var rect = new RECT();
            var success = GetWindowRect(hwnd, out rect);
            return new Vector2((rect.left + rect.right)/2, (rect.top+rect.bottom)/2);
        }
        return new Vector2(0, 0);
    }
}

// 使用部




/// <summary>
/// 目線の追従を行うクラス
/// </summary>
public class GazeController : MonoBehaviour
{
    [SerializeField]
    Transform Anchor = null;
    Vector3 centerOnScreen;
    public bool is_enable = true;
    void Start()
    {
        centerOnScreen = Camera.main.WorldToScreenPoint(Anchor.position);
    }
    void LateUpdate()
    {
        if (is_enable)
        {
            var mousePos = Input.mousePosition - centerOnScreen;
            var focusWindowPos = NativeMethods.GetFocusWindowPosition() - NativeMethods.GetMyWindowPosition();
            focusWindowPos.y *= -1;

            var target = mousePos;

            UpdateRotate(new Vector3(target.x, target.y, 0) * 0.1f);
        }
    }
    Vector3 currentRotateion = Vector3.zero;
    Vector3 eulerVelocity = Vector3.zero;

    [SerializeField]
    CubismParameter HeadAngleX = null, HeadAngleY = null, HeadAngleZ = null;
    [SerializeField]
    CubismParameter EyeBallX = null, EyeBallY = null;
    [SerializeField]
    float EaseTime = 0.5f;
    [SerializeField]
    float EyeBallXRate = 0.05f;
    [SerializeField]
    float EyeBallYRate = 0.02f;
    [SerializeField]
    bool ReversedGazing = false;



    void UpdateRotate(Vector3 targetEulerAngle)
    {
        currentRotateion = Vector3.SmoothDamp(currentRotateion, targetEulerAngle, ref eulerVelocity, EaseTime);
        // 頭の角度
        SetParameter(HeadAngleX, currentRotateion.x);
        SetParameter(HeadAngleY, currentRotateion.y);
        SetParameter(HeadAngleZ, currentRotateion.z);
        // 眼球の向き
        SetParameter(EyeBallX, currentRotateion.x * EyeBallXRate * (ReversedGazing ? -1 : 1));
        SetParameter(EyeBallY, currentRotateion.y * EyeBallYRate * (ReversedGazing ? -1 : 1));
    }
    void SetParameter(CubismParameter parameter, float value)
    {
        if (parameter != null)
        {
            parameter.Value = Mathf.Clamp(value, parameter.MinimumValue, parameter.MaximumValue);
        }
    }
}