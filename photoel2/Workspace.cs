using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Drawing;

namespace photoel2
{
    public partial class Workspace : UserControl
    {
        protected static readonly StringFormat TEXT_STRING_FORMAT = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
        protected static readonly Font BASE_FONT = new Font("Arial", 20, FontStyle.Regular);
        protected const int BORDER = 3;
        protected const int MAX_FONT_SIZE = 32;
        protected const int MIN_FONT_SIZE = 6;

        private List<WObject> _objects = new List<WObject>();
        private int _selected_object = -1;
        private bool _dragging = false;
        private int _last_drag_x = 0, _last_drag_y = 0;
        protected double _draw_ratio;
        protected enum DragType { DragNone, DragAll, DragHoriz, DragPoint };
        private bool _done_init = false;
        private Size _old_size = new Size(0, 0);

        public Workspace()
        {
            InitializeComponent();
            _done_init = true;
        }

        private void Workspace_SizeChanged(object sender, EventArgs e)
        {
            if (!_done_init)
                return;

            Size s = get_total_size();
            if (s.Width == _old_size.Width && s.Height == _old_size.Height)
                return;

            double ratioW = (Width - 2.0 * BORDER) / (s.Width + 0.0);
            double ratioH = (Height - 2.0 * BORDER) / (s.Height + 0.0);
            _draw_ratio = ratioW < ratioH ? ratioW : ratioH;

            restart_ui();

            Invalidate();
        }

        protected void restart_ui()
        {
            _dragging = false;
            _selected_object = -1;
            _objects.Clear();
            reload_objects();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var graphics = e.Graphics;
            WObject dragged = null;
            for (int i = 0; i < _objects.Count; ++i)
            {
                if (_dragging && i == _selected_object)
                    dragged = _objects[i];
                else
                    _objects[i].draw(graphics);
            }

            if (dragged != null)
            {
                foreach (var obj in dragged.get_drag_targets())
                {
                    obj.draw(graphics);
                }

                dragged.draw(graphics);
            }
        }
        private void Workspace_MouseMove(object sender, MouseEventArgs e)
        {
            if (Bounds.Contains(e.Location))
            {
                if (_dragging)
                    do_drag_source(e);
                else
                    check_drag_source(e);
            }
        }

        private void Workspace_MouseUp(object sender, MouseEventArgs e)
        {
            if(_dragging)
            {
                _objects[_selected_object].end_drag();
                if(_selected_object != -1) // ui could have been reset
                    _objects[_selected_object].leave();
                _selected_object = -1;
                _dragging = false;
            }
            check_drag_source(e);
        }

        private void Workspace_MouseDown(object sender, MouseEventArgs e)
        {
            if(_selected_object >= 0 && _objects[_selected_object].Draggable)
            {
                _objects[_selected_object].begin_drag();
                _dragging = true;
                _last_drag_x = e.X;
                _last_drag_y = e.Y;
            }
        }

        protected Rectangle scale_and_shift(Rectangle rect, Rectangle rel)
        {
            return new Rectangle((int)(rect.Left * _draw_ratio + rel.Left),
                                  (int)(rect.Top * _draw_ratio + rel.Top),
                                  (int)Math.Max(rect.Width * _draw_ratio, 1),
                                  (int)Math.Max(rect.Height * _draw_ratio, 1));
        }

        private void do_drag_source(MouseEventArgs e)
        {
            var wobj = _objects[_selected_object];
            wobj.dragged_to(e.X, e.Y, _last_drag_x, _last_drag_y);
            _last_drag_x = e.X;
            _last_drag_y = e.Y;
            Cursor.Current = wobj.get_drag_cursor();
        }

        private void check_drag_source(MouseEventArgs e)
        {
            for (int i = _objects.Count - 1; i >= 0; --i)
            {
                if (_objects[i].contains(e.X, e.Y))
                {
                    if(_objects[i].Draggable)
                        Cursor.Current = _objects[i].get_drag_cursor();
                    if (_selected_object >= 0)
                        _objects[_selected_object].leave();
                    _selected_object = i;
                    _objects[i].enter();
                    return;
                }
            }
            if(_selected_object >= 0)
            {
                _objects[_selected_object].leave();
                _selected_object = -1;
            }
        }

        virtual protected void reload_objects()
        {
        }

        virtual protected Size get_total_size()
        {
            return new Size(1, 1);
        }

        protected Rectangle text_under(Rectangle rect, int max_height)
        {
            return new Rectangle(rect.Left, rect.Bottom + 5, rect.Width, max_height);
        }

        protected void add_object(WObject obj)
        {
            _objects.Add(obj);
            obj.Invalidate = (rect) => Invalidate(rect);
            Invalidate(obj.Rect);
        }
        protected void insert_object(WObject obj, int index)
        {
            _objects.Insert(index, obj);
            obj.Invalidate = (rect) => Invalidate(rect);
            for(int i = index; i < _objects.Count; ++ i)
                Invalidate(_objects[i].Rect);
        }

        protected Rectangle add_objects_above<T>(T[] objects, int bottom, int base_width, int max_width, int base_height, Func<Rectangle, T, WObject[]> generator)
        {
            int count = objects.Length;
            double width = Math.Min(base_width * _draw_ratio, max_width);
            double height = ((base_height + 0.0) * width / (base_width + 0.0));
            double allwidth = Math.Min((Width - 2.0 * BORDER), (width + 5) * count);
            double space = allwidth / count;
            double start = ((BORDER + (Width - allwidth)) / 2.0);
            int top = (int)(bottom - height - 1);

            for (int i = 0; i < count; ++i)
            {
                Rectangle pos = new Rectangle((int)(start + i * space), top, (int)width, (int)height);
                var objs = generator(pos, objects[i]);
                foreach (var obj in objs)
                {
                    obj.Tag = objects[i];
                    add_object(obj);
                }
            }

            return new Rectangle((int)start, top, (int)allwidth, (int)height);            
        }

        public static Image resizeImage(Image im, Rectangle r)
        {
            Bitmap b = new Bitmap(r.Width, r.Height);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(im, 0, 0, r.Width, r.Height);
            }

            return b;
        }

        protected class WObject
        {
            public WObject( Image image, Rectangle rectangle, int bound_width, int bound_height)
            {
                _image = resizeImage(image, rectangle);
                _rect = rectangle;
                _orig_rect = rectangle;
                Draggable = false;
                _bounds = new Rectangle(0, 0, bound_width - rectangle.Width, bound_height - rectangle.Height);
                Invalidate = NullInvalidate;
            }

            public Action<Rectangle> Invalidate { get; set; }
            private static void NullInvalidate(Rectangle rect) { }

            public bool Draggable { get; protected set;  }
            public void draw(Graphics g)
            {
                g.DrawImageUnscaled(_image, _rect);
            }

            public virtual void dragged_to(int x, int y, int last_x, int last_y)
            {
                int move_x = x - last_x;
                int move_y = y - last_y;
                offset_rectangle(move_x, move_y);
            }

            public bool contains(int x, int y)
            {
                return _rect.Contains(x, y);
            }

            public Cursor get_drag_cursor()
            {
                return _drag_cursers[(int)_drag_type];
            }

            public DragType get_drag_type()
            {
                return _drag_type;
            }

            public void reset()
            {
                Invalidate(_rect);
                _rect = _orig_rect;
                Invalidate(_rect);
            }

            public void offset_rectangle(int move_x, int move_y)
            {
                switch(_drag_type)
                {
                    case DragType.DragNone:
                        return;

                    case DragType.DragHoriz:
                        move_y = 0;
                        break;
                }

                Rectangle old = _rect;
                _rect.Offset(move_x, move_y);
                int left = Math.Min(Math.Max(_rect.Left, _bounds.Left), _bounds.Right);
                int top = Math.Min(Math.Max(_rect.Top, _bounds.Top), _bounds.Bottom);
                if (left != _rect.Left || top != _rect.Top)
                    _rect.Offset(left - _rect.Left, top - _rect.Top);

                if (OnDrag != null)
                    OnDrag(this, _rect.X, _rect.Y);

                if (OnRelDrag != null)
                    OnRelDrag(this,
                          (_rect.X - _bounds.X+ 0.0) / (_bounds.Width + 0.0),
                          (_rect.Y - _bounds.Y + 0.0) / (_bounds.Height + 0.0)
                          );
                Invalidate(old);
                Invalidate(_rect);
            }
            public virtual void begin_drag()
            {

            }
            public virtual void end_drag()
            {

            }
            public virtual void enter()
            {

            }
            public virtual void leave()
            {

            }
            public virtual ObjectTarget[] get_drag_targets()
            {
                return _no_targets;
            }

            public void set_position(int x, int y)
            {
                Invalidate(_rect);
                _rect.X = x;
                _rect.Y = y;
                Invalidate(_rect);
            }
            public Rectangle Rect { get { return _rect; } }
            public Image Image { get { return _image; } }

            public object Tag { get; set; }

            public delegate void OnDragDelegate(WObject obj, int new_x, int new_y);
            public event OnDragDelegate OnDrag;

            public delegate void OnRelDragDelegate(WObject obj, double posx, double posy);
            public event OnRelDragDelegate OnRelDrag;

            readonly Cursor[] _drag_cursers = new Cursor[] { Cursors.Arrow, Cursors.SizeAll, Cursors.SizeWE, Cursors.Hand };

            protected readonly ObjectTarget[] _no_targets = new ObjectTarget[0];
            protected Image _image;
            protected DragType _drag_type = DragType.DragNone;
            protected Rectangle _bounds;
            protected Rectangle _rect, _orig_rect;
        }

        protected class HorizBoundedObject : WObject
        {
            public HorizBoundedObject(Image image, Rectangle rectangle, int source_left_boundary, int source_area_width) : base(image, rectangle, 0, 0)
            {
                _source_left_boundary = source_left_boundary;

                _drag_type = DragType.DragHoriz;
                Draggable = true;
                _bounds = new Rectangle(_source_left_boundary, rectangle.Top, source_area_width, rectangle.Height);
            }

            int _source_left_boundary;
        }

        protected class TargetDraggableObject : WObject
        { 
            public TargetDraggableObject(Image image, Rectangle rectangle, int bound_width, int bound_height, ObjectTarget[] drag_targets) : base(image, rectangle, bound_width, bound_height)
            {
                _drag_type = DragType.DragAll;
                _drag_targets = drag_targets != null ? drag_targets : _no_targets;
                Draggable = true;
            }

            public override ObjectTarget[] get_drag_targets()
            {
                return _drag_targets;
            }

            public override void begin_drag()
            {
                foreach (var tgt in _drag_targets)
                {
                    Invalidate(tgt.Rect);
                    tgt.begin_highlight();
                }
            }
            public override void end_drag()
            {
                foreach (var tgt in _drag_targets)
                {
                    tgt.end_highlight();
                    Invalidate(tgt.Rect);
                }

                if (OnTgtDrag != null)
                {
                    ObjectTarget target = null;
                    foreach(var tgt in _drag_targets)
                    {
                        if(_rect.IntersectsWith(tgt.Rect))
                        {
                            target = tgt;
                            break;
                        }
                    }
                    OnTgtDrag(this, target);
                }
                    
            }

            public delegate void OnTgtDragDelegate(WObject obj, ObjectTarget tgt);
            public event OnTgtDragDelegate OnTgtDrag;

            ObjectTarget[] _drag_targets;
        }

        protected class TextLabelObject : WObject
        {
            public TextLabelObject(string str, Rectangle rectangle, int bound_width, int bound_height, Font font) : base(create_text_image(str, font, rectangle), rectangle, bound_width, bound_height)
            {
            }

            public static Image create_text_image(string str, Font font, Rectangle r)
            {
                Bitmap b = new Bitmap(r.Width, r.Height);
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawString(str, font, Brushes.Black, new Rectangle(0, 0, r.Width, r.Height), TEXT_STRING_FORMAT);
                }

                return b;
            }
        }

        protected class ObjectTarget : WObject
        {
            public ObjectTarget(Image img, Rectangle rectangle, int bound_width, int bound_height) : base(img, rectangle, bound_width, bound_height)
            {
                _orig_image = _image;
                _highlight_image = create_highlight(_orig_image);
            }

            public static Image create_highlight(Image img)
            {
                return (Image)img.Clone();
            }

            public void replace_image(Image img)
            {
                _image = img;
                _orig_image = img;
                _highlight_image = create_highlight(_orig_image);
            }

            public void begin_highlight()
            {
                _image = _highlight_image;
            }

            public void end_highlight()
            {
                _image = _orig_image;
            }

            Image _orig_image, _highlight_image;
        }

        protected class SevenSegmentTarget : WObject
        {
            public SevenSegmentTarget(int digits, string fmt, double init_val, Rectangle rectangle, int bound_width, int bound_height) : base(create_image(digits, rectangle, fmt, init_val), rectangle, bound_width, bound_height)
            {
                _fmt = fmt;
                _val = init_val;
                _digits = digits + 1; 
            }

            public void set_val(double val)
            {
                _val = val;
                _image = create_image(_digits, _rect, _fmt, val);
                Invalidate(_rect);
            }

            public static Image create_image(int digits, Rectangle rect, string _fmt, double val)
            {
                string valstr = string.Format(_fmt, val);
                
                Bitmap b = new Bitmap(rect.Width, rect.Height);
                Rectangle r = new Rectangle(0, 0, rect.Width, rect.Height);
                using (Graphics g = Graphics.FromImage(b))
                {
                    int dgt_width = r.Width / digits;
                    int pix1 = Math.Max(1, Math.Min((r.Height + 23) / 24, (dgt_width + 23) / 24));
                    int pix3 = Math.Max(1, Math.Min((r.Height + 7) / 8, (dgt_width + 7) / 8));

                    g.FillRectangle(Brushes.Black, r);
                    Pen pen = new Pen(Color.Red, pix3);

                    if (val >= 0)
                        valstr = " " + valstr;
                    int dotpos = valstr.IndexOf('.');
                    if(dotpos >= 0)
                        valstr = valstr.Remove(dotpos, 1);
                    for(int pos = 0;pos < valstr.Length && pos < digits; ++ pos)
                        draw_digit(g, r, valstr[pos], pos, dgt_width, pen, pix1);
                    if (dotpos >= 0)
                        g.FillRectangle(Brushes.Red, dgt_width * dotpos, r.Height - 3 * pix1 - pix3, pix3, pix3);

                }
                return b;
            }

            static readonly byte[][] pattern =
                            {new byte[] {1, 1, 1, 0, 1, 1, 1},
                             new byte[] {0, 0, 1, 0, 0, 1, 0},
                             new byte[] {1, 0, 1, 1, 1, 0, 1},
                             new byte[] {1, 0, 1, 1, 0, 1, 1},
                             new byte[] {0, 1, 1, 1, 0, 1, 0},
                             new byte[] {1, 1, 0, 1, 0, 1, 1},
                             new byte[] {1, 1, 0, 1, 1, 1, 1},
                             new byte[] {1, 0, 1, 0, 0, 1, 0},
                             new byte[] {1, 1, 1, 1, 1, 1, 1},
                             new byte[] {1, 1, 1, 1, 0, 1, 1},
                             new byte[] {0, 0, 0, 0, 0, 0, 0},
                             new byte[] {0, 0, 0, 1, 0, 0, 0}};

            static readonly int space_idx = 10;
            static readonly int minus_idx = 11;

            private static void draw_digit(Graphics g, Rectangle r, char c, int pos, int width, Pen pen, int pix1)
            {
                int ofs = get_pattern_offset(c);

                int hpadding = width <= 9 ? 1 : (width <= 12 ? 2 : 3);
                int vpadding = width <= 9 ? 1 : (width <= 12 ? 2 : 3);
                int left = width * pos + hpadding * pix1;
                int right = width * (pos + 1)- hpadding * pix1;
                int top = vpadding * pix1;
                int middle = r.Height / 2;
                int bottom = r.Height - vpadding * pix1;

                byte[] pat = pattern[ofs];
                if (pat[0] > 0) g.DrawLine(pen, left + pix1, top, right - pix1, top);
                if (pat[1] > 0) g.DrawLine(pen, left, top + pix1, left, middle - pix1);
                if (pat[2] > 0) g.DrawLine(pen, right, top + pix1, right, middle - pix1);
                if (pat[3] > 0) g.DrawLine(pen, left + pix1, middle, right - pix1, middle);
                if (pat[4] > 0) g.DrawLine(pen, left, bottom - pix1, left, middle + pix1);
                if (pat[5] > 0) g.DrawLine(pen, right, bottom - pix1, right, middle + pix1);
                if (pat[6] > 0) g.DrawLine(pen, left + pix1, bottom, right - pix1, bottom);
            }

            private static int get_pattern_offset(char c)
            {
                int ofs;
                switch (c)
                {
                    case ' ': ofs = space_idx; break;
                    case '-': ofs = minus_idx; break;
                    default:
                        if (c >= '0' && c <= '9')
                            ofs = c - '0';
                        else
                            ofs = space_idx;
                        break;
                }

                return ofs;
            }

            int _digits;
            string _fmt;
            double _val;
        }

        protected class KnobObject: WObject
        {
            public KnobObject(double init_val, double length, Color color, Rectangle rectangle, int bound_width, int bound_height) : base(create_image(create_pen(color), rectangle, length, init_val), rectangle, bound_width, bound_height)
            {
                _length = length;
                _value = init_val;
                _pen = create_pen(color);
                _drag_type = DragType.DragPoint;
                Draggable = true;
            }

            private static Pen create_pen(Color color)
            {
                return new Pen(color, 2);
            }

            public static Image create_image(Pen pen, Rectangle r, double length, double pos)
            {
                Bitmap b = new Bitmap(r.Width, r.Height);

                using (Graphics g = Graphics.FromImage(b))
                {
                    int midx = r.Width / 2;
                    int midy = r.Height / 2;

                    double angle = (pos - 0.5) * RANGE * 2 * Math.PI;
                    int epx = midx + (int)(length * Math.Sin(angle));
                    int epy = midy - (int)(length * Math.Cos(angle));

                    g.DrawLine(pen, midx, midy, epx, epy);
                }
                return b;
            }

            public override void dragged_to(int x, int y, int last_x, int last_y)
            {
                int midx = _rect.X + _rect.Width / 2;
                int midy = _rect.Y + _rect.Height / 2;

                double xr = midy - y;
                double yr = x - midx;
                double new_value = Math.Atan2(yr, xr);
                new_value = new_value / RANGE / 2 / Math.PI + 0.5;

                if (new_value > 1)
                    new_value = 1;
                else if (new_value < 0)
                    new_value = 0;
                if (Math.Abs(new_value - _value) < 0.5)
                {
                    if (_value != new_value)
                        ValueChanged(new_value);
                    _value = new_value;
                }

                _image = create_image(_pen, _rect, _length, _value);
                Invalidate(_rect);
                
            }

            public delegate void ValueChangedDelegate(double value);
            public event ValueChangedDelegate ValueChanged;

            const double RANGE = 0.95;

            Pen _pen;
            double _length;
            double _value;
        }

        public static Font get_max_font_size(Graphics g, string str, Font font, int width)
        {
            Font fnt;
            for (int size = MAX_FONT_SIZE; size >= MIN_FONT_SIZE; -- size)
            {
                fnt = new Font(font.Name, size, font.Style);
                var sz = g.MeasureString(str, fnt);

                if (width > (int)sz.Width)
                    return fnt;
            }
            return font;
        }

        public static double scale(double val, double min, double max)
        {
            return val * (max - min) + min;
        }
    }
}
