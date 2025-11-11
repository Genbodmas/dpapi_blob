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

        // POST api/keys/save
        // Body: { "keyBase64": "..." }
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] KeyDto dto, CancellationToken ct)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.KeyBase64))
                return BadRequest(new { message = "keyBase64 is required" });

            byte[] raw;
            try
            {
                raw = Convert.FromBase64String(dto.KeyBase64);
            }
            catch
            {
                return BadRequest(new { message = "invalid base64" });
            }

            try
            {
                await _store.ProtectAndSaveAsync(raw, ct);   // use your interface method
                Array.Clear(raw, 0, raw.Length);
                return Ok(new { message = "saved" });
            }
            catch (Exception ex)
            {
                Array.Clear(raw, 0, raw.Length);
                return Problem(detail: ex.Message);
            }
        }

        // GET api/keys/load
        // Returns { "keyBase64": "..." }
        [HttpGet("load")]
        public async Task<IActionResult> Load(CancellationToken ct)
        {
            try
            {
                var raw = await _store.UnprotectAsync(ct);    // use your interface method
                var b64 = Convert.ToBase64String(raw);
                Array.Clear(raw, 0, raw.Length);
                return Ok(new { keyBase64 = b64 });
            }
            catch (FileNotFoundException)
            {
                return NotFound(new { message = "blob not found" });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }
    }
}
