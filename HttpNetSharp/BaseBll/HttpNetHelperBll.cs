
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpNetHelper.Base;
using System.Drawing;
using System.IO;
using HttpNetHelper.Enum;
using HttpNetHelper.Helper;
using HttpNetHelper;

namespace HttpNetHelper.BaseBll
{
	/// <summary>
	/// 具体实现方法  
	/// </summary>
	internal class HttpHelperBll
	{
		/// <summary>
		/// Httphelper原始访问类对象
		/// </summary>
		private HttpNetHelperBase httpbase = new HttpNetHelperBase();
		/// <summary>
		/// 根据相传入的数据，得到相应页面数据
		/// </summary>
		/// <param name="item">参数类对象</param>
		/// <returns>返回HttpResult类型</returns>
		internal HttpResult GetHtml(HttpItem item)
		{
			if (item.Allowautoredirect && item.AutoRedirectCookie)
			{
				HttpResult result = null;
				for (int i = 0; i <= 99; i++)
				{
					item.Allowautoredirect = false;
					result = httpbase.GetHtml(item);
					if (string.IsNullOrWhiteSpace(result.RedirectUrl))
					{
						break;
					}
					else
					{
						item.URL = result.RedirectUrl;
						item.Method = "GET";
						if (item.ResultCookieType == ResultCookieType.String)
						{
							item.Cookie = HttpCookieHelper.MergerCookies(item.Cookie, HttpCookieHelper.GetSmallCookie(result.Cookie));
						}
						else if (item.ResultCookieType == ResultCookieType.CookieCollection)
						{
							item.CookieCollection = HttpCookieHelper.MergerCookies(item.CookieCollection, result.CookieCollection);
						}
						else
						{
							if (result.CookieCollection != null && result.CookieCollection.Count > 0)
							{
								item.CookieContainer.Add(new Uri(item.URL), result.CookieCollection);
							}
						}
					}
				}
				return result;
			}
			return httpbase.GetHtml(item);
		}
		/// <summary>
		/// 根据Url获取图片
		/// </summary>
		/// <param name="item">参数类对象</param>
		/// <returns>返回图片</returns>
		internal Image GetImage(HttpItem item)
		{
			item.ResultType = ResultType.Byte;
			byte[] pic = GetHtml(item).ResultByte;
			item.ResultType = ResultType.String;
			return ImageHelper.ByteToImage(pic);
		}
		/// <summary>
		/// 快速Post数据这个访求与GetHtml一样，只是不接收返回数据，只做提交。
		/// </summary>
		/// <param name="item">参数类对象</param>
		/// <returns>返回HttpResult类型</returns>
		internal HttpResult FastRequest(HttpItem item)
		{
			return httpbase.FastRequest(item);
		}

	}
}
