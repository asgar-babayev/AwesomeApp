using AwesomeAppBack.DAL;
using AwesomeAppBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;

namespace AwesomeAppBack.Areas.Manage.Controllers
{
    [Area("Manage"),Authorize]
    public class TestimonialController : Controller
    {
        readonly Context _context;
        readonly IWebHostEnvironment _env;

        public TestimonialController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_context.Testimonials.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(Testimonial testimonial)
        {
            if (testimonial.ImageFile != null)
            {
                if (testimonial.ImageFile.ContentType != "image/jpeg" && testimonial.ImageFile.ContentType != "image/png" && testimonial.ImageFile.ContentType != "image/webp")
                {
                    ModelState.AddModelError("", "This is not image format");
                    return View(testimonial);
                }
                if (testimonial.ImageFile.Length / 1024 > 2000)
                {
                    ModelState.AddModelError("", "Image can't many 2mb");
                    return View(testimonial);
                }
                string fileName = testimonial.ImageFile.FileName;
                if (fileName.Length > 64)
                {
                    fileName.Substring(fileName.Length - 64, 64);
                }
                string newFileName = Guid.NewGuid().ToString() + fileName;
                string path = Path.Combine(_env.WebRootPath, "assets", "images", newFileName);
                using (FileStream st = new FileStream(path, FileMode.Create))
                {
                    testimonial.ImageFile.CopyTo(st);
                }
                testimonial.Image = newFileName;
                _context.Testimonials.Add(testimonial);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int id)
        {
            return View(_context.Testimonials.FirstOrDefault(x => x.Id == id));
        }

        [HttpPost,ValidateAntiForgeryToken]
        public IActionResult Edit(Testimonial testimonial)
        {
            var existTestimonial = _context.Testimonials.FirstOrDefault(x => x.Id == testimonial.Id);
            if (existTestimonial == null) return NotFound();
            string newFileName = null;

            if (testimonial.ImageFile != null)
            {
                if (testimonial.ImageFile.ContentType != "image/jpeg" && testimonial.ImageFile.ContentType != "image/png" && testimonial.ImageFile.ContentType != "image/webp")
                {
                    ModelState.AddModelError("ImageFile", "Image can be only .jpeg or .png");
                    return View();
                }
                if (testimonial.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "Image size must be lower than 2mb");
                    return View();
                }
                string fileName = testimonial.ImageFile.FileName;
                if (fileName.Length > 64)
                {
                    fileName = fileName.Substring(fileName.Length - 64, 64);
                }
                newFileName = Guid.NewGuid().ToString() + fileName;

                string path = Path.Combine(_env.WebRootPath, "assets", "images", newFileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    testimonial.ImageFile.CopyTo(stream);
                }
            }
            if (newFileName != null)
            {
                string deletePath = Path.Combine(_env.WebRootPath, "assets", "images", existTestimonial.Image);

                if (System.IO.File.Exists(deletePath))
                {
                    System.IO.File.Delete(deletePath);
                }

                existTestimonial.Image = newFileName;
            }

            existTestimonial.CustomerName = testimonial.CustomerName;
            existTestimonial.CompanyName = testimonial.CompanyName;
            existTestimonial.FeedBack = testimonial.FeedBack;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            var c = _context.Testimonials.FirstOrDefault(x => x.Id == id);
            string path = Path.Combine(_env.WebRootPath, "assets", "images" + c.Image);
            _context.Testimonials.Remove(c);
            _context.SaveChanges();
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            return View();
        }
    }
}
