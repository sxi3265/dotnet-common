using System;
using System.Threading.Tasks;
using EasyNow.Dto;
using Org.BouncyCastle.Asn1.Ocsp;
using Refit;

namespace EasyNow.ApiClient.Getui
{
    public interface IGetuiApi
    {
        /// <summary>
        /// 认证
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Post("/auth")]
        Task<ReturnResult<int,AuthResp>> Auth([Body]AuthReq req);

        /// <summary>
        /// 单推
        /// </summary>
        /// <param name="token"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        [Post("/push/single/cid")]
        Task<ReturnResult<int,dynamic>> PushSingle([Header("token")]string token,[Body]PushReq req);
    }
}
