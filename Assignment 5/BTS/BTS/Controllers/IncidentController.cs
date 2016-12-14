﻿//
// author: Mona and Jagmeet
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BTS.Controllers
{
    public class IncidentController : Controller
    {
        private Manager m = new Manager();
        // GET: Incident

        //Mona
        public ActionResult Index()
        {
            m.LoadData();
            return View(m.IncidentGetAll());
        }

        // GET: Incident/Details/5

        //Mona
        public ActionResult Details(int? id)
        {
            var o = m.IncidentGetOne(id.GetValueOrDefault());

            if (o == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(o);
            }
        }

        /*
        // GET: Incident/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Incident/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        */
        // GET: Incident/Edit/5

        //Jagmeet
        public ActionResult Edit(int? id)
        {
            var o = m.IncidentGetOne(id.GetValueOrDefault());

            if (o == null)
            {
                return HttpNotFound();
            }
            else
            {
                // Create and configure an "edit form"

                // Notice that o is a IncidentBase object
                // We must map it to a IncidentEditForm object
                // Notice that we can use AutoMapper anywhere,
                // and not just in the Manager class!
                var editForm = AutoMapper.Mapper.Map<IncidentEditForm>(o);

                editForm.InstructorName = o.Instructor.name;
                editForm.InstructorId = o.Instructor.Id;

                editForm.StudentIds = new List<string>();
                editForm.StudentNames = new List<string>();
                foreach (var stud in o.Students)
                {
                    editForm.StudentIds.Add(stud.studentId);
                    editForm.StudentNames.Add(stud.name);
                }

                editForm.StudentIds.Add("");
                editForm.StudentNames.Add("");



                return View(editForm);
                //return View();
            }
        }

        
        // POST: Incident/Edit/5

        //Jagmeet
        [HttpPost]
        public ActionResult Edit(int ? id, IncidentEdit newItem)
        {
            // Validate the input
            if (!ModelState.IsValid)
            {
                // Our "version 1" approach is to display the "edit form" again
                return RedirectToAction("edit", new { id = newItem.Id });
            }

           if (id.GetValueOrDefault() != newItem.Id)
            {
                // This appears to be data tampering, so redirect the user away
                return RedirectToAction("index");
            }

            newItem.StudentIds.Remove("");
            newItem.StudentNames.Remove("");
           
            // Attempt to do the update
            var editedItem = m.IncidentEdit(newItem);

            if (editedItem == null)
            {
                // There was a problem updating the object
                // Our "version 1" approach is to display the "edit form" again
                return RedirectToAction("edit", new { id = newItem.Id });
            }
            else
            {
                // Show the details view, which will have the updated data
                return RedirectToAction("details", new { id = newItem.Id });
            }
        }
        /* try
         {
             // TODO: Add update logic here

             return RedirectToAction("Index");
         }
         catch
         {
             return View();
         }
     }*/

        /*
        // GET: Incident/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Incident/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        */
    }
}