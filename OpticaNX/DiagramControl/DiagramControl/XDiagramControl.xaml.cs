using DiagramControl.Model;
using DiagramControl.Models;
using SharpGL;
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
	/// XDiagramControl.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class XDiagramControl : UserControl
	{
		public event EventHandler MouseLeave = delegate { };
		public new event EventHandler MouseMove = delegate { };
		public event EventHandler MouseDoubleClicked = delegate { };
		public new event EventHandler MouseDown = delegate { };
		public new event EventHandler MouseUp = delegate { };
		public new event EventHandler KeyUp = delegate { };
		public new event EventHandler KeyDown = delegate { };
		public event DiagramDrawHandler Draw = delegate { };

		private DiagramViewer _diagramViewer = new DiagramViewer();

		public XDiagramControl()
		{
			InitializeComponent();

			_diagramViewer.MouseLeave += _diagramViewer_MouseLeave;
			_diagramViewer.MouseDown += _diagramViewer_MouseDown;
			_diagramViewer.MouseUp += _diagramViewer_MouseUp;
			_diagramViewer.MouseMove += _diagramViewer_MouseMove;
			_diagramViewer.MouseDoubleClicked += _diagramViewer_MouseDoubleClicked;
			_diagramViewer.KeyDown += _diagramViewer_KeyDown;
			_diagramViewer.KeyUp += _diagramViewer_KeyUp;
			_diagramViewer.Draw += _diagramViewer_Draw;

			windowsFormsHost.Child = _diagramViewer;
		}

		private void _diagramViewer_MouseLeave(object sender, EventArgs e)
		{
			MouseLeave(this, e);
		}

		private void _diagramViewer_KeyUp(object sender, EventArgs e)
		{
			KeyUp(this, e);
		}

		private void _diagramViewer_KeyDown(object sender, EventArgs e)
		{
			KeyDown(this, e);
		}

		public float PixelSizeX
		{
			get
			{
				return _diagramViewer.PixelSizeX;
			}
			set
			{
				_diagramViewer.PixelSizeX = value;
			}
		}

		public float PixelSizeY
		{
			get
			{
				return _diagramViewer.PixelSizeY;
			}
			set
			{
				_diagramViewer.PixelSizeY = value;
			}
		}

		public System.Drawing.Size ControlSize
		{
			set
			{
				_diagramViewer.Size = value;
			}
		}

		private void _diagramViewer_MouseDoubleClicked(object sender, EventArgs e)
		{
			MouseDoubleClicked(this, e);
		}

		private void _diagramViewer_MouseMove(object sender, EventArgs e)
		{
			TKMouseEventArgs arg = e as TKMouseEventArgs;
			if (arg == null)
				return;

			MouseMove(this, e);
		}

		private void _diagramViewer_MouseUp(object sender, EventArgs e)
		{
			MouseUp(this, e);
		}

		private void _diagramViewer_MouseDown(object sender, EventArgs e)
		{
			MouseDown(this, e);
		}

		private void _diagramViewer_Draw(object sender, SharpGL.OpenGL gl)
		{
			Draw(this, gl);

		}
		public void AddTexture(Bitmap textureImage, RectangleF textureArea)
		{
			_diagramViewer.AddTexture(textureImage, textureArea);
		}

		public void FitToRect(RectangleF rect)
		{
			_diagramViewer.FitToRect(rect);
		}

		public void AddDiagram(DiagramInfoBase diagram)
		{
			_diagramViewer.AddDiagram(diagram);
		}

		public void ClearDiagram()
		{
			_diagramViewer.ClearDiagram();
		}

		public void Refresh()
		{
			if(_diagramViewer.InvokeRequired == true)
			{
				_diagramViewer.Invoke(new System.Windows.Forms.MethodInvoker(delegate
				{
					_diagramViewer.Refresh();
				}));
			}			
			else
			{
				_diagramViewer.Refresh();
			}
		}

		public PointF ScreenToRobot(System.Drawing.Point pt)
		{
			return _diagramViewer.ScreenToRobot(pt);
		}

		public void DrawLine(OpenGL gl, LineInfo line)
		{
			_diagramViewer.DrawLine(gl, line);
		}

		public void DrawText(OpenGL gl, int x, int y, float r, float g, float b, string faceName, float fontSize, string text)
		{
			_diagramViewer.DrawText(gl, x, y, r, g, b, faceName, fontSize, text);
		}

		public void DrawLine(OpenGL gl, IEnumerable<PointF> points, float lineWidth, float[] lineColorRGB)
		{
			_diagramViewer.DrawLine(gl, points, lineWidth, lineColorRGB);
		}
		public void DrawLineLoopPx(OpenGL gl, IEnumerable<PointF> points, float lineWidth, float[] lineColorRGB)
		{
			_diagramViewer.DrawLineLoopPx(gl, points, lineWidth, lineColorRGB);
		}
	}
}
