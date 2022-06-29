using AwesomeAppBack.DAL;
using AwesomeAppBack.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using X.PagedList;

namespace AwesomeAppBack.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SettingController : Controller
    {
        readonly Context _context;

        public SettingController(Context context)
        {
            _context = context;
        }

        public IActionResult Index(int? page)
        {
            if (page <= 0) page = 1;
            int pageNumber = page ?? 1;
            int pageSize = 1;
            if (_context.Settings.ToPagedList(pageNumber, pageSize).Count > 0)
                return View(_context.Settings.ToPagedList(pageNumber, pageSize));
            return View(_context.Settings.ToPagedList(1, pageSize));
        }
        public IActionResult Edit(int id)
        {
            return View(_context.Settings.FirstOrDefault(x => x.Id == id));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(Setting setting)
        {
            Setting existSetting = _context.Settings.FirstOrDefault(x => x.Id == setting.Id);
            existSetting.Value = setting.Value;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
