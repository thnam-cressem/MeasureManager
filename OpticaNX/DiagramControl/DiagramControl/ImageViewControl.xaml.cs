using DiagramControl.Model;
using DiagramControl.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiagramControl
{
	/// <summary>
	/// Interaction logic for ImageViewControl.xaml
	/// </summary>
	public partial class ImageViewControl : UserControl
	{
		public event DiagramControlPaintEvent DiagramControlPaint = delegate { };
		public event MouseEventExHandler MouseMove = delegate { };
		public event MouseEventExHandler MouseDown = delegate { };
		public event MouseEventExHandler MouseClick = delegate { };
		public event MouseEventExHandler MouseRightClick = delegate { };
		public event MouseEventExHandler MouseDoubleClick = delegate { };
		public event DiagramEventHandler<DiagramEventArg> DiagramEditingCompleted = delegate { };
		public event DiagramEventHandler<DiagramEventArg> DiagramAdded = delegate { };

		public event MouseEventExHandler MouseButtonUp = delegate { };
		public new event System.Windows.Forms.KeyEventHandler KeyDown = delegate { };
		public new event System.Windows.Forms.KeyEventHandler KeyUp = delegate { };

		//private DiagramControl _diagramControl = new DiagramControl();
		private DiagramControl _diagramControl = new DiagramControl();
		private System.Drawing.Image _image;
		private RectangleF _rect;
		private List<RectangleF> _defectRects = new List<RectangleF>();

		public ImageViewControl()
		{
			InitializeComponent();

			_diagramControl.MouseMove += _diagramControl_MouseMove1;
			_diagramControl.DiagramPaint += _diagramControl_DiagramPaint;
			_diagramControl.KeyDown += _diagramControl_KeyDown;
			_diagramControl.KeyUp += _diagramControl_KeyUp;
			_diagramControl.MouseDown += _diagramControl_MouseDown;
			_diagramControl.MouseClick += _diagramControl_MouseClick;
			_diagramControl.MouseDoubleClick += _diagramControl_MouseDoubleClick1;
			_diagramControl.MouseRightClick += _diagramControl_MouseRightClick;
			_diagramControl.DiagramAdded += _diagramControl_DiagramAdded;
			_diagramControl.DiagramEditingCompleted += _diagramControl_DiagramEditingCompleted;
			_diagramControl.InvertYAxis = false;

			windowsFormsHost.Child = _diagramControl;
		}

		private void _diagramControl_DiagramEditingCompleted(object sender, DiagramEventArg t)
		{
			DiagramEditingCompleted(this, t);
		}

		private void _diagramControl_MouseRightClick(object sender, MouseEventArgsEx t)
		{
			MouseRightClick(this, t);
		}

		private void _diagramControl_DiagramAdded(object sender, DiagramEventArg t)
		{
			DiagramAdded(this, t);
		}

		private void _diagramControl_MouseDoubleClick1(object sender, MouseEventArgsEx t)
		{
			MouseDoubleClick(this, t);
		}


		private void _diagramControl_MouseClick(object sender, MouseEventArgsEx t)
		{
			MouseClick(this, t);
		}

		private void _diagramControl_MouseMove1(object sender, MouseEventArgsEx t)
		{
			MouseMove(this, t);
		}

		private void _diagramControl_DiagramPaint(object sender, Graphics t)
		{
			DiagramControlPaint(this, t);
		}

		private void _diagramControl_MouseDown(object sender, MouseEventArgsEx e)
		{
			MouseDown(this, e);
		}

		private void _diagramControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			KeyDown(this, e);
		}

		private void _diagramControl_MouseUp(object sender, MouseEventArgsEx e)
		{
			MouseButtonUp(this, e);
		}

		private void _diagramControl_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (KeyUp != null)
				KeyUp(this, e);
		}

		public bool InvertYAxis
		{
			get
			{
				return _diagramControl.InvertYAxis;
			}
			set
			{
				_diagramControl.InvertYAxis = value;
			}
		}

		public float ZoomScale
		{
			get
			{
				return _diagramControl.Zoom;
			}
		}

		public bool IsAddingDiagramMode
		{
			get
			{
				return _diagramControl.IsAddingDiagramMode;
			}
			set
			{
				_diagramControl.IsAddingDiagramMode = value;
			}
		}

		public NXRect<float> EditableRect
		{
			set
			{
				_diagramControl.EditableRect = value;
			}
		}

		private void _diagramControl_MouseMove(object sender, Model.MouseEventArgsEx e)
		{
			MouseMove(this, e);
		}

		//private void _diagramControl_DiagramControlPaint(object sender, Graphics g)
		//{
		//	if (_image != null)
		//	{
		//		g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
		//		g.DrawImage(_image, _rect.Left, _rect.Top, _rect.Width, _rect.Height);
		//	}

		//	foreach (var rect in _defectRects)
		//	{
		//		System.Drawing.Rectangle rc = new System.Drawing.Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);

		//		System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Brushes.Red);
		//		pen.Width = 1.0f / _diagramControl.Zoom;

		//		g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
		//		g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
		//		g.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);
		//		g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
		//	}

		//	DiagramControlPaint(this, g);
		//}

		//public void SetImage(System.Drawing.Image image, RectangleF rect)
		//{
		//	_image = image;
		//	_rect = rect;

		//	//_diagramControl.ClearDiagram();
		//	//_diagramControl.SetImageArea(rect);
		//	//_diagramControl.FitToImage();
		//	_diagramControl.Invalidate();
		//}

		public void Clear()
		{
			_defectRects.Clear();
			_image = null;
			_rect = new RectangleF();
			//_diagramControl.ClearDiagram();
			//_diagramControl.ClearDiagram();
			_diagramControl.Invalidate();
		}

		public void AddRect(RectangleF rect, System.Drawing.Color? color = null)
		{
			_defectRects.Add(rect);
			//_diagramControl.AddDiagram(rect, color);
		}

		public void FitToRect(RectangleF rect)
		{
			_diagramControl.FitToRect(rect);
			_diagramControl.Invalidate();
		}

		public void Refresh()
		{
			if(_diagramControl.InvokeRequired)
            {
				_diagramControl.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate {
					Refresh();
				}));
            }
			else
				_diagramControl.Refresh();
		}

	}
}
