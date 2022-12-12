﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Cressem.Util.Helpers
{
	public class XmlHelper
	{
		/// <summary>
		/// Loads a XML file from a specified path
		/// </summary>
		/// <param name="path">File path to load</param>
		/// <returns>XML element</returns>
		public static XElement LoadLinqXml(string path)
		{
			XElement doc = null;

			// Make sure existance of the file
			if (File.Exists(path) == false)
			{
				throw new Exception("No xml file found.");
			}

			try
			{
				using (StreamReader xr = new StreamReader(path))
				{
					doc = XElement.Load(xr);
				}
			}
			catch (FileNotFoundException ffe)
			{
				throw ffe;
			}
			catch (DirectoryNotFoundException dnfe)
			{
				throw dnfe;
			}
			catch (IOException ioe)
			{
				throw ioe;
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return doc;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static XmlDocument LoadReadOnlyXml(string path)
		{
			XmlDocument doc;

			if (File.Exists(path) == false)
				throw new Exception("No xml file found.");

			using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				doc = new XmlDocument();
				doc.Load(stream);
			}

			return doc;
		}

		/// <summary>
		/// Loads XmlDocument from a xml string
		/// </summary>
		/// <param name="xml">xml string</param>
		/// <returns></returns>
		public static XmlDocument LoadXml(string xml)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			return doc;
		}

		/// <summary>
		/// Converts the content of XmlDocument to string
		/// </summary>
		/// <param name="doc">XmlDocument to convert</param>
		/// <returns>String content of XmlDocument</returns>
		public static string XmlToString(XmlDocument doc)
		{
			string txt = null;

			if (doc == null)
				return null;

			using (StringWriter sw = new StringWriter())
			{
				using (XmlTextWriter xtw = new XmlTextWriter(sw))
				{
					doc.WriteTo(xtw);
					txt = sw.ToString();
				}
			}

			return txt;
		}

		/// <summary>
		/// Indicates whether a node has a specified attribute.
		/// </summary>
		/// <param name="xelmt"></param>
		/// <param name="attrName"></param>
		/// <returns></returns>
		public static bool HasAttribute(XElement xelmt, string attrName)
		{
			if (xelmt.Attribute(attrName) != null)
				return true;

			return false;
		}

		/// <summary>
		/// Indicates whether a node contains a specified node.
		/// </summary>
		/// <param name="xelmt"></param>
		/// <param name="elmtName"></param>
		/// <returns></returns>
		public static bool HasElement(XElement xelmt, string elmtName)
		{
			if (xelmt.Element(elmtName) != null)
				return true;

			return false;
		}

		#region Serialization

		/// <summary>
		/// Loads serialized xml file as an object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fileName"></param>
		/// <returns></returns>
		[CLSCompliant(false)]
		public static T LoadXML<T>(string fileName)
		{
			try
			{
				// In case of non-existence of the file, create a new file
				if (File.Exists(fileName) == false)
					SaveXml<T>(fileName, (T)Activator.CreateInstance(typeof(T)));

				// In case of a zero size file, delete it first and recreate it
				FileInfo file = new FileInfo(fileName);
				if (file.Length <= 0)
				{
					file.Delete();
					SaveXml<T>(fileName, (T)Activator.CreateInstance(typeof(T)));
				}

				using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					// Restore data from a XML document
					XmlSerializer serializer = new XmlSerializer(typeof(T));

					return (T)serializer.Deserialize(stream);
				}
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Saves an object to a serialized xml file
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fileName"></param>
		/// <param name="obj"></param>
		public static void SaveXml<T>(string fileName, T obj)
		{
			using (Stream stream = new FileStream(fileName, FileMode.Create))
			{
				try
				{
					XmlSerializer serializer = new XmlSerializer(typeof(T));
					serializer.Serialize(stream, obj);
				}
				catch
				{
					throw;
				}
			}
		}

		#endregion

		/// <summary>
		/// 객체를 Linq XML문서형태로 변환하여 반환한다.
		/// </summary>
		/// <typeparam name="T">변환하고 자하는 객체의 타입</typeparam>
		/// <param name="obj">변환하고 자하는 객체</param>
		/// <returns>Linq XML문서</returns>
		public static XDocument ObjectToLinqXmlDoc<T>(T obj)
		{
			string xmlString = ObjectToXmlString<T>(obj);

			return XDocument.Parse(xmlString);
		}

		/// <summary>
		/// 객체를 XML형태로 변환하여 반환한다.
		/// </summary>
		/// <typeparam name="T">변환하고 자하는 객체의 타입</typeparam>
		/// <param name="obj">변환하고 자하는 객체</param>
		/// <returns>XML</returns>
		public static string ObjectToXmlString<T>(T obj)
		{
			string xmlString = null;

			XmlSerializer serializer = new XmlSerializer(typeof(T));
			using (StringWriter writer = new StringWriter())
			{
				serializer.Serialize(writer, obj);
				writer.Close();

				xmlString = writer.ToString();
			}

			return xmlString;
		}

		/// <summary>
		/// XML 이 주어진 스키마 규칙에 부합하는 지 여부를 판단한다.
		/// </summary>
		/// <param name="xmlString">XML</param>
		/// <param name="schemaString">스키마</param>
		/// <param name="targetNamespace">XML 네임스페이스</param>
		/// <returns>부합하면 <c>true</c>, 그렇지 않으면 <c>false</c></returns>
		public static bool ValidateXml(string xmlString, string schemaString, string targetNamespace = null)
		{
			bool isValid = true;

			var stringReader = new StringReader(schemaString);
			XmlReader xmlReader = XmlReader.Create(stringReader);

			var schemas = new XmlSchemaSet();
			schemas.Add("tempuri", xmlReader);

			var xmlDoc = XDocument.Parse(xmlString);

			xmlDoc.Validate(schemas, (s, e) =>
			{
				isValid = false;
			});

			return isValid;
		}
	}
}
