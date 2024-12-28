using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace KuaförRandevuSistemi.Controllers
{
    public class AIController : Controller
    {
        private readonly string _replicateApiToken;

        public AIController()
        {
            // Retrieve the API key from the environment
            _replicateApiToken = Environment.GetEnvironmentVariable("REPLICATE_API_TOKEN");
            if (string.IsNullOrEmpty(_replicateApiToken))
            {
                throw new InvalidOperationException("Replicate API Token is not set in the environment.");
            }
        }

        [HttpGet]
        public IActionResult UploadImage()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> GenerateHairstyles(IFormFile image, string prompt)
        {
            if (image == null || image.Length == 0)
            {
                TempData["ErrorMessage"] = "Lütden uygun bir resim seçin.";
                return RedirectToAction("UploadImage");
            }

            if (string.IsNullOrEmpty(prompt))
            {
                TempData["ErrorMessage"] = "Lütfen bir prompt girin.";
                return RedirectToAction("UploadImage");
            }

            try
            {
                var imageUrl = await UploadImageToTemporaryStorage(image);
                if (imageUrl == null)
                {
                    TempData["ErrorMessage"] = "Resim yüklenemedi.";
                    return RedirectToAction("UploadImage");
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", _replicateApiToken);

                    var requestBody = new
                    {
                        input = new
                        {
                            prompt = prompt,
                            prompt_strength = 0.55,
                            image = imageUrl
                        }
                    };

                    var response = await client.PostAsync(
                        "https://api.replicate.com/v1/models/stability-ai/stable-diffusion-3.5-large/predictions",
                        new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));

                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Saç stili yüklenemedi. Lütfen tekrar deneyin.";
                        return RedirectToAction("UploadImage");
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);

                    var status = result.status.ToString();
                    var getUrl = result.urls.get.ToString();

                    while (status == "starting" || status == "processing")
                    {
                        await Task.Delay(2000);
                        var statusResponse = await client.GetAsync(getUrl);

                        if (!statusResponse.IsSuccessStatusCode)
                        {
                            TempData["ErrorMessage"] = "Failed to retrieve prediction status.";
                            return RedirectToAction("UploadImage");
                        }

                        var statusJson = await statusResponse.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject(statusJson);
                        status = result.status.ToString();
                    }

                    if (status != "succeeded")
                    {
                        TempData["ErrorMessage"] = "The prediction failed. Please try again.";
                        return RedirectToAction("UploadImage");
                    }

                    var outputUrls = new List<string>();
                    foreach (var output in result.output)
                    {
                        outputUrls.Add(output.ToString());
                    }

                    ViewBag.UploadedImageUrl = imageUrl; // Pass the uploaded image URL
                    ViewBag.OutputUrls = outputUrls; // Pass the generated images
                    return View("GeneratedHairstyles");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                TempData["ErrorMessage"] = "Beklenmeyen bir hata oluştu. Lütfen tekrar deneyin.";
                return RedirectToAction("UploadImage");
            }
        }


        private async Task<string> UploadImageToTemporaryStorage(IFormFile image)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var content = new MultipartFormDataContent();
                    var imageContent = new StreamContent(image.OpenReadStream());
                    content.Add(imageContent, "image", image.FileName);

                    var response = await client.PostAsync("https://api.imgbb.com/1/upload?key=d593187f14ac851878af319ea4fe238c", content);
                    if (!response.IsSuccessStatusCode) return null;

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                    return result.data.url.ToString();
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
