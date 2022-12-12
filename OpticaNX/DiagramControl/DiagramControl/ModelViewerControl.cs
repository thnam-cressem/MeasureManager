using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiagramControl.Model;

namespace DiagramControl
{
	public partial class ModelViewerControl : DiagramControl
	{
		public event DiagramControlPaintEvent DiagramControlPaint = delegate { };
		public event MouseEventExHandler MouseMove = delegate { };
		public event MouseEventExHandler MouseDown = delegate { };

		//private struct DiagramShape
		//{
		//	public RectangleF Rectangle;
		//	public Color BorderColor;
		//	public bool IsFill;
		//	public Color FillColor;
		//	public int Thick;

		//	public DiagramShape(RectangleF rectangle)
		//	{
		//		Rectangle = rectangle;
		//		BorderColor = Color.Red;
		//		IsFill = false;
		//		FillColor = Color.Transparent;
		//		Thick = 1;
		//	}
		//}

		//private struct DiagramLine
		//{
		//	public PointF Start;
		//	public PointF End;
		//	public Color BorderColor;
		//	public bool IsFill;
		//	public int Thick;

		//	public DiagramLine(PointF start, PointF end)
		//	{
		//		Start = start;
		//		End = end;
		//		BorderColor = Color.Red;
		//		IsFill = false;

		//		Thick = 1;
		//	}
		//}

		//private struct DiagramCross
		//{
		//	public PointF Center;
		//	public float Length;
		//	public Color BorderColor;
		//	public int Thick;

		//	public DiagramCross(PointF center, float length, int thick)
		//	{
		//		Center = center;
		//		Length = length;
		//		BorderColor = Color.Red;
		//		Thick = thick;
		//	}
		//}

		//private Image _backImage = null;             // 배경에 그릴 이미지
		//private RectangleF _imagaArea;               // 배경 이미지 크기

		//private bool _isDrawModuleArea = false;
		//private bool _isDrawPackageArea = false;
		//private bool _isDrawWindowArea = false;
		//private bool _isDrawModelImage = false;

		//private List<DiagramShape> _diagrams = new List<DiagramShape>();
		//private List<DiagramLine> _diagramLines = new List<DiagramLine>();
		//private List<DiagramCross> _diagramCrosses = new List<DiagramCross>();

		////private ModelInfo _modelInfo;

		//public ModelViewerControl()
		//{
		//	InitializeComponent();
		//}

		//public float Zoom
		//{
		//	get
		//	{
		//		return _zoom;
		//	}
		//}

		//public bool IsDrawModuleArea
		//{
		//	get
		//	{
		//		return _isDrawModuleArea;
		//	}
		//	set
		//	{
		//		_isDrawModuleArea = value;
		//	}
		//}

		//public bool IsDrawPackageArea
		//{
		//	get
		//	{
		//		return _isDrawPackageArea;
		//	}
		//	set
		//	{
		//		_isDrawPackageArea = value;
		//	}
		//}

		//public bool IsDrawWindowArea
		//{
		//	get
		//	{
		//		return _isDrawWindowArea;
		//	}
		//	set
		//	{
		//		_isDrawWindowArea = value;
		//	}
		//}

		//public bool IsDrawModelImage
		//{
		//	get
		//	{
		//		return _isDrawModelImage;
		//	}
		//	set
		//	{
		//		_isDrawModelImage = value;
		//	}
		//}

		//public void LoadModel(ModelInfo modelInfo)
		//{
		//	ClearDiagram();

		//	_modelInfo = modelInfo;

		//	if (modelInfo == null)
		//		return;
			
		//	if (IsDrawModelImage == true)
		//		SetImage(_modelInfo.ModelImage);

		//	RectangleF rectF = RectangleF.FromLTRB(_modelInfo.Left, _modelInfo.Top, _modelInfo.Right, _modelInfo.Bottom);
		//	SetImageArea(rectF);

		//	if (IsDrawModuleArea == true)
		//	{
		//		if(_modelInfo.Modules != null)
		//		{
		//			foreach (ModuleInfo moduleInfo in _modelInfo.Modules)
		//			{
		//				rectF = RectangleF.FromLTRB(moduleInfo.Left, moduleInfo.Top, moduleInfo.Right, moduleInfo.Bottom);
		//				AddDiagram(rectF, System.Drawing.Color.Green);
		//			}
		//		}
				
		//	}

		//	//Parallel.ForEach(_modelInfo.Packages, (packageInfo) =>
		//	//{
		//	//	if (IsDrawPackageArea == true)
		//	//	{
		//	//		rectF = RectangleF.FromLTRB(packageInfo.Left, packageInfo.Top, packageInfo.Right, packageInfo.Bottom);
		//	//		//rectF = new RectangleF(packageInfo.Left, packageInfo.Top, packageInfo.Width, packageInfo.Height);
		//	//		AddDiagram(rectF, System.Drawing.Color.Green);
		//	//	}

		//	//	if (IsDrawWindowArea == true)
		//	//	{
		//	//		foreach (WindowInfo windowInfo in packageInfo.Windows)
		//	//		{
		//	//			//RectangleF windowRect = new RectangleF(windowInfo.Left + rectF.Left + rectF.Width / 2, windowInfo.Top + rectF.Top + rectF.Height / 2, windowInfo.Width, -windowInfo.Height);

		//	//			// ObjType == 12는 Wire이다. Rect로 그리지 말고, Line으로 그려야 한다.
		//	//			if (windowInfo.ObjType == Cressem.Model.Enums.ObjType.OBJ_TYPE_WIRE)
		//	//			{
		//	//				PointF start = new PointF(windowInfo.Left, windowInfo.Top);
		//	//				PointF end = new PointF(windowInfo.Right, windowInfo.Bottom);


		//	//				AddLine(start, end, System.Drawing.Color.Green);
		//	//			}
		//	//			else
		//	//			{
		//	//				RectangleF windowRect = RectangleF.FromLTRB(windowInfo.Left, windowInfo.Top, windowInfo.Right, windowInfo.Bottom);
		//	//				AddDiagram(windowRect, System.Drawing.Color.Green);
		//	//			}
		//	//		}
		//	//	}
		//	//});

		//	foreach (PackageInfo packageInfo in _modelInfo.Packages)
		//	{
		//		if (IsDrawPackageArea == true)
		//		{
		//			rectF = RectangleF.FromLTRB(packageInfo.Left, packageInfo.Top, packageInfo.Right, packageInfo.Bottom);
		//			//rectF = new RectangleF(packageInfo.Left, packageInfo.Top, packageInfo.Width, packageInfo.Height);
		//			AddDiagram(rectF, System.Drawing.Color.Green);
		//		}

		//		if (IsDrawWindowArea == true)
		//		{
		//			foreach (WindowInfo windowInfo in packageInfo.Windows)
		//			{
		//				//RectangleF windowRect = new RectangleF(windowInfo.Left + rectF.Left + rectF.Width / 2, windowInfo.Top + rectF.Top + rectF.Height / 2, windowInfo.Width, -windowInfo.Height);

		//				// ObjType == 12는 Wire이다. Rect로 그리지 말고, Line으로 그려야 한다.
		//				if (windowInfo.ObjType == Cressem.Model.Enums.ObjType.OBJ_TYPE_WIRE)
		//				{
		//					PointF start = new PointF(windowInfo.Left, windowInfo.Top);
		//					PointF end = new PointF(windowInfo.Right, windowInfo.Bottom);


		//					AddLine(start, end, System.Drawing.Color.Green);
		//				}
		//				else
		//				{
		//					RectangleF windowRect = RectangleF.FromLTRB(windowInfo.Left, windowInfo.Top, windowInfo.Right, windowInfo.Bottom);
		//					AddDiagram(windowRect, System.Drawing.Color.Green);
		//				}
		//			}
		//		}
		//	}
		//}
		//public void FitToImage()
		//{
		//	Fit(_imagaArea);
		//}

		//public void SetImage(Image image)
		//{
		//	if (_backImage != null)
		//		_backImage.Dispose();
		//	_backImage = image;
		//}

		//public void SetImageArea(RectangleF imagaArea)
		//{
		//	_imagaArea = imagaArea;

		//	if (_backImage != null)
		//		_zoom = _backImage.Width / _imagaArea.Width;

		//	DiagramShape shape = new DiagramShape(imagaArea);
		//	shape.Thick = 2;
		//	shape.BorderColor = Color.DarkGreen;
		//	_diagrams.Add(shape);
		//	//_offsetX = (int)(-_imagaArea.Left);
		//	//_offsetY = (int)(_imagaArea.Top);
		//}

		//public void ClearDiagram()
		//{
		//	_diagrams.Clear();
		//	_diagramLines.Clear();
		//	_diagramCrosses.Clear();
		//	_backImage = null;
		//}

		//public void AddDiagram(RectangleF rect, Color? borderColor = null)
		//{
		//	DiagramShape shape = new DiagramShape(rect);
		//	shape.Rectangle = rect;
		//	shape.BorderColor = borderColor.HasValue ? borderColor.Value : Color.Green;

		//	if (borderColor.HasValue)
		//		shape.BorderColor = borderColor.Value;

		//	_diagrams.Add(shape);
		//}

		//public void AddLine(PointF start, PointF end, Color? borderColor = null)
		//{
		//	DiagramLine line = new DiagramLine(start, end);
		//	line.BorderColor = borderColor.HasValue ? borderColor.Value : Color.Green;
		//	line.Thick = 2;

		//	if (borderColor.HasValue)
		//		line.BorderColor = borderColor.Value;

		//	_diagramLines.Add(line);
		//}

		//public void AddCross(PointF center, float length)
		//{
		//	DiagramCross cross = new DiagramCross(center, length, 1);
		//	_diagramCrosses.Add(cross);
		//}


		//private RectangleF GetShowingArea()
		//{
		//	float width = this.Size.Width / _zoom;
		//	float height = this.Size.Height / _zoom;
		//	float x = _offsetX * -1.0f;
		//	float y = _offsetY - height;

		//	RectangleF showingArea = new RectangleF(x, y, width, height);

		//	return showingArea;
		//}

		//protected override void DrawItems(Graphics g)
		//{
		//	//Graphics g = this.CreateGraphics();

		//	if (_backImage != null && IsDrawModelImage)
		//	{
		//		g.DrawImage(_backImage, _imagaArea.Left, _imagaArea.Top, _imagaArea.Width, _imagaArea.Height);
		//	}
			
		//	RectangleF showingArea = GetShowingArea();

		//	Pen pen2 = new Pen(Brushes.Blue);
		//	pen2.Width = 1 / _zoom;
		//	//e.Graphics.DrawRectangle(pen2, Rectangle.FromLTRB(0, 0, (int)_imagaArea.Right, (int)_imagaArea.Bottom));

		//	//e.Graphics.FillRectangle(Brushes.Green, _imagaArea.Left, _imagaArea.Bottom, _imagaArea.Width, -_imagaArea.Height);
		//	foreach (DiagramShape shape in _diagrams)
		//	{
		//		if (showingArea.Contains(shape.Rectangle) == false)
		//			continue;

		//		RectangleF rect = shape.Rectangle;
		//		Rectangle rc = new Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);
		//		//Rectangle rc = new Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, _invertValue * (int)rect.Height);

		//		Pen pen = new Pen(shape.BorderColor);
		//		pen.Width = shape.Thick / _zoom;

		//		//e.Graphics.DrawRectangle(pen, rc);
		//		/*/
		//		//e.Graphics.DrawRectangle(pen, rect.Left, rect.Top + rect.Height, rect.Width, Math.Abs(rect.Height));
		//		e.Graphics.DrawRectangle(pen, rect.Left, rect.Top, rect.Width, rect.Height);
		//		/*/
		//		g.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
		//		g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
		//		g.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);
		//		g.DrawLine(pen, rect.Left, rect.Bottom, rect.Right, rect.Bottom);
		//		//*/
				
		//		pen.Width = 100;
		//		//GDI.DrawXORLine(e.Graphics, pen, (int)0, (int)0, (int)100, (int)100);

		//	}

		//	foreach (DiagramLine line in _diagramLines)
		//	{
		//		if (showingArea.Contains(line.Start) == false || showingArea.Contains(line.End) == false)
		//			continue;

		//		Pen pen = new Pen(line.BorderColor);
		//		pen.Width = line.Thick / _zoom;

		//		g.DrawLine(pen, line.Start, line.End);
		//	}

		//	foreach (DiagramCross cross in _diagramCrosses)
		//	{
		//		Pen pen = new Pen(cross.BorderColor);
		//		pen.Width = cross.Thick / _zoom;

		//		g.DrawLine(pen, new PointF(cross.Center.X - cross.Length, cross.Center.Y), new PointF(cross.Center.X + cross.Length, cross.Center.Y));
		//		g.DrawLine(pen, new PointF(cross.Center.X , cross.Center.Y - cross.Length), new PointF(cross.Center.X, cross.Center.Y + cross.Length));

		//	}
			

		//	//GDI gdi = new GDI();
		//	//gdi.BeginGDI(g);
		//	//IntPtr gPen = gdi.CreatePEN(PenStyles.PS_DASH, 2, Color.Red.ToArgb());
		//	//gdi.SetROP2(drawingMode.R2_XORPEN);

		//	//IntPtr gdipen = gdi.CreatePEN(PenStyles.PS_DASH, 20, GDI.RGB(255, 0, 0));
		//	//IntPtr oldpen;
		//	//oldpen = gdi.SelectObject(gdipen);

		//	//int X1 = 0, Y1 = 0;
		//	//int X2 = 1000, Y2 = 1000;
		//	//gdi.MoveTo(X1, Y1);
		//	//gdi.LineTo(X2, Y2);
		//	//gdi.SelectObject(oldpen);
		//	//gdi.DeleteOBJECT(gdipen);
		//	//gdi.EndGDI();



		//	DiagramControlPaint(this, g);
		//}

		//protected override void OnPaint(PaintEventArgs e)
		//{
		//	base.OnPaint(e);
		//}

		//protected override void OnMouseMove(MouseEventArgs e)
		//{
		//	var arg = CreateMouseEventArgEx(e);
		//	MouseMove(this, arg);
		//	base.OnMouseMove(e);
		//}

		//protected override void OnMouseDown(MouseEventArgs e)
		//{
		//	var arg = CreateMouseEventArgEx(e);
		//	MouseDown(this, arg);
		//	if (arg.Handled == false)
		//		base.OnMouseDown(e);
		//}

		//protected MouseEventArgsEx CreateMouseEventArgEx(MouseEventArgs e)
		//{
		//	int robotX = (int)((float)e.X / _zoom - _offsetX);
		//	int robotY = 0;
		//	if (_invertValue == 1)
		//	{
		//		robotY = (int)((float)_invertValue * e.Y / _zoom - _offsetY);
		//	}
		//	else
		//	{
		//		robotY = (int)((float)_invertValue * e.Y / _zoom + _offsetY);
		//	}

		//	MouseEventArgsEx arg = new MouseEventArgsEx(e.Button, e.Clicks, e.X, e.Y, e.Delta, robotX, robotY);

		//	return arg;
		//}
	}
}
