using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiagramControl
{
	public delegate void DiagramControlPaintEvent(object sender, Graphics g);

	//public delegate void SelectedPackageEvent(object sender, PackageInfo packageInfo);

	
	public class TKMouseEventArgs : MouseEventArgs
	{
		private bool _handled = false;
		public TKMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
			: base(button, clicks, x, y, delta)
		{

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
		public PointF RobotLocation { get; set; }

		public Point PixelLocation { get; set; }
	}
}
