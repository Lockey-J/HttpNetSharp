
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using HttpNetHelper.Enum;
using System.Collections.Specialized;

namespace HttpNetHelper.Base
{
	/// <summary>
	/// Http连接操作帮助类  
	/// </summary>
	internal class HttpNetHelperBase
	{
		#region 预定义方变量
		//默认的编码
		public Encoding Encoding { get; set; } = System.Text.Encoding.Default;
		//Post数据编码
		public Encoding Postencoding { get; set; } = System.Text.Encoding.Default;
		//HttpWebRequest对象用来发起请求
		public HttpWebRequest Request { get; set; } = null;
		//获取影响流的数据对象
		public HttpWebResponse Response { get; set; } = null;
		//设置本地的出口ip和端口
		public IPEndPoint IPEndPoint { get; set; } = null;
		#endregion
		#region internal
		/// <summary>
		/// 根据相传入的数据，得到相应页面数据
		/// </summary>
		/// <param name="item">参数类对象</param>
		/// <returns>返回HttpResult类型</returns>
		internal HttpResult GetHtml(HttpItem item)
		{
			//返回参数
			HttpResult result = new HttpResult
			{
				Item = item
			};
			try
			{
				//准备参数
				SetRequest(item);
			}
			catch (Exception ex)
			{
				//配置参数时出错
				return new HttpResult()
				{
					Cookie = string.Empty,
					Header = null,
					Html = ex.Message,
					StatusDescription = "配置参数时出错：" + ex.Message
				};
			}
			try
			{
				//请求数据
				
				using (Response = (HttpWebResponse)Request.GetResponse())
				{
					GetData(item, ref result);
				}
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{					
					using (Response = (HttpWebResponse)ex.Response)
					{
						GetData(item, ref result);
					}
				}
				else
				{
					result.Html = ex.Message;
				}
			}
			catch (Exception ex)
			{
				result.Html = ex.Message;
			}
			if (item.IsToLower)
			{
				result.Html = result.Html.ToLower();

			}
			//重置request，response为空
			if (item.IsReset)
			{
				if (Request != null)
				{
					Request.Abort();
					Request = null;
				}
				if (Response != null)
				{
					Response.Close();
					Response = null;
				}

			}
			return result;
		}
		/// <summary>
		/// 快速Post数据这个访求与GetHtml一样，只是不接收返回数据，只做提交。
		/// </summary>
		/// <param name="item">参数类对象</param>
		/// <returns>返回HttpResult类型</returns>
		internal HttpResult FastRequest(HttpItem item)
		{
			//返回参数
			HttpResult result = new HttpResult
			{
				Item = item
			};
			try
			{
				//准备参数
				SetRequest(item);
			}
			catch (Exception ex)
			{
				//配置参数时出错
				return new HttpResult()
				{
					Cookie = (Response.Headers["set-cookie"] ?? string.Empty),
					Header = null,
					Html = ex.Message,
					StatusDescription = "配置参数时出错：" + ex.Message
				};
			}
			try
			{
				//请求数据
				
				using (Response = (HttpWebResponse)Request.GetResponse())
				{
					//成功 不做处理只回成功状态
					return new HttpResult()
					{
						Cookie = ((Response.Headers["set-cookie"] != null) ? Response.Headers["set-cookie"] : string.Empty),
						Header = Response.Headers,
						StatusCode = Response.StatusCode,
						StatusDescription = Response.StatusDescription
					};
				}
			}
			catch (WebException ex)
			{
				
				using (Response = (HttpWebResponse)ex.Response)
				{
					//不做处理只回成功状态
					return new HttpResult()
					{
						Cookie = Response.Headers["set-cookie"] ?? string.Empty,
						Header = Response.Headers,
						StatusCode = Response.StatusCode,
						StatusDescription = Response.StatusDescription
					};
				}
			}
			catch (Exception ex)
			{
				result.Html = ex.Message;
			}
			if (item.IsToLower)
			{
				result.Html = result.Html.ToLower();
			}
			return result;
		}
#endregion

#region GetData
		/// <summary>
		/// 获取数据的并解析的方法
		/// </summary>
		/// <param name="item"></param>
		/// <param name="result"></param>
		private void GetData(HttpItem item, ref HttpResult result)
		{
			if (Response == null)
			{
				return;
			}
			#region "base"
			//获取StatusCode
			result.StatusCode = Response.StatusCode;
			//获取最后访问的URl
			result.ResponseUri = Response.ResponseUri.ToString();
			//获取StatusDescription
			result.StatusDescription = Response.StatusDescription;
			//获取Headers
			result.Header = Response.Headers;
			//获取CookieCollection
			if (Response.Cookies != null)
			{
				result.CookieCollection = Response.Cookies;
			}
			//获取set-cookie
			if (Response.Headers["set-cookie"] != null)
			{
				result.Cookie = Response.Headers["set-cookie"];
			}
			//Cookie是否自动更新为请求所获取的新Cookie值 
			if (item.IsUpdateCookie)
			{
				item.Cookie = result.Cookie;
				item.CookieCollection = result.CookieCollection;
			}
			#endregion

			//			#Region "用户设置用编码"
			//处理网页Byte
			byte[] ResponseByte = GetByte(item);

			if (ResponseByte != null && ResponseByte.Length > 0)
			{
				//设置编码
				SetEncoding(item, result, ResponseByte);

				//设置返回的Byte
				SetResultByte(item, result, ResponseByte);
			}
			else
			{
				//没有返回任何Html代码
				result.Html = string.Empty;
			}


		}
#endregion
		/// <summary>
		/// 设置返回的Byte
		/// </summary>
		/// <param name="item">HttpItem</param>
		/// <param name="result">result</param>
		/// <param name="enByte">byte</param>
		private void SetResultByte(HttpItem item, HttpResult result, byte[] enByte)
		{
			//是否返回Byte类型数据
			if (item.ResultType == ResultType.Byte)
			{
				//Byte数据
				result.ResultByte = enByte;
			}
			else if (item.ResultType == ResultType.String)
			{
				//得到返回的HTML
				result.Html = Encoding.GetString(enByte);
			}
			else if (item.ResultType == ResultType.StringByte)
			{
				//Byte数据
				result.ResultByte = enByte;
				//得到返回的HTML
				result.Html = Encoding.GetString(enByte);
			}
		}
		/// <summary>
		/// 设置编码
		/// </summary>
		/// <param name="item">HttpItem</param>
		/// <param name="result">HttpResult</param>
		/// <param name="ResponseByte">byte[]</param>
		private void SetEncoding(HttpItem item, HttpResult result, byte[] ResponseByte)
		{
			//从这里开始我们要无视编码了
			if (Encoding == null)
			{
				Match meta = Regex.Match(Encoding.Default.GetString(ResponseByte), "<meta[^<]*charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
				string c = string.Empty;
				if (meta != null && meta.Groups.Count > 0)
				{
					c = meta.Groups[1].Value.ToLower().Trim();
				}
				string cs = string.Empty;
				if (!string.IsNullOrWhiteSpace(Response.CharacterSet))
				{
					cs = Response.CharacterSet.Trim().Replace("\"", "").Replace("'", "");
				}

				if (c.Length > 2)
				{
					try
					{
						Encoding = Encoding.GetEncoding(c.Replace("\"", string.Empty).Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk").Trim());
					}
					catch
					{
						if (string.IsNullOrEmpty(cs))
						{
							Encoding = Encoding.UTF8;
						}
						else
						{
							Encoding = Encoding.GetEncoding(cs);
						}
					}
				}
				else
				{
					if (string.IsNullOrEmpty(cs))
					{
						Encoding = Encoding.UTF8;
					}
					else
					{
						Encoding = Encoding.GetEncoding(cs);
					}
				}
			}
		}
		/// <summary>
		/// 提取网页Byte
		/// </summary>
		/// <returns></returns>
		private byte[] GetByte(HttpItem item)
		{
			byte[] ResponseByte = null;
			using (MemoryStream _stream = new MemoryStream())
			{
				if (item.IsGzip)
				{
					//开始读取流并设置编码方式
					(new GZipStream(Response.GetResponseStream(), CompressionMode.Decompress)).CopyTo(_stream);
				}
				else
				{
					//GZIIP处理
					if (Response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
					{
						//开始读取流并设置编码方式
						(new GZipStream(Response.GetResponseStream(), CompressionMode.Decompress)).CopyTo(_stream);
					}
					else
					{
						//开始读取流并设置编码方式
						Response.GetResponseStream().CopyTo(_stream);
					}
				}
				//获取Byte
				ResponseByte = _stream.ToArray();
			}
			return ResponseByte;
		}


#region SetRequest
		/// <summary>
		/// 为请求准备参数
		/// </summary>
		///<param name="item">参数列表</param>
		private void SetRequest(HttpItem item)
		{
			GC.Collect();

			if (item.URL.StartsWith("https", StringComparison.OrdinalIgnoreCase) || !string.IsNullOrWhiteSpace(item.CerPath))
			{
				//这一句一定要写在创建连接的前面。使用回调的方法进行证书验证。
				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
			}
			//初始化对像，并设置请求的URL地址

			Request = (HttpWebRequest)WebRequest.Create(item.URL);
			if (item.IPEndPoint != null)
			{
				IPEndPoint = item.IPEndPoint;
				//设置本地的出口ip和端口
				Request.ServicePoint.BindIPEndPointDelegate = new BindIPEndPoint(BindIPEndPointCallback);
			}

			// 验证证书
			SetCer(item);
			SetCerList(item);
			//设置Header参数
			if (item.Header != null && item.Header.Count > 0)
			{
				foreach (string key in item.Header.AllKeys)
				{
					Request.Headers.Add(key, item.Header[key]);
				}
			}
			// 设置代理
			SetProxy(item);
			if (item.ProtocolVersion != null)
			{
				Request.ProtocolVersion = item.ProtocolVersion;
			}
			Request.ServicePoint.Expect100Continue = item.Expect100Continue;
			//请求方式Get或者Post
			Request.Method = item.Method;
			Request.Timeout = item.Timeout;
			//来源地址
			Request.Referer = item.Referer;
			//UserAgent客户端的访问类型，包括浏览器版本和操作系统信息
			Request.UserAgent = item.UserAgent;
			//设置Cookie
			SetCookie(item);
			if (!string.IsNullOrWhiteSpace(item.Host))
			{
				Request.Host = item.Host;
			}
			Request.AutomaticDecompression = item.AutomaticDecompression;
			//keep-live
			Request.ServicePoint.GetType().GetProperty("HttpBehaviour", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(Request.ServicePoint, (byte)0, null);
			Request.KeepAlive = item.KeepAlive;

			Request.ReadWriteTimeout = item.ReadWriteTimeout;

			if (item.IfModifiedSince != null)
			{
				Request.IfModifiedSince = Convert.ToDateTime(item.IfModifiedSince);
			}

			//Accept
			if (!string.IsNullOrWhiteSpace(item.Accept))
			{
				Request.Accept = item.Accept;
			}


			//ContentType返回类型
			if (!item.Method.ToLower().Contains("get") && !string.IsNullOrWhiteSpace(item.ContentType))
			{
				Request.ContentType = item.ContentType;
			}

			// 编码
			Encoding = item.Encoding;
			//设置安全凭证
			Request.Credentials = item.ICredentials;


			//是否执行跳转功能
			Request.AllowAutoRedirect = item.Allowautoredirect;
			if (item.MaximumAutomaticRedirections > 0)
			{
				Request.MaximumAutomaticRedirections = item.MaximumAutomaticRedirections;
			}
			//设置最大连接
			if (item.Connectionlimit > 0)
			{
				Request.ServicePoint.ConnectionLimit = item.Connectionlimit;
			}
			//当出现"请求被中止: 未能创建 SSL/TLS 安全通道"时需要配置此属性 
			if (item.SecurityProtocol > 0)
			{
				ServicePointManager.SecurityProtocol = item.SecurityProtocol;
			}
			//设置Post数据
			SetPostData(item);
		}


		/// <summary>
		/// 设置证书
		/// </summary>
		/// <param name="item"></param>
		private void SetCer(HttpItem item)
		{
			if (!string.IsNullOrWhiteSpace(item.CerPath))
			{
				//将证书添加到请求里
				if (!string.IsNullOrWhiteSpace(item.CerPwd))
				{
					Request.ClientCertificates.Add(new X509Certificate(item.CerPath, item.CerPwd));
				}
				else
				{
					Request.ClientCertificates.Add(new X509Certificate(item.CerPath));
				}
			}
		}
		/// <summary>
		/// 设置多个证书
		/// </summary>
		/// <param name="item"></param>
		private void SetCerList(HttpItem item)
		{
			if (item.ClentCertificates != null && item.ClentCertificates.Count > 0)
			{
				foreach (X509Certificate c in item.ClentCertificates)
				{
					Request.ClientCertificates.Add(c);
				}
			}
		}
		/// <summary>
		/// 设置Cookie
		/// </summary>
		/// <param name="item">Http参数</param>
		private void SetCookie(HttpItem item)
		{
			if (!string.IsNullOrEmpty(item.Cookie))
			{
				Request.Headers[HttpRequestHeader.Cookie] = item.Cookie;
			}
			//设置CookieCollection
			if (item.ResultCookieType == ResultCookieType.CookieCollection)
			{
				Request.CookieContainer = new CookieContainer();
				if (item.CookieCollection != null && item.CookieCollection.Count > 0)
				{
					try
					{
						Request.CookieContainer.Add(new Uri(item.URL), item.CookieCollection);
					}
					catch (Exception )
					{

					}
				}
			}
			else if (item.ResultCookieType == ResultCookieType.CookieContainer)
			{
				Request.CookieContainer = item.CookieContainer;
			}
		}
		/// <summary>
		/// 设置Post数据
		/// </summary>
		/// <param name="item">Http参数</param>
		private void SetPostData(HttpItem item)
		{
			//验证在得到结果时是否有传入数据
			if (!(Request.Method.Trim().ToLower().Contains("get")))
			{
				if (item.PostEncoding != null)
				{
					Postencoding = item.PostEncoding;
				}
				byte[] buffer = null;
				//写入Byte类型
				if (item.PostDataType == PostDataType.Byte && item.PostdataByte != null && item.PostdataByte.Length > 0)
				{
					//验证在得到结果时是否有传入数据
					buffer = item.PostdataByte; //写入文件
				}
				else if (item.PostDataType == PostDataType.FilePath && !string.IsNullOrWhiteSpace(item.Postdata))
				{
					try
					{
						buffer = CreatFileToPostBytes(item.Boundary, item.StringDict, item.PostFile, item.PostFileName, item.PostEncoding);
					}
					catch (Exception )
					{
						buffer = null;
					}

				}
				else if (!string.IsNullOrWhiteSpace(item.Postdata))
				{
					buffer = Postencoding.GetBytes(item.Postdata);
				}
				if (buffer != null)
				{
					Request.ContentLength = buffer.Length;
					Request.GetRequestStream().Write(buffer, 0, buffer.Length);
				}
				else
				{
					Request.ContentLength = 0;
				}
			}
		}
		/// <summary>
		/// 设置代理
		/// </summary>
		/// <param name="item">参数对象</param>
		private void SetProxy(HttpItem item)
		{
			bool isIeProxy = false;
			if (!string.IsNullOrWhiteSpace(item.ProxyIp))
			{
				isIeProxy = item.ProxyIp.ToLower().Contains("ieproxy");
			}
			if (!string.IsNullOrWhiteSpace(item.ProxyIp) && !isIeProxy)
			{
				//设置代理服务器
				if (item.ProxyIp.Contains(":"))
				{
					string[] plist = item.ProxyIp.Split(':');
					WebProxy myProxy = new WebProxy(plist[0].Trim(), Convert.ToInt32(plist[1].Trim()));
					//建议连接
					myProxy.Credentials = new NetworkCredential(item.ProxyUserName, item.ProxyPwd);
					//给当前请求对象
					Request.Proxy = myProxy;
				}
				else
				{
					WebProxy myProxy = new WebProxy(item.ProxyIp, false)
					{
						//建议连接
						Credentials = new NetworkCredential(item.ProxyUserName, item.ProxyPwd)
					};
					//给当前请求对象
					Request.Proxy = myProxy;
				}
			}
			else if (isIeProxy)
			{
				//设置为IE代理
			}
			else
			{
				Request.Proxy = item.WebProxy;
			}
		}
#endregion

#region private main
		/// <summary>
		/// 回调验证证书问题
		/// </summary>
		/// <param name="sender">流对象</param>
		/// <param name="certificate">证书</param>
		/// <param name="chain">X509Chain</param>
		/// <param name="errors">SslPolicyErrors</param>
		/// <returns>bool</returns>
		private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
		{
			return true;
		}

		/// <summary>
		/// 通过设置这个属性，可以在发出连接的时候绑定客户端发出连接所使用的IP地址。 
		/// </summary>
		/// <param name="servicePoint"></param>
		/// <param name="remoteEndPoint"></param>
		/// <param name="retryCount"></param>
		/// <returns></returns>
		public IPEndPoint BindIPEndPointCallback(ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount)
		{
			return IPEndPoint; //端口号
		}
		/// <summary>
		/// 根据文件路径创建上传的字节集
		/// </summary>
		/// <param name="boundary">分割符</param>
		/// <param name="stringDict">上传文件的附带参数字典</param>
		/// <param name="filePath">上传文件路径</param>
		/// <param name="fileName">文件头数据体中filename的value值</param>
		/// <returns>返回http POST上POSTDAT的数组</returns>
		public byte[] CreatFileToPostBytes(string boundary, IDictionary<string, string> stringDict, string filePath, object fileName,Encoding mPostencoding)
		{
			//判断字典跟文件路径是否为空
			if (stringDict == null || string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(boundary))
			{
				return null;
			}

			// 边界符
			byte[] beginBoundary = mPostencoding.GetBytes("--" + boundary + Environment.NewLine);

			// Key-Value数据
			string stringKeyHeader = "Content-Disposition: form-data; name=\"{0}\"" + Environment.NewLine +
										"Content-Length: {1}" + Environment.NewLine + Environment.NewLine +
										"{2}" + Environment.NewLine;
			//& "--" & boundary & vbCrLf
			//文件头数据体
			string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" + Environment.NewLine +
									   "Content-Type: image/jpeg" + Environment.NewLine +
									   "Content-Length: {2}" + Environment.NewLine + Environment.NewLine;

			// 最后的结束符
			var endBoundary = mPostencoding.GetBytes(Environment.NewLine + "--" + boundary + "--" + Environment.NewLine);

			//获取文件二进制数组
			FileStream fileStream = new FileStream(filePath, FileMode.Open);
			byte[] fileByte = new byte[fileStream.Length];
			fileStream.Read(fileByte, 0, fileByte.Length);
			fileStream.Dispose();

			string fileHeader = string.Format(filePartHeader, "file", fileName, fileByte.Length);

			byte[] fileHeaderBytes = mPostencoding.GetBytes(fileHeader);

			// 开始拼数据
			using (var memStream = new MemoryStream())
			{
				//组装开始分界线数据体 到内存流中
				memStream.Write(beginBoundary, 0, beginBoundary.Length);

				// 组装上传文件附加携带的参数 到内存流中
				foreach (byte[] formitembytes in
					from string key in stringDict.Keys
					select string.Format(stringKeyHeader, key, mPostencoding.GetBytes(stringDict[key]).Length, stringDict[key]) into formitem
					select mPostencoding.GetBytes(formitem))
				{
					memStream.Write(formitembytes, 0, formitembytes.Length);
					//写入分界符
					memStream.Write(beginBoundary, 0, beginBoundary.Length);
				}

				// 组装文件头数据体到内存流中
				memStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
				// 组装文件数据体到内存流中
				memStream.Write(fileByte, 0, fileByte.Length);
				// 写入最后的结束边界符
				memStream.Write(endBoundary, 0, endBoundary.Length);
				byte[] ResultBytes = memStream.ToArray();
				return ResultBytes;
			}
		}

#endregion
	}

}