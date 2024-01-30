using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CarInsurance.Models;
using Microsoft.Ajax.Utilities;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        
        private int View(Func<ActionResult> index)
        {
            throw new NotImplementedException();
        }

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
           
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                insuree.Quote = CalculateQuote(insuree);                 //the FUNCTION dito tatawagin ang function na ginawa ko sa baba
                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }



        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
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


        //public int Claculate;

        //Calculations are written below:
        public int CalculateQuote(Insuree insuree)
        //parameters are data that the funtion takes 
        {
            decimal baseQuote = 50;                                                     //Base
            int edad = DateTime.Now.Year - insuree.DateOfBirth.Year;                //Stores age of insuree
            int ticket = insuree.SpeedingTickets;                                   //Count of speeding ticket/s    
            bool dD = insuree.DUI;                                                  //Checks if DUI is true or false
            bool FullCov = insuree.CoverageType;
            string carMake = insuree.CarMake.ToLower();                             //Stores car make and model Porsche and 911 Carrera
            string carModel = insuree.CarModel.ToLower();

            
            //IF Porsche and 911 Carera with coverage and DUI
            if (carMake == "porsche" && carModel == "911 carrera" && edad <= 18 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == true 
                && insuree.DUI == true)
            {                             //Age   year  Por      if there's ticket    
                decimal Basic1 = baseQuote + 100 + 25 + 50;
                decimal Basic2 = (Basic1 / 4) + Basic1; //25% for DUI
                decimal Basic3 = (Basic2 / 2) + Basic2; //for full coverage
                decimal FinalQuote = Basic3 + ticket * 10;                               
                return (int)FinalQuote;
            }

            //IF Porsche and 911 Carera without coverage but with DUI
            if (carMake == "porsche" && carModel == "911 carrera" && edad <= 18 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == false
                && insuree.DUI == true)
            {                                 
                decimal Basic1 = baseQuote + 100 + 25 + 50;
                decimal Basic2 = (Basic1 / 4) + Basic1; //25% for DUI
                //decimal Basic3 = (Basic2 / 2) + Basic2; //for full coverage
                decimal FinalQuote = Basic2 + ticket * 10;
                return (int)FinalQuote;
            }

            //IF Porsche and 911 Carera with coverage but no DUI.
            if (carMake == "porsche" && carModel == "911 carrera" && edad <= 18 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == true
                && insuree.DUI == false)
            {                                
                decimal Basic1 = baseQuote + 100 + 25 + 50;               
                decimal Basic3 = (Basic1 / 2) + Basic1; //for full coverage
                decimal FinalQuote = Basic3 + ticket * 10;
                return (int)FinalQuote;
            }


            //IF Porsche where age is less than 18, Car Year is before 2000 or after 2015, No coverage and no DUI
            if (carMake == "porsche" && carModel == "911 carrera" && edad <= 18 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == false
                && insuree.DUI == false)
            {                                 
                decimal Basic1 = baseQuote + 100 + 25;                
                decimal FinalQuote = Basic1 + ticket * 10; //add 10 for each ticket (*10)
                return (int)FinalQuote;
            }

            //NOT PORSCHE STARTS HERE

            //Not Porsche where age is below 18 , Car Year is before 2000 or after 2015, Coverage is FULL, and has DUI
            if (carMake != "porsche" && edad < 18 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == true
                && insuree.DUI == true)
            {
                decimal Basic1 = baseQuote + 100 + 25;
                decimal Basic2 = (Basic1 / 4) + Basic1; //25% for DUI
                decimal Basic3 = (Basic2 / 2) + Basic2; //for full coverage
                decimal FinalQuote = Basic3 + ticket * 10; //add 10 for each ticket (*10)
                return (int)FinalQuote;
            }

            //Not Porsche where age is below 18 , Car Year is before 2000 or after 2015, without coverage, and has DUI
            if (carMake != "porsche" && edad < 18 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == false
                && insuree.DUI == true)
            {
                decimal Basic1 = baseQuote + 100 + 25;
                decimal Basic2 = (Basic1 / 4) + Basic1; //25% for DUI                
                decimal FinalQuote = Basic2 + (ticket * 10); //add 10 for each ticket (*10)
                return (int)FinalQuote;
            }

            //Not Porsche where age is below 18 , Car Year is before 2000 or after 2015, with coverage, and no DUI
            if (carMake != "porsche" && edad < 18 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == true
                && insuree.DUI == false)
            {
                decimal Basic1 = baseQuote + 100 + 25;                
                decimal Basic3 = (Basic1 / 2) + Basic1; //for full coverage
                decimal FinalQuote = Basic3 + (ticket * 10); //add 10 for each ticket (*10)
                return (int)FinalQuote;
            }

            //Not Porsche where age is below 18 , Car Year is before 2000 or after 2015, No coverage and No DUI
            if (carMake != "porsche" && edad < 18 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == false
                && insuree.DUI == false)
            {
                decimal Basic1 = baseQuote + 100 + 25;                
                decimal FinalQuote = Basic1 + (ticket * 10); //add 10 for each ticket (*10)
                return (int)FinalQuote;
            }

            //Not Porsche where age is between 18 and 25, Car Year is before 2000 or after 2015, Coverage is FULL, and has DUI
            if (carMake != "porsche" && edad > 18 && edad < 25 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == true
                && insuree.DUI == true)
            {
                decimal Basic1 = baseQuote + 50 + 25;
                decimal Basic2 = (Basic1 / 4) + Basic1; //25% for DUI
                decimal Basic3 = (Basic2 / 2) + Basic2; //for full coverage
                decimal FinalQuote = Basic3 + ticket * 10; //add 10 for each ticket (*10)
                return (int)FinalQuote;
            }

            //Not Porsche where age is between 18 and 25, Car Year is before 2000 or after 2015, No Coverage is but has DUI
            if (carMake != "porsche" && edad > 18 && edad < 25 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == false
                && insuree.DUI == true)
            {
                decimal Basic1 = baseQuote + 50 + 25;
                decimal Basic2 = (Basic1 / 4) + Basic1; //25% for DUI                
                decimal FinalQuote = Basic2 + (ticket * 10); //add 10 for each ticket (*10)
                return (int)FinalQuote;
            }

            //Not Porsche where age is between 18 and 25, Car Year is before 2000 or after 2015, With Coverage is but No DUI
            if (carMake != "porsche" && edad > 18 && edad < 25 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == true
                && insuree.DUI == false)
            {
                decimal Basic1 = baseQuote + 50 + 25;                               
                decimal FinalQuote = ((Basic1 / 2) + Basic1) + (ticket * 10); //add 10 for each ticket (*10)
                return (int)FinalQuote;
            }

            //Not Porsche where age is between 18 and 25, Car Year is before 2000 or after 2015, Without Coverage & No DUI
            if (carMake != "porsche" && edad > 18 && edad < 25 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == false
                && insuree.DUI == false)
            {
                decimal Basic1 = baseQuote + 50 + 25;
                decimal FinalQuote = Basic1 + (ticket * 10); //add 10 for each ticket (*10)
                return (int)FinalQuote;
            }

            //Not Porsche where age is older or = 26, Car Year is before 2000 or after 2015, Coverage is FULL, and has DUI
            if (carMake != "porsche" && edad >= 26 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == true
                && insuree.DUI == true)
            {
                decimal Basic1 = baseQuote + 25 + 25;
                decimal Basic2 = (Basic1 / 4) + Basic1; //25% for DUI
                decimal Basic3 = (Basic2 / 2) + Basic2; //for full coverage
                decimal FinalQuote = Basic3 + ticket * 10; //add 10 for each ticket (*10)
                return (int)FinalQuote;
            }

            //Not Porsche where age is older or = 26, Car Year is before 2000 or after 2015, No Coverage but has DUI
            if (carMake != "porsche" && edad >= 26 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == false
                && insuree.DUI == true)
            {
                decimal Basic1 = baseQuote + 25 + 25;
                decimal Basic2 = (Basic1 / 4) + Basic1; //25% for DUI                
                decimal FinalQuote = Basic2 + (ticket * 10); //add 10 for each ticket (*10)
                return (int)FinalQuote;
            }

            //Not Porsche where age is older or = 26, Car Year is before 2000 or after 2015, With Coverage but No DUI
            if (carMake != "porsche" && edad >= 26 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == true
                && insuree.DUI == false)
            {
                decimal Basic1 = baseQuote + 25 + 25;                              
                decimal FinalQuote = ((Basic1/2)+Basic1) + (ticket * 10); //add 10 for each ticket (*10)
                return (int)FinalQuote;
            }

            //Not Porsche where age is older or = 26, Car Year is before 2000 or after 2015, No Coverage & No DUI
            if (carMake != "porsche" && edad >= 26 && (insuree.CarYear > 2015 || insuree.CarYear < 2000) && insuree.CoverageType == true
                && insuree.DUI == false)
            {
                decimal Basic1 = baseQuote + 25 + 25;
                decimal FinalQuote = Basic1 + (ticket * 10); //add 10 for each ticket (*10)
                return (int)FinalQuote;
            }

            throw new InvalidOperationException("No applicable conditions for insurance quote calculation.");

        }
    }
}
