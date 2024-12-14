using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ytgenerator.Shared.Dtos;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProcessController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProcessController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("get-outlines")]
    public async Task<IActionResult> GetOutlines([FromBody] List<string> youtubeLinks)
    {
        if (youtubeLinks == null || youtubeLinks.Count == 0)
        {
            return BadRequest("No YouTube links provided.");
        }

        var outlines = new List<VideoOutline>();
        var httpClient = _httpClientFactory.CreateClient("ExternalApi");

        var tasks = new List<Task>();

        foreach (var link in youtubeLinks)
        {
            tasks.Add(ProcessLinkAsync(httpClient, link, outlines));
        }

        await Task.WhenAll(tasks);

        return Ok(outlines);
    }

    private async Task ProcessLinkAsync(HttpClient httpClient, string link, List<VideoOutline> outlines)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<ApiOutLineResponse>($"generate-outline/?youtube_url={System.Net.WebUtility.UrlEncode(link)}");

            if (response != null && response.Outline != null)
            {
                lock (outlines)
                {
                    outlines.Add(new VideoOutline
                    {
                        Title = response.Title,
                        VideoUrl = link,
                        Sections = response.Outline.Sections
                    });
                }
            }
        }
        catch
        {
            // Log the error or handle it as needed
        }
    }

    // API write content from outline
    [HttpPost("generate-content")]
    public async Task<IActionResult> WriteContent([FromBody] List<VideoOutline> outlines)
    {
        if (outlines == null)
        {
            return BadRequest("No outline provided.");
        }

        var httpClient = _httpClientFactory.CreateClient("ExternalApi");
        var tasks = new List<Task>();
        var data = new List<GenerateContentDto>();

        foreach (var outline in outlines)
        {
            tasks.Add(WriteContentAsync(httpClient, outline, data));
        }

        await Task.WhenAll(tasks);

        return Ok(data);
    }

    private async Task WriteContentAsync(HttpClient httpClient, VideoOutline outline, List<GenerateContentDto> data)
    {
        try
        {
            var body = new GenerateContentRequest
            {
                youtube_url = outline.VideoUrl,
                Outline = outline
            };

            var response = await httpClient.PostAsJsonAsync<GenerateContentRequest>("generate-content", body);

            if (response.IsSuccessStatusCode)
            {
                // convert response to GenerateContentDto
                var content = await response.Content.ReadFromJsonAsync<GenerateContentDto>();

                lock (data)
                {
                    data.Add(content);
                }
            }

        }
        catch (Exception)
        {

        }
    }

    public class GenerateContentRequest
    {
        public string youtube_url { get; set; }
        public string Name { get; set; }
        public VideoOutline Outline { get; set; }
    }

    [HttpPost("generate-docx")]
    public async Task<IActionResult> GenerateDocx([FromBody] List<VideoOutline> content)
    {
        if (content == null)
        {
            return BadRequest("No content provided.");
        }

        var httpClient = _httpClientFactory.CreateClient("ExternalApi");
        var tasks = new List<Task>();
        var data = new List<GenerateDocxDto>();
        foreach (var item in content)
        {
            tasks.Add(GenerateDocxAsync(httpClient, item, data));
        }

        await Task.WhenAll(tasks);

        return Ok(data);
    }

    private async Task GenerateDocxAsync(HttpClient httpClient, VideoOutline outline, List<GenerateDocxDto> data)
    {
        try
        {
            var body = new GenerateContentRequest
            {
                youtube_url = outline.VideoUrl,
                Outline = outline,
                Name = outline.Title
            };

            var responseTask = httpClient.PostAsJsonAsync("generate-docx", body);
            var responseSeoTask = httpClient.PostAsJsonAsync("generate-seo-article", body);

            await Task.WhenAll(responseTask, responseSeoTask);

            if (responseTask.Result.IsSuccessStatusCode && responseSeoTask.Result.IsSuccessStatusCode)
            {
                var contentDocxTask = responseTask.Result.Content.ReadFromJsonAsync<GenerateDocxDto>();
                var contentSeoTask = responseSeoTask.Result.Content.ReadFromJsonAsync<GenerateDocxDto>();

                await Task.WhenAll(contentDocxTask, contentSeoTask);

                lock (data)
                {
                    data.Add(new GenerateDocxDto
                    {
                        FilePath = contentDocxTask.Result.FilePath,
                        FilePathSeo = contentSeoTask.Result.FilePath,
                        Title = contentDocxTask.Result.Title,
                        Url = contentDocxTask.Result.Url
                    });
                }
            }
        }
        catch (Exception)
        {
            // Log the error or handle it as needed
        }
    }

    [HttpPost("generate-new-outline")]
    public async Task<IActionResult> GenerateNewOutLine([FromBody] List<VideoOutline> content)
    {
        if (content == null)
        {
            return BadRequest("No content provided.");
        }

        var httpClient = _httpClientFactory.CreateClient("ExternalApi");
        var tasks = new List<Task>();
        var data = new List<VideoOutline>();
        foreach (var item in content)
        {
            tasks.Add(GenerateNewOutlineAsync(httpClient, item, data));
        }

        await Task.WhenAll(tasks);

        return Ok(data);
    }

    private async Task GenerateNewOutlineAsync(HttpClient httpClient, VideoOutline outline, List<VideoOutline> data)
    {
        try
        {
            var body = new GenerateContentRequest
            {
                youtube_url = outline.VideoUrl,
                Outline = outline,
                Name = outline.Title
            };

            var response = await httpClient.PostAsJsonAsync("generate-new-outline", body);

            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadFromJsonAsync<ApiOutLineResponse>();

                if (res != null && res.Outline != null)
                {
                    lock (data)
                    {
                        data.Add(new VideoOutline
                        {
                            Title = res.Title,
                            VideoUrl = outline.VideoUrl,
                            Sections = res.Outline.Sections
                        });
                    }
                }
            }
        }
        catch (Exception)
        {
            // Log the error or handle it as needed
        }
    }

}