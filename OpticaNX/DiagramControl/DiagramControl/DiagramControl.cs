using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using DiagramControl.Model;
using DiagramControl.Models;
using DiagramControl.Extension;

namespace DiagramControl
{
	public partial class DiagramControl : UserControl
	{
		public event DiagramEventHandler<Graphics> DiagramPaint = delegate { };
		public event DiagramEventHandler<MouseEventArgsEx> MouseMove = delegate { };
		public event DiagramEventHandler<MouseEventArgsEx> MouseDown = delegate { };
		public event DiagramEventHandler<MouseEventArgsEx> MouseClick = delegate { };
		public event DiagramEventHandler<MouseEventArgsEx> MouseDoubleClick = delegate { };
		public event DiagramEventHandler<DiagramEventArg> DiagramAdded = delegate { };
		public event DiagramEventHandler<DiagramEventArg> DiagramEditingCompleted = delegate { };
		public event DiagramEventHandler<MouseEventArgsEx> MouseRightClick = delegate { };


		protected float FIT_EXT_SIZE = 1.05f;           // 너무 Fit을 하면 Border가 모서리에 붙으므로 Rect의 크기를 약간 크게 그려서 Fit Rect를 안쪽으로그리기 위함
		
		protected float _zoom = 1.0f;                  // Zoom Scale

		protected int _offsetX = 0;                     // current offset of image
		protected int _offsetY = 0;

		#region 드래그하여 위치이동 시 필요한 변수들

		private Point _mouseDown;
		private bool _mousepressed = false; // true as long as left mousebutton is pressed

		private int _startx = 0;            // offset of image when mouse was pressed
		private int _starty = 0;

		#endregion // 드래그하여 위치이동 시 필요한 변수들

		protected int _invertValue = 1;                // Y축 방향이 Bottom to Top일 경우에는, 그래픽 좌표와 반대여서 -1을 곱해준다.

		private bool _isAddingDiagramMode = false;
		private Point _preMousePos;
		private Point _postMousePos;
		protected NXRect<float> _editableRect;

		public DiagramControl()
		{
			InitializeComponent();
		}

		public bool InvertYAxis
		{
			get
			{
				return _invertValue == 1;
			}
			set
			{
				if (value == true)
					_invertValue = -1;
				else
					_invertValue = 1;
			}
		}

		public float Zoom
		{
			get
			{
				return _zoom;
			}
		}
	
		public bool IsAddingDiagramMode
		{
			get
			{
				return _isAddingDiagramMode;
			}
			set
			{
				_isAddingDiagramMode = value;
			}
		}

		public NXRect<float> EditableRect
		{
			set
			{
				_editableRect = value;
			}
		}

		public void FitToRect(RectangleF rect)
		{
			Fit(rect);
		}

		public PointF PixelToRobot(int x, int y)
		{
			PointF point = new PointF();

			point.X = ((float)x / _zoom - _offsetX);
			point.Y = ((float)_invertValue * y / _zoom + _offsetY);

			return point;
		}

		protected void Fit(RectangleF fitSize)
		{
			float scaleWidth = Math.Abs((float)this.Size.Width / ((float)fitSize.Width * FIT_EXT_SIZE));
			float scaleHeight = Math.Abs((float)this.Size.Height / ((float)fitSize.Height * FIT_EXT_SIZE));

			if (scaleWidth < scaleHeight)
				_zoom = scaleWidth;
			else
				_zoom = scaleHeight;

			if (float.IsInfinity(_zoom) == true)
				_zoom = 1.0f;

			PointF center = new PointF(fitSize.Left + fitSize.Width / 2, fitSize.Top + fitSize.Height / 2);

			_offsetX = (int)((int)-center.X + (float)this.Size.Width / (_zoom * 2));// + (int)fitSize.Width / 2;
			_offsetY = (int)((float)this.Size.Height / (_zoom * 2) - _invertValue * (int)center.Y);//

			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.ResetTransform();

			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

			e.Graphics.ScaleTransform(_zoom, _invertValue * _zoom);
			e.Graphics.TranslateTransform(_offsetX, _invertValue * _offsetY);

			DiagramPaint(this, e.Graphics);

			DrawItems(e.Graphics);

			if(_editableRect != null)
			{
				DrawEditableRect(e.Graphics);
			}

			if( _isAddingDiagramMode == true)
			{
				Point[] points = new Point[] { 
					new Point(Math.Min(_postMousePos.X, _preMousePos.X), Math.Min(_postMousePos.Y, _preMousePos.Y)) ,
					new Point(Math.Max(_postMousePos.X, _preMousePos.X), Math.Min(_postMousePos.Y, _preMousePos.Y)) ,
					new Point(Math.Max(_postMousePos.X, _preMousePos.X), Math.Max(_postMousePos.Y, _preMousePos.Y)) ,
					new Point(Math.Min(_postMousePos.X, _preMousePos.X), Math.Max(_postMousePos.Y, _preMousePos.Y)) ,
					new Point(Math.Min(_postMousePos.X, _preMousePos.X), Math.Min(_postMousePos.Y, _preMousePos.Y)) ,
				};

				e.Graphics.DrawLines(Pens.Red, points);
			}

			e.Graphics.ResetTransform();

		}

		private void DrawEditableRect(Graphics g)
		{
			Pen pen = new Pen(Brushes.DarkBlue, 3 / _zoom);
			pen.DashStyle = DashStyle.Dot;

			var dots = _editableRect.DiagramDots.ToArray();
			for(int i = 0; i < dots.Count()-1;  ++i)
			{
				g.DrawLine(pen, dots[i].X, dots[i].Y, dots[i + 1].X, dots[i + 1].Y);
			}

			g.DrawLine(pen, dots[dots.Count() - 1].X, dots[dots.Count() - 1].Y, dots[0].X, dots[0].Y);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);

			Point mousePosNow = e.Location;

			float oldzoom = _zoom;

			if (e.Delta > 0)
			{
				_zoom = _zoom + _zoom * 0.3F;
			}

			else if (e.Delta < 0)
			{
				_zoom = _zoom - _zoom * 0.3F;
			}
			if (_zoom > 20)
				_zoom = 20;

			if (_zoom < 0.0005)
				_zoom = 0.0005f;

			int x = mousePosNow.X - Location.X;    // Where location of the mouse in the pictureframe
			int y = mousePosNow.Y - Location.Y;

			int oldimagex = (int)(x / oldzoom);  // Where in the IMAGE is it now
			int oldimagey = (int)(y / oldzoom);

			int newimagex = (int)(x / _zoom);     // Where in the IMAGE will it be when the new zoom i made
			int newimagey = (int)(y / _zoom);

			_offsetX = newimagex - oldimagex + _offsetX;  // Where to move image to keep focus on one point
			_offsetY = newimagey - oldimagey + _offsetY;

			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (e.Button == MouseButtons.Left)
			{
				if (!_mousepressed)
				{
					_mousepressed = true;
					_mouseDown = e.Location;
					_startx = _offsetX;
					_starty = _offsetY;
				}

				if(_isAddingDiagramMode == true)
				{
					_preMousePos = GetCurrentRobotPos(e.X, e.Y);
				}


				if (_editableRect != null && _editableRect.EditingType != EditingType.None)
				{
					_editableRect.IsEditing = true;
				}
			}
			MouseDown(this, CreateMouseEventArgEx(e));

		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (_editableRect != null && _editableRect.IsEditing == true)
			{
				_editableRect.IsEditing = false;

				var currentRobotyPos = GetCurrentRobotPos(e.X, e.Y);
				DiagramEventArg arg = new DiagramEventArg();
				arg.RobotPos = currentRobotyPos.ToPointF(currentRobotyPos);
				DiagramEditingCompleted(this, arg);
			}
			_mousepressed = false;

			if(_mouseDown == e.Location)
				MouseClick(this, CreateMouseEventArgEx(e));

			if( _isAddingDiagramMode == true)
			{
				DiagramEventArg arg = new DiagramEventArg();

				arg.RobotPos = _preMousePos.ToPointF(_postMousePos);
				DiagramAdded(this, arg);
			}

			_preMousePos = new Point();
			_postMousePos = new Point();
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			if( e.Button == MouseButtons.Right)
			{
				MouseRightClick(this, CreateMouseEventArgEx(e));
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			var currentRobotyPos = GetCurrentRobotPos(e.X, e.Y);

			if ( _editableRect != null)
			{
				if (_editableRect != null && _editableRect.IsEditing == true)
				{
					_editableRect.Move(currentRobotyPos.X, currentRobotyPos.Y);
				}

				var lineOverType = _editableRect.IsOnTheLine(currentRobotyPos.X, currentRobotyPos.Y);
				switch (lineOverType)
				{
					case LineOverType.None:
						Cursor = Cursors.Default;
						break;
					case LineOverType.HSplit:
						Cursor = Cursors.HSplit;
						break;
					case LineOverType.VSplit:
						Cursor = Cursors.VSplit;
						break;
					case LineOverType.SizeNWSE:
						Cursor = Cursors.SizeNWSE;
						break;
					case LineOverType.SizeNESE:
						Cursor = Cursors.SizeNESW;
						break;
				}

				Invalidate();
			}
			else if (_mousepressed)
			{
				Point mousePosNow = e.Location;

				if (_isAddingDiagramMode == false)
				{
					int deltaX = mousePosNow.X - _mouseDown.X; // the distance the mouse has been moved since mouse was pressed
					int deltaY = mousePosNow.Y - _mouseDown.Y;

					int offsetX = 0, offsetY = 0;
					offsetX = (int)((float)_startx + (deltaX / _zoom));  // calculate new offset of image based on the current zoom factor
					offsetY = (int)((float)_starty + (deltaY / _zoom));
					// 전체의 offset 정보를 계산한다.
					_offsetX = offsetX;
					_offsetY = offsetY;
				}
				else
				{
					_postMousePos.X = currentRobotyPos.X;
					_postMousePos.Y = currentRobotyPos.Y;
				}

				Invalidate();
			}

			MouseMove(this, CreateMouseEventArgEx(e));
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			MouseDoubleClick(this, CreateMouseEventArgEx(e));
		}

		protected Point GetCurrentRobotPos(int mouseLocationX, int mouseLocationY)
		{
			Point pos = new Point();

			pos.X = (int)((float)mouseLocationX / _zoom - _offsetX);
			if (_invertValue == 1)
			{
				pos.Y = (int)((float)_invertValue * mouseLocationY / _zoom - _offsetY);
			}
			else
			{
				pos.Y = (int)((float)_invertValue * mouseLocationY / _zoom + _offsetY);
			}

			return pos;
		}

		protected MouseEventArgsEx CreateMouseEventArgEx(MouseEventArgs e)
		{
			int robotX = GetCurrentRobotPos(e.Location.X, e.Location.Y).X;
			int robotY = GetCurrentRobotPos(e.Location.X, e.Location.Y).Y;

			MouseEventArgsEx arg = new MouseEventArgsEx(e.Button, e.Clicks, e.X, e.Y, e.Delta, robotX, robotY, 0, 0);

			return arg;
		}

		protected virtual void DrawItems(Graphics g)
		{

		}

	}
}
