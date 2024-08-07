using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.EventSystems;

public class TransparentWindow : MonoBehaviour
{
    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll")]
    public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int leftWidth;
        public int rightWidth;
        public int topHeight;
        public int bottomHeight;
    }

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

    const int GWL_EXSTYLE = -20;

    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;

    static readonly IntPtr HWND_TOPMOST = new(-1);

    private IntPtr hwnd;

    private bool isTransparent = false;
    [SerializeField] private GameObject plane;
    public void OnClick()
    {
        hwnd = GetActiveWindow();
#if !UNITY_EDITOR_
        if (!isTransparent)
        {
            plane.SetActive(false); 
            MARGINS margins = new() { leftWidth = -1 };
            DwmExtendFrameIntoClientArea(hwnd, ref margins);

            SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);

            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
            isTransparent = true;
        }
        else
        {
            plane.SetActive(true);
            MARGINS margins = new() { leftWidth = -1 };
            DwmExtendFrameIntoClientArea(hwnd, ref margins);

            SetWindowLong(hwnd, GWL_EXSTYLE, 0);

            SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
            isTransparent = false;
        }

#endif
    }

    private void Update()
    {
#if !UNITY_EDITOR_
        if(isTransparent) SetClickThrough(!IsPointerOverUI());
#endif
    }

    private void SetClickThrough(bool isOverlaping)
    {
        if (isOverlaping)
        {
            SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
        else
        {
            SetWindowLong(hwnd, GWL_EXSTYLE, WS_EX_LAYERED);
        }  
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        else
        {
            PointerEventData pe = new(EventSystem.current);
            pe.position = Input.mousePosition;
            List<RaycastResult> hits = new();
            EventSystem.current.RaycastAll(pe, hits);
            return hits.Count > 0;
        }
    }

    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }
}
