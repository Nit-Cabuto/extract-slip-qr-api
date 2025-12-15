using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Threading.Tasks;
using extract_slip_api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using RestSharp;
using ZXing.Windows.Compatibility;

namespace extract_slip_api.Services
{
    public class ExtractSlipService
    {
        private readonly ILogger<ExtractSlipService> _logger;
        public ExtractSlipService(ILogger<ExtractSlipService> logger)
        {
            _logger = logger;
        }

        public async Task<ResultData> ExtractService(IFormFile file)
        {
            var result = new ResultData();
            // Implementation goes here
            var client = new HttpClient();
            string content = null!;

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var apiKey = "sk-kLAANJHQv4LYw8XiYIETjXT1XpjP4nJJQPxOkc9HqUEM1U4L";
                    var imagePath = @"C:\Users\PThongma\Downloads\messageImage_1765190227406.jpg";
                    var model = "typhoon-ocr";
                    var taskType = "default";
                    var maxTokens = 16000;
                    var temperature = 0.1;
                    var topP = 0.6;
                    var repetitionPenalty = 1.1;
                    // Process the file stream as needed
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);

                        var imageContent = new ByteArrayContent(memoryStream.ToArray());

                        using (var formData = new MultipartFormDataContent())
                        {
                            formData.Add(imageContent, "file", Path.GetFileName(imagePath));
                            formData.Add(new StringContent(model), "model");
                            formData.Add(new StringContent(taskType), "task_type");
                            formData.Add(new StringContent(maxTokens.ToString()), "max_tokens");
                            formData.Add(new StringContent(temperature.ToString()), "temperature");
                            formData.Add(new StringContent(topP.ToString()), "top_p");
                            formData.Add(new StringContent(repetitionPenalty.ToString()), "repetition_penalty");

                            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                            var response = await client.PostAsync("https://api.opentyphoon.ai/v1/ocr", formData);

                            if (response.IsSuccessStatusCode)
                            {
                                var resultContent = await response.Content.ReadAsStringAsync();

                                var doc = JsonDocument.Parse(resultContent);

                                content = doc.RootElement
                                            .GetProperty("results")[0]
                                            .GetProperty("message")
                                            .GetProperty("choices")[0]
                                            .GetProperty("message")
                                            .GetProperty("content")
                                            .GetString()!;

                                result.result = content.Split('\n');            
                            }
                            else
                            {
                                //Console.WriteLine($"Error: {response.StatusCode}");
                                var errorContent = await response.Content.ReadAsStringAsync();
                                //Console.WriteLine($"Error details: {errorContent}");

                                result.message = $"Error: {errorContent}";
                                result.status = (int)response.StatusCode;
                            }

                            result.status = 200;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error ExtractSlipController ExtractTextImage : " + ex.Message);
                result.status = 500;
                result.message = ex.Message;
            }

            return result;
        }

        [SupportedOSPlatform("windows")]
        public async Task<ResultData> ExtractQRCode(IFormFile file)
        {
            var resultData = new ResultData();
            var reader = new BarcodeReader();

            try
            {
                var stream = file.OpenReadStream();
                var barcodeBitmap = (Bitmap)Image.FromStream(stream);
                reader.Options.TryHarder = true;
                
                resultData.result = reader.Decode(barcodeBitmap).Text;
                resultData.status = 200;
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error ExtractSlipController ReadQRCode : " + ex.Message);
                resultData.status = 500;
                resultData.message = ex.Message;
            }

            return resultData;
        }

        public async Task<ResultData> ExtractQRCodeExternalAPI(IFormFile file)
        {
            var resultData = new ResultData();

            try
            {
                var client = new RestClient("https://developer.easyslip.com/api/v1/verify");
                var request = new RestRequest("", Method.Post);
                request.AlwaysMultipartFormData = true;

                request.AddHeader("authorization", "Bearer 81cae111-e8de-4266-b21c-1aa364d0ecfb");

                using (var stream = file.OpenReadStream())
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    request.AddFile("file", memoryStream.ToArray(), file.FileName, file.ContentType);
                }

                _logger.LogInformation("Sending request to EasySlip API for file: " + file.FileName);

                var res = await client.ExecuteAsync(request);

                if (!res.IsSuccessful) throw new Exception(res.Content);

                //var doc = JsonDocument.Parse(res.Content!);

                resultData.result = res.Content;
              
                //return Content(res.Content!, "application/json");
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error ExtractSlipController ReadQRCode : " + ex.Message);
                resultData.status = 500;
                resultData.message = ex.Message;
            }

            return resultData;
        }
    }
}