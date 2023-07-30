using Microsoft.AspNetCore.Mvc;
using FinanSyncApi.Core;
using FinanSyncData;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FinanSyncApi.Controllers
{
    [ApiController]
    [Route("user/settings")]
    public sealed class SettingsController : ControllerBase
    {

        #region Dependencies

        private readonly ISettingsService _settingsService;
        private readonly ILogger<SettingsController> _logger;

        #endregion

        #region Constructor

        public SettingsController(ISettingsService settingsService, ILogger<SettingsController> logger)
        {
            _settingsService= settingsService;
            _logger = logger;
        }

        #endregion


        #region Controller Actions

        [HttpGet]
        public Task<SettingResponseDto[]> GetSettingsAsync(CancellationToken cancellationToken)
        {
            return _settingsService.GetSettingsAsync(GetAuthenticatedId(), cancellationToken);
        }

        [HttpPost("set")]
        public async Task<ActionResult<SettingResponseDto[]>> SetSettingsAsync([FromBody][Required] ICollection<SettingRequestDto> settings,
                                                                               CancellationToken cancellationToken)
        {
            var result = await _settingsService.SetSettingsAsync(GetAuthenticatedId(), settings, cancellationToken);
            if (result.Output is SettingResponseDto[] output)
            {
                return output;
            }
            if (result.ErrorType == OperationError.DatabaseError)
            {
                _logger.LogWarning("An attempt of setting user settings has failed due to unknown database problem.");
                return Problem(title: "Failed to set settings.", detail: "An error occurred while setting the user settings.", statusCode: 500);
            }

            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Key, error.Value);
            }
            return ValidationProblem(ModelState);
        }

        [HttpPost("reset")]
        public async Task<ActionResult<SettingResponseDto[]>> ResetSettingsAsync(CancellationToken cancellationToken)
        {
            var result = await _settingsService.ResetSettingsAsync(GetAuthenticatedId(), cancellationToken);
            if (result.Output is SettingResponseDto[] output)
            {
                return output;
            }
            _logger.LogWarning("An attempt of resetting user settings has failed due to unknown error.");
            return Problem(title: "Failed to reset settings.", detail: "An error occurred while resetting the user settings.", statusCode: 500);
        }

        #endregion

        #region Helper Methods

        private string GetAuthenticatedId() => HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Couldn't resolve authenticated account Id.");

        #endregion

    }
}
