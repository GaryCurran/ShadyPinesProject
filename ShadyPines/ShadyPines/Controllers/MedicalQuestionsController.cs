﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShadyPines.Models;
using System.Collections;

namespace ShadyPines.Controllers
{
    public class MedicalQuestionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MedicalQuestions
        public ActionResult Index()
        {
            return View(db.MedicalQuestions.ToList());
        }

        // GET: MedicalQuestions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicalQuestion medicalQuestion = db.MedicalQuestions.Find(id);
            if (medicalQuestion == null)
            {
                return HttpNotFound();
            }
            return View(medicalQuestion);
        }

        // GET: MedicalQuestions/Create
        public ActionResult Create(int id)
        {
            Patient pt = new Patient();

            pt.PatientID = id;

            //ArrayList nList = new ArrayList();

            //var n = from name in db.Nurses select name;

            //foreach (var item in n)
            //{
            //    nList.Add(item.Name);
            //}

            List <SelectListItem> nurs = new List<SelectListItem>();
            var n = from name in db.Nurses select name;

            foreach (var item in n)
            {
                nurs.Add(new SelectListItem { Text = item.Name});
            }

            // temp patient
            pt = db.Patients.Where(p => p.PatientID == pt.PatientID).SingleOrDefault();

            ViewBag.nurses = nurs;
            ViewBag.name = pt.Name;
            return View(new MedicalQuestion() {Date = DateTime.Now});
        }

        // POST: MedicalQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MedicalQuestionID,HasFallen,Question1,Question2,Question3,Question4,Question5,Question6,Question7,Question8,Question9,NurseTaken,Date")] MedicalQuestion medicalQuestion, int id)
        {
            if (ModelState.IsValid)
            {
                Patient pt = new Patient();
                
              
                pt.PatientID = id;

                // temp patient
                pt = db.Patients.Where(p => p.PatientID == pt.PatientID).SingleOrDefault();

                
                medicalQuestion.patientID = id;
                if (medicalQuestion.HasFallen.Equals (true))
                {
                    medicalQuestion.DailyTotal = (int)medicalQuestion.Question1 + 1 + (int)medicalQuestion.Question2 + 1
                                                               + (int)medicalQuestion.Question3 + 1 + (int)medicalQuestion.Question4 + 1+
                                                               (int)medicalQuestion.Question5 + 1 + (int)medicalQuestion.Question6 + 1
                                                               + (int)medicalQuestion.Question7 + 1 + (int)medicalQuestion.Question8 + 1 + (int)medicalQuestion.Question9 + 1;

                    int helper = medicalQuestion.DailyTotal / 2;
                    medicalQuestion.DailyTotal += helper;
                }
                else
                {
                    medicalQuestion.DailyTotal = (int)medicalQuestion.Question1 + 1 + (int)medicalQuestion.Question2 + 1
                                                               + (int)medicalQuestion.Question3 + 1 + (int)medicalQuestion.Question4 + 1 +
                                                               (int)medicalQuestion.Question5 + 1 + (int)medicalQuestion.Question6 + 1
                                                               + (int)medicalQuestion.Question7 + 1 + (int)medicalQuestion.Question8 + 1 + (int)medicalQuestion.Question9 + 1;
                }
                
                

                pt.questions.Add(medicalQuestion);
                
                ViewBag.count = pt.questions.Count();
                

                db.MedicalQuestions.Add(medicalQuestion);
                db.SaveChanges();
                return RedirectToAction("Index", "Patients");
            }

            return View(medicalQuestion);
        }

        // GET: MedicalQuestions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicalQuestion medicalQuestion = db.MedicalQuestions.Find(id);
            if (medicalQuestion == null)
            {
                return HttpNotFound();
            }
            return View(medicalQuestion);
        }

        // POST: MedicalQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MedicalQuestionID,Question1,Question2,NurseTaken,Date")] MedicalQuestion medicalQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicalQuestion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(medicalQuestion);
        }

        // GET: MedicalQuestions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicalQuestion medicalQuestion = db.MedicalQuestions.Find(id);
            if (medicalQuestion == null)
            {
                return HttpNotFound();
            }
            return View(medicalQuestion);
        }

        // POST: MedicalQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MedicalQuestion medicalQuestion = db.MedicalQuestions.Find(id);
            db.MedicalQuestions.Remove(medicalQuestion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
