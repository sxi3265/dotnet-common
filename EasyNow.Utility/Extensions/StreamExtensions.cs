using System.IO;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

namespace EasyNow.Utility.Extensions
{
    /// <summary>
    /// 流扩展
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// 得到MD5加密后的数据
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ToMD5String([NotNull]this Stream stream)
        {
            using (var md5=MD5.Create())
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                var s = md5.ComputeHash(stream);
                var sb=new StringBuilder();
                s.Foreach(e => { sb.Append(e.ToString("X2")); });
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 获取SHA1
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ToSha1String([NotNull] this Stream stream)
        {
            using var sha1 = SHA1.Create();
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            var s = sha1.ComputeHash(stream);
            var sb=new StringBuilder();
            s.Foreach(e => { sb.Append(e.ToString("X2")); });
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            return sb.ToString();
        }
    }
}