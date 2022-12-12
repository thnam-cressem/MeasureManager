using DiagramControl.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiagramControl.Model
{
	public class DiagramEventArg
	{
		private bool _handled = false;
		private IEnumerable<PointF> _robotPos = new List<PointF>();
		private IEnumerable<Point> _pixelPos = new List<Point>();

		public DiagramEventArg()
		{

		}


		public DiagramEventArg(IEnumerable<PointF> robotPos, IEnumerable<Point> pixelPos)
		{
			_robotPos = robotPos;
			_pixelPos = pixelPos;
		}

		public bool Handled => _handled;

		public IEnumerable<PointF> RobotPos
		{
			get
			{
				return _robotPos;
			}
			set
			{
				_robotPos = value;
			}
		}
		public IEnumerable<Point> PixelPos
		{
			get
			{
				return _pixelPos;
			}
			set
			{
				_pixelPos = value;

			}
		}

		public override string ToString()
		{
			return String.Format($"Robot : {String.Join(",", RobotPos.Select(x=>String.Format($"({x.X},{x.Y})")))}, PixelPos : {String.Join(",", PixelPos.Select(x => String.Format($"({x.X},{x.Y})")))}");
		}
	}

	public class MouseEventArgsEx : System.Windows.Forms.MouseEventArgs
	{
		private bool _handled = false;
		private PointF _robotPos;
		private Point _pxelPos;
		private Color _pixelColor;
		public MouseEventArgsEx(MouseButtons button, int clicks, int x, int y, int delta, float robotX, float robotY, int pixelX, int pixelY)
			: base(button, clicks, x, y, delta)
		{
			_robotPos = new PointF(robotX, robotY) ;
			_pxelPos = new Point(pixelX, pixelY);
		}

		public bool Handled
		{
			get
			{
				return _handled;
			}
			set
			{
				_handled = value;
			}
		}

		public PointF RobotPos
		{
			get
			{
				return _robotPos;
			}
		}

		public Point PixelPos
		{
			get
			{
				return _pxelPos;
			}
			set
			{
				_pxelPos = value;
			}
		}

		public Color PixelColor
		{
			get
			{
				return _pixelColor;
			}
			set
			{
				_pixelColor = value;
			}
		}
	}
}
