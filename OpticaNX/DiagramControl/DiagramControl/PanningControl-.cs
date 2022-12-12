using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpGL.Enumerations;
using SharpGL;
using System.Drawing.Imaging;
using SharpGL.SceneGraph.Assets;
using DiagramControl.Model;
using System.Runtime.InteropServices;

namespace DiagramControl
{
	public partial class PanningControl : UserControl
	{
		#region Events

		public new event EventHandler MouseLeave = delegate { };
		public new event EventHandler MouseMove = delegate { };
		public event EventHandler MouseDoubleClicked = delegate { };
		public new event EventHandler MouseDown = delegate { };
		public new event EventHandler MouseUp = delegate { };
		public new event EventHandler KeyDown = delegate { };
		public new event EventHandler KeyUp = delegate { };

		#endregion

		#region Variables

		/// <summary>
		/// About Zoom Variables
		/// </summary>
		private float _zoom = 100000.0f;
		private float _aspectRatio;

		private float _xSpan;
		private float _ySpan;

		/// <summary>
		/// Storage the texture itself.
		/// </summary>
		protected Texture _texture = new Texture();
		protected RectangleF _textureArea = new RectangleF();

		protected List<Tuple<uint, RectangleF>> _textureAreas = new List<Tuple<uint, RectangleF>>();

		/// <summary>
		/// About Panning Variables
		/// </summary>
		private bool _isMouseDown = false;
		private Point _mouseDownPoint;
		private SizeF _offsetStart;
		private SizeF _offsetSize = new SizeF(0.0f, 0.0f);
		private System.Drawing.Imaging.BitmapData _glBitmapData;

		private RobotOriginPosition _robotOriginPosition = RobotOriginPosition.LeftBottom;

		/// <summary>
		/// Cache of font bitmap enties.
		/// </summary>
		private readonly List<FontBitmapEntry> fontBitmapEntries = new List<FontBitmapEntry>();

		#endregion

		#region Constructor

		public PanningControl()
		{
			InitializeComponent();
			Disposed += PanningControl_Disposed;

			openGLControl1.MouseDoubleClick += OnMouseDoubleClick;
			openGLControl1.PreviewKeyDown += OpenGLControl1_PreviewKeyDown;
			openGLControl1.MouseLeave += OpenGLControl1_MouseLeave;
			PixelSizeX = PixelSizeY = 1.0f;
		}

		private void PanningControl_Disposed(object sender, EventArgs e)
		{
			openGLControl1.OpenGL.DeleteTextures(_textureAreas.Count, _textureAreas.Select(x=>x.Item1).ToArray());
		}

		private void OpenGLControl1_MouseLeave(object sender, EventArgs e)
		{
			MouseLeave(this, e);
		}

		private void OpenGLControl1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			KeyDown(this, e);
		}

		#endregion

		#region Properties

		public float PixelSizeX { get; set; }

		public float PixelSizeY { get; set; }

		public RobotOriginPosition RobotOriginPosition
		{
			get
			{
				return _robotOriginPosition;
			}
			set
			{
				_robotOriginPosition = value;
			}
		}
		#endregion

		#region // public methods

		public new void Move(PointF pt)
		{
			if(openGLControl1.InvokeRequired == true)
			{
				openGLControl1.Invoke(new MethodInvoker(delegate
				{
					Move(pt);
				}));
			}
			else
			{
				_offsetSize.Width = _xSpan / 2 - pt.X;
				_offsetSize.Height = _ySpan / 2 - pt.Y;
				openGLControl1.Refresh();
			}			
		}

		public void FitToRect(RectangleF rect)
		{
			float scaleWidth = (float)openGLControl1.Width / rect.Width;
			float scaleHeight = (float)openGLControl1.Height / rect.Height;

			if (scaleHeight < scaleWidth)
			{
				_zoom = rect.Width / (rect.Width / rect.Height);
			}
			else
			{
				if (_aspectRatio < 1.0f)
					_zoom = rect.Height * (rect.Width / rect.Height) / 1;
				else
					_zoom = rect.Height * (rect.Width / rect.Height) / _aspectRatio;
			}

			Reshape();
			Move(new PointF(rect.Left + rect.Width / 2, rect.Y + rect.Height / 2));
		}

		#endregion

		#region // protected methods

		/// <summary>
		/// 이 클래스를 상속한 하위 클래스에서 그려야 할 항목을 그린다.
		/// </summary>
		/// <param name="gl"></param>
		protected virtual void OnDraw(OpenGL gl) { }

		public void DrawTextOnStaticPosition(OpenGL gl, int x, int y, float r, float g, float b, string faceName, float fontSize, string text)
		{
			gl.DrawText(x, y, r, g, b, faceName, fontSize / _zoom, text);
		}

		/// <summary>
		/// Draws the text.
		/// </summary>
		/// <param name="gl">The gl.</param>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <param name="r">The r.</param>
		/// <param name="g">The g.</param>
		/// <param name="b">The b.</param>
		/// <param name="faceName">Name of the face.</param>
		/// <param name="fontSize">Size of the font.</param>
		/// <param name="text">The text.</param>
		public void DrawText(OpenGL gl, int x, int y, float r, float g, float b, string faceName, float fontSize, string text)
		{
			//  Get the font size in pixels.
			var fontHeight = (int)(fontSize * (16.0f / 12.0f));

			//  Do we have a font bitmap entry for this OpenGL instance and face name?
			var result = (from fbe in fontBitmapEntries
							  where fbe.HDC == gl.RenderContextProvider.DeviceContextHandle
							  && fbe.HRC == gl.RenderContextProvider.RenderContextHandle
							  && String.Compare(fbe.FaceName, faceName, StringComparison.OrdinalIgnoreCase) == 0
							  && fbe.Height == fontHeight
							  select fbe).ToList();

			//  Get the FBE or null.
			var fontBitmapEntry = result.FirstOrDefault();

			//  If we don't have the FBE, we must create it.
			if (fontBitmapEntry == null)
				fontBitmapEntry = CreateFontBitmapEntry(gl, faceName, fontHeight);

			double width = gl.RenderContextProvider.Width;
			double height = gl.RenderContextProvider.Height;

			//  Create the appropriate projection matrix.
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.PushMatrix();
			gl.LoadIdentity();
			
			int[] viewport = new int[4];
			gl.GetInteger(OpenGL.GL_VIEWPORT, viewport);
			//gl.Ortho(0, width, 0, height, -1, 1);
			gl.Ortho2D(0, _xSpan, 0, _ySpan);

			gl.Translate(_offsetSize.Width, _offsetSize.Height, -0f);            // Move Left And Into The Screen

																										 //  Create the appropriate modelview matrix.
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
			gl.PushMatrix();
			gl.LoadIdentity();
			gl.Color(r, g, b);
			gl.RasterPos(x, y);

			gl.PushAttrib(OpenGL.GL_LIST_BIT | OpenGL.GL_CURRENT_BIT | OpenGL.GL_ENABLE_BIT | OpenGL.GL_TRANSFORM_BIT);
			gl.Color(r, g, b);
			gl.Disable(OpenGL.GL_LIGHTING);
			gl.Disable(OpenGL.GL_TEXTURE_2D);
			gl.Disable(OpenGL.GL_DEPTH_TEST);
			gl.RasterPos(x, y);

			//  Set the list base.
			gl.ListBase(fontBitmapEntry.ListBase);

			//  Create an array of lists for the glyphs.
			var lists = text.Select(c => (byte)c).ToArray();

			//  Call the lists for the string.
			gl.CallLists(lists.Length, lists);
			gl.Flush();

			//  Reset the list bit.
			gl.PopAttrib();

			//  Pop the modelview.
			gl.PopMatrix();

			//  back to the projection and pop it, then back to the model view.
			gl.MatrixMode(OpenGL.GL_PROJECTION);
			gl.PopMatrix();
			gl.MatrixMode(OpenGL.GL_MODELVIEW);
		}
		#endregion

		#region // Event Handlers

		protected void OnGLControlLoad(object sender, EventArgs e)
		{
			//openGLControl1.CreateControl();
			//openGLControl1.OpenGL.Create(SharpGL.Version.OpenGLVersion.OpenGL2_1, RenderContextType.FBO, 500, 500, 32, null);

			//InitTexture();
		}

		protected virtual void OnGLControlResized(object sender, EventArgs e)
		{
			Reshape();
		}

		protected virtual void OnOpenGLDraw(object sender, SharpGL.RenderEventArgs args)
		{
			//  Get OpenGL.
			var gl = openGLControl1.OpenGL;

			//  Clear and load identity.
			gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
			gl.LoadIdentity();

			gl.ClearColor(1.0f, 1.0f, 1.0f, 0.0f);

			gl.Translate(_offsetSize.Width, _offsetSize.Height, -0f);            // Move Left And Into The Screen

			OnDraw(gl);

			gl.Flush();
		}

		protected virtual void OnMouseWheel(object sender, MouseEventArgs e)
		{
			const float MIN_ZOOM_SCALE = 5000F;

			float oldZoom = _zoom;

			if (e.Delta < 0)
			{
				_zoom *= 1.20F;
			}
			else
			{
				_zoom *= 0.80F;
			}

			if (_zoom <= 0.0f)
				_zoom = MIN_ZOOM_SCALE;

			_zoom = (float)Math.Round((decimal)_zoom, 3);

			Point mousePosNow = e.Location;

			int x = mousePosNow.X / 2 - this.openGLControl1.Size.Width / 2; // - this.openGLControl1.Location.X;    // Where location of the mouse in the pictureframe
			int y = this.openGLControl1.Size.Height / 2 - mousePosNow.Y / 2;// - this.openGLControl1.Location.Y;

			float oldimagex = (mousePosNow.X * (oldZoom / GetControlMinSize()));  // Where in the IMAGE is it now
			float oldimagey = ((mousePosNow.Y - this.openGLControl1.Size.Height) * (oldZoom / GetControlMinSize()));

			float newimagex = (mousePosNow.X * (_zoom / GetControlMinSize()));     // Where in the IMAGE will it be when the new zoom i made
			float newimagey = ((mousePosNow.Y - this.openGLControl1.Size.Height) * (_zoom / GetControlMinSize()));

			_offsetSize.Width = _offsetSize.Width + (newimagex - oldimagex);  // Where to move image to keep focus on one point
			_offsetSize.Height = _offsetSize.Height + (oldimagey - newimagey);


			Reshape();

			OnMouseMove(e);
		}

		protected virtual void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				_mouseDownPoint = e.Location;
				_offsetStart = _offsetSize;
				_isMouseDown = true;

				TKMouseEventArgs arg = CreateMouseEventArg(e.Location, e.Button, e.Clicks, e.X, e.Y, e.Delta);
				MouseDown(this, arg);
			}
			else
			{
				_mouseDownPoint = e.Location;
				_offsetStart = _offsetSize;
				//_isMouseDown = true;

				TKMouseEventArgs arg = CreateMouseEventArg(e.Location, e.Button, e.Clicks, e.X, e.Y, e.Delta);
				MouseDown(this, arg);
			}

		}

		protected virtual void OnMouseUp(object sender, MouseEventArgs e)
		{
			_isMouseDown = false;

			TKMouseEventArgs arg = CreateMouseEventArg(e.Location, e.Button, e.Clicks, e.X, e.Y, e.Delta);
			MouseUp(this, arg);
		}

		protected virtual void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (_isMouseDown == true)
			{
				_offsetSize.Width = _offsetStart.Width + (float)(e.Location.X - _mouseDownPoint.X) * _zoom / GetControlMinSize();
				_offsetSize.Height = _offsetStart.Height - (float)(e.Location.Y - _mouseDownPoint.Y) * _zoom / GetControlMinSize();

				System.Diagnostics.Debug.WriteLine(String.Format($"OffsetSize Width : ({_offsetSize.Width}, {_offsetSize.Height})"));

				openGLControl1.Refresh();
			}

			TKMouseEventArgs arg = CreateMouseEventArg(e.Location, e.Button, e.Clicks, e.X, e.Y, e.Delta);

			MouseMove(this, arg);
		}

		private TKMouseEventArgs CreateMouseEventArg(Point pt, MouseButtons button, int clicks, int x, int y, int delta)
		{
			TKMouseEventArgs arg = new TKMouseEventArgs(button, clicks, x, y, delta);
			arg.RobotLocation = new PointF((pt.X * _zoom / GetControlMinSize() - _offsetSize.Width), ((this.openGLControl1.Size.Height - pt.Y) * _zoom / GetControlMinSize() - _offsetSize.Height));

			if (_textureArea.IsEmpty == false)
			{
				arg.PixelLocation = new Point((int)(arg.RobotLocation.X / PixelSizeX - _textureArea.Left / PixelSizeX), (int)((_textureArea.Bottom - arg.RobotLocation.Y) / PixelSizeY));
			}
			
			return arg;
		}

		protected virtual void OnMouseDoubleClick(object sender, MouseEventArgs e)
		{


		}

		public PointF ScreenToRobot(Point pt)
		{
			PointF robot = new PointF();

			robot.X = pt.X * _zoom / GetControlMinSize() - _offsetSize.Width;
			robot.Y = _offsetSize.Height - (pt.Y - this.openGLControl1.Size.Height) * _zoom / GetControlMinSize();

			return robot;
		}
		
		protected void DrawCenterScreenCross()
		{
			var gl = openGLControl1.OpenGL;

			PointF leftCenter = ScreenToRobot(new Point(0, openGLControl1.Height / 2));
			PointF rightCenter = ScreenToRobot(new Point(openGLControl1.Width, openGLControl1.Height / 2));

			PointF topCenter = ScreenToRobot(new Point(openGLControl1.Width / 2, 0));
			PointF bottomCenter = ScreenToRobot(new Point(openGLControl1.Width / 2, openGLControl1.Height));

			gl.Disable(OpenGL.GL_LINE_STIPPLE);
			gl.LineWidth(1.0f);
			gl.Begin(OpenGL.GL_LINES);               // Start Drawing The Pyramid
			{
				gl.Color(0.3f, 0.3f, 0.3f);         // Red	
				gl.Vertex(leftCenter.X, leftCenter.Y);
				gl.Vertex(rightCenter.X, rightCenter.Y);

				gl.Vertex(topCenter.X, topCenter.Y);
				gl.Vertex(bottomCenter.X, bottomCenter.Y);
			}
			gl.End();                  // Done Drawing The Pyramid
		}

		protected virtual void OnKeyUp(object sender, KeyEventArgs e)
		{
			KeyUp(this, e);
		}

		protected virtual void OnKeyDown(object sender, KeyEventArgs e)
		{
			KeyDown(this, e);
		}

		private void openGLControl1_DoubleClick(object sender, EventArgs e)
		{
			if (e is MouseEventArgs)
			{
				var arg = e as MouseEventArgs;
				TKMouseEventArgs tArg = CreateMouseEventArg((e as MouseEventArgs).Location, arg.Button, arg.Clicks, arg.X, arg.Y, arg.Delta);
				MouseDoubleClicked(this, tArg);
			}

		}

		#endregion

		#region // private methods

		private FontBitmapEntry CreateFontBitmapEntry(OpenGL gl, string faceName, int height)
		{
			//  Make the OpenGL instance current.
			gl.MakeCurrent();

			//  Create the font based on the face name.
			var hFont = Win32.CreateFont(height, 0, 0, 0, Win32.FW_DONTCARE, 0, 0, 0, Win32.DEFAULT_CHARSET,
				 Win32.OUT_OUTLINE_PRECIS, Win32.CLIP_DEFAULT_PRECIS, Win32.CLEARTYPE_QUALITY, Win32.VARIABLE_PITCH, faceName);

			//  Select the font handle.
			var hOldObject = Win32.SelectObject(gl.RenderContextProvider.DeviceContextHandle, hFont);

			//  Create the list base.
			var listBase = gl.GenLists(1);

			//  Create the font bitmaps.
			bool result = Win32.wglUseFontBitmaps(gl.RenderContextProvider.DeviceContextHandle, 0, 255, listBase);

			//  Reselect the old font.
			Win32.SelectObject(gl.RenderContextProvider.DeviceContextHandle, hOldObject);

			//  Free the font.
			Win32.DeleteObject(hFont);

			//  Create the font bitmap entry.
			var fbe = new FontBitmapEntry()
			{
				HDC = gl.RenderContextProvider.DeviceContextHandle,
				HRC = gl.RenderContextProvider.RenderContextHandle,
				FaceName = faceName,
				Height = height,
				ListBase = listBase,
				ListCount = 255
			};

			//  Add the font bitmap entry to the internal list.
			fontBitmapEntries.Add(fbe);

			return fbe;
		}

		/// <summary>
		/// 현재 화면에 보이는 Robot영역을 반환한다.
		/// </summary>
		/// <returns></returns>
		protected Rectangle GetCurrentViewArea()
		{
			int x = -(int)_offsetSize.Width;
			int width = (int)(_zoom * (_aspectRatio < 1.0f ? 1 : _aspectRatio));
			int y = (int)_offsetSize.Height;
			int height = (int)(_zoom / (_aspectRatio < 1.0f ? _aspectRatio : 1));

			Rectangle area = new Rectangle(x, y, width, height);

			return area;
		}

		/// <summary>
		/// point파라미터가 현재 화면영역안에 있는지 반환한다.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		private bool IsInCurentView(PointF point)
		{
			Rectangle area = GetCurrentViewArea();

			return area.Contains((int)point.X, (int)point.Y);
		}

		/// <summary>
		/// point파라미터가 현재 화면영역안에 있는지 반환한다.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		private bool IsInCurentView(Point point)
		{
			Rectangle area = GetCurrentViewArea();

			return area.Contains(point.X, point.Y);
		}

		/// <summary>
		/// rectangle영역이 현재 화면영역안에 있는지 반환한다.
		/// </summary>
		/// <param name="rectangle"></param>
		/// <returns></returns>
		private bool IsInCurentView(Rectangle rectangle)
		{
			Rectangle area = GetCurrentViewArea();

			return area.Contains(rectangle);
		}

		/// <summary>
		/// 컨트롤 사이즈가 변했을 경우, ViewPort를 재 생성한다.
		/// </summary>
		private void Reshape()
		{
			if(openGLControl1.InvokeRequired == true)
			{
				openGLControl1.Invoke(new MethodInvoker(delegate
				{
					Reshape();
				}));
			}
			else
			{
				var gl = openGLControl1.OpenGL;

				gl.Viewport(0, 0, this.openGLControl1.Width, this.openGLControl1.Height);

				//  Create an orthographic projection.
				gl.MatrixMode(MatrixMode.Projection);
				gl.LoadIdentity();

				_aspectRatio = ((float)this.openGLControl1.Width) / this.openGLControl1.Height;
				_xSpan = _zoom; // Feel free to change this to any xSpan you need.
				_ySpan = _zoom; // Feel free to change this to any ySpan you need.

				if (_aspectRatio > 1)
				{
					// Width > Height, so scale xSpan accordinly.
					_xSpan *= _aspectRatio;
				}
				else
				{
					// Height >= Width, so scale ySpan accordingly.
					_ySpan = _xSpan / _aspectRatio;
				}
				gl.Ortho2D(0, _xSpan, 0, _ySpan);

				//  Back to the modelview.
				gl.MatrixMode(MatrixMode.Modelview);
				gl.LoadIdentity();

				openGLControl1.Refresh();
			}
			
		}

		/// <summary>
		/// 컨트롤의 가로/세로 크기 중 작은 값을 반환한다.
		/// </summary>
		/// <returns></returns>
		private int GetControlMinSize()
		{
			return Math.Min(this.openGLControl1.Height, this.openGLControl1.Width);
		}

		/// <summary>
		/// (x,y)자리에 text를 출력한다.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="text"></param>
		protected void DrawText(int x, int y, string text)
		{
			if( openGLControl1.InvokeRequired == true)
			{
				openGLControl1.Invoke(new MethodInvoker(delegate
				{
					DrawText(x, y, text);
				}));
			}
			else
			{
				var gl = openGLControl1.OpenGL;

				var LocationX = (x + _offsetSize.Width) / _zoom * GetControlMinSize();
				var LocationY = -1 * (_offsetSize.Height - y) / _zoom * GetControlMinSize();
				gl.DrawText((int)LocationX, (int)LocationY, 0.0f, 256.0f, 0.0f, "Arial", 10, text);
			}
		}

		/// <summary>
		/// path의 이미지를 생성하여 내부 변수에 담는다.
		/// </summary>
		/// <param name="path"></param>
		protected void InitTexture(string path)
		{
			if (_texture.Id > 0)
				_texture.Destroy(openGLControl1.OpenGL);

			Bitmap textureImage = new Bitmap(path);
			// 이미지는 좌 상단이 0,0이고 아래로 갈수록 Y값이 커지지만, 여기서 사용하는 좌표계는 좌 하단이 0,0이고 위로갈수록 Y값이 커진다.
			// (Robot좌표계로 설계, gl.Translate(_offsetSize.Width, -_offsetSize.Height, -0f); 처럼 Y를 아래에서 위로 증가하게 설계함.
			// 그래서이미지가 위 아래가 거꾸로 표시되는걸 막기위해, 이미지를 Y축으로 변환한다.
			textureImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
			_texture.Create(openGLControl1.OpenGL, textureImage);
		}

		protected void DeleteTexture()
		{
			openGLControl1.OpenGL.DeleteTextures(_textureAreas.Count, _textureAreas.Select(x => x.Item1).ToArray());
			_textureAreas.Clear();
		}

		protected uint AddTexture(IntPtr data, int width, int height, uint bpp)
		{
			uint[] gtexture = new uint[1];

			openGLControl1.OpenGL.GenTextures(1, gtexture);
			openGLControl1.OpenGL.BindTexture(OpenGL.GL_TEXTURE_2D, gtexture[0]);

			openGLControl1.OpenGL.TexParameter(SharpGL.OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MIN_FILTER, SharpGL.OpenGL.GL_LINEAR);
			openGLControl1.OpenGL.TexParameter(SharpGL.OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MAG_FILTER, SharpGL.OpenGL.GL_NEAREST);

			int[] managedArray2 = new int[width * height * bpp / sizeof(int)];
			Marshal.Copy(data, managedArray2, 0, (int)(width * height * bpp / sizeof(int)));

			if (bpp == 3)
			{
				// for Color
				openGLControl1.OpenGL.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);
				openGLControl1.OpenGL.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB, width, height, 0,
				 SharpGL.OpenGL.GL_BGR,
				 OpenGL.GL_UNSIGNED_BYTE,
				 IntPtr.Zero
				 );
				//openGLControl1.OpenGL.TexSubImage2D(OpenGL.GL_TEXTURE_2D, 0, 0, 0, width, height, SharpGL.OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, managedArray2);
			}
			else if (bpp == 1)
			{
				// for Mono
				openGLControl1.OpenGL.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);
				openGLControl1.OpenGL.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_LUMINANCE, width, height, 0,
					OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
				//openGLControl1.OpenGL.TexSubImage2D(OpenGL.GL_TEXTURE_2D, 0, 0, 0, width, height, SharpGL.OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, managedArray2);
			}

			openGLControl1.OpenGL.GenerateMipmapEXT(OpenGL.GL_TEXTURE_2D);

			openGLControl1.OpenGL.BindTexture(1, gtexture[0]);

			return gtexture[0];
		}

		protected void IntiTexture(IntPtr data, int width, int height, uint bpp)
		{
			
		}

		protected void InitTexture(Bitmap textureImage)
		{
			uint[] gtexture = new uint[1];

			openGLControl1.OpenGL.GenTextures(1, gtexture);
			openGLControl1.OpenGL.BindTexture(OpenGL.GL_TEXTURE_2D, gtexture[0]);

			openGLControl1.OpenGL.TexParameter(SharpGL.OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MIN_FILTER, SharpGL.OpenGL.GL_LINEAR);
			openGLControl1.OpenGL.TexParameter(SharpGL.OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MAG_FILTER, SharpGL.OpenGL.GL_NEAREST);

			int[] managedArray2 = new int[width * height * bpp / sizeof(int)];
			Marshal.Copy(data, managedArray2, 0, (int)(width * height * bpp / sizeof(int)));

			if (bpp == 3)
			{
				// for Color
				openGLControl1.OpenGL.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);
				openGLControl1.OpenGL.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGB, width, height, 0,
				 SharpGL.OpenGL.GL_BGR,
				 OpenGL.GL_UNSIGNED_BYTE,
				 IntPtr.Zero
				 );
				//openGLControl1.OpenGL.TexSubImage2D(OpenGL.GL_TEXTURE_2D, 0, 0, 0, width, height, SharpGL.OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, managedArray2);
			}
			else if (bpp == 1)
			{
				// for Mono
				openGLControl1.OpenGL.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1);
				openGLControl1.OpenGL.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_LUMINANCE, width, height, 0,
					OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, IntPtr.Zero);
				//openGLControl1.OpenGL.TexSubImage2D(OpenGL.GL_TEXTURE_2D, 0, 0, 0, width, height, SharpGL.OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, managedArray2);
			}

		}



		protected void UpdateTexture(IntPtr data, int width, int height, uint bpp, uint textureId)
		{
			openGLControl1.OpenGL.BindTexture(SharpGL.OpenGL.GL_TEXTURE_2D, textureId);

			int[] managedArray2 = new int[width * height * bpp / sizeof(int)];
			Marshal.Copy(data, managedArray2, 0, (int)(width * height * bpp / sizeof(int)));

			if( bpp == 3)
			{
				// for color
				openGLControl1.OpenGL.TexSubImage2D(SharpGL.OpenGL.GL_TEXTURE_2D, 0, 0, 0, width, height, SharpGL.OpenGL.GL_BGR, OpenGL.GL_UNSIGNED_BYTE, managedArray2);
			}
			else if( bpp == 1)
			{
				// for mono
				openGLControl1.OpenGL.TexSubImage2D(SharpGL.OpenGL.GL_TEXTURE_2D, 0, 0, 0, width, height, SharpGL.OpenGL.GL_LUMINANCE, OpenGL.GL_UNSIGNED_BYTE, managedArray2);
			}
			
			openGLControl1.OpenGL.TexParameter(SharpGL.OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MIN_FILTER, SharpGL.OpenGL.GL_LINEAR);
			openGLControl1.OpenGL.TexParameter(SharpGL.OpenGL.GL_TEXTURE_2D, SharpGL.OpenGL.GL_TEXTURE_MAG_FILTER, SharpGL.OpenGL.GL_NEAREST);

			openGLControl1.OpenGL.GenerateMipmapEXT(SharpGL.OpenGL.GL_TEXTURE_2D);
			openGLControl1.OpenGL.BindTexture(SharpGL.OpenGL.GL_TEXTURE_2D, 0);

			openGLControl1.Refresh();
		}
		
		#endregion
		
		protected PointF PixelToRobot(float x, float y)
		{
			float robotX = (x * PixelSizeX + _textureArea.Left);
			float robotY = (y * PixelSizeY * -1 + _textureArea.Bottom);

			return new PointF(robotX, robotY);
		}

		protected void DrawCrossOfImage()
		{
			var gl = openGLControl1.OpenGL;

			PointF leftCenter = new PointF(_textureArea.Left, _textureArea.Bottom - _textureArea.Height / 2.0f);
			PointF rightCenter = new PointF(_textureArea.Right, _textureArea.Bottom - _textureArea.Height / 2.0f);

			PointF topCenter = new PointF(_textureArea.Left + _textureArea.Width/2.0f, _textureArea.Top);
			PointF bottomCenter = new PointF(_textureArea.Left + _textureArea.Width / 2.0f, _textureArea.Bottom);

			gl.Disable(OpenGL.GL_LINE_STIPPLE);
			gl.LineWidth(1.0f);
			gl.Begin(OpenGL.GL_LINES);               // Start Drawing The Pyramid
			{
				gl.Color(0.3f, 0.3f, 0.3f);         // Red	
				gl.Vertex(leftCenter.X, leftCenter.Y);
				gl.Vertex(rightCenter.X, rightCenter.Y);

				gl.Vertex(topCenter.X, topCenter.Y);
				gl.Vertex(bottomCenter.X, bottomCenter.Y);
			}
			gl.End();                  // Done Drawing The Pyramid
		}

	}
}
