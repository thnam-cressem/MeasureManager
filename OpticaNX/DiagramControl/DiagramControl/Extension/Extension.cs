using DiagramControl.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiagramControl.Extension
{
	public static class Extension
	{
		/// <summary>
		/// pt1과 pt2로 Rect용 4개 Point반환
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		/// <returns></returns>
		public static IEnumerable<PointF> ToPointF(this Point pt1, Point pt2)
		{
			List<PointF> points = new List<PointF>();

			points.Add(new PointF(Math.Min(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y)));
			points.Add(new PointF(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y)));
			points.Add(new PointF(Math.Max(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y)));
			points.Add(new PointF(Math.Max(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y)));

			return points;
		}

		/// <summary>
		/// Points를 LeftTop/RighTop/RightBottom/LeftBottom순으로 반환한다.
		/// </summary>
		/// <param name="points"></param>
		/// <returns></returns>
		public static IEnumerable<DotInfo> OrderByLTRB(this IEnumerable<DotInfo> points)
		{
			return points.OrderByDescending(x => x.Y).OrderBy(x => x.X);
		}
	}
}
