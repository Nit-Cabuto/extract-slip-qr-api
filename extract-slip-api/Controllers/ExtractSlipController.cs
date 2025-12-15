using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Threading.Tasks;
using extract_slip_api.Models;
using extract_slip_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;
using ZXing.Windows.Compatibility;

namespace extract_slip_api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExtractSlipController : ControllerBase
    {
        private readonly ILogger<ExtractSlipController> _logger;
        private readonly ExtractSlipService _extractSlipService;

        public ExtractSlipController(ILogger<ExtractSlipController> logger, ExtractSlipService extractSlipService)
        {
            _logger = logger;
            _extractSlipService = extractSlipService;
        }

        [HttpPost("ExtractTextImage")]
        public async Task<ResultData> ExtractTextImage(IFormFile file)
        {
            var result = new ResultData();

            if (file != null || file!.Length > 0)
            {
                result =  await _extractSlipService.ExtractService(file);
            }
            else
            {
                result.status = 200;
                result.message = "No File";
            }

            return result;
        }

        [HttpPost("ReadQRCode")]
        [SupportedOSPlatform("windows")]
        public async Task<ResultData> ExtractQRCode(IFormFile file)
        {
            var result = new ResultData();

            if (file != null || file!.Length > 0)
            {
                result = await _extractSlipService.ExtractQRCode(file);
            }
            else
            {
                result.status = 200;
                result.message = "No File";
            }

            return result;
        }

        // [HttpPost("ExtractSlipQRCodeExternal")]
        // public async Task<IActionResult> ExtractQRCodeExternal(IFormFile file)
        // {
        //     //var resultData = new ResultData();

        //     if (file != null || file!.Length > 0)
        //     {
        //         //resultData.result =  await _extractSlipService.ExtractQRCodeExternalAPI(file);
        //         var client = new RestClient("https://developer.easyslip.com/api/v1/verify");
        //         var request = new RestRequest("", Method.Post);
        //         request.AlwaysMultipartFormData = true;

        //         request.AddHeader("authorization", "Bearer 81cae111-e8de-4266-b21c-1aa364d0ecfb");

        //         using (var stream = file.OpenReadStream())
        //         using (var memoryStream = new MemoryStream())
        //         {
        //             await stream.CopyToAsync(memoryStream);
        //             memoryStream.Position = 0;
        //             request.AddFile("file", memoryStream.ToArray(), file.FileName, file.ContentType);
        //         }

        //         _logger.LogInformation("Sending request to EasySlip API for file: " + file.FileName);

        //         var res = await client.ExecuteAsync(request);

        //         if (!res.IsSuccessful) throw new Exception(res.Content);

        //         var doc = JsonDocument.Parse(res.Content!);

        //         var content = doc.RootElement
        //                                     .GetProperty("status")
        //                                     .GetProperty("data")
        //                                     .GetString()!;

        //         return Content(content);                        

        //         //return Content(res.Content!, "application/json");
        //         //return Ok( new {Content = res.Content!, ContentType = "application/json" });
        //     }

        //     return Ok();
        // }
   
        [HttpPost("ExtractSlipQRCodeExternal")]
        public async Task<ResultData> ExtractQRCodeExternal(IFormFile file)
        {
            var resultData = new ResultData();

            if (file != null || file!.Length > 0)
            {
                resultData.result =  await _extractSlipService.ExtractQRCodeExternalAPI(file);
            }
            else
            {
                resultData.status = 200;
                resultData.message = "No File";
            }

            return resultData;
        }
    }
}