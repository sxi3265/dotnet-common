using System.Collections.Generic;
using System.IO;

namespace EasyNow.File
{
    public interface IFileHelper
    {
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="stream">文件流</param>
        /// <param name="metadata">元数据</param>
        /// <param name="sameMd5NotSave">md5相同文件不再存储</param>
        /// <returns>文件id</returns>
        string SaveFile(string filename, Stream stream, Dictionary<string, object> metadata = null, bool sameMd5NotSave = true);

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="bytes">文件字节数组</param>
        /// <param name="metadata">元数据</param>
        /// <param name="sameMd5NotSave">md5相同文件不再存储</param>
        /// <returns>文件id</returns>
        string SaveFile(string filename, byte[] bytes, Dictionary<string, object> metadata = null, bool sameMd5NotSave = true);

        /// <summary>
        /// 批量保存文件
        /// </summary>
        /// <param name="files">包括文件名、文件流和元数据的元组数组</param>
        /// <param name="sameMd5NotSave">md5相同文件不再存储</param>
        /// <returns>文件id数组(按照传入文件顺序返回)</returns>
        string[] SaveFiles((string filename, Stream stream, Dictionary<string, object> metadata)[] files,bool sameMd5NotSave = true);

        /// <summary>
        /// 批量保存文件
        /// </summary>
        /// <param name="files">包括文件名、文件字节数组和元数据的元组数组</param>
        /// <param name="sameMd5NotSave">md5相同文件不再存储</param>
        /// <returns>文件id数组(按照传入文件顺序返回)</returns>
        string[] SaveFiles((string filename, byte[] bytes, Dictionary<string, object> metadata)[] files,bool sameMd5NotSave = true);

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="id">文件id</param>
        /// <returns>文件字节数组与元数据</returns>
        (byte[] bytes,string filename, Dictionary<string, object> metadata) GetFile(string id);

        /// <summary>
        /// 查找文件
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        (string id,byte[] bytes,string filename, Dictionary<string, object> metadata)[] FindFiles(Dictionary<string,object> metadata);

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <param name="id">文件id</param>
        /// <returns>文件流与元数据</returns>
        (Stream stream,string filename, Dictionary<string, object> metadata) GetFileStream(string id);

        /// <summary>
        /// 批量获取文件
        /// </summary>
        /// <param name="ids">文件id数组</param>
        /// <returns>文件字节数组与元数据的元组数组(按照传入文件id顺序返回)</returns>
        (byte[] bytes,string filename, Dictionary<string, object> metadata)[] GetFiles(string[] ids);

        /// <summary>
        /// 批量获取文件
        /// </summary>
        /// <param name="ids">文件id数组</param>
        /// <returns>文件流与元数据的元组数组(按照传入文件id顺序返回)</returns>
        (Stream stream,string filename, Dictionary<string, object> metadata)[] GetFileStreams(string[] ids);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="id">文件id</param>
        /// <returns>删除结果</returns>
        bool DeleteFile(string id);

        /// <summary>
        /// 批量删除文件
        /// </summary>
        /// <param name="ids">文件id数组</param>
        /// <returns>全部删除成功则返回true，否则返回false</returns>
        bool DeleteFiles(string[] ids);

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="id">文件id</param>
        /// <returns>存在返回true，否则返回false</returns>
        bool FileExist(string id);

        /// <summary>
        /// 批量查看文件是否存在
        /// </summary>
        /// <param name="ids">文件id数组</param>
        /// <returns>存在结果数组(按照传入文件id顺序返回)</returns>
        bool[] FilesExist(string[] ids);

        /// <summary>
        /// 更新文件
        /// </summary>
        /// <remarks>如果传入的文件id已存在，则更新并返回文件id；否则保存文件并返回文件id</remarks>
        /// <param name="id">文件id</param>
        /// <param name="filename">文件名</param>
        /// <param name="bytes">文件字节数组</param>
        /// <param name="metadata">元数据</param>
        /// <returns>文件id</returns>
        string UpdateFile(string id, string filename, byte[] bytes, Dictionary<string, object> metadata = null);

        /// <summary>
        /// 批量更新文件
        /// </summary>
        /// <remarks>如果传入的文件id已存在，则更新并返回文件id；否则保存文件并返回文件id</remarks>
        /// <param name="fileIdDic">key为文件id，value包括文件名、文件流和元数据</param>
        /// <returns>文件id数组(按照传入文件id顺序返回)</returns>
        string[] UpdateFiles(
            Dictionary<string, (string filename, Stream stream, Dictionary<string, object> metadata)> fileIdDic);

        /// <summary>
        /// 批量更新文件
        /// </summary>
        /// <remarks>如果传入的文件id已存在，则更新并返回文件id；否则保存文件并返回文件id</remarks>
        /// <param name="fileIdDic">key为文件id，value包括文件名、文件字节数组和元数据</param>
        /// <returns>文件id数组(按照传入文件id顺序返回)</returns>
        string[] UpdateFiles(
            Dictionary<string, (string filename, byte[] bytes, Dictionary<string, object> metadata)> fileIdDic);
    }
}