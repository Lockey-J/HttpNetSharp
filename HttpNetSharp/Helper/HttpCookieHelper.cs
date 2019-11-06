
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Net;

namespace HttpNetHelper.Helper
{



	/// <summary>
	/// Cookie操作帮助类 
	/// </summary>
	internal class HttpCookieHelper
	{
		/// <summary>
		/// 根据字符生成Cookie和精简串，将排除path,expires,domain以及重复项
		/// </summary>
		/// <param name="strcookie">Cookie字符串</param>
		/// <returns>精简串</returns>
		internal static string GetSmallCookie(string strcookie)
		{
			if (string.IsNullOrWhiteSpace(strcookie))
			{
				return string.Empty;
			}
			List<string> cookielist = new List<string>();
			//将Cookie字符串以,;分开，生成一个字符数组，并删除里面的空项
			string[] list = strcookie.ToString().Split(new string[] {",", ";"}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string item in list)
			{
				string itemcookie = item.ToLower().Trim().Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty);
				//排除空字符串
				if (string.IsNullOrWhiteSpace(itemcookie))
				{
					continue;
				}
				//排除不存在=号的Cookie项
				if (!itemcookie.Contains("="))
				{
					continue;
				}
				//排除path项
				if (itemcookie.Contains("path="))
				{
					continue;
				}
				//排除expires项
				if (itemcookie.Contains("expires="))
				{
					continue;
				}
				//排除domain项
				if (itemcookie.Contains("domain="))
				{
					continue;
				}
				if (itemcookie.Contains("max-age="))
				{
					continue;
				}
				//排除重复项
				if (cookielist.Contains(item))
				{
					continue;
				}

				//对接Cookie基本的Key和Value串
				cookielist.Add(string.Format("{0};", item));
			}
			return string.Join(";", cookielist);
		}
		/// <summary>
		/// 将字符串Cookie转为CookieCollection
		/// </summary>
		/// <param name="strcookie">Cookie字符串</param>
		/// <returns>List-CookieItem</returns>
		internal static CookieCollection StrCookieToCookieCollection(string strcookie)
		{
			//排除空字符串
			if (string.IsNullOrWhiteSpace(strcookie))
			{
				return null;
			}
			CookieCollection cookielist = new CookieCollection();
			//先简化Cookie
			strcookie = GetSmallCookie(strcookie);
			//将Cookie字符串以,;分开，生成一个字符数组，并删除里面的空项
			string[] list = strcookie.ToString().Split(new string[] {";"}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string item in list)
			{
				string[] cookie = item.ToString().Split(new string[] {"="}, StringSplitOptions.RemoveEmptyEntries);
				if (cookie.Length == 2)
				{
					cookielist.Add(new Cookie()
					{
						Name = cookie[0].Trim(),
						Value = cookie[1].Trim()
					});
				}
			}
			return cookielist;
		}
		/// <summary>
		/// 将CookieCollection转为字符串Cookie
		/// </summary>
		/// <param name="cookie">Cookie字符串</param>
		/// <returns>strcookie</returns>
		internal static string CookieCollectionToStrCookie(CookieCollection cookie)
		{
			string result = null;
			if (cookie == null)
			{
				result = string.Empty;
			}
			else
			{
				string text = string.Empty;
				foreach (Cookie cookie2 in cookie)
				{
					if (!(cookie2.Value.ToLower().Contains("deleted")))
					{
						text += string.Format("{0}={1};", cookie2.Name, cookie2.Value);
					}

				}
				result = text;
			}
			return result.TrimEnd(';');
		}

		/// <summary>
		/// 自动合并两个Cookie的值返回更新后结果 
		/// </summary>
		/// <param name="OldCookie">Cookie1</param>
		/// <param name="NewCookie">Cookie2</param>
		/// <returns>返回更新后的Cookie</returns>
		internal static string MergerCookies(string OldCookie, string NewCookie)
		{
			if (string.IsNullOrWhiteSpace(NewCookie))
			{
				return OldCookie;
			}

			if (string.IsNullOrWhiteSpace(OldCookie))
			{
				List<string> OnlyNew = new List<string>(NewCookie.Split(';'));
				foreach (string nCookie in OnlyNew)
				{
					if (nCookie.Split('=')[1].ToString().ToLower().Contains("deleted"))
					{
						OnlyNew.Remove(nCookie);
					}
				}
				return string.Join(";", OnlyNew.ToArray()).TrimEnd(';');
			}
			List<string> Old = new List<string>(OldCookie.Split(';'));
			List<string> New = new List<string>(NewCookie.Split(';'));
			foreach (string n in New)
			{
				foreach (string o in Old)
				{
					if (o == n || o.Split('=')[0] == n.Split('=')[0])
					{
						Old.Remove(o);
						if (n.Split('=')[1].ToString().ToLower().Contains("deleted")) //如果新cookie表明要deleted，就在删除旧cookie的情况上把新指明删除的cookie名一起删除。
						{
							New.Remove(n);
						}
						break;
					}
				}
			}
			List<string> list = new List<string>(Old);
			list.AddRange(New);
			return string.Join(";", list.ToArray()).TrimEnd(';');

		}
		internal static CookieCollection MergerCookies(CookieCollection OldCookie, CookieCollection NewCookie)
		{
			if (OldCookie == null)
			{
				return NewCookie;
			}

			if (NewCookie == null)
			{
				return OldCookie;
			}

			OldCookie.Add(NewCookie);
			CookieCollection mFinalCookieCoolection = new CookieCollection();

			foreach (Cookie mCookie in OldCookie)
			{
				if (!mCookie.Value.ToLower().Equals("deleted"))
				{
					mFinalCookieCoolection.Add(mCookie);
				}
			}
			return mFinalCookieCoolection;

		}


	}


}