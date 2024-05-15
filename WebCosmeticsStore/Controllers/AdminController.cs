using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCosmeticsStore.Models;

namespace WebCosmeticsStore.Controllers
{
   
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userNanager;
        private readonly ApplicationDbContext _context;
        public AdminController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userNanager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var getUser = await _userNanager.GetUserAsync(User);
            var getRole = await _userNanager.GetRolesAsync(getUser);

            ViewBag.Role = new SelectList(await _context.Roles.ToListAsync(), "Id", "Name");
            var query = from user in _context.Users
                        join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                        join roles in _context.Roles on userRoles.RoleId equals roles.Id
                        select new
                        {
                            UserId = user.Id,
                            user.UserName,
                            roles.Name,
                        };
            var list = await query.ToListAsync();
            return View(list);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUserRole(string userId, string roleId)
        {
            var user = await _userNanager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userNanager.GetRolesAsync(user);
            await _userNanager.RemoveFromRolesAsync(user, roles.ToArray());

            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                return NotFound();
            }

            await _userNanager.AddToRoleAsync(user, role.Name);
            return RedirectToAction(nameof(Index));
        }
    }
}
