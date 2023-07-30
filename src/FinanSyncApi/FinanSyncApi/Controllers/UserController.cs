using Microsoft.AspNetCore.Mvc;
using FinanSyncApi.Core;
using FinanSyncData;
using System.Security.Claims;

namespace FinanSyncApi.Controllers
{
    [ApiController]
    [Route("user")]
    public sealed class UserController : ControllerBase
    {

        #region Dependencies

        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        #endregion

        #region Constructor

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        #endregion


        #region Controller Actions

        [HttpGet]
        public async Task<ActionResult<UserResponseDto>> GetUserDataAsync(CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserDataAsync(GetAuthenticatedId(), cancellationToken);
            if (user is null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpGet("authorization-status")]
        public async Task<IActionResult> GetAuthorizationStatusAsync(CancellationToken cancellationToken)
        {
            var userExists = await _userService.UserExistsAsync(GetAuthenticatedId(), cancellationToken);
            return userExists
                ? Ok()
                : NotFound();
        }

        #endregion

        #region Helper Methods

        private string GetAuthenticatedId() => HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Couldn't resolve authenticated account Id.");

        #endregion

    }
}
