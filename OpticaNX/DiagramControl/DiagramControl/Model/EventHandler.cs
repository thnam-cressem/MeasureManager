using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramControl.Model
{
	public delegate void MouseEventExHandler(object sender, MouseEventArgsEx e);

	public delegate void DiagramDrawHandler(object sender, OpenGL gl);

	public delegate void DiagramEventHandler<T>(object sender, T t);
}
