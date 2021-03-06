﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CIS726_Assignment2.Models;
using PagedList;
using CIS726_Assignment2.Repositories;
using CIS726_Assignment2.ViewModels;
using CIS726_Assignment2.SystemBus;

namespace CIS726_Assignment2.Controllers
{
    public class DegreeProgramsController : Controller
    {
        //private IGenericRepository<DegreeProgram> degreePrograms;
        //private IGenericRepository<RequiredCourse> requiredCourses;
        //private IGenericRepository<ElectiveCourse> electiveCourses;
        //private IGenericRepository<ElectiveList> electiveLists;
        //private IGenericRepository<Course> courses;

        IMessageQueueProducer<DegreeProgram> _degreeProgramProducer;
        IMessageQueueProducer<RequiredCourse> _requiredCourseProducer;
        IMessageQueueProducer<ElectiveCourse> _electiveCourseProducer;
        IMessageQueueProducer<ElectiveList> _electiveListProducer;
        IMessageQueueProducer<Course> _courseProducer;

        /// <summary>
        /// Constructor used by the web application itself
        /// </summary>
        public DegreeProgramsController()
        {
            CourseDBContext context = new CourseDBContext();
            //degreePrograms = new GenericRepository<DegreeProgram>(new StorageContext<DegreeProgram>(context));
            //requiredCourses = new GenericRepository<RequiredCourse>(new StorageContext<RequiredCourse>(context));
            //electiveCourses = new GenericRepository<ElectiveCourse>(new StorageContext<ElectiveCourse>(context));
            //electiveLists = new GenericRepository<ElectiveList>(new StorageContext<ElectiveList>(context));
            //courses = new GenericRepository<Course>(new StorageContext<Course>(context));

            _degreeProgramProducer = new BasicMessageQueueProducer<DegreeProgram>();
            _requiredCourseProducer = new BasicMessageQueueProducer<RequiredCourse>();
            _electiveCourseProducer = new BasicMessageQueueProducer<ElectiveCourse>();
            _electiveListProducer = new BasicMessageQueueProducer<ElectiveList>();
            _courseProducer = new BasicMessageQueueProducer<Course>();
        }

        /// <summary>
        /// Constructor for UnitTesting (allows passing in a fake repository)
        /// </summary>
        public DegreeProgramsController(IGenericRepository<DegreeProgram> fakeDegree, IGenericRepository<RequiredCourse> fakeRequired, IGenericRepository<ElectiveCourse> fakeElecCourse, IGenericRepository<ElectiveList> fakeElecList, IGenericRepository<Course> fakeCourse)
        {
            //degreePrograms = fakeDegree;
            //requiredCourses = fakeRequired;
            //electiveCourses = fakeElecCourse;
            //electiveLists = fakeElecList;
            //courses = fakeCourse;
        }

        //
        // GET: /DegreePrograms/

        public ActionResult Index(string sortOrder, int? page)
        {
            int pageSize = 100;
            int pageNumber = (page ?? 1);

            String currentSort = "";

            bool titleAsc = false;

            //var degreeProgramList = from s in degreePrograms.GetAll() select s;
            var degreeProgramList = _degreeProgramProducer.GetAll().AsQueryable();
            if (sortOrder == null)
            {
                sortOrder = "title_asc";
            }

            String[] sorts = sortOrder.Split(';');

            int lastTitle = -1;

            for (int i = 0; i < sorts.Length; i++)
            {
                if (sorts[i].StartsWith("title"))
                {
                    if (lastTitle > 0)
                    {
                        sorts[lastTitle] = "";
                    }
                    else
                    {
                        lastTitle = i;
                    }
                }
            }

            foreach (string s in sorts)
            {
                if (s.Length <= 0)
                {
                    continue;
                }
                currentSort = currentSort + s + ";";
                if (s.Equals("title_asc"))
                {
                    degreeProgramList = degreeProgramList.OrderBy(x => x.degreeProgramName);
                    titleAsc = true;
                }
                if (s.Equals("title_desc"))
                {
                    degreeProgramList = degreeProgramList.OrderByDescending(x => x.degreeProgramName);
                    titleAsc = false;
                }
            }

            ViewBag.titleAsc = titleAsc;
            ViewBag.currentSort = currentSort;

            return View(degreeProgramList.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /DegreePrograms/Details/5

        public ActionResult Details(int id = 0)
        {
            //DegreeProgram degreeprogram = degreePrograms.Find(id);
            DegreeProgram degreeprogram = _degreeProgramProducer.Get(new DegreeProgram() { ID = id });
            if (degreeprogram == null)
            {
                return HttpNotFound();
            }
            degreeprogram.requiredCourses = degreeprogram.requiredCourses.OrderBy(reqcourse => reqcourse.semester).ThenBy(reqcourse => reqcourse.course.coursePrefix).ThenBy(reqcourse => reqcourse.course.courseNumber).ToList();
            degreeprogram.electiveCourses = degreeprogram.electiveCourses.OrderBy(eleccourse => eleccourse.semester).ThenBy(eleccourse => eleccourse.electiveList.electiveListName).ToList();
            return View(degreeprogram);
        }

        //
        // GET: /DegreePrograms/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DegreePrograms/Create

        [HttpPost]
        public ActionResult Create(DegreeProgram degreeprogram)
        {
            if (ModelState.IsValid)
            {
                //degreePrograms.Add(degreeprogram);
                //degreePrograms.SaveChanges();
                degreeprogram = _degreeProgramProducer.Create(degreeprogram).First();
                degreeprogram = _degreeProgramProducer.Get(degreeprogram);
                return RedirectToAction("Edit", new { id = degreeprogram.ID });
            }

            return View(degreeprogram);
        }

        //
        // GET: /DegreePrograms/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id = 0)
        {
            //DegreeProgram degreeprogram = degreePrograms.Find(id);
            DegreeProgram degreeprogram = _degreeProgramProducer.Get(new DegreeProgram() { ID = id });
            if (degreeprogram == null)
            {
                return HttpNotFound();
            }
            degreeprogram.requiredCourses = degreeprogram.requiredCourses.OrderBy(reqcourse => reqcourse.semester).ThenBy(reqcourse => reqcourse.course.coursePrefix).ThenBy(reqcourse => reqcourse.course.courseNumber).ToList();
            degreeprogram.electiveCourses = degreeprogram.electiveCourses.OrderBy(eleccourse => eleccourse.semester).ThenBy(eleccourse => eleccourse.electiveList.electiveListName).ToList();
            return View(degreeprogram);
        }

        //
        // POST: /DegreePrograms/Edit/5

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(DegreeProgram degreeprogram, IEnumerable<RequiredCourse> RequiredCourses, IEnumerable<ElectiveCourse> ElectiveCourses)
        {
            if (ModelState.IsValid)
            {
                //DegreeProgram degreeAttached = degreePrograms.Where(degree => degree.ID == degreeprogram.ID).First();
                DegreeProgram degreeAttached = _degreeProgramProducer.Get(new DegreeProgram() { ID = degreeprogram.ID });
                degreeprogram.requiredCourses = degreeAttached.requiredCourses;
                degreeprogram.electiveCourses = degreeAttached.electiveCourses;

                if(RequiredCourses == null){
                    RequiredCourses = new List<RequiredCourse>();
                }

                if (ElectiveCourses == null)
                {
                    ElectiveCourses = new List<ElectiveCourse>();
                }

                //figures out which courses were deleted from the form
                LinkedList<RequiredCourse> toRemove = new LinkedList<RequiredCourse>();
                foreach (RequiredCourse course in degreeAttached.requiredCourses)
                {
                    if (course.ID > 0)
                    {
                        if (!RequiredCourses.Contains(course))
                        {
                            //RequiredCourse reqcourseAttached = requiredCourses.Where(reqc => reqc.ID == course.ID).First();
                            RequiredCourse reqcourseAttached = _requiredCourseProducer.Get(new RequiredCourse() { ID = course.ID });
                            toRemove.AddFirst(reqcourseAttached);
                        }
                    }
                }
                //deletes those courses from the list
                while(toRemove.Count > 0){
                    RequiredCourse removeme = toRemove.First();
                    toRemove.RemoveFirst();
                    //requiredCourses.Remove(removeme);
                    _requiredCourseProducer.Remove(removeme);
                }
                //clears the list
                degreeprogram.requiredCourses.Clear();
                //adds the courses that exist to the list
                foreach (RequiredCourse reqcourse in RequiredCourses)
                {
                    RequiredCourse reqcourseAttached = null; ;
                    if (reqcourse.ID > 0)
                    {
                        //reqcourseAttached = requiredCourses.Where(reqc => reqc.ID == reqcourse.ID).First();
                        reqcourseAttached = _requiredCourseProducer.Get(new RequiredCourse() { ID = reqcourse.ID });
                        //requiredCourses.UpdateValues(reqcourseAttached, reqcourse);
                        _requiredCourseProducer.Update(reqcourse);
                    }
                    else
                    {
                        //if (courses.Find(reqcourse.courseID) != null)
                        if(_courseProducer.Get(new Course(){ID = reqcourse.courseID}) != null)
                        {
                            //requiredCourses.Add(reqcourse);
                            //requiredCourses.SaveChanges();
                            _requiredCourseProducer.Create(reqcourse);
                            //reqcourseAttached = requiredCourses.Where(reqc => reqc.ID == reqcourse.ID).First();
                            reqcourseAttached = _requiredCourseProducer.Get(new RequiredCourse() { ID = reqcourse.ID });
                        }
                    }
                    if (reqcourseAttached != null)
                    {
                        degreeprogram.requiredCourses.Add(reqcourseAttached);
                    }
                }

                //figures out which elective lists were deleted from the list
                LinkedList<ElectiveCourse> toRemoveMe = new LinkedList<ElectiveCourse>();
                foreach (ElectiveCourse course in degreeAttached.electiveCourses)
                {
                    if (!ElectiveCourses.Contains(course))
                    {
                        //ElectiveCourse elcourseAttached = electiveCourses.Where(elc => elc.ID == course.ID).First();
                        ElectiveCourse elcourseAttached = _electiveCourseProducer.Get(new ElectiveCourse() { ID = course.ID });
                        toRemoveMe.AddFirst(elcourseAttached);
                    }
                }
                //remove those elective lists from the list
                while (toRemoveMe.Count > 0)
                {
                    ElectiveCourse removeme = toRemoveMe.First();
                    toRemoveMe.RemoveFirst();
                    //electiveCourses.Remove(removeme);
                    _electiveCourseProducer.Remove(removeme);
                }
                //clear the list
                degreeprogram.electiveCourses.Clear();
                //add the existing elective lists to the list
                foreach (ElectiveCourse elcourse in ElectiveCourses)
                {
                    ElectiveCourse elcourseAttached = null; ;
                    if (elcourse.ID > 0)
                    {
                        //elcourseAttached = electiveCourses.Where(elc => elc.ID == elcourse.ID).First();
                        elcourseAttached = _electiveCourseProducer.Get(new ElectiveCourse() { ID = elcourse.ID });
                        //electiveCourses.UpdateValues(elcourseAttached, elcourse);
                        _electiveCourseProducer.Update(elcourse);
                    }
                    else
                    {
                        //if (electiveLists.Find(elcourse.electiveListID) != null)
                        if(_electiveListProducer.Get(new ElectiveList(){ID = elcourse.electiveListID}) != null)
                        {
                            //electiveCourses.Add(elcourse);
                            //electiveCourses.SaveChanges();
                            _electiveCourseProducer.Create(elcourse);
                            //elcourseAttached = electiveCourses.Where(elc => elc.ID == elcourse.ID).First();
                            elcourseAttached = _electiveCourseProducer.Get(new ElectiveCourse() { ID = elcourse.ID });
                        }
                    }
                    if (elcourseAttached != null)
                    {
                        degreeprogram.electiveCourses.Add(elcourseAttached);
                    }
                }    
                
                //degreePrograms.UpdateValues(degreeAttached, degreeprogram);
                //degreePrograms.SaveChanges();
                _degreeProgramProducer.Update(degreeprogram);
                return RedirectToAction("Index");
            }
            if (RequiredCourses != null)
            {
                foreach (RequiredCourse course in RequiredCourses)
                {
                    if (course.courseID > 0)
                    {
                        //course.course = courses.Find(course.courseID);
                        course.course = _courseProducer.Get(new Course() { ID = course.courseID });
                    }
                }
            }
            degreeprogram.requiredCourses = RequiredCourses.ToList();

            if (ElectiveCourses != null)
            {
                foreach (ElectiveCourse course in ElectiveCourses)
                {
                    if (course.electiveListID > 0)
                    {
                        //course.electiveList = electiveLists.Find(course.electiveListID);
                        course.electiveList = _electiveListProducer.Get(new ElectiveList() { ID = course.electiveListID });
                    }
                }
            }
            degreeprogram.electiveCourses = ElectiveCourses.ToList();

            return View(degreeprogram);
        }

        /// <summary>
        /// @russfeld
        /// This uses AJAX to return the HTML code for a new required course row on the form
        /// Mad props to http://ivanz.com/2011/06/16/editing-variable-length-reorderable-collections-in-asp-net-mvc-part-1/
        /// </summary>
        /// <returns></returns>
        public ActionResult RequiredCourseRow(int id = 0)
        {
            if (id > 0)
            {
                RequiredCourse reqcourse = new RequiredCourse()
                {
                    degreeProgramID = id,
                    courseID = -1,
                    semester = 1
                };
                return PartialView("RequiredCourseFormPartial", reqcourse);
            }
            else
            {
                return PartialView("RequiredCourseFormPartial");
            }
        }

        /// <summary>
        /// @russfeld
        /// This uses AJAX to return the HTML code for a new elective course row on the form
        /// Mad props to http://ivanz.com/2011/06/16/editing-variable-length-reorderable-collections-in-asp-net-mvc-part-1/
        /// </summary>
        /// <returns></returns>
        public ActionResult ElectiveCourseRow(int id = 0)
        {
            if (id > 0)
            {
                ElectiveCourse elcourse = new ElectiveCourse()
                {
                    degreeProgramID = id,
                    electiveListID = -1,
                    semester = 1,
                    credits = 3,
                };
                return PartialView("ElectiveCourseFormPartial", elcourse);
            }
            else
            {
                return PartialView("ElectiveCourseFormPartial");
            }
        }

        //
        // GET: /DegreePrograms/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id = 0)
        {
            //DegreeProgram degreeprogram = degreePrograms.Find(id);
            DegreeProgram degreeprogram = _degreeProgramProducer.Get(new DegreeProgram() { ID = id });
            if (degreeprogram == null)
            {
                return HttpNotFound();
            }
            degreeprogram.requiredCourses = degreeprogram.requiredCourses.OrderBy(reqcourse => reqcourse.semester).ThenBy(reqcourse => reqcourse.course.coursePrefix).ThenBy(reqcourse => reqcourse.course.courseNumber).ToList();
            degreeprogram.electiveCourses = degreeprogram.electiveCourses.OrderBy(eleccourse => eleccourse.semester).ThenBy(eleccourse => eleccourse.electiveList.electiveListName).ToList();
            return View(degreeprogram);
        }

        //
        // POST: /DegreePrograms/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(int id)
        {
            //DegreeProgram degreeprogram = degreePrograms.Find(id);
            DegreeProgram degreeprogram = _degreeProgramProducer.Get(new DegreeProgram() { ID = id });
            //degreePrograms.Remove(degreeprogram);
            //degreePrograms.SaveChanges();
            _degreeProgramProducer.Remove(degreeprogram);
            return RedirectToAction("Index");
        }

        public JsonResult GetCourses(int id)
        {
            //DegreeProgram degreeProgram = degreePrograms.Find(id);
            DegreeProgram degreeProgram = _degreeProgramProducer.Get(new DegreeProgram() { ID = id });
            if (degreeProgram != null)
            {
                List<FlowchartCourse> results = new List<FlowchartCourse>();
                Dictionary<int, int> semesterOrders = new Dictionary<int, int>();
                for (int i = 1; i <= 8; i++)
                {
                    semesterOrders[i] = 0;
                }

                List<RequiredCourse> requirements = degreeProgram.requiredCourses.ToList();
                foreach (RequiredCourse req in requirements)
                {
                    FlowchartCourse pcourse = new FlowchartCourse();
                    pcourse.pcourseID = req.ID;
                    pcourse.courseID = req.courseID;
                    pcourse.courseTitle = req.course.courseCatalogNumber;
                    pcourse.courseName = req.course.courseTitle;
                    int order = semesterOrders[req.semester];
                    pcourse.order = order;
                    semesterOrders[req.semester] = order + 1;
                    pcourse.semester = req.semester;
                    pcourse.hours = req.course.shortHours;
                    pcourse.prereq = new int[req.course.prerequisites.Count];
                    int place = 0;
                    foreach (PrerequisiteCourse prereq in req.course.prerequisites)
                    {
                        pcourse.prereq[place++] = prereq.prerequisiteCourseID;
                    }
                    results.Add(pcourse);
                }

                List<ElectiveCourse> elects = degreeProgram.electiveCourses.ToList();
                foreach (ElectiveCourse elect in elects)
                {
                    FlowchartCourse pcourse = new FlowchartCourse();
                    pcourse.pcourseID = elect.ID;
                    pcourse.elistID = elect.electiveListID;
                    pcourse.elistName = elect.electiveList.shortName;
                    int order = semesterOrders[elect.semester];
                    pcourse.order = order;
                    semesterOrders[elect.semester] = order + 1;
                    pcourse.semester = elect.semester;
                    pcourse.hours = elect.credits.ToString();
                    results.Add(pcourse);
                }
                return Json(results.ToArray(), JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }

        protected override void Dispose(bool disposing)
        {
            //degreePrograms.Dispose();
            //electiveLists.Dispose();
            //requiredCourses.Dispose();
            //courses.Dispose();
            //electiveCourses.Dispose();
            base.Dispose(disposing);
        }
    }
}