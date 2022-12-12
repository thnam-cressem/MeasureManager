using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Annotations;
using System.Xml.Serialization;

namespace DiagramControl.Models
{
	public enum LineType
	{
		Solid,
		Dot,
		ThickSolid
	}

	public enum DiagramType
	{
		Line,
		Rect,
		Ellipse
	}
	

	public interface DiagramInfoBase
	{
		DiagramType DiagramType { get; }
		LineType LineType { get; }
		float[] LineColor { get; }

		IEnumerable<DotInfo> DiagramDots { get; }

		bool IsLocked { get; set; }

		void UpdateDiagrams();
	}

	public class DotInfo : ICloneable, IEqualityComparer<DotInfo>
	{
		private PointF _dot = new PointF();

		public DotInfo()
		{

		}

		public DotInfo(float x, float y)
		{
			_dot.X = x;
			_dot.Y = y;
		}

		public virtual float X
		{
			get
			{
				return _dot.X;
			}
			set
			{
				_dot.X = value;
			}
		}

		public virtual float Y
		{
			get
			{
				return _dot.Y;
			}
			set
			{
				_dot.Y = value;
			}
		}

		public virtual object Clone()
		{
			var clone = new DotInfo(X, Y);
			return clone;
		}

		public virtual bool Equals(DotInfo other)
		{
			return this.Equals(other);
		}

		public virtual bool Equals(DotInfo x, DotInfo y)
		{
			return x.Equals(y);
		}

		public virtual int GetHashCode(DotInfo obj)
		{
			int hCode = obj.GetHashCode() ^ obj.GetHashCode();
			return hCode.GetHashCode();
		}

		public static bool operator ==(DotInfo lhs, DotInfo rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(DotInfo lhs, DotInfo rhs)
		{
			return !(lhs == rhs);
		}

	}

	public class LineInfo : DiagramInfoBase
	{
		private LineType _lineType;
		private DotInfo _startDot;
		private DotInfo _endDot;
		private float _width;
		private float[] _lineColor = new float[3] { 0.0f, 0.0f, 0.0f };
		public bool _isLocked = true;

		public LineInfo(DotInfo startDot, DotInfo endDot, LineType lineType, float width)
		{
			_startDot = startDot;
			_endDot = endDot;
			_lineType = lineType;
			_width = width;
		}

		public DiagramType DiagramType
		{
			get
			{
				return DiagramType.Line;
			}
		}

		public LineType LineType
		{
			get
			{
				return _lineType;
			}
			set
			{
				_lineType = value;
			}
		}

		public DotInfo StartDot
		{
			get
			{
				return _startDot;
			}
			set
			{
				_startDot = value;
			}
		}

		public DotInfo EndDot
		{
			get
			{
				return _endDot;
			}
			set
			{
				_endDot = value;
			}
		}

		public float[] LineColor
		{
			get
			{
				return _lineColor;
			}
			set
			{
				_lineColor = value;
			}
		}

		public float Width
		{
			get
			{
				return _width;
			}
			private set
			{
				_width = value;
			}
		}

		public bool IsLocked
		{
			get
			{
				return _isLocked;
			}
			set
			{
				_isLocked = value;
			}
		}

		public IEnumerable<DotInfo> DiagramDots
		{
			get
			{
				return new List<DotInfo>() { _startDot, _endDot };
			}
		}

		public void UpdateDiagrams()
		{

		}
	}

	[Serializable]
	[XmlRoot("Rect")]
	public class RectInfo : DiagramInfoBase
	{
		private LineType _lineType;
		private float[] _lineColor = new float[3] { 0.0f, 0.0f, 0.0f };

		private DotInfo _leftTop = new DotInfo();
		private DotInfo _leftBottom = new DotInfo();
		private DotInfo _rightTop = new DotInfo();
		private DotInfo _rightBottom = new DotInfo();

		public bool _isLocked = true;

		public RectInfo()
		{

		}

		public RectInfo(DotInfo lt, DotInfo lb, DotInfo rt, DotInfo rb, LineType lineType)
		{
			_lineType = lineType;
			_leftTop = lt;
			_leftBottom = lb;
			_rightTop = rt;
			_rightBottom = rb;
		}

		[XmlIgnore]
		public virtual DiagramType DiagramType
		{
			get
			{
				return DiagramType.Rect;
			}

		}

		[XmlIgnore]
		public virtual LineType LineType
		{
			get
			{
				return _lineType;
			}
			set
			{
				_lineType = value;
			}
		}

		[XmlIgnore]
		public virtual float[] LineColor
		{
			get
			{
				return _lineColor;
			}
			set
			{
				_lineColor = value;
			}
		}

		[XmlIgnore]
		public virtual IEnumerable<DotInfo> DiagramDots
		{
			get
			{
				return new List<DotInfo>() { _leftTop, _leftBottom, _rightBottom, _rightTop };
			}
		}

		[XmlIgnore]
		public virtual bool IsLocked
		{
			get
			{
				return _isLocked;
			}
			set
			{
				_isLocked = value;
			}
		}

		public void UpdateDiagrams()
		{

		}

		public static RectInfo From(float x, float y, float radius, float[] rgb)
		{
			DotInfo lt = new DotInfo(x - radius, y + radius);
			DotInfo lb = new DotInfo(x - radius, y - radius);
			DotInfo rt = new DotInfo(x + radius, y + radius);
			DotInfo rb = new DotInfo(x + radius, y - radius);

			RectInfo r = new RectInfo(lt, lb, rt, rb, LineType.Solid);
			r.LineColor = rgb;

			return r;
		}

		public static RectInfo From(float x, float y, float width, float height, float[] rgb)
		{
			DotInfo lt = new DotInfo(x, y);
			DotInfo lb = new DotInfo(x, y - height);
			DotInfo rt = new DotInfo(x + width, y);
			DotInfo rb = new DotInfo(x + width, y - height);

			RectInfo r = new RectInfo(lt, lb, rt, rb, LineType.Solid);
			r.LineColor = rgb;
			return r;
		}

		public static RectInfo From(RectangleF rect, float[] rgb)
		{
			return From(rect.X, rect.Y, rect.Width, rect.Height, rgb);
		}

		
	}

}