using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR_Applications.Data;
using SignalR_Applications.Hubs;
using SignalR_Applications.Models;
using SignalR_Applications.ViewModel;
using System.Diagnostics;
using System.Security.Claims;

namespace SignalR_Applications.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<DeathlyHallowsHub> _deathlyHub;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, IHubContext<DeathlyHallowsHub> deathlyHub,
            ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
            _deathlyHub = deathlyHub;
        }

        public async Task<IActionResult> DeathlyHallows(string type)
        {
            if (SD.DealthyHallowRace.ContainsKey(type))
            {
                SD.DealthyHallowRace[type]++;
            }
            await _deathlyHub.Clients.All.SendAsync("updateDealthyHallowCount",
                SD.DealthyHallowRace[SD.Cloak],
                SD.DealthyHallowRace[SD.Stone],
                SD.DealthyHallowRace[SD.Wand]);

            return Accepted();
        }

        public IActionResult Notification()
        {
            return View();
        }

        public IActionResult VoteSystem()
        {
            return View();
        }

        public IActionResult HouseGroups()
        {
            return View();
        }

        public IActionResult BasicChat()
        {
            return View();
        }

        [Authorize]
        public IActionResult Chat()
        {
            var model = new ChatVM()
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Rooms = _context.ChatRoom.ToList(),
                MaxRoomAllowed = 4
            };
            return View(model);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}