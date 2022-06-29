using Lumia.DAL;
using Lumia.Helpers;
using Lumia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lumia.Areas.LumiaAdmin.Controllers
{
    [Authorize]
    [Area("LumiaAdmin")]
    public class TeamController : Controller
    {
        public AppDbContext _context { get;  }
        public IWebHostEnvironment _env { get; set; }
        public TeamController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Teams);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Team team)
        {
            if (!team.Photo.FileSize(200))
            {
                ModelState.AddModelError("Photo", "Size>200kb");
            }
            if (!team.Photo.FileType("image/"))
            {
                ModelState.AddModelError("Photo", "Type!=image");
            }
            team.Image = await team.Photo.SaveFileAsync(_env.WebRootPath, "img");
           await _context.Teams.AddAsync(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));   
            
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id==null)
            {
                return BadRequest();
            }
            var team = _context.Teams.Find(id);
            if (team==null)
            {
                return NotFound();
            }
            var path = Helper.GetPath(_env.WebRootPath, "img", team.Image);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Upload(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            Team team = _context.Teams.Find(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        public async Task<IActionResult> Upload(Team tm,int? id)
        {
            if (id==null)
            {
                return BadRequest();
            }
            Team teamDb = _context.Teams.Find(id);
            if (teamDb==null)
            {
                return NotFound();
            }
            tm.Image = await tm.Photo.SaveFileAsync(_env.WebRootPath, "img");
            var pathdb = Helper.GetPath(_env.WebRootPath, "img", teamDb.Image);
            if (System.IO.File.Exists(pathdb))
            {
                System.IO.File.Delete(pathdb);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Team");
        }
    }
}
