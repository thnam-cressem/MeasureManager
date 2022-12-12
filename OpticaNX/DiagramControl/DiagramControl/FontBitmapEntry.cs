﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramControl
{
	/// <summary>
	/// A FontBitmap entry contains the details of a font face.
	/// </summary>
	internal class FontBitmapEntry
	{
		public IntPtr HDC
		{
			get;
			set;
		}

		public IntPtr HRC
		{
			get;
			set;
		}

		public string FaceName
		{
			get;
			set;
		}

		public int Height
		{
			get;
			set;
		}

		public uint ListBase
		{
			get;
			set;
		}

		public uint ListCount
		{
			get;
			set;
		}
	}
}
