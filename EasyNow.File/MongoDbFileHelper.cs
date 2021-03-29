using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyNow.Utility.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace EasyNow.File
{
    public class MongoDbFileHelper : IFileHelper
    {
        private readonly GridFSBucket _gridFsBucket;

        public MongoDbFileHelper(string connectionString, string dbName, string bucketName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);

            _gridFsBucket = new GridFSBucket(database, new GridFSBucketOptions
            {
                BucketName = bucketName,
                ChunkSizeBytes = 10485760, // 10MB
            });
        }

        public string SaveFile(string filename, Stream stream, Dictionary<string, object> metadata = null, bool sameMd5NotSave = true)
        {
            if (sameMd5NotSave)
            {
                var md5Str = stream.ToMD5String().ToLower();
                var file = _gridFsBucket.Find(Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(f => f.MD5, md5Str))
                    .FirstOrDefault();
                if (file!=null)
                {
                    if (file.Metadata.Contains("realId"))
                    {
                        return file.Metadata["realId"].AsString;
                    }
                    file.Metadata["realId"] = Guid.NewGuid().ToString("D");
                    _gridFsBucket.Database
                        .GetCollection<GridFSFileInfo<ObjectId>>($"{_gridFsBucket.Options.BucketName}.files")
                        .UpdateOne(Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(f => f.MD5, md5Str),
                            Builders<GridFSFileInfo<ObjectId>>.Update.Set(s => s.Metadata, file.Metadata));
                    return file.Metadata["realId"].AsString;
                }

                stream.Position = 0;
            }
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument(metadata??new Dictionary<string, object>())
            };
            var id = Guid.NewGuid().ToString("D");
            if (options.Metadata.Contains("readId"))
            {
                options.Metadata["realId"] = id;
            }
            else
            {
                options.Metadata.Add("realId", id);
            }
            
            _gridFsBucket.UploadFromStream(filename, stream, options);
            return id;
        }

        public string SaveFile(string filename, byte[] bytes, Dictionary<string, object> metadata = null, bool sameMd5NotSave = true)
        {
            if (sameMd5NotSave)
            {
                var md5Str = bytes.ToMD5String().ToLower();
                var file = _gridFsBucket.Find(Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(f => f.MD5, md5Str))
                    .FirstOrDefault();
                if (file!=null)
                {
                    return file.Metadata["realId"].AsString;
                }
            }
            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument(metadata??new Dictionary<string, object>())
            };
            var id = Guid.NewGuid().ToString("D");
            if (options.Metadata.Contains("readId"))
            {
                options.Metadata["realId"] = id;
            }
            else
            {
                options.Metadata.Add("realId", id);
            }
            _gridFsBucket.UploadFromBytes(filename, bytes, options);
            return id;
        }

        public string[] SaveFiles((string filename, Stream stream, Dictionary<string, object> metadata)[] files, bool sameMd5NotSave = true)
        {
            var result = new string[files.Length];
            Parallel.ForEach(files, (file, state, index) =>
            {
                var id=this.SaveFile(file.filename, file.stream, file.metadata,sameMd5NotSave);
                result[index] = id;
            });
            return result;
        }

        public string[] SaveFiles((string filename, byte[] bytes, Dictionary<string, object> metadata)[] files, bool sameMd5NotSave = true)
        {
            var result = new string[files.Length];
            Parallel.ForEach(files, (file, state, index) =>
            {
                var id=this.SaveFile(file.filename, file.bytes, file.metadata,sameMd5NotSave);
                result[index] = id;
            });
            return result;
        }

        public (byte[] bytes,string filename, Dictionary<string, object> metadata) GetFile(string id)
        {
            var fileinfo = _gridFsBucket.Find(Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(f => f.Metadata["realId"], id))
                .FirstOrDefault();
            if (fileinfo != null)
            {
                var bytes = _gridFsBucket.DownloadAsBytes(fileinfo.Id);
                return (bytes,fileinfo.Filename, fileinfo.Metadata.ToDictionary());
            }

            return (null,null, null);
        }

        public (string id, byte[] bytes, string filename, Dictionary<string, object> metadata)[] FindFiles(Dictionary<string, object> metadata)
        {
            FilterDefinition<GridFSFileInfo<ObjectId>> filter=null;
            foreach (var item in metadata)
            {
                if (filter == null)
                {
                    filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(f => f.Metadata[item.Key], item.Value);
                }
                else
                {
                    filter=filter&Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(f => f.Metadata[item.Key], item.Value);
                }
            }

            var fileInfos = this._gridFsBucket.Find(filter).ToList();
            return fileInfos.Select(e =>
            {
                var bytes = _gridFsBucket.DownloadAsBytes(e.Id);
                return (e.Metadata["realId"].AsString,bytes,e.Filename,e.Metadata.ToDictionary());
            }).ToArray();
        }

        public (Stream stream, string filename, Dictionary<string, object> metadata) GetFileStream(string id)
        {
            var fileinfo = _gridFsBucket.Find(Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(f => f.Metadata["realId"], id))
                .FirstOrDefault();
            if (fileinfo != null)
            {
                var ms = new MemoryStream();
                _gridFsBucket.DownloadToStream(fileinfo.Id, ms);
                ms.Position = 0;
                return (ms,fileinfo.Filename, fileinfo.Metadata.ToDictionary());
            }

            return (null,null, null);
        }

        public (byte[] bytes, string filename, Dictionary<string, object> metadata)[] GetFiles(string[] ids)
        {
            var fileInfos = _gridFsBucket
                .Find(Builders<GridFSFileInfo<ObjectId>>.Filter.In(f => f.Metadata["realId"],
                    ids.Select(e => new BsonString(e)))).ToList();
            return ids.Select(id =>
            {
                var fileInfo = fileInfos.FirstOrDefault(e => e.Metadata["realId"] == id);
                if (fileInfo == null)
                {
                    return (null,null,null);
                }
                var bytes = _gridFsBucket.DownloadAsBytes(fileInfo.Id);
                return (bytes,fileInfo.Filename, fileInfo.Metadata.ToDictionary());
            }).ToArray();
        }

        public (Stream stream, string filename, Dictionary<string, object> metadata)[] GetFileStreams(string[] ids)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteFile(string id)
        {
            var fileinfo = _gridFsBucket.Find(Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(f => f.Metadata["realId"], id))
                .FirstOrDefault();
            if (fileinfo != null)
            {
                try
                {
                    _gridFsBucket.Delete(fileinfo.Id);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public bool DeleteFiles(string[] ids)
        {
            var fileInfos = _gridFsBucket
                .Find(Builders<GridFSFileInfo<ObjectId>>.Filter.In(f => f.Metadata["realId"],
                    ids.Select(e => new BsonString(e)))).ToList();
            foreach (var fileInfo in fileInfos)
            {
                try
                {
                    _gridFsBucket.Delete(fileInfo.Id);
                }
                catch
                {
                    return false;
                }

                return true;
            }

            return false;
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