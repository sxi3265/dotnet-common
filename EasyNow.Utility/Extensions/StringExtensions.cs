using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using EasyNow.Utility.Tools;
using Newtonsoft.Json;

namespace EasyNow.Utility.Extensions
{
    /// <summary>
    /// The string extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 得到MD5加密后的数据
        /// </summary>
        /// <param name="plaintext">
        /// </param>
        /// <param name="encoding">
        /// 默认为<see cref="Encoding.Unicode"/>
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToMD5String(this string plaintext, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            var cl1 = plaintext;
            using (var md5 = MD5.Create())
            {
                var s = md5.ComputeHash(encoding.GetBytes(cl1));
                var sb = new StringBuilder();
                s.Foreach(e => { sb.Append(e.ToString("X2")); });
                return sb.ToString();
            }
        }

        /// <summary>
        /// 通配符匹配
        /// </summary>
        /// <param name="wildcard"></param>
        /// <param name="text"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static bool IsMatchWildcard(this string text, string wildcard, bool ignoreCase = false)
        {
            var sb = new StringBuilder(wildcard.Length + 10);
            sb.Append("^");
            sb.Append(string.Join(string.Empty, wildcard.Select(e =>
             {
                 switch (e)
                 {
                     case '*':
                         return ".*";
                     default:
                         return Regex.Escape(e.ToString());
                 }
             })));
            sb.Append("$");
            Regex regex;
            if (!ignoreCase)
            {
                regex = new Regex(sb.ToString(),
                    RegexOptions.None);
            }
            else
            {
                regex = new Regex(sb.ToString(),
                    RegexOptions.IgnoreCase);
            }
            return regex.IsMatch(text);
        }

        /// <summary>
        /// 从json字符串转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string jsonStr, object jsonSerializerSettings)
        {
            if (jsonSerializerSettings != null && jsonSerializerSettings is JsonSerializerSettings settings)
            {
                return JsonConvert.DeserializeObject<T>(jsonStr, settings);
            }
            return JsonConvert.DeserializeObject<T>(jsonStr, JsonTool.JsonSerializerSettings);
        }

        /// <summary>
        /// 从json字符串转换为对象
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="type"></param>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns></returns>
        public static object FromJson(this string jsonStr, Type type, object jsonSerializerSettings)
        {
            if (jsonSerializerSettings != null && jsonSerializerSettings is JsonSerializerSettings settings)
            {
                return JsonConvert.DeserializeObject(jsonStr, type, settings);
            }
            return JsonConvert.DeserializeObject(jsonStr, type, JsonTool.JsonSerializerSettings);
        }

        /// <summary>
        /// 从json字符串转换为对象
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object FromJson(this string jsonStr, Type type)
        {
            return jsonStr.FromJson(type, null);
        }

        /// <summary>
        /// 从json字符串转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string jsonStr)
        {
            return jsonStr.FromJson<T>(null);
        }

        /// <summary>
        /// 当前base64字符串存在异常时，进行修复
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToFixedBase64Str(this string str)
        {
            return str.Replace('-', '+').Replace('_', '/').PadRight(4 * ((str.Length + 3) / 4), '=');
        }

        public static Type GetTypeByName(this string typeName, string assemblyName=null)
        {
            var type = Type.GetType(typeName, false);
            if (type != null)
                return type;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // try to find manually
            foreach (Assembly asm in assemblies)
            {
                type = asm.GetType(typeName, false);

                if (type != null)
                    break;
            }
            if (type != null)
                return type;

            // see if we can load the assembly
            if (!string.IsNullOrEmpty(assemblyName))
            {
                var a = assemblyName.GetAssembly();
                if (a != null)
                {
                    type = Type.GetType(typeName, false);
                    if (type != null)
                        return type;
                }
            }

            return null;
        }

        public static Assembly GetAssembly(this string assemblyName)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch { }

            if (assembly != null)
                return assembly;

            if (File.Exists(assemblyName))
            {
                assembly = Assembly.LoadFrom(assemblyName);
                if (assembly != null)
                    return assembly;
            }
            return null;
        }

        public static string ToSha256String(this string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = SHA256.Create().ComputeHash(bytes);
 
            var builder = new StringBuilder();
            foreach (var b in hash)
            {
                builder.Append(b.ToString("X2"));
            }
 
            return builder.ToString();
        }
    }
}