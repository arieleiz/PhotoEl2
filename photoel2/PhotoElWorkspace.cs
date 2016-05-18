using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace photoel2
{
    class PhotoElWorkspace : Workspace
    {
        readonly Size total_size, system_size, filter_size, source_size, metal_size, target_size;
        const int SOURCE_LEFT_BOUNDARY = 40;
        const int SOURCE_RIGHT_BOUNDARY = 368;
        const int SOURCE_TOP = 116;
        const int FILTER_TEXT_HEIGHT = 30;
        const int FILTER_SPACE = FILTER_TEXT_HEIGHT + 20;
        const int FILTER_MAX_WIDTH = 70;
        const int METAL_SPACE = 30;
        const int METAL_MAX_WIDTH = 70;
        readonly Point METAL_TARGET = new Point(692, 287);
        readonly Point FILTER_TARGET = new Point(637, 251);
        readonly Point POWER_SRC_CONNECTOR = new Point(158, 172);
        readonly Point POWER_SYS_CONNECTOR = new Point(423, 309);
        const int CORD_WIDTH = 7;
        readonly Rectangle VOLTMETER = new Rectangle(1154, 298, 84, 25);
        const int VOLTMETER_DIGITS = 4;
        readonly Rectangle VOLTKNOB = new Rectangle(1140, 354, 23, 23);
        const int VOLTKNOB_LEN = 11;
        readonly Rectangle AMPERMETER = new Rectangle(1166, 137, 66, 25);
        const int AMPERMETER_DIGITS = 3;
        readonly Rectangle LENSE = new Rectangle(640, 211, 1, 80);

        readonly Rectangle STAGE1_START = new Rectangle(855, 355, 89,74);
        readonly Point STAGE1_END = new Point(794, 227);
        readonly Rectangle STAGE2_START = new Rectangle(801, 370, 47 , 45);
        readonly Point STAGE2_END = new Point(775, 315);

        ObjectTarget _target;
        SevenSegmentTarget _voltmeter, _ammeter;
        PowerCord _cord;
        Rectangle _system_rectangle;
        TargetDraggableObject _power1, _power2;

        public enum Stage { Stage1, Stage2, Stage3, Stage4 };
        Stage _stage = Stage.Stage1;
        double _source_pos = 0;
        Constants.Metal _selected_metal = Constants.DefaultMetal;
        Constants.Filter _selected_filter = Constants.DefaultFilter;
        double _voltage = 0.5, _current = 0;

        public delegate void SourcePositionChangedDelegate(double position);
        public event SourcePositionChangedDelegate SourcePositionChanged;

        public delegate void SourceMetalChangedDelegate(Constants.Metal metal);
        public event SourceMetalChangedDelegate SourceMetalChanged;

        public delegate void FilterChangedDelegate(Constants.Filter filter);
        public event FilterChangedDelegate FilterChanged;

        public delegate void VoltageChangedDelegate(double voltage);
        public event VoltageChangedDelegate VoltageChanged;

        public delegate void StageChangedDelegate(Stage stage);
        public event StageChangedDelegate StageChanged;

        public PhotoElWorkspace() 
        {
            system_size = Properties.Resources.stage_1.Size;
            filter_size = Properties.Resources.singlefilter_red.Size;
            source_size = Properties.Resources.lightsource.Size;
            metal_size = Properties.Resources.sym_aluminium.Size;
            target_size = Properties.Resources.targetsmall.Size;
            total_size = new Size(system_size.Width, system_size.Height + FILTER_SPACE + filter_size.Height + METAL_SPACE + metal_size.Height);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double SourcePosition
        {
            get {
                return scale(_source_pos, Constants.MaxDistance, Constants.MinDistance);
            }
            set {
                _source_pos = value;
                if (SourcePositionChanged != null)
                    SourcePositionChanged(SourcePosition); }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Constants.Filter SelectedFilter
        {
            get {
                return _selected_filter;
            }
            set {
                _selected_filter = value;
                if (FilterChanged != null)
                    FilterChanged(_selected_filter);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Constants.Metal SelectedMetal
        {
            get {
                return _selected_metal;
            }
            set {
                _selected_metal = value;
                if (SourceMetalChanged != null)
                    SourceMetalChanged(_selected_metal);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),DefaultValue(0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double Voltage
        {
            get {
                return scale(_voltage, Constants.MinVoltage, Constants.MaxVoltage);
            }
            set {
                _voltage = value;
                double val = Voltage;
                _voltmeter.set_val(val);
                if (VoltageChanged != null)
                    VoltageChanged(val);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double Current
        {
            get {
                return _current;
            }
            set {
                _current = value;
                if(_ammeter != null)
                    _ammeter.set_val(_current);
            }
        }

        protected override Size get_total_size()
        {
            return total_size;
        }

        protected void set_stage(Stage stage)
        {
            _stage = stage;
            if (StageChanged != null)
                StageChanged(_stage);
        }

        protected override void reload_objects()
        {
            int sheight = (int)(system_size.Height * _draw_ratio);
            int swidth = (int)(system_size.Width * _draw_ratio);
            _system_rectangle = new Rectangle(BORDER + (Width - swidth) / 2, Height - BORDER - sheight, swidth, sheight);

            _target = new ObjectTarget(Properties.Resources.targetsmall, new Rectangle(0, 0, (int)(target_size.Width * _draw_ratio), (int)(target_size.Height * _draw_ratio)),
                                Width, Height);

            switch (_stage)
            {
                case Stage.Stage1:
                    addStage1Objects();
                    break;
                case Stage.Stage2:
                    addStage2Objects();
                    break;
                case Stage.Stage3:
                    add_object(new WObject(Properties.Resources.stage_3, _system_rectangle, Width, Height));
                    break;
                case Stage.Stage4:
                    addStage4Objects();

                    break;
            }
        }


        private void addStage1Objects()
        {
            _target.set_position((int)(_system_rectangle.Left + (STAGE1_END.X - target_size.Width / 2) * _draw_ratio),
                                            (int)(_system_rectangle.Top + (STAGE1_END.Y - target_size.Height / 2) * _draw_ratio));
            ObjectTarget[] tgtfilter = new[] { _target };

            add_object(new WObject(Properties.Resources.stage_1, _system_rectangle, Width, Height));
            _power1 = new TargetDraggableObject(Properties.Resources.power1, scale_and_shift(STAGE1_START, _system_rectangle), Width, Height, tgtfilter);
            _power1.OnTgtDrag += (wobj, tgt) => {
                    wobj.reset();
                    if (tgt != null)
                    {
                        set_stage(Stage.Stage2);
                        restart_ui();
                    }
                };
            add_object(_power1);
        }


        private void addStage2Objects()
        {
            _target.set_position((int)(_system_rectangle.Left + (STAGE2_END.X - target_size.Width / 2) * _draw_ratio),
                                            (int)(_system_rectangle.Top + (STAGE2_END.Y - target_size.Height / 2) * _draw_ratio));
            ObjectTarget[] tgtfilter = new[] { _target };

            add_object(new WObject(Properties.Resources.stage_2, _system_rectangle, Width, Height));
            _power2 = new TargetDraggableObject(Properties.Resources.power2, scale_and_shift(STAGE2_START, _system_rectangle), Width, Height, tgtfilter);
            _power2.OnTgtDrag += (wobj, tgt) => {
                wobj.reset();
                if (tgt != null)
                {
                    set_stage(Stage.Stage4);
                    restart_ui();
                }
            };
            add_object(_power2);
        }

        private void addStage4Objects()
        {
            /////////////////////////////////
            // source
            /////////////////////////////////

            add_object(new WObject(Properties.Resources.stage_4, _system_rectangle, Width, Height));

            _cord = new PowerCord((float)(CORD_WIDTH * _draw_ratio), new Point(0, 0), new Point(1, 1), Width, Height);
            add_object(_cord);

            int source_left_boundary = _system_rectangle.Left + (int)(SOURCE_LEFT_BOUNDARY * _draw_ratio);
            int source_right_boundary = _system_rectangle.Left + (int)(SOURCE_RIGHT_BOUNDARY * _draw_ratio);
            int source_area_width = source_right_boundary - source_left_boundary;
            var source = new HorizBoundedObject(Properties.Resources.lightsource,
                            new Rectangle(source_left_boundary + (int)(_source_pos * source_area_width),
                                          _system_rectangle.Top + (int)(SOURCE_TOP * _draw_ratio),
                                          (int)(source_size.Width * _draw_ratio), (int)(source_size.Height * _draw_ratio)),
                            source_left_boundary, source_area_width);

            add_object(source);
            source.OnRelDrag += (wobj, relx, rely) =>
            {
                rebuild_cord(wobj.Rect);
                SourcePosition = relx;
            };

            rebuild_cord(source.Rect);

            /////////////////////////////////
            // metal
            /////////////////////////////////

            int metal_max_width = (int)(METAL_MAX_WIDTH * _draw_ratio);
            int metal_bottom = _system_rectangle.Top - METAL_SPACE;

            ObjectTarget[] tgtmetal = null;
            Point metal_target = new Point((int)(_system_rectangle.Left + METAL_TARGET.X * _draw_ratio), 
                                            (int)(_system_rectangle.Top + METAL_TARGET.Y * _draw_ratio));
            Func<Rectangle, ObjectTarget[]> get_metal_target = (rect) =>
            {
                if(tgtmetal == null)
                {
                    var metal = new ObjectTarget(SelectedMetal.image, new Rectangle(metal_target, rect.Size), Width, Height);
                    add_object(metal);
                    tgtmetal = new[] { metal };
                }
                return tgtmetal;
            };

            Func<TargetDraggableObject, TargetDraggableObject> register_metal = (obj) =>
            {
                obj.OnTgtDrag += (wobj, tgt) => {
                    wobj.reset();
                    if (tgt != null)
                    {
                        tgt.replace_image(wobj.Image);
                        SelectedMetal = (Constants.Metal)wobj.Tag;
                    }
                };
                return obj;
            };

            var metals = add_objects_above(Constants.Metals, metal_bottom, metal_size.Width, metal_max_width, metal_size.Height,
                (pos, obj) => new[] { register_metal(new TargetDraggableObject(obj.image, pos, Width, Height, get_metal_target(pos))) });

            /////////////////////////////////
            // lense
            /////////////////////////////////

            Lense lense = new Lense(SelectedFilter.brush, scale_and_shift(LENSE, _system_rectangle), Width, Height);
            add_object(lense);

            /////////////////////////////////
            // filter
            /////////////////////////////////

            int filter_bottom = metals.Top - METAL_SPACE;
            int filter_max_width = (int)(FILTER_MAX_WIDTH * _draw_ratio);
            Font filter_font = null;
            Func<Rectangle, Font> getCachedFont = (rect) =>
            {
                if (filter_font == null)
                {
                    using (Graphics g = this.CreateGraphics())
                    {
                        filter_font = get_max_font_size(g, "888 nm", BASE_FONT, rect.Width);
                    }
                }
                return filter_font;
            };

            _target.set_position((int)(_system_rectangle.Left + (FILTER_TARGET.X - target_size.Width / 2) * _draw_ratio),
                                            (int)(_system_rectangle.Top + (FILTER_TARGET.Y - target_size.Height / 2)* _draw_ratio));
            ObjectTarget[] tgtfilter = new[] { _target };
            
            Func<TargetDraggableObject, TargetDraggableObject> register_filter = (obj) =>
            {
                obj.OnTgtDrag += (wobj, tgt) => {
                    wobj.reset();
                    if (tgt != null)
                    {
                        var filter = (Constants.Filter)wobj.Tag;
                        SelectedFilter = filter;
                        lense.set_color(filter.brush); 
                    }
                };
                return obj;
            };
            
            var filters = add_objects_above(Constants.Filters, filter_bottom, filter_size.Width, filter_max_width, filter_size.Height,
                (pos, obj) => new WObject[] { new TextLabelObject(String.Format("{0}nm", obj.wave_length), text_under(pos, FILTER_TEXT_HEIGHT), Width, Height, getCachedFont(pos)),
                                                      register_filter(new TargetDraggableObject(obj.image, pos, Width, Height, tgtfilter)) });

            /////////////////////////////////
            // meters
            /////////////////////////////////

            _voltmeter = new SevenSegmentTarget(VOLTMETER_DIGITS, "{0:F3}", Voltage, scale_and_shift(VOLTMETER, _system_rectangle), Width, Height);
            add_object(_voltmeter);
            _ammeter = new SevenSegmentTarget(AMPERMETER_DIGITS, "{0:F2}", Current, scale_and_shift(AMPERMETER, _system_rectangle), Width, Height);
            add_object(_ammeter);

            KnobObject voltknob = new KnobObject(_voltage, VOLTKNOB_LEN * _draw_ratio, Color.White, scale_and_shift(VOLTKNOB, _system_rectangle), Width, Height);
            add_object(voltknob);
            voltknob.ValueChanged += (value) => { Voltage = value; };

            /////////////////////////////////
            // box
            /////////////////////////////////

            //Rectangle bounding = Rectangle.Union(filters, metals);
            //bounding.Inflate(filter_max_width, 25);
            //WObject box = new WObject(Properties.Resources.woodcrate, bounding, Width, Height);
            //insert_object(box, 1);


        }

        private void rebuild_cord(Rectangle source_rect)
        {
            Point start = new Point((int)(_system_rectangle.Left + POWER_SYS_CONNECTOR.X * _draw_ratio), (int)(_system_rectangle.Top + POWER_SYS_CONNECTOR.Y * _draw_ratio));
            Point end = new Point((int)(source_rect.Left + POWER_SRC_CONNECTOR.X * _draw_ratio), (int)(source_rect.Top + POWER_SRC_CONNECTOR.Y * _draw_ratio));
            _cord.rebuild(start, end);
        }

        class PowerCord : WObject
        {
            readonly Pen _black_pen;
            public PowerCord(float width, Point start, Point end, int bound_width, int bound_height) : base(make_image(make_pen(width), start, end), make_rect(start, end), bound_width, bound_height)
            {
                _black_pen = make_pen(width);
            }

            public void rebuild(Point start, Point end)
            {
                Invalidate(_rect);
                _image = make_image(_black_pen, start, end);
                _rect = make_rect(start, end);
                Invalidate(_rect);
            }
            static Image make_image(Pen pen, Point start, Point end)
            {
                int top = Math.Min(start.Y, end.Y);
                int left = Math.Min(start.X, end.X);
                int bottom = Math.Max(start.Y, end.Y);
                int right = Math.Max(start.X, end.X);
                int width = right - left;
                int height = bottom - top;

                if(width == 0) width = 1;
                if(height == 0) height = 1;
                
                Bitmap b = new Bitmap(width, height);

                using (Graphics g = Graphics.FromImage(b))
                {
                    g.DrawBezier(pen, start.X - left, start.Y - top, width, 0, 0, height, end.X - left, end.Y - top);
                }
                return b;
            }
            static Pen make_pen(float width)
            {
                 return new Pen(Color.Black, width);
            }

            static Rectangle make_rect(Point start, Point end)
            {
                Point corner1 = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
                Point corner2 = new Point(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));
                return new Rectangle(corner1.X, corner1.Y, corner2.X - corner1.X, corner2.Y - corner1.Y);
            }
        }

        class Lense : WObject
        {
            public Lense(Brush brush, Rectangle rect, int bound_width, int bound_height) : base(make_image(brush, rect), rect, bound_width, bound_height)
            {
            }

            public void set_color(Brush brush)
            {
                _image = make_image(brush, _rect);
                Invalidate(_rect);
            }

            static Image make_image(Brush brush, Rectangle rect)
            {
                Bitmap b = new Bitmap(rect.Width, rect.Height);

                using (Graphics g = Graphics.FromImage(b))
                {
                    g.FillRectangle(brush, 0, 0, rect.Width, rect.Height);
                }
                return b;
            }

            static Rectangle make_rect(Point start, Point end)
            {
                Point corner1 = new Point(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));
                Point corner2 = new Point(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));
                return new Rectangle(corner1.X, corner1.Y, corner2.X - corner1.X, corner2.Y - corner1.Y);
            }

        }

    }
}
