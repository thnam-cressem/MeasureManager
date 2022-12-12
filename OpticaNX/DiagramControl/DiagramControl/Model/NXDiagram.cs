using DiagramControl.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DiagramControl.Model
{
	public enum LineOverType
	{
		None,
		HSplit,		// 수평 크기 변경
		VSplit,		// 수직 크기 변경
		SizeNWSE,	// 우하에서 좌상으로 가로세로 크기 변경
		SizeNESE,	// 좌하에서 우상으로 가로세로 크기 변경
		Move2D,     // Shape 이동
	}

	public enum EditingType
	{
		None,
		Left,
		Top,
		Right,
		Bottom,
	}

	[Serializable]
	[XmlRoot("NXPoint")]
	public class NXPoint<T> : ICloneable, IEqualityComparer<NXPoint<T>>
	{
		public NXPoint()
		{

		}

		public NXPoint(T x, T y)
		{
			X = x;
			Y = y;
		}

		[XmlAttribute("X")]
		public T X { get; set; }

		[XmlAttribute("Y")]
		public T Y { get; set; }


		public override string ToString()
		{
			return String.Format($"({X},{Y})");
		}

		public virtual object Clone()
		{
			var clone = new NXPoint<T>(X, Y);
			return clone;
		}

		public virtual bool Equals(NXPoint<T> other)
		{
			return this.Equals(other);
		}

		public virtual bool Equals(NXPoint<T> x, NXPoint<T> y)
		{
			return x.Equals(y);
		}

		public virtual int GetHashCode(NXPoint<T> obj)
		{
			int hCode = obj.X.GetHashCode() ^ obj.Y.GetHashCode();
			return hCode.GetHashCode();
		}

		private PointF? _pointF;
		private Point? _point;
		public PointF ToPointF()
		{
			//if (_pointF.HasValue == true)
			//	return _pointF.Value;

			_pointF = new PointF() { X = Convert.ToSingle(X), Y = Convert.ToSingle(Y) };

			return _pointF.Value;
		}
		public Point ToPoint()
		{
			if (_point.HasValue == true)
				return _point.Value;

			_point = new Point() { X = (int)Convert.ToSingle(X), Y = (int)Convert.ToSingle(Y) };

			return _point.Value;
		}
		public static bool operator ==(NXPoint<T> lhs, NXPoint<T> rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(NXPoint<T> lhs, NXPoint<T> rhs)
		{
			return !(lhs == rhs);
		}

		public void Move(NXSize<T> size)
		{
			if (typeof(T) == typeof(int))
			{
				X = (T)((object)Convert.ToInt32(((X as int?).GetValueOrDefault() - (size.Width as int?).GetValueOrDefault())));
				Y = (T)((object)Convert.ToInt32(((Y as int?).GetValueOrDefault() - (size.Height as int?).GetValueOrDefault())));
			}
			else if (typeof(T) == typeof(float))
			{
				X = (T)((object)Convert.ToSingle(((X as int?).GetValueOrDefault() - (size.Width as int?).GetValueOrDefault())));
				Y = (T)((object)Convert.ToSingle(((Y as int?).GetValueOrDefault() - (size.Height as int?).GetValueOrDefault())));
			}
			else if (typeof(T) == typeof(double))
			{
				X = (T)((object)Convert.ToDouble(((X as int?).GetValueOrDefault() - (size.Width as int?).GetValueOrDefault())));
				Y = (T)((object)Convert.ToDouble(((Y as int?).GetValueOrDefault() - (size.Height as int?).GetValueOrDefault())));
			}
		}

		private readonly float ExtendSize = 20.0f;
		//public NXRect<T> GetExtend(NXSize<T> size)
		//{
		//	// 새로운 객체 복사
		//	var cloned = this.Clone() as NXPoint<T>;

		//	// 원한는 만큼 이동
		//	cloned.Move(size);

		//	var leftTop = cloned.Clone() as NXPoint<T>;
		//	leftTop.Move(new NXSize<T>((T)Convert.ChangeType((ExtendSize * -1), typeof(T)), (T)Convert.ChangeType((ExtendSize * +1), typeof(T))));

		//	var rightTop = cloned.Clone() as NXPoint<T>;
		//	rightTop.Move(new NXSize<T>((T)Convert.ChangeType((ExtendSize * +1), typeof(T)), (T)Convert.ChangeType((ExtendSize * +1), typeof(T))));

		//	var rightBottom = cloned.Clone() as NXPoint<T>;
		//	rightBottom.Move(new NXSize<T>((T)Convert.ChangeType((ExtendSize * +1), typeof(T)), (T)Convert.ChangeType((ExtendSize * -1), typeof(T))));

		//	var leftBottom = cloned.Clone() as NXPoint<T>;
		//	leftBottom.Move(new NXSize<T>((T)Convert.ChangeType((ExtendSize * -1), typeof(T)), (T)Convert.ChangeType((ExtendSize * -1), typeof(T))));

		//	NXRect<T> result = new NXRect<T>(leftTop, rightTop, rightBottom, leftBottom);
		//	return result;
		//}
	}

	[Serializable]
	[XmlRoot("NXPoint")]
	public class NXSize<T> : ICloneable, IEqualityComparer<NXSize<T>>
	{
		public NXSize()
		{

		}

		public NXSize(T width, T height)
		{
			Width = width;
			Height = height;
		}

		[XmlAttribute("Width")]
		public T Width { get; set; }

		[XmlAttribute("Height")]
		public T Height { get; set; }

		public object Clone()
		{
			var clone = new NXSize<T>(Width, Height);
			return clone;
		}

		public bool Equals(NXSize<T> other)
		{
			return this.Equals(other);
		}

		public bool Equals(NXSize<T> x, NXSize<T> y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(NXSize<T> obj)
		{
			int hCode = obj.Width.GetHashCode() ^ obj.Height.GetHashCode();
			return hCode.GetHashCode();
		}

		public static bool operator ==(NXSize<T> lhs, NXSize<T> rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(NXSize<T> lhs, NXSize<T> rhs)
		{
			return !(lhs == rhs);
		}
	}

	[Serializable]
	[XmlRoot("Rect")]
	public class NXRect<T> : IEqualityComparer<NXRect<T>>, DiagramInfoBase
	{
		public event DiagramEventHandler<NXRect<T>> NXRectChanged = delegate { };

		private readonly float MOUSE_SELECTION_WIDTH = 5.0f;

		#region Private Variables

		private NXPoint<T>[] _points = new NXPoint<T>[4] { new NXPoint<T>(), new NXPoint<T>(), new NXPoint<T>(), new NXPoint<T>() };
		private T _width;
		private T _height;
		private NXPoint<T> _leftTop;
		private NXPoint<T> _rightTop;
		private NXPoint<T> _leftBottom;
		private NXPoint<T> _rightBottom;
		private NXPoint<T> _center;

		private T _left;
		private T _top;
		private T _right;
		private T _bottom;

		private NXPoint<T> _centerOfLeft;
		private NXPoint<T> _centerOfTop;
		private NXPoint<T> _centerOfRight;
		private NXPoint<T> _centerOfBottom;

		private bool _isLocked = true;

		private EditingType _editingType = EditingType.None;
		private bool _isEditing = false;

		#endregion

		public NXRect()
		{

		}

		public NXRect(NXPoint<T> pt1, NXPoint<T> pt2, NXPoint<T> pt3, NXPoint<T> pt4)
		{
			_points[0] = pt1;
			_points[1] = pt2;
			_points[2] = pt3;
			_points[3] = pt4;

			Normalize();
		}

		#region Properties

		[XmlIgnore]
		public NXPoint<T> this[int i]
		{
			get
			{
				return _points[i];
			}
			set
			{
				_points[i] = value;
			}
		}

		[XmlArray("Points")]
		[XmlArrayItem("Point")]
		public NXPoint<T>[] Points
		{
			get
			{
				return _points;
			}
			set
			{
				_points = value;
				Normalize();
			}
		}

		[XmlIgnore]
		public NXPoint<T> LeftTop
		{
			get
			{
				return _leftTop;
			}
		}

		[XmlIgnore]
		public NXPoint<T> LeftBottom
		{
			get
			{
				return _leftBottom;
			}
		}

		[XmlIgnore]
		public NXPoint<T> RightTop
		{
			get
			{
				return _rightTop;
			}
		}

		[XmlIgnore]
		public NXPoint<T> RightBottom
		{
			get
			{
				return _rightBottom;
			}
		}

		[XmlIgnore]
		public T Width
		{
			get { return _width; }
		}

		[XmlIgnore]
		public T Height
		{
			get { return _height; }
		}

		[XmlIgnore]
		public T Left
		{
			get
			{
				return _left;
			}
			set
			{
				_left = value;
				//NormalizePoints();
			}
		}

		[XmlIgnore]
		public T Top
		{
			get
			{
				return _top;
			}
			set
			{
				_top = value;
				//NormalizePoints();
			}
		}

		[XmlIgnore]
		public T Right
		{
			get
			{
				return _right;
			}
			set
			{
				_right = value;
				//NormalizePoints();
			}
		}

		[XmlIgnore]
		public T Bottom
		{
			get
			{
				return _bottom;
			}
			set
			{
				_bottom = value;
				//NormalizePoints();
			}
		}

		[XmlIgnore]
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

		[XmlIgnore]
		public NXPoint<T> Center
		{
			get
			{
				return _center;
			}
		}


		//[XmlAttribute("DiagramType")]
		[XmlIgnore]
		public DiagramType DiagramType => DiagramType.Rect;

		//[XmlAttribute("LineType")]
		[XmlIgnore]
		public LineType LineType => LineType.Solid;

		[XmlIgnore]
		public float[] LineColor => new float[] { 255.0f, 255.0f, 255.0f };

		private IEnumerable<DotInfo> _diamgramDots;
		[XmlIgnore]
		public IEnumerable<DotInfo> DiagramDots
		{
			get
			{
				_diamgramDots = Points.Select(x => new DotInfo((float)(object)x.X, (float)(object)x.Y));
				return _diamgramDots;
			}
		}

		[XmlIgnore]
		public bool IsEditing
		{
			get
			{
				return _isEditing;
			}
			set
			{
				_isEditing = value;
			}
		}

		[XmlIgnore]
		public EditingType EditingType
		{
			get
			{
				return _editingType;
			}
		}

		#endregion

		public void UpdateDiagrams()
		{

		}

		public bool Contain(T x, T y)
		{
			if (Comparer<T>.Default.Compare(_leftTop.X, x) > 0)
				return false;

			if (Comparer<T>.Default.Compare(_rightTop.X, x) < 0)
				return false;

			if (Comparer<T>.Default.Compare(_leftTop.Y, y) < 0)
				return false;

			if (Comparer<T>.Default.Compare(_rightBottom.Y, y) > 0)
				return false;

			return true;
		}

		public LineOverType IsOnTheLine(T x, T y)
		{
			if (((Convert.ToSingle(_leftTop.Y) - MOUSE_SELECTION_WIDTH) < Convert.ToSingle(y) && (Convert.ToSingle(_leftTop.Y) + MOUSE_SELECTION_WIDTH) > Convert.ToSingle(y)))
			{
				_editingType = EditingType.Top;
				return LineOverType.HSplit;
			}
				
			if (((Convert.ToSingle(_rightBottom.Y) - MOUSE_SELECTION_WIDTH) < Convert.ToSingle(y) && (Convert.ToSingle(_rightBottom.Y) + MOUSE_SELECTION_WIDTH) > Convert.ToSingle(y)))
			{
				_editingType = EditingType.Bottom;
				return LineOverType.HSplit;
			}
				
			if (((Convert.ToSingle(_leftTop.X) - MOUSE_SELECTION_WIDTH) < Convert.ToSingle(x) && (Convert.ToSingle(_leftTop.X) + MOUSE_SELECTION_WIDTH) > Convert.ToSingle(x)))
			{
				_editingType = EditingType.Left;
				return LineOverType.VSplit;
			}
			
			if (((Convert.ToSingle(_rightBottom.X) - MOUSE_SELECTION_WIDTH) < Convert.ToSingle(x) && (Convert.ToSingle(_rightBottom.X) + MOUSE_SELECTION_WIDTH) > Convert.ToSingle(x)))
			{
				_editingType = EditingType.Right;
				return LineOverType.VSplit;
			}

			IsEditing = false;
			_editingType = EditingType.None;

			return LineOverType.None;
		}

		private void NormalizePoints()
		{
			_points[0].X = Left;
			_points[0].Y = Top;

			_points[1].X = Right;
			_points[1].Y = Top;

			_points[2].X = Right;
			_points[2].Y = Bottom;

			_points[3].X = Left;
			_points[3].Y = Bottom;

			Normalize();
		}

		private void Normalize()
		{
			var maxX = _points.Max(x => x.X);
			var minX = _points.Min(x => x.X);

			var maxY = _points.Max(x => x.Y);
			var minY = _points.Min(x => x.Y);

			var ordered = _points.OrderBy(x => x.X).ThenBy(x => x.Y).ToArray();
			_leftBottom = ordered[0];
			_leftTop = ordered[1];
			_rightBottom = ordered[2];
			_rightTop = ordered[3];

			if (typeof(T) == typeof(int))
			{
				_width = (T)((object)Convert.ToInt32(((maxX as int?).GetValueOrDefault() - (minX as int?).GetValueOrDefault())));
				_height = (T)((object)Convert.ToInt32(((maxY as int?).GetValueOrDefault() - (minY as int?).GetValueOrDefault())));

				T x = (T)((object)Convert.ToInt32(((_leftBottom.X as int?).GetValueOrDefault() + (_width as int?).GetValueOrDefault() / 2)));
				T y = (T)((object)Convert.ToInt32(((_leftBottom.Y as int?).GetValueOrDefault() + (_height as int?).GetValueOrDefault() / 2)));
				_center = new NXPoint<T>(x, y);

				_centerOfLeft = (_center.Clone() as NXPoint<T>);
				_centerOfLeft.Move(new NXSize<T>((T)(object)(Convert.ToInt32(_width) / 2 * -1), (T)(object)Convert.ToInt32(0)));

				_centerOfTop = (_center.Clone() as NXPoint<T>);
				_centerOfTop.Move(new NXSize<T>((T)(object)(Convert.ToInt32(0)), (T)(object)(Convert.ToInt32(_height) / 2 * +1)));

				_centerOfRight = (_center.Clone() as NXPoint<T>);
				_centerOfRight.Move(new NXSize<T>((T)(object)(Convert.ToInt32(_width) / 2 * +1), (T)(object)Convert.ToInt32(0)));

				_centerOfBottom = (_center.Clone() as NXPoint<T>);
				_centerOfBottom.Move(new NXSize<T>((T)(object)(Convert.ToInt32(0)), (T)(object)(Convert.ToInt32(_height) / 2 * -1)));

				//_centerOfLeft = _center.MoveAndExtend(new NXSize<T>((T)(object)(Convert.ToInt32(_width) / 2 * -1), (T)(object)0));
				//_centerOfTop = _center.MoveAndExtend(new NXSize<T>((T)(object)0, (T)(object)(Convert.ToInt32(_height) / 2)));
				//_centerOfRight = _center.MoveAndExtend(new NXSize<T>((T)(object)(Convert.ToInt32(_width) / 2), (T)(object)0));
				//_centerOfBottom = _center.MoveAndExtend(new NXSize<T>((T)(object)0, (T)(object)(Convert.ToInt32(_height) / 2 * -1)));
			}
			else if (typeof(T) == typeof(float))
			{
				_width = (T)((object)Convert.ToSingle(((maxX as float?).GetValueOrDefault() - (minX as float?).GetValueOrDefault())));
				_height = (T)((object)Convert.ToSingle(((maxY as float?).GetValueOrDefault() - (minY as float?).GetValueOrDefault())));

				T x = (T)((object)Convert.ToSingle(((_leftBottom.X as float?).GetValueOrDefault() + (_width as float?).GetValueOrDefault() / 2)));
				T y = (T)((object)Convert.ToSingle(((_leftBottom.Y as float?).GetValueOrDefault() + (_height as float?).GetValueOrDefault() / 2)));
				_center = new NXPoint<T>(x, y);

				_centerOfLeft = (_center.Clone() as NXPoint<T>);
				_centerOfLeft.Move(new NXSize<T>((T)(object)(Convert.ToSingle(_width) / 2 * -1), (T)(object)Convert.ToSingle(0)));

				_centerOfTop = (_center.Clone() as NXPoint<T>);
				_centerOfTop.Move(new NXSize<T>((T)(object)(Convert.ToSingle(0)), (T)(object)(Convert.ToSingle(_height) / 2 * +1)));

				_centerOfRight = (_center.Clone() as NXPoint<T>);
				_centerOfRight.Move(new NXSize<T>((T)(object)(Convert.ToSingle(_width) / 2 * +1), (T)(object)Convert.ToSingle(0)));

				_centerOfBottom = (_center.Clone() as NXPoint<T>);
				_centerOfBottom.Move(new NXSize<T>((T)(object)(Convert.ToSingle(0)), (T)(object)(Convert.ToSingle(_height) / 2 * -1)));

				//_centerOfLeft = _center.MoveAndExtend(new NXSize<T>((T)(object)(Convert.ToSingle(_width) / 2 * -1), (T)(object)0));
				//_centerOfTop = _center.MoveAndExtend(new NXSize<T>((T)(object)0, (T)(object)(Convert.ToSingle(_height) / 2)));
				//_centerOfRight = _center.MoveAndExtend(new NXSize<T>((T)(object)(Convert.ToSingle(_width) / 2), (T)(object)0));
				//_centerOfBottom = _center.MoveAndExtend(new NXSize<T>((T)(object)0, (T)(object)(Convert.ToSingle(_height) / 2 * -1)));
			}
			else if (typeof(T) == typeof(double))
			{
				_width = (T)((object)Convert.ToDouble(((maxX as double?).GetValueOrDefault() - (minX as double?).GetValueOrDefault())));
				_height = (T)((object)Convert.ToDouble(((maxY as double?).GetValueOrDefault() - (minY as double?).GetValueOrDefault())));

				T x = (T)((object)Convert.ToDouble(((_leftBottom.X as double?).GetValueOrDefault() + (_width as double?).GetValueOrDefault() / 2)));
				T y = (T)((object)Convert.ToDouble(((_leftBottom.Y as double?).GetValueOrDefault() + (_height as double?).GetValueOrDefault() / 2)));
				_center = new NXPoint<T>(x, y);

				_centerOfLeft = (_center.Clone() as NXPoint<T>);
				_centerOfLeft.Move(new NXSize<T>((T)(object)(Convert.ToDouble(_width) / 2 * -1), (T)(object)Convert.ToDouble(0)));

				_centerOfTop = (_center.Clone() as NXPoint<T>);
				_centerOfTop.Move(new NXSize<T>((T)(object)(Convert.ToDouble(0)), (T)(object)(Convert.ToDouble(_height) / 2 * +1)));

				_centerOfRight = (_center.Clone() as NXPoint<T>);
				_centerOfRight.Move(new NXSize<T>((T)(object)(Convert.ToDouble(_width) / 2 * +1), (T)(object)Convert.ToDouble(0)));

				_centerOfBottom = (_center.Clone() as NXPoint<T>);
				_centerOfBottom.Move(new NXSize<T>((T)(object)(Convert.ToDouble(0)), (T)(object)(Convert.ToDouble(_height) / 2 * -1)));

				//_centerOfLeft = _center.MoveAndExtend(new NXSize<T>((T)(object)(Convert.ToDouble(_width) / 2 * -1), (T)(object)0));
				//_centerOfTop = _center.MoveAndExtend(new NXSize<T>((T)(object)0, (T)(object)(Convert.ToDouble(_height) / 2)));
				//_centerOfRight = _center.MoveAndExtend(new NXSize<T>((T)(object)(Convert.ToDouble(_width) / 2), (T)(object)0));
				//_centerOfBottom = _center.MoveAndExtend(new NXSize<T>((T)(object)0, (T)(object)(Convert.ToDouble(_height) / 2 * -1)));
			}

			_left = _leftBottom.X;
			_top = _leftTop.Y;
			_right = _rightBottom.X;
			_bottom = _rightBottom.Y;

		}

		public override string ToString()
		{
			return String.Format($"LT({_leftTop}),RT({_rightTop}),LB({_leftBottom}),RB({_rightBottom})");
		}

		public object Clone()
		{
			var clone = new NXRect<T>(_points[0], _points[1], _points[2], _points[3]);
			clone.Normalize();
			return clone;
		}

		public bool Equals(NXRect<T> other)
		{
			return this.Equals(other);
		}

		public bool Equals(NXRect<T> x, NXRect<T> y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(NXRect<T> obj)
		{
			int hCode = obj.Width.GetHashCode() ^ obj.Height.GetHashCode() ^
				obj.LeftBottom.GetHashCode() ^ obj.RightBottom.GetHashCode() ^
				obj.LeftTop.GetHashCode() ^ obj.RightTop.GetHashCode();
			return hCode.GetHashCode();
		}

		public bool IsEmpty()
		{
			if (Width.Equals(0.0f) && Height.Equals(0.0f))
				return true;
			return false;
		}

		public RectangleF ToRectangleF()
		{
			RectangleF rect = new RectangleF(Convert.ToSingle(Left), Convert.ToSingle(Bottom), Convert.ToSingle(Width), Convert.ToSingle(Height));
			return rect;
		}

		public Rectangle ToRectangle()
		{
			Rectangle rect = new Rectangle(Convert.ToInt32(Left), Convert.ToInt32(Bottom), Convert.ToInt32(Width), Convert.ToInt32(Height));
			return rect;
		}

		public void OffsetMove(NXSize<T> size)
		{
			foreach (var point in _points)
			{
				if (size.Width is int)
				{
					point.X = (T)Convert.ChangeType(Convert.ToInt32(point.X) + Convert.ToInt32(size.Width), typeof(T));
					point.Y = (T)Convert.ChangeType(Convert.ToInt32(point.Y) + Convert.ToInt32(size.Height), typeof(T));
				}
				else if (size.Width is float)
				{
					point.X = (T)Convert.ChangeType(Convert.ToSingle(point.X) + Convert.ToSingle(size.Width), typeof(T));
					point.Y = (T)Convert.ChangeType(Convert.ToSingle(point.Y) + Convert.ToSingle(size.Height), typeof(T));
				}
				else if (size.Width is double)
				{
					point.X = (T)Convert.ChangeType(Convert.ToDouble(point.X) + Convert.ToDouble(size.Width), typeof(T));
					point.Y = (T)Convert.ChangeType(Convert.ToDouble(point.Y) + Convert.ToDouble(size.Height), typeof(T));
				}
			}
			Normalize();
			NXRectChanged(this, this);
		}

		public void Move(float posX, float posY)
		{
			if (_editingType == EditingType.None)
				return;

			switch (_editingType)
			{
				case EditingType.Left:
					_points[0].X = (T)Convert.ChangeType(posX, typeof(T));
					_points[3].X = (T)Convert.ChangeType(posX, typeof(T));
					break;
				case EditingType.Top:
					_points[0].Y = (T)Convert.ChangeType(posY, typeof(T));
					_points[1].Y = (T)Convert.ChangeType(posY, typeof(T));
					break;
				case EditingType.Right:
					_points[1].X = (T)Convert.ChangeType(posX, typeof(T));
					_points[2].X = (T)Convert.ChangeType(posX, typeof(T));
					break;
				case EditingType.Bottom:
					_points[2].Y = (T)Convert.ChangeType(posY, typeof(T));
					_points[3].Y = (T)Convert.ChangeType(posY, typeof(T));
					break;
			}
			Normalize();
			NXRectChanged(this, this);
		}

		public static bool operator ==(NXRect<T> lhs, NXRect<T> rhs)
		{
			if (lhs is null)
				return false;

			if (rhs is null)
				return false;

			return lhs.Equals(rhs);
		}

		public static bool operator !=(NXRect<T> lhs, NXRect<T> rhs)
		{
			return !(lhs is null && rhs is null);
		}
	}

	[Serializable]
	[XmlRoot("Rect")]
	public class PXRect<T> : IEqualityComparer<PXRect<T>>, DiagramInfoBase
	{
		private readonly float MOUSE_SELECTION_WIDTH = 5.0f;

		#region Private Variables

		private NXPoint<T>[] _points = new NXPoint<T>[4] { new NXPoint<T>(), new NXPoint<T>(), new NXPoint<T>(), new NXPoint<T>() };
		private T _width;
		private T _height;
		private NXPoint<T> _leftTop;
		private NXPoint<T> _rightTop;
		private NXPoint<T> _leftBottom;
		private NXPoint<T> _rightBottom;
		private NXPoint<T> _center;

		private T _left;
		private T _top;
		private T _right;
		private T _bottom;

		private NXPoint<T> _centerOfLeft;
		private NXPoint<T> _centerOfTop;
		private NXPoint<T> _centerOfRight;
		private NXPoint<T> _centerOfBottom;

		private bool _isLocked = true;

		private EditingType _editingType = EditingType.None;
		private bool _isEditing = false;

		#endregion

		public PXRect()
		{

		}

		public PXRect(NXPoint<T> pt1, NXPoint<T> pt2, NXPoint<T> pt3, NXPoint<T> pt4)
		{
			_points[0] = pt1;
			_points[1] = pt2;
			_points[2] = pt3;
			_points[3] = pt4;

			Normalize();
		}

		#region Properties

		[XmlIgnore]
		public NXPoint<T> this[int i]
		{
			get
			{
				return _points[i];
			}
			set
			{
				_points[i] = value;
			}
		}

		[XmlArray("Points")]
		[XmlArrayItem("Point")]
		public NXPoint<T>[] Points
		{
			get
			{
				return _points;
			}
			set
			{
				_points = value;
				Normalize();
			}
		}

		[XmlIgnore]
		public NXPoint<T> LeftTop
		{
			get
			{
				return _leftTop;
			}
		}

		[XmlIgnore]
		public NXPoint<T> LeftBottom
		{
			get
			{
				return _leftBottom;
			}
		}

		[XmlIgnore]
		public NXPoint<T> RightTop
		{
			get
			{
				return _rightTop;
			}
		}

		[XmlIgnore]
		public NXPoint<T> RightBottom
		{
			get
			{
				return _rightBottom;
			}
		}

		[XmlIgnore]
		public T Width
		{
			get { return _width; }
		}

		[XmlIgnore]
		public T Height
		{
			get { return _height; }
		}

		[XmlIgnore]
		public T Left
		{
			get
			{
				return _left;
			}
			set
			{
				_left = value;
				//NormalizePoints();
			}
		}

		[XmlIgnore]
		public T Top
		{
			get
			{
				return _top;
			}
			set
			{
				_top = value;
				//NormalizePoints();
			}
		}

		[XmlIgnore]
		public T Right
		{
			get
			{
				return _right;
			}
			set
			{
				_right = value;
				//NormalizePoints();
			}
		}

		[XmlIgnore]
		public T Bottom
		{
			get
			{
				return _bottom;
			}
			set
			{
				_bottom = value;
				//NormalizePoints();
			}
		}

		[XmlIgnore]
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

		[XmlIgnore]
		public NXPoint<T> Center
		{
			get
			{
				return _center;
			}
		}


		public NXPoint<T> CenterOfLeft
		{
			get
			{
				return _centerOfLeft;
			}
		}
		public NXPoint<T> CenterOfTop
		{
			get
			{
				return _centerOfTop;
			}
		}
		public NXPoint<T> CenterOfRight
		{
			get
			{
				return _centerOfRight;
			}
		}
		public NXPoint<T> CenterOfBottom
		{
			get
			{
				return _centerOfBottom;
			}
		}


		//[XmlAttribute("DiagramType")]
		[XmlIgnore]
		public DiagramType DiagramType => DiagramType.Rect;

		//[XmlAttribute("LineType")]
		[XmlIgnore]
		public LineType LineType => LineType.Solid;

		[XmlIgnore]
		public float[] LineColor => new float[] { 255.0f, 255.0f, 255.0f };

		private IEnumerable<DotInfo> _diamgramDots;
		[XmlIgnore]
		public IEnumerable<DotInfo> DiagramDots
		{
			get
			{
				_diamgramDots = Points.Select(x => new DotInfo((float)(object)x.X, (float)(object)x.Y));
				return _diamgramDots;
			}
		}

		[XmlIgnore]
		public bool IsEditing
		{
			get
			{
				return _isEditing;
			}
			set
			{
				_isEditing = value;
			}
		}

		[XmlIgnore]
		public EditingType EditingType
		{
			get
			{
				return _editingType;
			}
		}

		#endregion

		public void UpdateDiagrams()
		{

		}

		public bool Contain(T x, T y)
		{
			if (Comparer<T>.Default.Compare(_leftTop.X, x) > 0)
				return false;

			if (Comparer<T>.Default.Compare(_rightTop.X, x) < 0)
				return false;

			if (Comparer<T>.Default.Compare(_leftTop.Y, y) < 0)
				return false;

			if (Comparer<T>.Default.Compare(_rightBottom.Y, y) > 0)
				return false;

			return true;
		}

		public LineOverType IsOnTheLine(T x, T y)
		{
			if (((Convert.ToSingle(Top) - MOUSE_SELECTION_WIDTH) < Convert.ToSingle(y) && (Convert.ToSingle(Top) + MOUSE_SELECTION_WIDTH) > Convert.ToSingle(y)))
			{
				_editingType = EditingType.Top;
				return LineOverType.HSplit;
			}

			if (((Convert.ToSingle(Bottom) - MOUSE_SELECTION_WIDTH) < Convert.ToSingle(y) && (Convert.ToSingle(Bottom) + MOUSE_SELECTION_WIDTH) > Convert.ToSingle(y)))
			{
				_editingType = EditingType.Bottom;
				return LineOverType.HSplit;
			}

			if (((Convert.ToSingle(Left) - MOUSE_SELECTION_WIDTH) < Convert.ToSingle(x) && (Convert.ToSingle(Left) + MOUSE_SELECTION_WIDTH) > Convert.ToSingle(x)))
			{
				_editingType = EditingType.Left;
				return LineOverType.VSplit;
			}

			if (((Convert.ToSingle(Right) - MOUSE_SELECTION_WIDTH) < Convert.ToSingle(x) && (Convert.ToSingle(Right) + MOUSE_SELECTION_WIDTH) > Convert.ToSingle(x)))
			{
				_editingType = EditingType.Right;
				return LineOverType.VSplit;
			}

			IsEditing = false;
			_editingType = EditingType.None;

			return LineOverType.None;
		}

		private void NormalizePoints()
		{
			_points[0].X = Left;
			_points[0].Y = Top;

			_points[1].X = Right;
			_points[1].Y = Top;

			_points[2].X = Right;
			_points[2].Y = Bottom;

			_points[3].X = Left;
			_points[3].Y = Bottom;

			Normalize();
		}

		private void Normalize()
		{
			var maxX = _points.Max(x => x.X);
			var minX = _points.Min(x => x.X);

			var maxY = _points.Max(x => x.Y);
			var minY = _points.Min(x => x.Y);

			var ordered = _points.OrderBy(x => x.X).ThenByDescending(x => x.Y).ToArray();
			_leftBottom = ordered[0];
			_leftTop = ordered[1];
			_rightBottom = ordered[2];
			_rightTop = ordered[3];

			if (typeof(T) == typeof(int))
			{
				_width = (T)((object)Convert.ToInt32(((maxX as int?).GetValueOrDefault() - (minX as int?).GetValueOrDefault())));
				_height = (T)((object)Convert.ToInt32(((maxY as int?).GetValueOrDefault() - (minY as int?).GetValueOrDefault())));

				T x = (T)((object)Convert.ToInt32(((_leftBottom.X as int?).GetValueOrDefault() + (_width as int?).GetValueOrDefault() / 2)));
				T y = (T)((object)Convert.ToInt32(((_leftBottom.Y as int?).GetValueOrDefault() + (_height as int?).GetValueOrDefault() / 2)));
				_center = new NXPoint<T>(x, y);

				_centerOfLeft = (_center.Clone() as NXPoint<T>);
				_centerOfLeft.Move(new NXSize<T>((T)(object)(Convert.ToInt32(_width) / 2 * -1), (T)(object)Convert.ToInt32(0)));

				_centerOfTop = (_center.Clone() as NXPoint<T>);
				_centerOfTop.Move(new NXSize<T>((T)(object)(Convert.ToInt32(0)), (T)(object)(Convert.ToInt32(_height) / 2 * +1)));

				_centerOfRight = (_center.Clone() as NXPoint<T>);
				_centerOfRight.Move(new NXSize<T>((T)(object)(Convert.ToInt32(_width) / 2 * +1), (T)(object)Convert.ToInt32(0)));

				_centerOfBottom = (_center.Clone() as NXPoint<T>);
				_centerOfBottom.Move(new NXSize<T>((T)(object)(Convert.ToInt32(0)), (T)(object)(Convert.ToInt32(_height) / 2 * -1)));

				//_centerOfLeft = _center.MoveAndExtend(new NXSize<T>((T)(object)(Convert.ToInt32(_width) / 2 * -1), (T)(object)0));
				//_centerOfTop = _center.MoveAndExtend(new NXSize<T>((T)(object)0, (T)(object)(Convert.ToInt32(_height) / 2)));
				//_centerOfRight = _center.MoveAndExtend(new NXSize<T>((T)(object)(Convert.ToInt32(_width) / 2), (T)(object)0));
				//_centerOfBottom = _center.MoveAndExtend(new NXSize<T>((T)(object)0, (T)(object)(Convert.ToInt32(_height) / 2 * -1)));
			}
			else if (typeof(T) == typeof(float))
			{
				_width = (T)((object)Convert.ToSingle(((maxX as float?).GetValueOrDefault() - (minX as float?).GetValueOrDefault())));
				_height = (T)((object)Convert.ToSingle(((maxY as float?).GetValueOrDefault() - (minY as float?).GetValueOrDefault())));

				T x = (T)((object)Convert.ToSingle(((_leftBottom.X as float?).GetValueOrDefault() + (_width as float?).GetValueOrDefault() / 2)));
				T y = (T)((object)Convert.ToSingle(((_leftBottom.Y as float?).GetValueOrDefault() + (_height as float?).GetValueOrDefault() / 2)));
				_center = new NXPoint<T>(x, y);

				_centerOfLeft = (_center.Clone() as NXPoint<T>);
				_centerOfLeft.Move(new NXSize<T>((T)(object)(Convert.ToSingle(_width) / 2 * -1), (T)(object)Convert.ToSingle(0)));

				_centerOfTop = (_center.Clone() as NXPoint<T>);
				_centerOfTop.Move(new NXSize<T>((T)(object)(Convert.ToSingle(0)), (T)(object)(Convert.ToSingle(_height) / 2 * +1)));

				_centerOfRight = (_center.Clone() as NXPoint<T>);
				_centerOfRight.Move(new NXSize<T>((T)(object)(Convert.ToSingle(_width) / 2 * +1), (T)(object)Convert.ToSingle(0)));

				_centerOfBottom = (_center.Clone() as NXPoint<T>);
				_centerOfBottom.Move(new NXSize<T>((T)(object)(Convert.ToSingle(0)), (T)(object)(Convert.ToSingle(_height) / 2 * -1)));

				//_centerOfLeft = _center.MoveAndExtend(new NXSize<T>((T)(object)(Convert.ToSingle(_width) / 2 * -1), (T)(object)0));
				//_centerOfTop = _center.MoveAndExtend(new NXSize<T>((T)(object)0, (T)(object)(Convert.ToSingle(_height) / 2)));
				//_centerOfRight = _center.MoveAndExtend(new NXSize<T>((T)(object)(Convert.ToSingle(_width) / 2), (T)(object)0));
				//_centerOfBottom = _center.MoveAndExtend(new NXSize<T>((T)(object)0, (T)(object)(Convert.ToSingle(_height) / 2 * -1)));
			}
			else if (typeof(T) == typeof(double))
			{
				_width = (T)((object)Convert.ToDouble(((maxX as double?).GetValueOrDefault() - (minX as double?).GetValueOrDefault())));
				_height = (T)((object)Convert.ToDouble(((maxY as double?).GetValueOrDefault() - (minY as double?).GetValueOrDefault())));

				T x = (T)((object)Convert.ToDouble(((_leftBottom.X as double?).GetValueOrDefault() + (_width as double?).GetValueOrDefault() / 2)));
				T y = (T)((object)Convert.ToDouble(((_leftBottom.Y as double?).GetValueOrDefault() + (_height as double?).GetValueOrDefault() / 2)));
				_center = new NXPoint<T>(x, y);

				_centerOfLeft = (_center.Clone() as NXPoint<T>);
				_centerOfLeft.Move(new NXSize<T>((T)(object)(Convert.ToDouble(_width) / 2 * -1), (T)(object)Convert.ToDouble(0)));

				_centerOfTop = (_center.Clone() as NXPoint<T>);
				_centerOfTop.Move(new NXSize<T>((T)(object)(Convert.ToDouble(0)), (T)(object)(Convert.ToDouble(_height) / 2 * +1)));

				_centerOfRight = (_center.Clone() as NXPoint<T>);
				_centerOfRight.Move(new NXSize<T>((T)(object)(Convert.ToDouble(_width) / 2 * +1), (T)(object)Convert.ToDouble(0)));

				_centerOfBottom = (_center.Clone() as NXPoint<T>);
				_centerOfBottom.Move(new NXSize<T>((T)(object)(Convert.ToDouble(0)), (T)(object)(Convert.ToDouble(_height) / 2 * -1)));

				//_centerOfLeft = _center.MoveAndExtend(new NXSize<T>((T)(object)(Convert.ToDouble(_width) / 2 * -1), (T)(object)0));
				//_centerOfTop = _center.MoveAndExtend(new NXSize<T>((T)(object)0, (T)(object)(Convert.ToDouble(_height) / 2)));
				//_centerOfRight = _center.MoveAndExtend(new NXSize<T>((T)(object)(Convert.ToDouble(_width) / 2), (T)(object)0));
				//_centerOfBottom = _center.MoveAndExtend(new NXSize<T>((T)(object)0, (T)(object)(Convert.ToDouble(_height) / 2 * -1)));
			}

			_left = _leftBottom.X;
			_top = _leftTop.Y;
			_right = _rightBottom.X;
			_bottom = _rightBottom.Y;
		}

		public override string ToString()
		{
			return String.Format($"LT({_leftTop}),RT({_rightTop}),LB({_leftBottom}),RB({_rightBottom})");
		}

		public object Clone()
		{
			var clone = new PXRect<T>(_points[0], _points[1], _points[2], _points[3]);
			clone.Normalize();
			return clone;
		}

		public bool Equals(PXRect<T> other)
		{
			return this.Equals(other);
		}

		public bool Equals(PXRect<T> x, PXRect<T> y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(PXRect<T> obj)
		{
			int hCode = obj.Width.GetHashCode() ^ obj.Height.GetHashCode() ^
				obj.LeftBottom.GetHashCode() ^ obj.RightBottom.GetHashCode() ^
				obj.LeftTop.GetHashCode() ^ obj.RightTop.GetHashCode();
			return hCode.GetHashCode();
		}

		public bool IsEmpty()
		{
			if (Width.Equals(0.0f) && Height.Equals(0.0f))
				return true;
			return false;
		}

		public RectangleF ToRectangleF()
		{
			RectangleF rect = new RectangleF(Convert.ToSingle(Left), Convert.ToSingle(Bottom), Convert.ToSingle(Width), Convert.ToSingle(Height));
			return rect;
		}

		public Rectangle ToRectangle()
		{
			Rectangle rect = new Rectangle(Convert.ToInt32(Left), Convert.ToInt32(Top), Convert.ToInt32(Width), Convert.ToInt32(Height));
			return rect;
		}

		public void OffsetMove(NXSize<T> size)
		{
			foreach (var point in _points)
			{
				if (size.Width is int)
				{
					point.X = (T)Convert.ChangeType(Convert.ToInt32(point.X) + Convert.ToInt32(size.Width), typeof(T));
					point.Y = (T)Convert.ChangeType(Convert.ToInt32(point.Y) + Convert.ToInt32(size.Height), typeof(T));
				}
				else if (size.Width is float)
				{
					point.X = (T)Convert.ChangeType(Convert.ToSingle(point.X) + Convert.ToSingle(size.Width), typeof(T));
					point.Y = (T)Convert.ChangeType(Convert.ToSingle(point.Y) + Convert.ToSingle(size.Height), typeof(T));
				}
				else if (size.Width is double)
				{
					point.X = (T)Convert.ChangeType(Convert.ToDouble(point.X) + Convert.ToDouble(size.Width), typeof(T));
					point.Y = (T)Convert.ChangeType(Convert.ToDouble(point.Y) + Convert.ToDouble(size.Height), typeof(T));
				}
			}
			Normalize();
		}

		public void Move(float posX, float posY)
		{
			if (_editingType == EditingType.None)
				return;

			switch (_editingType)
			{
				case EditingType.Left:
					_points[0].X = (T)Convert.ChangeType(posX, typeof(T));
					_points[3].X = (T)Convert.ChangeType(posX, typeof(T));
					break;
				case EditingType.Top:
					_points[0].Y = (T)Convert.ChangeType(posY, typeof(T));
					_points[1].Y = (T)Convert.ChangeType(posY, typeof(T));
					break;
				case EditingType.Right:
					_points[1].X = (T)Convert.ChangeType(posX, typeof(T));
					_points[2].X = (T)Convert.ChangeType(posX, typeof(T));
					break;
				case EditingType.Bottom:
					_points[2].Y = (T)Convert.ChangeType(posY, typeof(T));
					_points[3].Y = (T)Convert.ChangeType(posY, typeof(T));
					break;
			}
			Normalize();

		}

		public static bool operator ==(PXRect<T> lhs, PXRect<T> rhs)
		{
			if (lhs is null)
				return false;

			if (rhs is null)
				return false;

			return lhs.Equals(rhs);
		}

		public static bool operator !=(PXRect<T> lhs, PXRect<T> rhs)
		{
			return !(lhs is null && rhs is null);
		}
	}
}
