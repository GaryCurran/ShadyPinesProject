using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShadyPines.Models;
using System.Web.Helpers;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Enums;
using System.Drawing;
// This is where all the magic happens!!!!!!
namespace ShadyPines.Controllers
{
    public class PatientsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Patients
        public ActionResult Index(string sarchString)
        // public ActionResult Index()
        {
            var res = from r in db.Patients select r;
            if (!String.IsNullOrEmpty(sarchString))
            {
                res = res.Where(s => s.Name.Contains(sarchString));
            }
            return View(res);
            // return View(db.Patients.ToList());
        }

        public ActionResult All()
        {
            return View(db.Patients.ToList());
        }

        public ActionResult Chart(int? id)
        {
            //Mine
            Patient pt = new Patient();

            MedicalQuestion mq = new MedicalQuestion();


            //mq = db.MedicalQuestions.Where(p => p.patientID == id).SingleOrDefault();
            // temp patient
            pt = db.Patients.Where(p => p.PatientID == id).SingleOrDefault();

            // Patients name
            ViewBag.name = pt.Name;
            ViewBag.a = pt.Gender;
            ViewBag.b = pt.MedicalCard;
            ViewBag.c = pt.DoctorName;

            // get all daily scores
            // from DB and transfer to array
            var tot = from e in db.MedicalQuestions
                      where e.patientID == id
                      select e;


            // get total amount of objects to set size of object arrays
            int size = tot.Count();

            string[] t = new string[size];
            int pos = 0;
            foreach (var it in tot)
            {
                t[pos] = it.Date.Date.ToShortDateString();
                pos++;
            }
            //DateTime date1 = new DateTime(2008, 6, 1, 7, 47, 0);
            //DateTime dateOnly = date1.Date;
            string[] dy = new string[size];
            for (int j = 0; j < size; j++)
            {

                dy[j] = t[j].ToString();

            }
            // Use total amount of daily reports
            // to size array
            // pass to chart as an object
            object[] numbers = new object[size];
            int i = 0;

            // holding the total daily scores
            foreach (var item in tot)
            {
                numbers[i] = item.DailyTotal;
                i++;
            }

            // Sleep Paterns for period
            object[] quest1 = new object[size];
            int count = 0;
            foreach (var item in tot)
            {
                quest1[count] = (int)item.Question1 + 1 * size;
                count++;
            }

            // calculate fall period
            int fall = 0;
            int fell = 0;
            int noFall = 0;
            object[] falls = new object[size];
            foreach (var item in tot)
            {
                if (item.HasFallen.Equals(true))
                {
                    fell++;
                    fall++;
                }
                else
                {
                    noFall++;
                    fall++;
                }
            }

            // Appetite for period
            object[] quest2 = new object[size];
            int count2 = 0;
            foreach (var item in tot)
            {
                quest2[count2] = (int)item.Question2 + 1 * size;
                count2++;
            }

            // Appetite for period
            object[] quest3 = new object[size];
            int count3 = 0;
            foreach (var item in tot)
            {
                quest3[count3] = (int)item.Question3 + 1 * size;
                count3++;
            }
            // length of stay determined by size of array
            string[] days = new string[size];

            // display No. of days on X axis
            for (int j = 0; j < days.Length; j++)
            {
                days[j] = "Day " + (j + 1);
            }

            // create another array to run comparissions
            object[] n = numbers;

            // sum total daily scores
            int s = 0;
            for (int k = 0; k < numbers.Length; k++)
            {
                s += (int)n[k];
            }

            // get avg daily score
            @ViewBag.avg = s / n.Length;

            // avg daily score to pass to graph
            object[] avgerageScore = new object[size];

            // populate avgerageScore []
            for (int g = 0; g < days.Length; g++)
            {
                avgerageScore[g] = @ViewBag.avg;
            }

            // total number of daily observations
            ViewBag.total = numbers;

            // total number of days in hospital
            int d = days.Count();
            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart")

                .SetXAxis(new XAxis
                {
                    Title = new XAxisTitle { Text = "Observation period from: " + dy[0] + " to: " + dy[dy.Length - 1] },
                    Categories = dy
                    
                })//days })
                    .SetTitle(new Title { Text = "Health Record of " + pt.Name })
                    .SetLegend(new Legend
                    {
                        Layout = Layouts.Vertical,
                        Align = HorizontalAligns.Right,
                        VerticalAlign = VerticalAligns.Top,
                        X = -10,
                        Y = 100,
                        BorderWidth = 0
                        })
                        .SetLabels(new Labels
                        {
                            
                        } )
                     .SetTooltip(new Tooltip
                     {
                         Shared = true,
                         Crosshairs = new Crosshairs(true)
                     })
                .SetPlotOptions(new PlotOptions
                {
                    Pie = new PlotOptionsPie
                    {
                        Center = new[] { new PercentageOrPixel(40), new PercentageOrPixel(20) },
                        Size = new PercentageOrPixel(80),
                        ShowInLegend = true,
                        DataLabels = new PlotOptionsPieDataLabels { Enabled = false }
                        
                    }
                })
                 .SetSeries(new[]
                            {
                                new Series {Name = "Daily Total",Color = ColorTranslator.FromHtml ("Blue"), Id= " Hello" ,Data = new Data (numbers)},

                                 new Series {Name = "Sleep Pattterns",Color = ColorTranslator.FromHtml ("Purple"), Data = new Data (quest1)},

                                 new Series {Name = "Appetite",Color = ColorTranslator.FromHtml ("Orange"), Data = new Data (quest2)},

                                 new Series {Name = "Resident Awareness",Color = ColorTranslator.FromHtml ("Brown"), Data = new Data (quest3)},

                                new Series {Name = "Daily Average " , Color = ColorTranslator.FromHtml("Green"), Data = new Data (avgerageScore)},

                                new Series
                    {
                        Type = ChartTypes.Pie,
                        Name = "Falls Observation",
                        Data = new Data(new[]
                        {
                            new DotNet.Highcharts.Options.Point
                            {
                                Name = "Days falls occured",
                                Y = fell,
                                Color = Color.FromName("Red")
                            },
                            new DotNet.Highcharts.Options.Point
                            {
                                Name = "Days no falls occured" ,
                                Y = noFall ,
                                Color = Color.FromName("Green")
                            },
                          }
                      )
                    }
           });

            // traverse the array to find lowest & highest score

            int bestDay = 0;
            int worstDay = 1000;
            for (int j = 0; j < numbers.Length; j++)
            {
                if ((int)numbers[j] > bestDay)
                {
                    bestDay = (int)numbers[j];
                }
            }

            for (int j = 0; j < numbers.Length; j++)
            {
                if ((int)numbers[j] < worstDay)
                {
                    worstDay = (int)numbers[j];
                }
            }
            @ViewBag.worst = worstDay;
            @ViewBag.best = bestDay;

            // total observations passed to view
            @ViewBag.tot = n.Length;
            return View(chart);
        }

        //public ActionResult SortArray(object  [] n)
        //{
        //    //Array.Sort(n);
        //    //return n.ElementAt<;
        //}

        public ActionResult Show()
        {
            return View();
        }
        // GET: Patients/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // GET: Patients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PatientID,Name,MedicalCard,Gender,DoctorName")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Patients.Add(patient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(patient);
        }

        // GET: Patients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PatientID,Name,MedicalCard,Gender,DoctorName")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(patient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Patient patient = db.Patients.Find(id);
            if (patient == null)
            {
                return HttpNotFound();
            }
            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Patient patient = db.Patients.Find(id);
            db.Patients.Remove(patient);
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
