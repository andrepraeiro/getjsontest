using System;
using System.IO;
using System.Net;

namespace getjsontest
{

    public enum TypeData {
        Text = 0,
        Binary = 1,
        Undefined = 3,
    }
    class Program
    {
        private static string[] _args;
        private static string url;
        private static string file;
        private static string _typeData;
        private static string proxy;
        private static string userProxy;
        private static string passProxy;
        private const string binaryData = "binary";
        private const string textData = "text";
        private static TypeData typeData;
        static void Main(string[] args)
        {
            _args = args;
            if (!ValidarArgs()) return;
            try
            {
                using (WebClient wc = new WebClient())
                {
                    SetSSL();
                    var p = GetProxy();
                    wc.Proxy = p;
                    var path = file;
                    switch (typeData)
                    {
                        case TypeData.Text:
                            {
                                var json = wc.DownloadString(url);                                                                
                                File.WriteAllText(path, json);
                                break;
                            }
                        case TypeData.Binary:
                            {
                                wc.DownloadFile(url,path);                                
                                break;
                            }
                        default: throw new Exception("typeData not defined");
                    }

                    //Console.WriteLine(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void SetSSL()
        {
            if (url.ToLower().Contains("https"))
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            }
        }

        private static WebProxy GetProxy()
        {
            if (proxy != null)
            {
                WebProxy p = new WebProxy(proxy, true);
                p.Credentials = new NetworkCredential(userProxy, passProxy);
                WebRequest.DefaultWebProxy = p;
                return p;                
            }
            return null;
        }

        private static void SetValues(int i)
        {
            switch (i)
            {
                case 0: url = _args[i]; break;
                case 1: file = _args[i]; break;
                case 2: { _typeData = _args[i].ToLower(); SetTypeData(); break; };
                case 3: proxy = _args[i]; break;
                case 4: userProxy = _args[i]; break;
                case 5: passProxy = _args[i]; break;
            }
        }

        private static void SetTypeData()
        {
            typeData = _typeData == binaryData ? TypeData.Binary : _typeData == textData ? TypeData.Text : TypeData.Undefined;
        }

        private static bool ValidarArgs()
        {
            if (_args == null) return false;
            if (_args[0] == null) return false;
            if (_args[1] == null) return false;
            for(int i = 0; i< _args.GetLength(0); i++)
            {
                SetValues(i);
            }            
            return true;            
        }
    }
}
