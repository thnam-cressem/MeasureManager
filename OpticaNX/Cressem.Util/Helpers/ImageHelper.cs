using Cressem.Util.Generics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cressem.Util.Helpers
{
	public class ImageHelper
	{
		/// <summary>
		/// Converts a byte array to a bitmap image
		/// </summary>
		/// <param name="byteArray">Byte array to convert</param>
		/// <returns>Bitmap image</returns>
		public static Bitmap BytesToBmp(byte[] byteArray)
		{
			//Bitmap bitmap;
			//using (MemoryStream ms = new MemoryStream(byteArray))
			//{
			//   bitmap = new Bitmap(ms);				
			//}
			// return bitmap;

			MemoryStream ms = new MemoryStream(byteArray);
			return new Bitmap(ms);
		}

		/// <summary>
		/// Converts an image object to a byte array
		/// </summary>
		/// <param name="img">Image to convert</param>
		/// <returns>Byte array</returns>
		public static byte[] ImageToByteArray(Image img)
		{
			if (img == null)
				return null;

            ImageConverter converter = new ImageConverter();
			byte[] array = (byte[])converter.ConvertTo(img, typeof(byte[]));
			return array;
		}

		/// <summary>
		/// Coverts a byte array to an image object
		/// </summary>
		/// <param name="byteArray">Byte array to convert</param>
		/// <returns>Image object</returns>
		public static Image ByteArrayToImage(byte[] byteArray)
		{
			MemoryStream ms = new MemoryStream(byteArray, false);
			return Image.FromStream(ms, true);

			//using (MemoryStream ms = new MemoryStream(byteArray, 0, byteArray.Length))
			//{
			//   ms.Write(byteArray, 0, byteArray.Length);
			//   Bitmap bitmap = new Bitmap(ms);
			//   return bitmap;
			//}

			//using (MemoryStream ms = new MemoryStream(byteArray))
			//{				
			//   //Bitmap bitmap = new Bitmap(ms);
			//   return new Bitmap(ms);
			//}

			//using (MemoryTributary mt = new MemoryTributary(byteArray))
			//{
			//   return new Bitmap(mt);
			//}

			//return Image.FromStream(ms, true);			
		}

		/// <summary>
		/// Opens a file in a filestream from a specified path
		/// and reads its data in a byte array.
		/// </summary>
		/// <param name="path">File path to read</param>
		/// <returns>Byte array</returns>
		public static byte[] ReadFile(string path)
		{
			byte[] data = null;

			// Use FileInfo object to get file size.
			FileInfo fileInfo = new FileInfo(path);
			long size = fileInfo.Length;

			// Open FileStream to read file
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				BinaryReader reader = new BinaryReader(fs);
				data = reader.ReadBytes((int)size);
				reader.Dispose();
			}

			return data;
		}

		/// <summary>
		/// Reads an image from a specified path.
		/// </summary>
		/// <param name="path">Image path to read</param>
		/// <returns>Image object</returns>
		public static Image ReadImageFile(string path)
		{
			if (File.Exists(path) == false)
				return null;

			return ByteArrayToImage(ReadFile(path));
		}

		/// <summary>
		/// Reads an image from a specified path and resizes it if area of the image 
		/// is larger than the specified maxArea.
		/// </summary>
		/// <param name="path">Image path to read</param>
		/// <param name="maxWidth">Max. allowed width of image</param>
		/// <param name="maxHeight">Max. allowed height of image</param>
		/// <param name="maxArea">Max. allowed area of image</param>
		/// <returns>System.Drawing.Image</returns>
		public static Image ReadImageFileAfterResize(string path, int maxWidth, int maxHeight, int maxArea)
		{
			if (File.Exists(path) == false)
				return null;

			Size size = ImageHeaderHelper.GetDimensions(path);
			int area = size.Width * size.Height;
			if (area <= maxArea)
				return ByteArrayToImage(ReadFile(path));

			return ResizeIf(path, maxWidth, maxHeight);
		}

		/// <summary>
		/// Gets a byte array from a column of DataReader. The database column must be in binary format.
		/// </summary>
		/// <param name="dataReader">DataReader to fetch data</param>
		/// <param name="columnName">Name of column to read</param>
		/// <returns>A byte array</returns>
		public static byte[] GetByteArrayFromDataReader(IDataReader dataReader, string columnName)
		{
			// 사용 메모리량 크기 문제로 주석처리 [2016.03.08 jwlee3]
			//byte[] byteArray = null;

			//using (MemoryStream ms = new MemoryStream())
			//{
			//	byte[] buffer = new byte[1024];
			//	int index = 0;
			//	while (true)
			//	{
			//		long count = dataReader.GetBytes(dataReader.GetOrdinal(columnName), index, buffer, 0, buffer.Length);
			//		if (count == 0)
			//		{
			//			break;
			//		}
			//		else
			//		{
			//			index += (int)count;
			//			ms.Write(buffer, 0, (int)count);
			//		}
			//	}

			//	byteArray = ms.ToArray();
			//}

			//return byteArray;

			return dataReader[columnName] as byte[];
		}

		/// <summary>
		/// Gets an Image object from a column of DataReader. The database column must be in binary format.
		/// </summary>
		/// <param name="dataReader">DataReader to fetch data</param>
		/// <param name="columnName">Name of column to read</param>
		/// <returns>A byte array</returns>
		public static Image GetImageFromDataReader(IDataReader dataReader, string columnName)
		{
			return ByteArrayToImage(GetByteArrayFromDataReader(dataReader, columnName));
		}

		/// <summary>
		/// Resize an image if width or height of image is larger than specified max value.
		/// </summary>
		/// <param name="path">Image path to read</param>
		/// <param name="maxWidth">Max. allowed width of image</param>
		/// <param name="maxHeight">Max. allowed height of image</param>
		/// <returns>System.Drawing.Bitmap</returns>
		public static Image ResizeIf(string path, int maxWidth, int maxHeight)
		{
			Bitmap bitmap = null;

			using (System.Drawing.Image img = System.Drawing.Image.FromFile(path))
			{
				int w = (maxWidth > 0) ? maxWidth : img.Width;
				int h = (maxHeight > 0) ? maxHeight : img.Height;
				double scaleWidth = (double)w / (double)img.Width;
				double scaleHeight = (double)h / (double)img.Height;
				double scale = (scaleHeight < scaleWidth) ? scaleHeight : scaleWidth;
				scale = (scale > 1) ? 1 : scale;

				int newWidth = (int)(scale * img.Width);
				int newHeight = (int)(scale * img.Height);

				bitmap = new Bitmap(newWidth, newHeight);

				using (Graphics g = Graphics.FromImage(bitmap))
				{
					g.CompositingQuality = CompositingQuality.HighQuality;
					g.SmoothingMode = SmoothingMode.HighQuality;
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					g.PixelOffsetMode = PixelOffsetMode.HighQuality;
					g.DrawImage(img, new Rectangle(0, 0, newWidth, newHeight));
					g.Save();
				}
			}
			return bitmap;
		}
	}
}
