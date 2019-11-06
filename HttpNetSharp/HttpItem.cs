
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;

using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using HttpNetHelper.Enum;


namespace HttpNetHelper
{

	/// <summary>
	/// Http请求参考类 
	/// </summary>
	public class HttpItem
	{
		private bool InstanceFieldsInitialized = false;

		public HttpItem()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			_PostDataType = HttpNetHelper.Enum.PostDataType.String;
			_ResultCookieType = HttpNetHelper.Enum.ResultCookieType.CookieCollection;
			m_resulttype = HttpNetHelper.Enum.ResultType.String;
		}

#region base
		private string m_URL;
		/// <summary>
		/// 请求URL必须填写
		/// </summary>
		public string URL
		{
			get
			{
				return m_URL;
			}
			set
			{
				m_URL = value;
			}
		}
		private string _Method = "GET";
		/// <summary>
		/// 请求方式默认为GET方式,当为POST方式时必须设置Postdata的值
		/// </summary>
		public string Method
		{
			get
			{
				return _Method;
			}
			set
			{
				_Method = value;
			}
		}
		private int _Timeout = 30000;
		/// <summary>
		/// 默认请求超时时间
		/// </summary>
		public int Timeout
		{
			get
			{
				return _Timeout;
			}
			set
			{
				_Timeout = value;
			}
		}
		private int _ReadWriteTimeout = 10000;
		/// <summary>
		/// 默认写入和读取Post数据超时间
		/// </summary>
		public int ReadWriteTimeout
		{
			get
			{
				return _ReadWriteTimeout;
			}
			set
			{
				_ReadWriteTimeout = value;
			}
		}
		/// <summary>
		/// 设置Host的标头信息
		/// </summary>
		public string Host {get; set;}
		private bool _KeepAlive = true;
		/// <summary>
		///  获取或设置一个值，该值指示是否与 Internet 资源建立持久性连接默认为true。
		/// </summary>
		public bool KeepAlive
		{
			get
			{
				return _KeepAlive;
			}
			set
			{
				_KeepAlive = value;
			}
		}
		private string _Accept = "";
		/// <summary>
		/// 请求标头值 默认为text/html, application/xhtml+xml, */*
		/// </summary>
		public string Accept
		{
			get
			{
				return _Accept;
			}
			set
			{
				_Accept = value;
			}
		}
		private string _ContentType = "application/x-www-form-urlencoded";
		/// <summary>
		/// 请求返回类型默认 text/html
		/// </summary>
		public string ContentType
		{
			get
			{
				return _ContentType;
			}
			set
			{
				_ContentType = value;
			}
		}
		private string _UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
		/// <summary>
		/// 客户端访问信息默认Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)
		/// </summary>
		public string UserAgent
		{
			get
			{
				return _UserAgent;
			}
			set
			{
				_UserAgent = value;
			}
		}
		/// <summary>
		/// 来源地址，上次访问地址
		/// </summary>
		public string Referer {get; set;}
		/// <summary>
		///   获取或设置用于请求的 HTTP 版本。返回结果:用于请求的 HTTP 版本。默认为 System.Net.HttpVersion.Version11。
		/// </summary>
		public Version ProtocolVersion {get; set;}
		private bool _expect100continue = false;
		/// <summary>
		///  获取或设置一个 System.Boolean 值，该值确定是否使用 100-Continue 行为。如果 POST 请求需要 100-Continue 响应，则为 true；否则为 false。默认值为 true。
		/// </summary>
		public bool Expect100Continue
		{
			get
			{
				return _expect100continue;
			}
			set
			{
				_expect100continue = value;
			}
		}
		/// <summary>
		/// 设置请求将跟随的重定向的最大数目
		/// </summary>
		public int MaximumAutomaticRedirections {get; set;}
		private DateTime? _IfModifiedSince = null;
		/// <summary>
		/// 获取和设置IfModifiedSince，默认为当前日期和时间
		/// </summary>
		public DateTime? IfModifiedSince
		{
			get
			{
				return _IfModifiedSince;
			}
			set
			{
				_IfModifiedSince = value;
			}
		}
		private bool _isGzip = false;
		/// <summary>
		///  是否执行Gzip解压 默认为否
		/// </summary>
		public bool IsGzip
		{
			get
			{
				return _isGzip;
			}
			set
			{
				_isGzip = value;
			}
		}

#endregion

#region encoding
		/// <summary>
		/// 返回数据编码默认为NUll,可以自动识别,一般为utf-8,gbk,gb2312
		/// </summary>
		public Encoding Encoding {get; set;}

		/// <summary>
		/// 设置或获取Post参数编码,默认的为Default编码
		/// </summary>
		private Encoding m_PostEncoding {get; set;} = Encoding.UTF8;

		public Encoding PostEncoding
		{
			get
			{
				return m_PostEncoding;
			}
			set
			{
				m_PostEncoding = value;
			}
		}

		//设置所使用的解压缩类型。
		private DecompressionMethods _AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

		public DecompressionMethods AutomaticDecompression
		{
			get
			{
				return _AutomaticDecompression;
			}
			set
			{
				_AutomaticDecompression = value;
			}
		}

#endregion

#region post
		private PostDataType _PostDataType;
		/// <summary>
		/// Post的数据类型
		/// </summary>
		public PostDataType PostDataType
		{
			get
			{
				return _PostDataType;
			}
			set
			{
				_PostDataType = value;
			}
		}
		/// <summary>
		/// Post请求时要发送的字符串Post数据
		/// </summary>
		public string Postdata {get; set;}
		/// <summary>
		/// Post请求时要发送的Byte类型的Post数据
		/// </summary>
		public byte[] PostdataByte {get; set;}

		/// <summary>
		/// Post附件分割符
		/// </summary>
		/// <returns></returns>
		public string Boundary {get; set;}

		/// <summary>
		/// Post附件附带参数
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, string> StringDict {get; set;}
		/// <summary>
		/// 上传文件路径
		/// </summary>
		/// <returns></returns>
		public string PostFile {get; set;}

		/// <summary>
		/// 文件头数据体中Filename的value
		/// </summary>
		/// <returns></returns>
		public string PostFileName {get; set;}

#endregion
#region cookie
		/// <summary>
		/// Cookie对象集合
		/// </summary>
		public CookieCollection CookieCollection {get; set;}
		/// <summary>
		/// 请求时的Cookie
		/// </summary>
		public string Cookie {get; set;}
		private bool _AutoRedirectCookie = false;
		/// <summary>
		/// 请求时当设置allowautoredirect=true时是否自动处理Cookie
		/// </summary>
		public bool AutoRedirectCookie
		{
			get
			{
				return _AutoRedirectCookie;
			}
			set
			{
				_AutoRedirectCookie = value;
			}
		}
		private ResultCookieType _ResultCookieType;
		/// <summary>
		/// 设置返回/输入Cookie类型,默认的是只返回/输入字符串类型Cookie
		/// </summary>
		public ResultCookieType ResultCookieType
		{
			get
			{
				return _ResultCookieType;
			}
			set
			{
				_ResultCookieType = value;
			}
		}
		private bool _isUpdateCookie = false;
		/// <summary>
		/// 是否自动将Cookie自动更新为请求所获取的新Cookie值  默认为False
		/// </summary>
		public bool IsUpdateCookie
		{
			get
			{
				return _isUpdateCookie;
			}
			set
			{
				_isUpdateCookie = value;
			}
		}

		private CookieContainer _CookieContainer = new CookieContainer();
		/// <summary>
		/// Cookie对象的集合容器 模式Cookie，可容纳N个CookieCollection对象
		/// </summary>
		public CookieContainer CookieContainer
		{
			get
			{
				return _CookieContainer;
			}
			set
			{
				_CookieContainer = value;
			}
		}

#endregion
#region cer
		/// <summary>
		/// 证书绝对路径
		/// </summary>
		public string CerPath {get; set;}
		/// <summary>
		/// 证书密码
		/// </summary>
		public string CerPwd {get; set;}
		/// <summary>
		/// 设置509证书集合
		/// </summary>
		public X509CertificateCollection ClentCertificates {get; set;}
		private ICredentials _ICredentials = CredentialCache.DefaultCredentials;
		/// <summary>
		/// 获取或设置请求的身份验证信息。
		/// </summary>
		public ICredentials ICredentials
		{
			get
			{
				return _ICredentials;
			}
			set
			{
				_ICredentials = value;
			}
		}
#endregion
#region to

		private bool m_isToLower = false;
		/// <summary>
		/// 是否设置为全文小写，默认为不转化
		/// </summary>
		public bool IsToLower
		{
			get
			{
				return m_isToLower;
			}
			set
			{
				m_isToLower = value;
			}
		}
#endregion

#region link

		private bool m_allowautoredirect = false;
		/// <summary>
		/// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转
		/// </summary>
		public bool Allowautoredirect
		{
			get
			{
				return m_allowautoredirect;
			}
			set
			{
				m_allowautoredirect = value;
			}
		}

		private int m_connectionlimit = 1024;
		/// <summary>
		/// 最大连接数
		/// </summary>
		public int Connectionlimit
		{
			get
			{
				return m_connectionlimit;
			}
			set
			{
				m_connectionlimit = value;
			}
		}
#endregion

#region proxy
		/// <summary>
		/// 设置代理对象，不想使用IE默认配置就设置为Null，而且不要设置ProxyIp
		/// </summary>
		public WebProxy WebProxy {get; set;}
		/// <summary>
		/// 代理Proxy 服务器用户名
		/// </summary>
		public string ProxyUserName {get; set;}
		/// <summary>
		/// 代理 服务器密码
		/// </summary>
		public string ProxyPwd {get; set;}
		/// <summary>
		/// 代理 服务IP,如果要使用IE代理就设置为ieproxy
		/// </summary>
		public string ProxyIp {get; set;}
#endregion

#region result
		//
		private ResultType m_resulttype;
		/// <summary>
		/// 设置返回类型String和Byte
		/// </summary>
		public ResultType ResultType
		{
			get
			{
				return m_resulttype;
			}
			set
			{
				m_resulttype = value;
			}
		}

		private WebHeaderCollection m_header = new WebHeaderCollection();
		/// <summary>
		/// header对象
		/// </summary>
		public WebHeaderCollection Header
		{
			get
			{
				return m_header;
			}
			set
			{
				m_header = value;
			}
		}
#endregion

#region ip-port
		private IPEndPoint _IPEndPoint = null;
		/// <summary>
		/// 设置本地的出口ip和端口
		/// </summary>]
		/// <example>
		///item.IPEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.1"),80);
		/// </example>
		public IPEndPoint IPEndPoint
		{
			get
			{
				return _IPEndPoint;
			}
			set
			{
				_IPEndPoint = value;
			}
		}
#endregion

#region config

		/// <summary>
		/// 当出现"请求被中止: 未能创建 SSL/TLS 安全通道"时需要配置此属性 
		/// </summary>
		public SecurityProtocolType SecurityProtocol {get; set;}

		private bool _isReset = false;
		/// <summary>
		/// 是否重置request,response的值，默认不重置，当设置为True时request,response将被设置为Null
		/// </summary>
		public bool IsReset
		{
			get
			{
				return _isReset;
			}
			set
			{
				_isReset = value;
			}
		}
#endregion

	}
}
