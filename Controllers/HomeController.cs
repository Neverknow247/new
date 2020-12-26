using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CSBelt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CSBelt.Controllers
{
    public class HomeController : Controller
    {
        private Context _context;

        public HomeController(Context context)
        {
            _context = context;
        }
        private Users GetUserFromDB()
        {
            return _context.Users.FirstOrDefault(i => i.UserId == HttpContext.Session.GetInt32("LoggedInID"));
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return RedirectToAction("SignIn");
        }

        [HttpGet("signin")]
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost("register")]
        public IActionResult Register(Users user)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(i => i.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use!");
                    return View("SignIn", user);
                }
                PasswordHasher<Users> Hasher = new PasswordHasher<Users>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Add(user);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("LoggedInID", (int)user.UserId);
                return RedirectToAction("Home");
            }
            else
            {
                return View("SignIn", user);
            }
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                var userInDb = _context.Users.FirstOrDefault(i => i.Email == userSubmission.Email);
                if (userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("SignIn");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                if (result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("SignIn");
                }
                HttpContext.Session.SetInt32("LoggedInID", (int)userInDb.UserId);
                return Redirect($"home");
            }
            return View("SignIn");
        }
        [HttpGet("home")]
        public IActionResult Home()
        {
            Users LoggedInID = GetUserFromDB();
            if (LoggedInID == null)
            {
                return RedirectToAction("logout");
            }
            ViewBag.User = LoggedInID;
            List<Events> Events = _context.Events.Include(i => i.Planner).Include(i => i.EventParticipants).ThenInclude(i => i.User).OrderByDescending(i => i.CreatedAt).Where(i => i.EventStart > DateTime.Now).ToList();
            return View("Dashboard",Events);
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("SignIn");
        }

        [HttpGet("new")]
        public IActionResult New()
        {
            Users LoggedInID = GetUserFromDB();
            if (LoggedInID == null)
            {
                return RedirectToAction("logout");
            }
            return View();
        }
        [HttpPost("Create")]
        public IActionResult Create(Events newevent, string Time, string Duration)
        {
            Users LoggedInID = GetUserFromDB();
            if (ModelState.IsValid)
            {
                if (DateTime.Now > (DateTime)newevent.EventStart)
                {
                    ModelState.AddModelError("EventStart", "Activity Must be in the Future");
                    return View("New", newevent);
                }
                else
                {
                    newevent.CreatedAt = DateTime.Now;
                    newevent.UpdatedAt = DateTime.Now;
                    newevent.Planner = LoggedInID;
                    newevent.UserId = LoggedInID.UserId;
                    _context.Add(newevent);
                    _context.SaveChanges();
                    return RedirectToAction("Home");
                }
            }
            return View("New", newevent);
        }
        [HttpGet("View/{id}")]
        public IActionResult View(int id)
        {
            Users LoggedInID = GetUserFromDB();
            if (LoggedInID == null)
            {
                return RedirectToAction("logout");
            }
            ViewBag.User = LoggedInID;
            Events viewevent = _context.Events.Include(i => i.EventParticipants).ThenInclude(i => i.User).Include(i => i.Planner).FirstOrDefault(i => i.EventId == id);
            ViewBag.Event = viewevent;
            return View(viewevent);
        }
        [HttpGet("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            Events toRemove = _context.Events.SingleOrDefault(i => i.EventId == id);
            _context.Events.Remove(toRemove);
            _context.SaveChanges();
            return RedirectToAction("Home");
        }

        [HttpGet("Leave/{EId}/{PId}")]
        public IActionResult UnRSVP(int EId, int PId)
        {
            Participants Leave = _context.Participants.FirstOrDefault(u => u.UserId == PId && u.EventId == EId);
            _context.Participants.Remove(Leave);
            _context.SaveChanges();
            return RedirectToAction("Home");
        }
        [HttpGet("Join/{EId}/{PId}")]
        public IActionResult RSVP(int EId, int PId)
        {
            Events joinEvent = _context.Events.FirstOrDefault(i => i.EventId == EId);
            Users user = _context.Users.FirstOrDefault(j => j.UserId == PId);
            Participants Join = new Participants();
            Join.UserId = user.UserId;
            Join.User = user;
            Join.EventId = joinEvent.EventId;
            Join.Event = joinEvent;
            _context.Add(Join);
            _context.SaveChanges();
            return RedirectToAction("Home");
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
