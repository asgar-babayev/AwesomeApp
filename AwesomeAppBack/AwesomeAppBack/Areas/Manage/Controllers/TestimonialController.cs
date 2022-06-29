using AwesomeAppBack.DAL;
using AwesomeAppBack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using X.PagedList;

namespace AwesomeAppBack.Areas.Manage.Controllers
{
    [Area("Manage"), Authorize]
    public class TestimonialController : Controller
    {
        readonly Context _context;
        readonly IWebHostEnvironment _env;

        public TestimonialController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index(int? page)
        {
            if (page <= 0) page = 1;
            var pageNumber = page ?? 1;
            int pageSize = 1;
            if (_context.Testimonials.ToPagedList(pageNumber, pageSize).Count > 0)
            {
                return View(_context.Testimonials.ToPagedList(pageNumber, pageSize));
            }
            return View(_context.Testimonials.ToPagedList(1, pageSize));
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
                if (!CheckFileType(testimonial.ImageFile))
                {
                    ModelState.AddModelError("", "This is not image format");
                    return View(testimonial);
                }
                if (!CheckFileSize(testimonial.ImageFile, 2000))
                {
                    ModelState.AddModelError("", "Image can't many 2mb");
                    return View(testimonial);
                }
                string fileName = testimonial.ImageFile.FileName;
                FileNameShorter(fileName);
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
        
        public bool CheckFileSize(IFormFile imageFile, int size)
        {
            if (imageFile.Length / 1024 > size) return false;
            return true;
        }
        public bool CheckFileType(IFormFile imageFile)
        {
            if (imageFile.ContentType != "image/jpeg" && imageFile.ContentType != "image/png" && imageFile.ContentType != "image/webp")
                return false;
            return true;
        }

        public void FileNameShorter(string fileName)
        {
            if(fileName.Length > 64) fileName.Substring(fileName.Length - 64, 64);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(Testimonial testimonial)
        {
            var existTestimonial = _context.Testimonials.FirstOrDefault(x => x.Id == testimonial.Id);
            if (existTestimonial == null) return NotFound();
            string newFileName = null;

            if (testimonial.ImageFile != null)
            {
                if (!CheckFileType(testimonial.ImageFile))
                {
                    ModelState.AddModelError("", "This is not image format");
                    return View(testimonial);
                }
                if (!CheckFileSize(testimonial.ImageFile, 2000))
                {
                    ModelState.AddModelError("", "Image can't many 2mb");
                    return View(testimonial);
                }
                string fileName = testimonial.ImageFile.FileName;
                FileNameShorter(fileName);
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
