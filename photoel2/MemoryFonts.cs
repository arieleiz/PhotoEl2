using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace photoel2
{
    class MemoryFonts
    {
        private static PrivateFontCollection _pfc;

        static MemoryFonts()
        {
            _pfc = new PrivateFontCollection();
            add_font(Properties.Resources.font);
        }

        private static void add_font(byte[] font)
        {
            IntPtr p = Marshal.AllocCoTaskMem(font.Length);
            Marshal.Copy(font, 0, p, font.Length);
            uint c = 0;
            AddFontMemResourceEx(p, font.Length, IntPtr.Zero, ref c);
            _pfc.AddMemoryFont(p, font.Length);
            Marshal.FreeCoTaskMem(p);
        }

        public static Font get_font(int idx, float fontSize, FontStyle fontStyle = FontStyle.Regular)
        {
            return new Font(_pfc.Families[idx], fontSize, fontStyle);
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, int cbFont, IntPtr pdv, [In] ref uint pcFonts);
    }
}
