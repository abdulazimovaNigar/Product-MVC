using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductMVC.Contexts;
using ProductMVC.Models;
using ProductMVC.ViewModels.AuthViewModel;
using System.Threading.Tasks;

namespace ProductMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ProniaDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public AuthController(ProniaDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var existUser = await _userManager.FindByNameAsync(vm.UserName);
            if (existUser is { })
            {
                ModelState.AddModelError("Username", "This username is already taken");
            }

            existUser = await _userManager.FindByEmailAsync(vm.Email);
            if (existUser is { })
            {
                ModelState.AddModelError("Email", "This email is already exist");
            }

            AppUser user = new AppUser()
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                UserName = vm.UserName,
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(vm);
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email or password is wrong");
                return View(vm);
            }

            var result = await _userManager.CheckPasswordAsync(user, vm.Password);
            if (!result)
            {
                ModelState.AddModelError("", "Email or password is wrong");
                return View(vm);
            }

            await _signInManager.SignInAsync(user, vm.IsRemember);

            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> CreateRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Admin"
            });
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Member"
            });
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Moderator"
            });
            return Ok("Roles are created");
        }

        public async Task<IActionResult> CreateAdminModeratorMember()
        {
            var adminUserVM = _configuration.GetSection("AdminUser").Get<UserVM>();
            var moderatorUserVM = _configuration.GetSection("ModeratorUser").Get<UserVM>();

            if (adminUserVM != null)
            {
                AppUser adminUser = new AppUser()
                {
                    Email = adminUserVM.Email,
                    UserName = adminUserVM.UserName,
                    FirstName = adminUserVM.FirstName,
                    LastName = adminUserVM.LastName,
                };
                await _userManager.CreateAsync(adminUser, adminUserVM.Password);
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

            if (moderatorUserVM != null)
            {
                AppUser moderatorUser = new AppUser()
                {
                    Email = moderatorUserVM.Email,
                    UserName = moderatorUserVM.UserName,
                    FirstName = moderatorUserVM.FirstName,
                    LastName = moderatorUserVM.LastName,
                };
                await _userManager.CreateAsync(moderatorUser, moderatorUserVM.Password);
                await _userManager.AddToRoleAsync(moderatorUser, "Moderator");
            }

            return Ok("Created successfully");


        }
    }
}
