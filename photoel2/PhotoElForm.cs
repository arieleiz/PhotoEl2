using System;
using System.Windows.Forms;
using System.Drawing;
using OfficeOpenXml;

namespace photoel2
{
    public partial class PhotoElForm : Form
    {
        static readonly Color changed_color = Color.Red;
        static readonly Color normal_color = Color.Black;
        static readonly int[] controls_offsets_large = new[] { 66, 129, 192, 265 };
        static readonly int[] controls_offsets_small = new[] { 44, 86, 128, 176};
        readonly Action[] control_funcs;

        Model _model = new Model();
        int[] _controls_offsets;

        public PhotoElForm()
        {
            InitializeComponent();

            control_funcs = new Action[] { on_add, on_clear, on_clear_all, on_export_excel };

            workspace.StageChanged += Workspace_StageChanged;
            Workspace_StageChanged(PhotoElWorkspace.Stage.Stage1);

            lvLog.reinit();
            lvMonitor.reinit();

            _model.Distance = workspace.SourcePosition;
            _model.Metal = workspace.SelectedMetal;
            _model.Filter = workspace.SelectedFilter;
            _model.Voltage = workspace.Voltage;
            workspace.Current = _model.Current;

            ListViewItem lvi_metal = lvMonitor.add_item(new[] { "Metal", fmt_metal(workspace.SelectedMetal) });
            ListViewItem lvi_filter = lvMonitor.add_item(new[] { "Filter", fmt_filter(workspace.SelectedFilter) });
            ListViewItem lvi_distance = lvMonitor.add_item(new[] { "Distance", fmt_distance(workspace.SourcePosition) });
            ListViewItem lvi_voltage = lvMonitor.add_item(new[] { "Voltage", fmt_voltage(workspace.Voltage) });
            ListViewItem lvi_current = lvMonitor.add_item(new[] { "Current", fmt_current(workspace.Current) });

            ListViewItem[] lvi_inputs = new[] { lvi_metal, lvi_filter, lvi_distance, lvi_voltage };
            workspace.SourcePositionChanged += (pos) => { recolor(lvi_inputs, lvi_distance, fmt_distance, pos); _model.Distance = pos; };
            workspace.SourceMetalChanged += (metal) => { recolor(lvi_inputs, lvi_metal, fmt_metal, metal); _model.Metal = metal; };
            workspace.FilterChanged += (filter) => { recolor(lvi_inputs, lvi_filter, fmt_filter, filter); _model.Filter = filter; };
            workspace.VoltageChanged += (voltage) => { recolor(lvi_inputs, lvi_voltage, fmt_voltage, voltage); _model.Voltage = voltage; };

            _model.CurrentChanged += (current) =>
            {
                if (Math.Abs(workspace.Current - current) >= 1e-9)
                {
                    workspace.Current = current;
                    lvi_current.SubItems[2].ForeColor = changed_color;
                    lvi_current.SubItems[2].Text = fmt_current(current);
                    lvi_current.Tag = current;
                }
                else
                    lvi_current.SubItems[2].ForeColor = normal_color;
            };
        }

        private void Workspace_StageChanged(PhotoElWorkspace.Stage stage)
        {
            switch(stage)
            {
                case PhotoElWorkspace.Stage.Stage1:
                    lblDirections.Text = "Connect the DC amplifier to the cathode";
                    break;
                case PhotoElWorkspace.Stage.Stage2:
                    lblDirections.Text = "Connect the DC voltage source to the cathode";
                    break;
                case PhotoElWorkspace.Stage.Stage4:
                    lblDirections.Visible = false;
                    lvLog.Visible = true;
                    lvMonitor.Visible = true;
                    pbControls.Visible = true;
                    pbControls.BringToFront();
                    break;
            }
        }

        string fmt_metal(Constants.Metal m) { return string.Format("{0} ({1})", m.name, m.symbol); }
        string fmt_filter(Constants.Filter f) { return string.Format("\x03BB={0} nm", f.wave_length); }
        string fmt_distance(double d) { return string.Format("{0:F1} cm", d); }
        string fmt_voltage(double v) { return string.Format("{0:F2} V", v); }
        string fmt_current(double c) { return string.Format("{0:F2} nA", c); }

        private static void recolor<T>(ListViewItem[] items, ListViewItem changed, Func<T, string> fmt, T newval)
        {
            foreach(ListViewItem lvi in items)
            {
                if (lvi == changed)
                {
                    lvi.SubItems[2].ForeColor = changed_color;
                    lvi.SubItems[2].Text = fmt(newval);
                    lvi.Tag = newval;
                }
                else
                {
                    lvi.SubItems[2].ForeColor = normal_color;
                }
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            SuspendLayout();
            resize_controls();
            ResumeLayout();
        }

        private void resize_controls()
        {
            int ctl_height = Properties.Resources.controls_large.Height;
            Image ctl_image;
            if (Height <= 1280)
            {
                ctl_image = Properties.Resources.controls_small;
                _controls_offsets = controls_offsets_small;
            }
            else
            {
                ctl_image = Properties.Resources.controls_large;
                _controls_offsets = controls_offsets_large;
            }

            if (ctl_image != pbControls.Image)
                pbControls.Image = ctl_image;
            int div = Height - Math.Max(8 + ctl_height, Height  / 3);

            int totwidth = workspace.Width;
            int grpwidth = Width < 1200 ? (totwidth * 2) / 5 : totwidth / 2;
            lvMonitor.Left = workspace.Left;
            lvMonitor.Width = grpwidth;
            lvLog.Left = lvMonitor.Right;
            lvLog.Width = workspace.Right - lvLog.Left;
            workspace.Height = div;
            lvMonitor.Top = lvLog.Top = workspace.Bottom + 4;
            lvMonitor.Height = lvLog.Height = ClientSize.Height - 4 - workspace.Bottom - 4;
            pbControls.Left = lvMonitor.Right - pbControls.Width / 2;
            pbControls.Top = lvMonitor.Top + 5;
            lblDirections.Left = lvMonitor.Left;
            lblDirections.Top = lvMonitor.Top;
            lblDirections.Width = workspace.Width;
            lblDirections.Height = lvMonitor.Height;

            if (Width < 1200)
            {
                this.columnHeader5.Width = 100;
                this.columnHeader6.Width = 140;
            }
        }

        private void on_add()
        {
            ListViewItem lvi = lvLog.add_item(new string[] {
                workspace.SelectedMetal.symbol,
                workspace.SelectedFilter.wave_length.ToString(),
                workspace.SourcePosition.ToString("F2"),
                workspace.Voltage.ToString("F3"),
                workspace.Current.ToString("F3"),
            });
            lvi.Tag = new object[] { workspace.SelectedMetal.symbol, workspace.SelectedFilter.wave_length, workspace.SourcePosition, workspace.Voltage, workspace.Current };
            lvi.EnsureVisible();
        }

        private void on_clear()
        {
            lvLog.pop_top();
        }

        private void on_clear_all()
        {
            lvLog.clear();
        }

        private void on_export_excel()
        {
            Cursor old = pbControls.Cursor;
            try
            {
                pbControls.Cursor = Cursors.WaitCursor; 
                string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), String.Format("SP2_{0}.xlsx", DateTime.Now.ToString("yyyyMMdd_HHmmss")));
                using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(path)))
                {
                    // add a new worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("PhotoElectric");

                    for (int j = 1; j < lvLog.Columns.Count; ++j)
                        worksheet.Cells[1, j].Value = lvLog.Columns[j].Text;

                    int i = 0;
                    foreach (ListViewItem lvi in lvLog.Items)
                    {
                        ++i;
                        if (i > 1)
                        {
                            object[] data = (object[])lvi.Tag;
                            for (int j = 0; j < data.Length; ++j)
                                worksheet.Cells[i + 1, j + 1].Value = data[j];
                        }
                    }
                    package.Save();
                }
                System.Diagnostics.Process.Start(path);
            }
            finally
            {
                pbControls.Cursor = old;
            }
        }


        private void pbControls_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < _controls_offsets.Length; ++i)
            {
                if (e.Y < _controls_offsets[i])
                {
                    control_funcs[i]();
                    return;
                }
            }
        }
    }
}
