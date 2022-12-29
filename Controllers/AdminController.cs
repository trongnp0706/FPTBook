using FPTBook.Data;
using FPTBook.Models;
using FPTBook.Utils;
using FPTBook.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FPTBook.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<ApplicationUser> _passwordHash;
        private readonly ILogger<AdminController> _logger;


        public AdminController(ILogger<AdminController> logger,
                                UserManager<ApplicationUser> userManager,
                                ApplicationDbContext context,
                                IPasswordHasher<ApplicationUser> passwordHash)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _passwordHash = passwordHash;
        }

        public IActionResult Index()
        {
            return View();
        }
        // list User
        [HttpGet]
        public IActionResult Users(string searchString = "")
        {
            var result = _context.Users.Where(ur => ur.Id != "AdminID123")
                .Join(_context.UserRoles, u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                .Join(_context.Roles, ur => ur.ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                .Select(c => new UserRolesViewModel()
                {
                    Id = c.ur.u.Id,
                    FullName = c.ur.u.FullName,
                    Email = c.ur.u.Email,
                    Address = c.ur.u.Address,
                    Roles = c.r.Name
                }).ToList().GroupBy(uv => new { uv.FullName, uv.Email, uv.Address, uv.Id })
                .Select(r => new UserRolesViewModel()
                {
                    Id = r.Key.Id,
                    FullName = r.Key.FullName,
                    Email = r.Key.Email,
                    Address = r.Key.Address,
                    Roles = string.Join("", r.Select(c => c.Roles).ToArray())
                    // Roles = char.
                });
            if (searchString != "" && searchString != null)
            {
                result = result.Where(u => u.Email.Contains(searchString)
                                    || u.FullName.Contains(searchString)
                                    || u.Address.Contains(searchString)
                                    || u.Roles.Contains(searchString))
                                .ToList();
            }
            else
            {
                result = result.ToList();
            }

            return View(result);
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }


        [HttpGet]
        public IActionResult Customers(string searchString = "")
        {
            var usersWithPermission = _userManager.GetUsersInRoleAsync(Role.CUSTOMER).Result;
            // usersWithPermission = (IList<ApplicationUser>)usersWithPermission.Select(u=>u.Email.Contains(searchString)).ToList();
            if (searchString != "" && searchString != null)
            {
                usersWithPermission = usersWithPermission.Where(
                                    u => u.Email.Contains(searchString)
                                    || u.FullName.Contains(searchString)
                                    || u.Address.Contains(searchString))
                                    .ToList();
            }
            else
            {
                usersWithPermission = usersWithPermission.ToList();
            }
            return View(usersWithPermission);
        }
        [HttpGet]
        public IActionResult StoreOwners(string searchString = "")
        {
            var usersWithPermission = _userManager.GetUsersInRoleAsync(Role.OWNER).Result;

            // usersWithPermission = (IList<ApplicationUser>)usersWithPermission.Select(u=>u.Email.Contains(searchString)).ToList();
            if (searchString != "" && searchString != null)
            {
                usersWithPermission = usersWithPermission.Where(
                                    u => u.Email.Contains(searchString)
                                    || u.FullName.Contains(searchString)
                                    || u.Address.Contains(searchString))
                                    .ToList();
            }
            else
            {
                usersWithPermission = usersWithPermission.ToList();
            }
            return View(usersWithPermission);
        }
        //Change pass customer
        [HttpGet]
        public async Task<IActionResult> ChangePassword(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Customers");
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string id, string password)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {

                if (!string.IsNullOrEmpty(password))
                    user.PasswordHash = _passwordHash.HashPassword(user, password);
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (!string.IsNullOrEmpty(password))
                {
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User changed their password successfully.");
                        TempData["AlertMessage"] = "User changed their password successfully.";
                        return RedirectToAction();

                    }
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found"); ;
            return RedirectToAction();
        }
        [HttpGet]
        public IActionResult GenreList(string searchString = "")
        {
            List<Genre> genres;
            if (searchString != "" && searchString != null)
            {
                genres = _context.Genres
                .Where(g => g.Description.Contains(searchString))
                .ToList();
            }
            else
            {
                genres = _context.Genres.ToList();
            }
            return View(genres);
        }

        //Approve

        public IActionResult GenreApproval()
        {
            IEnumerable<Genre> genres = _context.Genres
                                                .Where(t => t.Status == Enums.GenreApproval.pending)
                                                .ToList();
            return View(genres);
        }
        //Approve Genre
        public IActionResult ApproveGenre(int id)
        {
            var genreInDb = _context.Genres.SingleOrDefault(t => t.Id == id);
            if (genreInDb is null)
            {
                return BadRequest();
            }

            genreInDb.Status = Enums.GenreApproval.approved;

            Notification noti = new Notification
            {
                Message = "Your genre request for " + genreInDb.Description + " has been approved",
                NotiStatus = Enums.NotiCheck.unSeen
            };
            _context.Add(noti);

            _context.SaveChanges();
            return RedirectToAction("GenreApproval");
        }
        public IActionResult RejectGenre(int id)
        {
            var genreInDb = _context.Genres.SingleOrDefault(t => t.Id == id);
            if (genreInDb is null)
            {
                return BadRequest();
            }

            genreInDb.Status = Enums.GenreApproval.rejected;

            Notification noti = new Notification
            {
                Message = "Your genre request for " + genreInDb.Description + " has been rejected",
                NotiStatus = Enums.NotiCheck.unSeen
            };
            _context.Add(noti);

            _context.SaveChanges();
            return RedirectToAction("GenreApproval");
        }

        [HttpGet]
        public IActionResult UpdateGenre(int id)
        {
            var genreInDb = _context.Genres.SingleOrDefault(t => t.Id == id);
            if (genreInDb != null)
            {

                var genreView = new Genre()
                {
                    Id = genreInDb.Id,
                    Description = genreInDb.Description,
                    Status = genreInDb.Status
                };
                return View(genreView);

            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateGenre(Genre viewGenre)
        {
            if (ModelState.IsValid)
            {
                var genre = new Genre()
                {
                    Id = viewGenre.Id,
                    Description = viewGenre.Description,
                    Status = viewGenre.Status
                };
                _context.Genres.Update(genre);
                _context.SaveChanges();
                TempData["successMessage"] = "Update SuccessFully";
                return RedirectToAction("GenreList");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genreInDb = _context.Genres.SingleOrDefault(t => t.Id == id);
            if (genreInDb is null)
            {
                return NotFound();
            }
            _context.Genres.Remove(genreInDb);
            _context.SaveChanges();
            return RedirectToAction("GenreList");
        }

    }
}