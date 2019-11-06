
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

using System;
using System.IO;
using System.Drawing;

namespace HttpNetHelper.Helper
{
	internal class ImageHelper
	{
		/// <summary>
		/// 将字节数组转为图片
		/// </summary>
		/// <param name=" b">字节数组</param>
		/// <returns>返回图片</returns>
		internal static Image ByteToImage(byte[] b)
		{
			try
			{
				MemoryStream ms = new MemoryStream(b);
				return Bitmap.FromStream(ms, true);
			}
			catch
			{
				return null;
			}
		}
	}
}
