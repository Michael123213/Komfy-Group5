using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Controllers
{
    [AllowAnonymous] // Temporarily allow access to all users for development
    public class UserSettingController : Controller
    {
        private readonly ILogger<UserSettingController> _logger;
        private readonly IUserSettingService _userSettingService;

        // Inject IUserSettingService
        public UserSettingController(ILogger<UserSettingController> logger, IUserSettingService userSettingService)
        {
            _logger = logger;
            _userSettingService = userSettingService;
        }

        // GET: /UserSetting/Index (READ: List all user settings)
        public IActionResult Index()
        {
            var userSettings = _userSettingService.GetAllUserSettings();
            return View(userSettings);
        }

        // GET: /UserSetting/Create (CREATE: Display form)
        public IActionResult Create()
        {
            return View();
        }

        // POST: /UserSetting/Create (CREATE: Process form submission)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserSettingModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _userSettingService.AddUserSetting(model);
                    TempData["SuccessMessage"] = "User setting created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (System.Exception ex)
                {
                    // Add model error if business logic fails
                    ModelState.AddModelError(string.Empty, $"Error creating user setting: {ex.Message}");
                    _logger.LogError(ex, "Error creating user setting.");
                }
            }
            return View(model);
        }

        // GET: /UserSetting/Edit/{id} (UPDATE: Display form with existing data)
        public IActionResult Edit(int id)
        {
            var userSetting = _userSettingService.GetUserSettingDetails(id);
            if (userSetting == null)
            {
                return NotFound();
            }
            return View(userSetting);
        }

        // POST: /UserSetting/Edit/{id} (UPDATE: Process form submission)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, UserSettingModel model)
        {
            if (id != model.SettingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _userSettingService.UpdateUserSetting(model);
                    TempData["SuccessMessage"] = "User setting updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error updating user setting: {ex.Message}");
                    _logger.LogError(ex, "Error updating user setting.");
                }
            }
            return View(model);
        }

        // POST: /UserSetting/UpdateTheme (UPDATE: Quick theme update)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTheme(string userId, string theme)
        {
            try
            {
                _userSettingService.UpdateTheme(userId, theme);
                TempData["SuccessMessage"] = "Theme updated successfully.";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating theme: {ex.Message}";
            }
            return RedirectToAction(nameof(UserSettings), new { userId });
        }

        // POST: /UserSetting/Delete/{id} (DELETE: Process deletion)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _userSettingService.DeleteUserSetting(id);
                TempData["SuccessMessage"] = "User setting deleted successfully.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "User setting not found or already deleted.";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting user setting: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /UserSetting/Details/{id} (READ: View user setting details)
        public IActionResult Details(int id)
        {
            var userSetting = _userSettingService.GetUserSettingDetails(id);
            if (userSetting == null)
            {
                return NotFound();
            }
            return View(userSetting);
        }

        // GET: /UserSetting/UserSettings/{userId} (READ: View settings for a specific user)
        public IActionResult UserSettings(string userId)
        {
            var userSetting = _userSettingService.GetUserSettingByUserId(userId);
            if (userSetting == null)
            {
                // If user doesn't have settings, create default ones
                try
                {
                    _userSettingService.CreateDefaultSettingForUser(userId);
                    userSetting = _userSettingService.GetUserSettingByUserId(userId);
                }
                catch (System.Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error creating default settings: {ex.Message}";
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(userSetting);
        }

        // POST: /UserSetting/CreateDefault (CREATE: Create default setting for user)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateDefault(string userId)
        {
            try
            {
                _userSettingService.CreateDefaultSettingForUser(userId);
                TempData["SuccessMessage"] = "Default settings created successfully.";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating default settings: {ex.Message}";
            }
            return RedirectToAction(nameof(UserSettings), new { userId });
        }
    }
}
