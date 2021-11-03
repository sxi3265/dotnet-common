using System.Security.Cryptography;
using System.Text;

namespace EasyNow.Utility.Extensions
{
    public static class ByteExtensions
    {
        /// <summary>
        /// 得到MD5加密后的数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToMD5String(this byte[] bytes)
        {
            using (var md5=MD5.Create())
            {
                var s = md5.ComputeHash(bytes);
                var sb=new StringBuilder();
                s.Foreach(e => { sb.Append(e.ToString("X2")); });
                return sb.ToString();
            }
        }

        /// <summary>
        /// 获取SHA1
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToSha1String(this byte[] bytes)
        {
            using var sha1 = SHA1.Create();
            var s = sha1.ComputeHash(bytes);
            var sb=new StringBuilder();
            s.Foreach(e => { sb.Append(e.ToString("X2")); });
            return sb.ToString();
        }
    }
}