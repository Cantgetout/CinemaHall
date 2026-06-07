using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaHub.Data.Models;
using CinemaHub.Data.Models.ViewModels;
using System.Security.Claims;

namespace CinemaHub.Controllers
{
    // Only Administrators can access anything in this file.
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewBag.CurrentSort = sortOrder;

            // 1. Setup sorting parameters for the view
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.RoleSortParm = sortOrder == "Role" ? "role_desc" : "Role";

            // 2. Get all users from the database
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<AdminUserViewModel>();

            // 3. Map them to our ViewModel and fetch their roles
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Role = roles.FirstOrDefault() ?? "None"
                });
            }

            // 4. Apply the sorting logic
            IEnumerable<AdminUserViewModel> sortedUsers = userViewModels;

            switch (sortOrder)
            {
                case "Name": sortedUsers = sortedUsers.OrderBy(u => u.FullName); break;
                case "name_desc": sortedUsers = sortedUsers.OrderByDescending(u => u.FullName); break;

                case "Email": sortedUsers = sortedUsers.OrderBy(u => u.Email); break;
                case "email_desc": sortedUsers = sortedUsers.OrderByDescending(u => u.Email); break;

                case "Role": sortedUsers = sortedUsers.OrderBy(u => u.Role); break;
                case "role_desc": sortedUsers = sortedUsers.OrderByDescending(u => u.Role); break;

                default:
                    // Default behavior: Put Admins first (0), then Moderators (1), then Users (2)
                    sortedUsers = sortedUsers.OrderBy(u => u.Role == "Administrator" ? 0 : u.Role == "Moderator" ? 1 : 2);
                    break;
            }

            return View(sortedUsers.ToList());
        }

        // --- MAKE MODERATOR ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Promote(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            // Safest way to get the currently logged-in users ID directly from their session claim
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (user != null && currentUserId != user.Id)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles); // Clear old roles
                await _userManager.AddToRoleAsync(user, "Moderator");        // Add new role
            }
            return RedirectToAction(nameof(Index));
        }

        // --- MAKE NORMAL USER ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Demote(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Safety check: Prevent the master admin from demoting themselves
            if (user != null && user.Id != currentUserId)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, "User");
            }
            return RedirectToAction(nameof(Index));
        }

        // --- BAN / DELETE USER ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (user != null && user.Id != currentUserId)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Contains("User") && !currentRoles.Contains("Moderator") && !currentRoles.Contains("Administrator"))  //Delete only if the user has the default role, dont allow the banning of moderators and admins
                {
                    await _userManager.DeleteAsync(user);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}