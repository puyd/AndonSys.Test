using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AndonSys.Common
{
    static class  Win32
    {
        [DllImportAttribute("gdi32.dll")]
        public static extern IntPtr CreateDC (  string lpszDriver ,string lpszDevice ,string lpszOutput ,IntPtr lpInitData);

        [DllImportAttribute("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImportAttribute("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImportAttribute("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        
        [DllImportAttribute("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImportAttribute("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hDC);
    }
}
