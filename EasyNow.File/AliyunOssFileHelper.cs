using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Aliyun.OSS;
using EasyNow.Utility.Extensions;

namespace EasyNow.File
{
    public class AliyunOssFileHelper:IFileHelper
    {
        private readonly OssClient _ossClient;
        private readonly string _bucketName;
        public AliyunOssFileHelper(string endpoint, string accessKeyId,string accessKeySecret, string bucketName)
        {
            _ossClient = new OssClient(endpoint, accessKeyId, accessKeySecret);
            _bucketName = bucketName;
        }

        public string SaveFile(string filename, Stream stream, Dictionary<string, object> metadata = null, bool sameFileNotSave = true)
        {
            var sha1Str = stream.ToSha1String().ToLower();
            if (sameFileNotSave)
            {
                if (_ossClient.DoesObjectExist(this._bucketName, sha1Str))
                {
                    return sha1Str;
                }

                stream.Seek(0, SeekOrigin.Begin);
            }

            var meta = new ObjectMetadata();
            if (metadata != null && metadata.Any())
            {
                metadata.Foreach(e =>
                {
                    meta.UserMetadata.Add(e.Key,WebUtility.UrlEncode(e.Value.ToString()));
                });
            }
            meta.AddHeader("filename",WebUtility.UrlEncode(filename));
            meta.ContentDisposition = $"attachment; filename* = UTF-8''{WebUtility.UrlEncode(filename)}";
            _ossClient.PutObject(this._bucketName, sha1Str, stream, meta);
            return sha1Str;
        }

        public string SaveFile(string filename, byte[] bytes, Dictionary<string, object> metadata = null, bool sameFileNotSave = true)
        {
            throw new System.NotImplementedException();
        }

        public string[] SaveFiles((string filename, Stream stream, Dictionary<string, object> metadata)[] files, bool sameFileNotSave = true)
        {
            throw new System.NotImplementedException();
        }

        public string[] SaveFiles((string filename, byte[] bytes, Dictionary<string, object> metadata)[] files, bool sameFileNotSave = true)
        {
            throw new System.NotImplementedException();
        }

        public (byte[] bytes, string filename, Dictionary<string, object> metadata) GetFile(string id)
        {
            if (!_ossClient.DoesObjectExist(this._bucketName, id))
            {
                return (null, null, null);
            }

            var ossObject = _ossClient.GetObject(this._bucketName, id);
            var bytes = new byte[ossObject.ContentLength];
            ossObject.Content.Read(bytes, 0, bytes.Length);
            var fileName = "未命名文件";
            if (ossObject.Metadata.UserMetadata.ContainsKey("filename"))
            {
                fileName = WebUtility.UrlDecode(ossObject.Metadata.UserMetadata["filename"]);
            }

            return (bytes, fileName,
                ossObject.Metadata.UserMetadata.ToDictionary(e => e.Key, e => (object) WebUtility.UrlDecode(e.Value)));
        }

        public (string id, byte[] bytes, string filename, Dictionary<string, object> metadata)[] FindFiles(Dictionary<string, object> metadata)
        {
            throw new System.NotImplementedException();
        }

        public (Stream stream, string filename, Dictionary<string, object> metadata) GetFileStream(string id)
        {
            throw new System.NotImplementedException();
        }

        public (byte[] bytes, string filename, Dictionary<string, object> metadata)[] GetFiles(string[] ids)
        {
            throw new System.NotImplementedException();
        }

        public (Stream stream, string filename, Dictionary<string, object> metadata)[] GetFileStreams(string[] ids)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteFile(string id)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteFiles(string[] ids)
        {
            throw new System.NotImplementedException();
        }

        public bool FileExist(string id)
        {
            throw new System.NotImplementedException();
        }

        public bool[] FilesExist(string[] ids)
        {
            throw new System.NotImplementedException();
        }

        public string UpdateFile(string id, string filename, byte[] bytes, Dictionary<string, object> metadata = null)
        {
            throw new System.NotImplementedException();
        }

        public string[] UpdateFiles(Dictionary<string, (string filename, Stream stream, Dictionary<string, object> metadata)> fileIdDic)
        {
            throw new System.NotImplementedException();
        }

        public string[] UpdateFiles(Dictionary<string, (string filename, byte[] bytes, Dictionary<string, object> metadata)> fileIdDic)
        {
            throw new System.NotImplementedException();
        }
    }
}