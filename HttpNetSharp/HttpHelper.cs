using System.Net;
using HttpNetHelper.BaseBll;
using System.Drawing;
using HttpNetHelper.Helper;

namespace HttpNetHelper
{
    /// <summary>
    /// Http帮助类  
    /// 版本：1.0.0
    /// 作者：零点
    /// 更新时间：2019-11-7
    /// </summary>

    public class HttpHelper
	{
#region Private Obj

		/// <summary>
		/// HttpHelperBLL
		/// </summary>
		private readonly HttpHelperBll bll = new HttpHelperBll();

#endregion
#region HttpHelper
		/// <summary>
		/// 根据相传入的数据，得到相应页面数据
		/// </summary>
		/// <param name="item">参数类对象</param>
		/// <returns>返回HttpResult类型</returns>
		public HttpResult GetHtml(HttpItem item)
		{
			return bll.GetHtml(item);
		}

		/// <summary>
		/// 根据Url获取图片
		/// </summary>
		/// <param name="item">HttpItem参数</param>
		/// <returns>返回图片，错误为NULL</returns>
		public Image GetImage(HttpItem item)
		{
			return bll.GetImage(item);
		}
		/// <summary>
		/// 快速请求方法FastRequest（极速请求不接收数据,只做提交）不返回Header、Cookie、Html
		/// </summary>
		/// <param name="item">参数类对象</param>
		/// <returns>返回HttpResult类型</returns>
		public HttpResult FastRequest(HttpItem item)
		{
			return bll.FastRequest(item);
		}
#endregion
#region Cookie
		/// <summary>
		/// 根据字符生成Cookie和精简串，将排除path,expires,domain以及重复项
		/// </summary>
		/// <param name="strcookie">Cookie字符串</param>
		/// <returns>精简串</returns>
		public static string GetSmallCookie(string strcookie)
		{
			return HttpCookieHelper.GetSmallCookie(strcookie);
		}
		/// <summary>
		/// 将字符串Cookie转为CookieCollection
		/// </summary>
		/// <param name="strcookie">Cookie字符串</param>
		/// <returns>List-CookieItem</returns>
		public static CookieCollection StrCookieToCookieCollection(string strcookie)
		{
			return HttpCookieHelper.StrCookieToCookieCollection(strcookie);
		}
		/// <summary>
		/// 将CookieCollection转为字符串Cookie
		/// </summary>
		/// <param name="cookie">Cookie字符串</param>
		/// <returns>strcookie</returns>
		public static string CookieCollectionToStrCookie(CookieCollection cookie)
		{
			return HttpCookieHelper.CookieCollectionToStrCookie(cookie);
		}
		/// <summary>
		/// 自动合并两个Cookie的值返回更新后结果 
		/// </summary>
		/// <param name="OldCookie">Cookie1</param>
		/// <param name="NewCookie">Cookie2</param>
		/// <returns>返回更新后的Cookie</returns>
		public static string GetMergeCookie(string OldCookie, string NewCookie)
		{
			return HttpCookieHelper.MergerCookies(OldCookie, NewCookie);
		}
		public static CookieCollection GetMergeCookie(CookieCollection OldCookie, CookieCollection NewCookie)
		{
			return HttpCookieHelper.MergerCookies(OldCookie, NewCookie);
		}
#endregion

	}
}

