using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MVC_ADMIN.Models;

namespace MVC_ADMIN.Controllers
{
    public class LearningMaterialController : Controller
    {
        // In-memory list for demo purposes. Replace with DB in production.
        private static List<LearningMaterial> materials = new List<LearningMaterial>
        {
            new LearningMaterial { Id = 1, Title = "Sample Material", Description = "Demo description", Url = "https://example.com", PublishedDate = DateTime.Now }
        };

        public ActionResult Index()
        {
            return View(materials);
        }

        public ActionResult Details(int id)
        {
            var material = materials.FirstOrDefault(m => m.Id == id);
            if (material == null) return HttpNotFound();
            return View(material);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LearningMaterial material)
        {
            if (ModelState.IsValid)
            {
                material.Id = materials.Any() ? materials.Max(m => m.Id) + 1 : 1;
                materials.Add(material);
                return RedirectToAction("Index");
            }
            return View(material);
        }

        public ActionResult Edit(int id)
        {
            var material = materials.FirstOrDefault(m => m.Id == id);
            if (material == null) return HttpNotFound();
            return View(material);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LearningMaterial material)
        {
            var existing = materials.FirstOrDefault(m => m.Id == material.Id);
            if (existing == null) return HttpNotFound();

            if (ModelState.IsValid)
            {
                existing.Title = material.Title;
                existing.Description = material.Description;
                existing.Url = material.Url;
                existing.PublishedDate = material.PublishedDate;
                return RedirectToAction("Index");
            }
            return View(material);
        }

        public ActionResult Delete(int id)
        {
            var material = materials.FirstOrDefault(m => m.Id == id);
            if (material == null) return HttpNotFound();
            return View(material);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var material = materials.FirstOrDefault(m => m.Id == id);
            if (material != null)
                materials.Remove(material);
            return RedirectToAction("Index");
        }
    }
}
