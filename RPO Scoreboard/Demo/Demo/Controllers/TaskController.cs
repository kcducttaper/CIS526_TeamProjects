﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Demo.Models;
using Demo.Repositories;

namespace Demo.Controllers
{
    /// <summary>
    /// Handles creating pages for tasks.
    /// </summary>
    public class TaskController : Controller
    {
        private IRepository<Task> _taskRepo;

        public TaskController()
        {
            _taskRepo = new BasicRepo<Task>();
        }

        //
        // GET: /Task/
        public ActionResult Index()
        {
            return View(_taskRepo.GetAll());
        }

        //
        // GET: /Task/Detials/{id}
        public ActionResult Details(int id)
        {
            return View(_taskRepo.Get(id));
        }

        //
        // GET: /Task/Create/
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Task/Create/
        [Authorize]
        public ActionResult Create(Task item)
        {
            if(ModelState.IsValid)
            {
                _taskRepo.Create(item);
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Task/Update/
        public ActionResult Update()
        {
            return View();
        }

        //
        // POST: /Task/Update/
        [Authorize]
        public ActionResult Update(Task item)
        {
            if(ModelState.IsValid)
            {
                _taskRepo.Create(item);
            }

            return RedirectToAction("Index");
        }

        //
        // GET: /Task/Delete/{id}
        [HttpGet]
        [Authorize]
        public ActionResult Delete(int id)
        {
            return View(_taskRepo.Get(id));
        }

        //
        // POST: /Task/Delete/{id}
        [HttpPost]
        [Authorize]
        public ActionResult DeleteComfirmed(int id)
        {
            _taskRepo.Delete(id);

            return RedirectToAction("Index");
        }
    }
}
