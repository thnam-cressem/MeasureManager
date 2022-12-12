using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpGL;
using SharpGL.SceneGraph.Assets;
using DiagramControl.Models;
using DiagramControl.Model;

namespace DiagramControl
{
	public partial class DiagramViewer : PanningControl
	{
		public event DiagramDrawHandler Draw = delegate { };

		private List<DiagramInfoBase> _diagrams = new List<DiagramInfoBase>();
		
		public DiagramViewer()
		{
			InitializeComponent();
		}

		public List<DiagramInfoBase> Diagrams
		{
			get
			{
				return _diagrams;
			}
		}
		public void AddTexture(Bitmap textureImage, RectangleF textureArea)
		{
			if (textureImage == null)
				return;

			InitTexture(textureImage);
			_textureArea = textureArea;
		}
		
		public uint AddTexture(IntPtr data, int x, int y, int width, int height, int imageWidth, int imageHeight, uint bpp)
		{
			//IntiTexture(data, imageWidth, imageHeight, bpp);
			uint texture = AddTexture(data, imageWidth, imageHeight, bpp);
			_textureArea = new RectangleF(x, y, width, height);
			
			_textureAreas.Add(new Tuple<uint, RectangleF>(texture, _textureArea));

			return texture;
		}


		public void DeleteTextures()
		{
			DeleteTexture();
		}
		public void AddDiagram(DiagramInfoBase diagram)
		{
			_diagrams.Add(diagram);
		}

		public new void UpdateTexture(IntPtr data, int width, int height, uint bpp, uint textureId)
		{
			base.UpdateTexture(data, width, height, bpp, textureId);
		}

		public void ClearDiagram()
		{
			_diagrams.Clear();
		}

		protected override void OnLoad(EventArgs e)
		{
		}

		protected override void OnDraw(OpenGL gl)
		{
			// Draw Texture	
			gl.Enable(OpenGL.GL_TEXTURE_2D);
			{
				gl.Color(1.0f, 1.0f, 1.0f); //Must have, weirdness!

				foreach(var textureArea in _textureAreas)
				{
					gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureArea.Item1);

					for (int i = 0; i < 1; i++)
					{
						gl.Begin(OpenGL.GL_QUADS);
						{
							gl.TexCoord(0.0f, 1.0f); gl.Vertex(textureArea.Item2.Left, textureArea.Item2.Top);
							gl.TexCoord(0.0f, 0.0f); gl.Vertex(textureArea.Item2.Left, textureArea.Item2.Bottom);
							gl.TexCoord(1.0f, 0.0f); gl.Vertex(textureArea.Item2.Right, textureArea.Item2.Bottom);
							gl.TexCoord(1.0f, 1.0f); gl.Vertex(textureArea.Item2.Right, textureArea.Item2.Top);
						}
						gl.End();
					}
				}

				
			}
			gl.Disable(OpenGL.GL_TEXTURE_2D);
			gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);

			gl.LineWidth(1.0f);
			gl.Begin(OpenGL.GL_LINES);               // Start Drawing The Pyramid
			{
				Rectangle area = GetCurrentViewArea();
				var rects = _diagrams.Where(x => x.DiagramType == DiagramType.Rect);
				foreach(var diagram in rects)
				{
					gl.Color(diagram.LineColor[0], diagram.LineColor[1], diagram.LineColor[2]);

					if( diagram.LineType == LineType.Dot)
						gl.Enable(OpenGL.GL_LINE_STIPPLE);
					else
						gl.Disable(OpenGL.GL_LINE_STIPPLE);
					var array = diagram.DiagramDots.ToArray();
					for(int i = 0; i < array.Length; ++i)
					{
						gl.Vertex(array[i].X, array[i].Y);
						if( i < 3)
						{
							gl.Vertex(array[i + 1].X, array[i + 1].Y);
						}
						else
						{
							gl.Vertex(array[0].X, array[0].Y);
						}				
					}
					
				}
			}
			gl.End();

			Draw(this, gl);

			DrawCrossOfImage();
		}
		
		protected override void OnMouseDown(object sender, MouseEventArgs e)
		{
			base.OnMouseDown(sender, e);
		}

		public void DrawLine(OpenGL gl, LineInfo line)
		{
			if (line.LineColor.Count() != 3)
				return;

			gl.LineWidth(line.Width);
			gl.Color(line.LineColor);
			gl.Begin(OpenGL.GL_LINES);               // Start Drawing The Pyramid
			{
				gl.Vertex(line.StartDot.X, line.StartDot.Y);
				gl.Vertex(line.EndDot.X, line.EndDot.Y);
			}
			gl.End();                  // Done Drawing The Pyramid
		}

		public void DrawLine(OpenGL gl, IEnumerable<PointF> points, float lineWidth, float[] lineColorRGB)
		{
			gl.LineWidth(lineWidth);
			gl.Color(lineColorRGB);
			gl.Begin(OpenGL.GL_LINE_LOOP);
			{
				foreach(var pt in points)
				{
					gl.Vertex(pt.X, pt.Y);
				}
			}
			gl.End();
		}

		public void DrawLineLoopPx(OpenGL gl, IEnumerable<PointF> points, float lineWidth, float[] lineColorRGB)
		{
			gl.LineWidth(lineWidth);
			gl.Color(lineColorRGB);
			gl.Begin(OpenGL.GL_LINE_LOOP);               // Start Drawing The Pyramid
			{
				foreach (var pt in points.Select(x=>PixelToRobot(x.X, x.Y)))
				{
					gl.Vertex(pt.X, pt.Y);
				}
			}
			gl.End();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="gl"></param>
		/// <param name="cx">Center X of Rectangle</param>
		/// <param name="cy">Center Y of Rectangle</param>
		/// <param name="rx">Half of Rectangle Width</param>
		/// <param name="ry">Half of Rectangle Height</param>
		/// <param name="num_segments">Ellipse를 라인으로 분할해서 그릴 개수</param>
		/// <param name="lineWidth"></param>
		/// <param name="lineColorRGB"></param>
		public void DrawEllipse(OpenGL gl, float cx, float cy, float rx, float ry, int num_segments, float lineWidth, float[] lineColorRGB)
		{
			double theta = 2 * Math.PI / (double)num_segments;
			double c = Math.Cos(theta);//precalculate the sine and cosine
			double s = Math.Sin(theta);
			double t;

			double x = 1;//we start at angle = 0 
			double y = 0;

			gl.LineWidth(lineWidth);
			gl.Begin(OpenGL.GL_LINE_LOOP);
			for (int ii = 0; ii < num_segments; ii++)
			{
				//apply radius and offset
				gl.Vertex(x * rx + cx, y * ry + cy);//output vertex 

				//apply the rotation matrix
				t = x;
				x = c * x - s * y;
				y = s * t + c * y;
			}
			gl.End();
		}

	}
}
