using System;
using System.Text;
using AutoMapper;
using EasyNow.Utility.Extensions;
using Newtonsoft.Json;

namespace EasyNow.Utility
{
    /// <inheritdoc />
    public class UtilityProfile:Profile
    {
        /// <inheritdoc />
        public UtilityProfile()
        {
            CreateMap<JsonSerializerSettings, JsonSerializerSettings>();
            //CreateMap<object, Guid?>().ConvertUsing(s =>s==null|| s is string && string.IsNullOrEmpty((string) s) ? (Guid?) null : Guid.Parse(s.ToString()));
            CreateMap<string, Guid>().ConvertUsing(s=>string.IsNullOrEmpty(s) ? Guid.Empty : Guid.Parse(s));
            CreateMap<string, Guid?>().ConvertUsing(s =>string.IsNullOrEmpty(s) ? (Guid?) null : Guid.Parse(s));
            CreateMap<Guid?, string>().ConvertUsing(g => g != null ? g.Value.ToString("N") : null);
            CreateMap<Guid, string>().ConvertUsing(g => g.ToString("N"));
            CreateMap<DateTime, string>().ConstructUsing(s => s.ToString("yyyy-MM-dd HH:mm:ss"));
            CreateMap<byte[], string>().ConstructUsing(s => s==null?null:Encoding.UTF8.GetString(s));
            CreateMap<string, byte[]>().ConstructUsing(s => s==null?null:Encoding.UTF8.GetBytes(s));
            CreateMap<string,int>().ConvertUsing(s=>Convert.ToInt32(s));
            //CreateMap<object,sbyte>().ConvertUsing(s=>Convert.ToSByte(s));
            //CreateMap<object,byte>().ConvertUsing(s=>Convert.ToByte(s));
            //CreateMap<object,short>().ConvertUsing(s=>Convert.ToInt16(s));
            //CreateMap<object,int>().ConvertUsing(s=>Convert.ToInt32(s));
            //CreateMap<object,long>().ConvertUsing(s=>Convert.ToInt64(s));
            //CreateMap<object,ushort>().ConvertUsing(s=>Convert.ToUInt16(s));
            //CreateMap<object,uint>().ConvertUsing(s=>Convert.ToUInt32(s));
            //CreateMap<object,ulong>().ConvertUsing(s=>Convert.ToUInt64(s));
            //CreateMap<object,double>().ConvertUsing(s=>Convert.ToDouble(s));
            //CreateMap<object,float>().ConvertUsing(s=>Convert.ToSingle(s));
            //CreateMap<object,decimal>().ConvertUsing(s=>Convert.ToDecimal(s));
            CreateMap<string,DateTime>().ConvertUsing(s=>Convert.ToDateTime(s));
            //CreateMap<object,bool>().ConvertUsing(s=>Convert.ToBoolean(s));
            //CreateMap<object,string>().ConvertUsing(s=>s==null?null:Convert.ToString(s));
            //CreateMap<object,sbyte?>().ConvertUsing(s=>s != null ? s.To<sbyte>() : (sbyte?) null);
            //CreateMap<object,byte?>().ConvertUsing(s=>s != null ? s.To<byte>() : (byte?) null);
            //CreateMap<object,short?>().ConvertUsing(s=>s != null ? s.To<short>() : (short?) null);
            //CreateMap<object,int?>().ConvertUsing(s=>s != null ? s.To<int>() : (int?) null);
            //CreateMap<object,long?>().ConvertUsing(s=>s != null ? s.To<long>() : (long?) null);
            //CreateMap<object,ushort?>().ConvertUsing(s=>s != null ? s.To<ushort>() : (ushort?) null);
            //CreateMap<object,uint?>().ConvertUsing(s=>s != null ? s.To<uint>() : (uint?) null);
            //CreateMap<object,ulong?>().ConvertUsing(s=>s != null ? s.To<ulong>() : (ulong?) null);
            //CreateMap<object,double?>().ConvertUsing(s=>s != null ? s.To<double>() : (double?) null);
            //CreateMap<object,float?>().ConvertUsing(s=>s != null ? s.To<float>() : (float?) null);
            //CreateMap<object,decimal?>().ConvertUsing(s=>s != null ? s.To<decimal>() : (decimal?) null);
            CreateMap<string,DateTime?>().ConvertUsing(s=>s != null ? s.To<DateTime>() : (DateTime?) null);
            //CreateMap<object,bool?>().ConvertUsing(s=>s != null ? s.To<bool>() : (bool?) null);
        }
    }
}
