using dpapi_test.models;
using Microsoft.AspNetCore.Mvc;

namespace dpapi_test.Controllers
{
    [ApiController]
    [Route("api/keys")]
    public sealed class KeysController : ControllerBase
    {
        private readonly IKeyStore _store;

        public KeysController(IKeyStore store) => _store = store;

        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] KeyDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.KeyBase64))
                return BadRequest();

            byte[] raw;
            try { raw = Convert.FromBase64String(dto.KeyBase64); }
            catch { return BadRequest(); }

            await _store.ProtectAndSaveAsync(raw);
            Array.Clear(raw, 0, raw.Length);
            return Ok();
        }

        [HttpGet("load")]
        public async Task<IActionResult> Load()
        {
            try
            {
                var raw = await _store.UnprotectAsync();
                var b64 = Convert.ToBase64String(raw);
                Array.Clear(raw, 0, raw.Length);
                return Ok(new { keyBase64 = b64 });
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
