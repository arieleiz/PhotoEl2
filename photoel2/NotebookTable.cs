using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace photoel2
{
    class NotebookTable : ListView
    {
	    const int DEFAULT_GRID_SIZE = 30;

        public NotebookTable()
        {
            HeaderStyle = ColumnHeaderStyle.None;
            MultiSelect = false;
            FullRowSelect = false;
            DoubleBuffered = true;
            HideSelection = true;
            View = View.Details;
            BackgroundImageTiled = true;
            rebuild_fonts();
        }

        
        [DefaultValue(DEFAULT_GRID_SIZE)]
        public int GridSize { get; set; }

        [DllImport("user32")]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);
        const int SB_HORZ = 0;
        const int  SB_VERT = 1;
        const int SB_BOTH = 3;

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            rebuild_fonts();
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            foreach(int i in SelectedIndices)
            {
                Items[i].Selected = false;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            rebuild_fonts();
            rebuild_background();
            set_col_width();
            ShowScrollBar(Handle, SB_HORZ, false);
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            rebuild_background();
        }

        private readonly object _hidden_tag = new object();

        public void reinit()
        {
            Items.Clear();
            if (Columns.Count == 0 || Columns[0].Tag != _hidden_tag)
                Columns.Insert(0, new ColumnHeader());

            ListViewItem lvi = new ListViewItem();
            for (int i = 1; i < Columns.Count; ++i)
            {
                Columns[i].TextAlign = HorizontalAlignment.Center;
                lvi.SubItems.Add(Columns[i].Text);
            }
            lvi.Font = _title_font;
                
            Items.Add(lvi);
            set_col_width();
        }

        private void set_col_width()
        {
            if (Columns.Count == 0)
                return;

            int width = 0;
            for (int i = 1; i < Columns.Count; ++i)
            {
                width += Columns[i].Width;
            }
            width = (Width - width) / 2;
            if (width <= 0)
                width = 0;
            if (Columns[0].Width != width)
                Columns[0].Width = width;
        }

        public ListViewItem add_item(string[] values)
        {
            ListViewItem lvi = new ListViewItem();
            for (int i = 0; i < values.Length; ++i)
                lvi.SubItems.Add(values[i]);
            Items.Add(lvi);
            lvi.UseItemStyleForSubItems = false;
            return lvi;
        }

        public void pop_top()
        {
            if (Items.Count > 1)
                Items.RemoveAt(Items.Count - 1);
        }

        public int num_items()
        {
            if(Items.Count > 0)
                return Items.Count - 1;
            return 0;
        }

        public void clear()
        {
            for (int i = Items.Count - 1; i >= 1; --i)
                Items.RemoveAt(i);
        }


        private void rebuild_background()
        {
            if (Width != 0 && Height != 0 && (BackgroundImage == null || Width != BackgroundImage.Width || Height != BackgroundImage.Height))
            {
                Bitmap b = new Bitmap(Width, Height);

                Pen blue = new Pen(Color.LightBlue, 1);
                int grid_size = GridSize;
                if (grid_size < 2) grid_size = DEFAULT_GRID_SIZE;

                using (Graphics g = Graphics.FromImage(b))
                {
                    int width = Width, height = Height;

                    g.FillRectangle(Brushes.White, 0, 0, width, height);
                    for (int i = grid_size; i < height; i += grid_size)
                        g.DrawLine(blue, 0, i, width, i);

                    for (int i = Left % grid_size; i < width; i += grid_size)
                        g.DrawLine(blue, i, 0, i, height);
                }

                if (BackgroundImage != null)
                {
                    var old = BackgroundImage;
                    BackgroundImage = null;
                    old.Dispose();
                }
                BackgroundImage = b;
            }
        }

        private void rebuild_fonts()
        {
            int size_big = 20;
            int size_small = 16;
            if (Height <= 100)
            { size_big = 15; size_small = 12; }
            else if (Height <= 200)
            { size_big = 17; size_small = 14; }

            if (size_small != _fnt_size_small || size_big != _fnt_size_big)
            {
                _title_font = MemoryFonts.get_font(0, size_big, FontStyle.Underline);
                _item_font = MemoryFonts.get_font(0, size_small, FontStyle.Regular);
                _fnt_size_small = size_small;
                _fnt_size_big = size_big;
            }
            Font = _item_font;
        }

        int _fnt_size_small = 0, _fnt_size_big;
        Font _title_font, _item_font;
    }
}
