using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackingVaksinApp.BPOMServiceReference;

namespace TrackingVaksinApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            EBPOMServiceClient obj = new EBPOMServiceClient();
            Models.ListViewModel viewmodel = new Models.ListViewModel
                {
                    bpom_vaksins = obj.GetVaksin(),
                    produsens = obj.GetListProdusen()
                };

            ViewBag.CNT = ResultArrival().Count();
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Index(string kat_input,string value_input)
        {
            EBPOMServiceClient obj = new EBPOMServiceClient();
            Session["InSearch"] = "True";
            IEnumerable<BPOM_Vaksin> list = new List<BPOM_Vaksin>();
            if (kat_input.Length < 1 || value_input.Length < 1)
            {
                list = obj.GetVaksin();
            }
            else
            {
                if (kat_input.Equals("0"))
                {
                    list = obj.GetVaksin().Where(X => X.no_registrasi.Equals(value_input)).ToList();
                }
                else if (kat_input.Equals("1"))
                {
                    BPOMServiceReference.Produsen produsen = obj.GetListProdusen().FirstOrDefault(X => X.Nama.Equals(value_input));
                    if (produsen != null)
                        list = obj.GetVaksin().Where(X => X.id_produsen == produsen.id).ToList();
                    else
                        list = null;
                }
            }
            Models.ListViewModel viewmodel = new Models.ListViewModel
            {
                bpom_vaksins = list,
                produsens = obj.GetListProdusen()
            };
            return View(viewmodel);
        }

        public ActionResult VisualizeProdusenVaksinResult()
        {
            return Json(Result(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult VisualizeArrivalVaccine()
        {
            return Json(ResultArrival(), JsonRequestBehavior.AllowGet);
        }


        public List<Models.ProdusenVisualization> Result()
        {
            EBPOMServiceClient obj = new EBPOMServiceClient();
            List<Models.ProdusenVisualization> list = new List<Models.ProdusenVisualization>();

            foreach(var data in obj.GetListProdusen())
            {
                list.Add(new Models.ProdusenVisualization
                {
                    Name = data.Nama,
                    ProdukCount = obj.GetVaksin().Where(X => X.id_produsen == data.id).Count()
                });
            }

            return list;
        }

        /*
        (from emp in Employee.GetAllEmployees()
                              join address in Address.GetAllAddresses()
                              on emp.AddressId equals address.ID
                              select new
                               {
                                   EmployeeName = emp.Name,
                                   AddressLine = address.AddressLine
    }).ToList();
    */

    public List<Models.ArrivalVaccineVisualization> ResultArrival()
        {
            EBPOMServiceClient obj = new EBPOMServiceClient();
            List<Models.ArrivalVaccineVisualization> list = new List<Models.ArrivalVaccineVisualization>();
            var data = (
                    from LGA in obj.GetLogArrivalVaccine()
                    join R in obj.GetListRumahSakit()
                    on LGA.id_RumahSakit equals R.id
                    group new { R } by new
                    {
                        R.Nama
                    }
                    into LGAR
                    select new
                    {
                        RSName = LGAR.Key.Nama,
                        VaccineCount = LGAR.Count()
                    }
                ).AsEnumerable();
            
            foreach(var newdata in data)
            {
                list.Add(new Models.ArrivalVaccineVisualization
                {
                    RS_Name = newdata.RSName,
                    Vaccine_count = newdata.VaccineCount
                });
            }

            return list;
                        
        }
    }
}