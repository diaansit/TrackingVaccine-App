using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using RestSharp;
using TrackingVaksinApp.BPOMServiceReference;

namespace TrackingVaksinApp.Controllers
{
    public class BPOMVaksinController : Controller
    {
        // GET: BPOMVaksin
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            Models.ListViewModel viewmodel = new Models.ListViewModel
            {
                bpom_vaksins = obj.GetVaksin(),
                produsens = obj.GetListProdusen()
            };
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Index(string no_registrasi,DateTime create_at, string status)
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            Produsen_Vaksin newDataProdusen = null;
            Produsen_Vaksin cekStatus = obj.GetProdusenVaksinByNoReg(no_registrasi);
             BPOM_Vaksin newData = new BPOM_Vaksin
            {
                create_at = create_at,
                no_registrasi = no_registrasi,
                kemasan = cekStatus.kemasan,
                jumlah = cekStatus.jumlah,
                status = status
             };
            if (cekStatus != null)
            {
                if (!cekStatus.status.Equals("Not Reported"))
                {
                    newDataProdusen = new Produsen_Vaksin
                    {
                        create_at = create_at,
                        no_registrasi = no_registrasi,
                        kemasan = cekStatus.kemasan,
                        jumlah = cekStatus.jumlah,
                        status = status
                    };
                    obj.UpdateProdusenVaksin(newDataProdusen);
                }
            }
            obj.UpdateVaksin(newData);
            /*
            ViewBag.no_registrasi = no_registrasi;
            ViewBag.status = status;
            */
            Models.ListViewModel viewmodel = new Models.ListViewModel
            {
                bpom_vaksins = obj.GetVaksin(),
                produsens = obj.GetListProdusen()
            };
            return View(viewmodel);
        }

        public ActionResult CheckLogArrivalVaccine()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            Models.ListViewModel viewmodel = new Models.ListViewModel
            {
                LogsArrivals = obj.GetLogArrivalVaccine(),
                produsens = obj.GetListProdusen(),
                rumahSakits = obj.GetListRumahSakit(),
                rsVaksins = obj.GetVaksinRS()
            };
            return View(viewmodel);
        }

        public ActionResult CheckLogVaccineUse()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            
            Models.ListViewModel viewmodel = new Models.ListViewModel
            {
                logUses = obj.GetLogVaccineUse(),
                masyarakts = obj.GetListMasyarakat(),
                pasiens = obj.GetListPasien(),
                produsens = obj.GetListProdusen(),
                rumahSakits = obj.GetListRumahSakit(),
                rsVaksins = obj.GetVaksinRS()
            };
            return View(viewmodel);
        }

    }
}