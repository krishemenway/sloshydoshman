using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.IO;

namespace SloshyDoshMan.Service.Maps
{
	[Route("Maps")]
	public class MapImagesController : ControllerBase
	{
		public MapImagesController(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}

		[HttpGet("{Map}-480x240")]
		public FileContentResult MapImage480(string map)
		{
			return File(TryReadFileContents($"MapImages/{map}-480x240.jpg"), "image/jpg");
		}

		private byte[] TryReadFileContents(string filePath)
		{
			try
			{
				return _memoryCache.GetOrCreate($"MapImage480-{filePath}", (cache) => System.IO.File.ReadAllBytes(Program.Settings.FilePathInExecutableFolder(filePath)));
			}
			catch (FileNotFoundException)
			{
				return _memoryCache.GetOrCreate($"MapImage480-Default", (cache) => System.IO.File.ReadAllBytes(Program.Settings.FilePathInExecutableFolder($"MapImages/Default-480x240.jpg")));
			}
		}

		private readonly IMemoryCache _memoryCache;
	}
}
