
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
        public HttpItem()
        {
 
        }  

        #region base
        /// <summary>
        /// 请求URL必须填写
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 请求方式默认为GET方式,当为POST方式时必须设置Postdata的值
        /// </summary>
        public string Method { get; set; } = "GET";

        /// <summary>
        /// 默认请求超时时间
        /// </summary>
        public int Timeout { get; set; } = 30000;

        /// <summary>
        /// 默认写入和读取Post数据超时间
        /// </summary>
        public int ReadWriteTimeout { get; set; } = 10000;
        /// <summary>
        /// 设置Host的标头信息
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///  获取或设置一个值，该值指示是否与 Internet 资源建立持久性连接默认为true。
        /// </summary>
        public bool KeepAlive { get; set; } = true;

        /// <summary>
        /// 请求标头值 默认为text/html, application/xhtml+xml, */*
        /// </summary>
        public string Accept { get; set; } = "";

        /// <summary>
        /// 请求返回类型默认 text/html
        /// </summary>
        public string ContentType { get; set; } = "application/x-www-form-urlencoded";

        /// <summary>
        /// 客户端访问信息默认Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)
        /// </summary>
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
        /// <summary>
        /// 来源地址，上次访问地址
        /// </summary>
        public string Referer { get; set; }
        /// <summary>
        ///   获取或设置用于请求的 HTTP 版本。返回结果:用于请求的 HTTP 版本。默认为 System.Net.HttpVersion.Version11。
        /// </summary>
        public Version ProtocolVersion { get; set; }

        /// <summary>
        ///  获取或设置一个 System.Boolean 值，该值确定是否使用 100-Continue 行为。如果 POST 请求需要 100-Continue 响应，则为 true；否则为 false。默认值为 true。
        /// </summary>
        public bool Expect100Continue { get; set; } = false;
        /// <summary>
        /// 设置请求将跟随的重定向的最大数目
        /// </summary>
        public int MaximumAutomaticRedirections { get; set; }

        /// <summary>
        /// 获取和设置IfModifiedSince，默认为当前日期和时间
        /// </summary>
        public DateTime? IfModifiedSince { get; set; } = null;

        /// <summary>
        ///  是否执行Gzip解压 默认为否
        /// </summary>
        public bool IsGzip { get; set; } = false;

        #endregion

        #region encoding
        /// <summary>
        /// 返回数据编码默认为NUll,可以自动识别,一般为utf-8,gbk,gb2312
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// 设置或获取Post参数编码,默认的为Default编码
        /// </summary>
        public Encoding PostEncoding { get; set; } = Encoding.UTF8;

        public DecompressionMethods AutomaticDecompression { get; set; } = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        #endregion

        #region post
        /// <summary>
        /// Post的数据类型
        /// </summary>
        public PostDataType PostDataType { get; set; } = PostDataType.String;
        /// <summary>
        /// Post请求时要发送的字符串Post数据
        /// </summary>
        public string Postdata { get; set; }
        /// <summary>
        /// Post请求时要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostdataByte { get; set; }

        /// <summary>
        /// Post附件分割符
        /// </summary>
        /// <returns></returns>
        public string Boundary { get; set; }

        /// <summary>
        /// Post附件附带参数
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> StringDict { get; set; }
        /// <summary>
        /// 上传文件路径
        /// </summary>
        /// <returns></returns>
        public string PostFile { get; set; }

        /// <summary>
        /// 文件头数据体中Filename的value
        /// </summary>
        /// <returns></returns>
        public string PostFileName { get; set; }

        #endregion
        #region cookie
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection { get; set; }
        /// <summary>
        /// 请求时的Cookie
        /// </summary>
        public string Cookie { get; set; }

        /// <summary>
        /// 请求时当设置allowautoredirect=true时是否自动处理Cookie
        /// </summary>
        public bool AutoRedirectCookie { get; set; } = false;

        /// <summary>
        /// 设置返回/输入Cookie类型,默认的是只返回/输入字符串类型Cookie
        /// </summary>
        public ResultCookieType ResultCookieType { get; set; } = ResultCookieType.CookieCollection;

        /// <summary>
        /// 是否自动将Cookie自动更新为请求所获取的新Cookie值  默认为False
        /// </summary>
        public bool IsUpdateCookie { get; set; } = false;

        /// <summary>
        /// Cookie对象的集合容器 模式Cookie，可容纳N个CookieCollection对象
        /// </summary>
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();

        #endregion
        #region cer
        /// <summary>
        /// 证书绝对路径
        /// </summary>
        public string CerPath { get; set; }
        /// <summary>
        /// 证书密码
        /// </summary>
        public string CerPwd { get; set; }
        /// <summary>
        /// 设置509证书集合
        /// </summary>
        public X509CertificateCollection ClentCertificates { get; set; }

        /// <summary>
        /// 获取或设置请求的身份验证信息。
        /// </summary>
        public ICredentials ICredentials { get; set; } = CredentialCache.DefaultCredentials;
        #endregion
        #region to

        /// <summary>
        /// 是否设置为全文小写，默认为不转化
        /// </summary>
        public bool IsToLower { get; set; } = false;
        #endregion

        #region link

        /// <summary>
        /// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转
        /// </summary>
        public bool Allowautoredirect { get; set; } = false;

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int Connectionlimit { get; set; } = 1024;
        #endregion

        #region proxy
        /// <summary>
        /// 设置代理对象，不想使用IE默认配置就设置为Null，而且不要设置ProxyIp
        /// </summary>
        public WebProxy WebProxy { get; set; }
        /// <summary>
        /// 代理Proxy 服务器用户名
        /// </summary>
        public string ProxyUserName { get; set; }
        /// <summary>
        /// 代理 服务器密码
        /// </summary>
        public string ProxyPwd { get; set; }
        /// <summary>
        /// 代理 服务IP,如果要使用IE代理就设置为ieproxy
        /// </summary>
        public string ProxyIp { get; set; }
        #endregion

        #region result
        //
        /// <summary>
        /// 设置返回类型String和Byte
        /// </summary>
        public ResultType ResultType { get; set; } = ResultType.String;

        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection Header { get; set; } = new WebHeaderCollection();
        #endregion

        #region ip-port
        /// <summary>
        /// 设置本地的出口ip和端口
        /// </summary>]
        /// <example>
        ///item.IPEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.1"),80);
        /// </example>
        public IPEndPoint IPEndPoint { get; set; } = null;
        #endregion

        #region config

        /// <summary>
        /// 当出现"请求被中止: 未能创建 SSL/TLS 安全通道"时需要配置此属性 
        /// </summary>
        public SecurityProtocolType SecurityProtocol { get; set; }

        /// <summary>
        /// 是否重置request,response的值，默认不重置，当设置为True时request,response将被设置为Null
        /// </summary>
        public bool IsReset { get; set; } = false;
        #endregion

    }
}
