using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace SloshyDoshMan.Service.Perks
{
	[Route("Perks")]
	public class PerkImagesController : ControllerBase
	{
		public PerkImagesController(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}

		[HttpGet("{Perk}-128")]
		public FileContentResult PerkImage128(string perk)
		{
			return File(ReadFileBytes($"PerksImages/{perk}-128.png"), "image/png");
		}

		private byte[] ReadFileBytes(string filePath)
		{
			return _memoryCache.GetOrCreate($"PerkImage128-{filePath}", (cache) => System.IO.File.ReadAllBytes(Program.Settings.FilePathInExecutableFolder(filePath)));
		}

		private readonly IMemoryCache _memoryCache;
	}
}
