using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace SloshyDoshMan.Service
{
	public class WebAssetsController : ControllerBase
	{
		public WebAssetsController(
			IMemoryCache memoryCache,
			ISettings settings = null)
		{
			_memoryCache = memoryCache;
			_settings = settings ?? Program.Settings;
		}

		[HttpGet("robots.txt")]
		public ContentResult Robots()
		{
			return Content(ReadFileContents("robots.txt"), "text/plain", Encoding.UTF8);
		}

		[HttpGet("favicon.ico")]
		public FileContentResult FavIcon()
		{
			return File(ReadFileBytes("favicon.ico"), "image/x-icon", false);
		}

		[HttpGet("app.js")]
		public ContentResult AppJavascript()
		{
			return Content(ReadFileContents("app.js"), "application/javascript", Encoding.UTF8);
		}

		[HttpGet("background.png")]
		public FileContentResult Background()
		{
			return File(ReadFileBytes("background.png"), "image/png", false);
		}

		[HttpGet("corners.png")]
		public FileContentResult Corners()
		{
			return File(ReadFileBytes("corners.png"), "image/png", false);
		}

		[HttpGet("{*url}", Order = int.MaxValue)]
		public ContentResult AppMarkup(string url)
		{
			return Content(ReadFileContents("app.html").Replace("{url}", url), "text/html", Encoding.UTF8);
		}

		private byte[] ReadFileBytes(string filePath)
		{
			return _memoryCache.GetOrCreate($"AssetsContents-{filePath}", (cache) => System.IO.File.ReadAllBytes(_settings.FilePathInExecutableFolder(filePath)));
		}

		private string ReadFileContents(string filePath)
		{
			return _memoryCache.GetOrCreate($"AssetsContents-{filePath}", (cache) => System.IO.File.ReadAllText(_settings.FilePathInExecutableFolder(filePath)));
		}

		private readonly IMemoryCache _memoryCache;
		private readonly ISettings _settings;
	}
}
