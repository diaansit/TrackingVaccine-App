using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackingVaksinApp.BPOMServiceReference;

namespace TrackingVaksinApp.Controllers.Produsen
{
    public class HomeProdusenController : Controller
    {
        // GET: Home
        [Route("HomeProdusen" ,Name="Produsen")]
        public ActionResult Index()
        {
            EBPOMServiceClient obj = new EBPOMServiceClient();
            if (Session["username"] == null)
                return RedirectToRoute("Autentication",new { controller="User" });
            else
            {
                Akun cekAkun = obj.GetAkun(Session["username"].ToString());
                if (cekAkun.role.Equals("Produsen"))
                {
                    BPOMServiceReference.Produsen curP = obj.GetProdusen(Session["username"].ToString());
                    ViewBag.TotalProduksiVaksin = obj.GetProdusenVaksin(curP.id.ToString()).Count();
                    return View(obj.GetProdusenVaksin(curP.id.ToString()));
                }
                Session.RemoveAll();
                return RedirectToAction("Login");

            }
        }

        [HttpGet]
        [Route(Name = "ProdusenClient")]
        public ActionResult Create()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication",new { controller="User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            BPOMServiceReference.Produsen curP = obj.GetProdusen(Session["username"].ToString());
            ViewBag.TotalProduksiVaksin = obj.GetProdusenVaksin(curP.id.ToString()).Count();
            return View();
        }


        [HttpPost]
        public ActionResult Create(string no_registrasi , string kemasan , int jumlah)
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication",new { controller="User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            BPOMServiceReference.Produsen curP = obj.GetProdusen(Session["username"].ToString());
            if (obj.GetVaksinDetails(no_registrasi) != null)
            {
                return View();
            }
            else
            {
                Produsen_Vaksin data = new Produsen_Vaksin
                {
                    create_at = DateTime.Now,
                    id_produsen = curP.id,
                    no_registrasi = no_registrasi,
                    kemasan = kemasan,
                    jumlah = jumlah,
                    status = "Not Reported"
                };
                if (obj.TambahProdusenVaksin(data) != null)
                {
                    return RedirectToAction("Index");
                }
            }
            return View();

        }

        public ActionResult LaporVaksin()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication",new { controller="User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            Produsen_Vaksin newProdusenVaksin = null;
            string no_registrasi = this.ControllerContext.RouteData.Values["id"].ToString();
            BPOMServiceReference.Produsen curP = obj.GetProdusen(Session["username"].ToString());
            Produsen_Vaksin detail = obj.GetProdusenVaksinDetails(curP.id.ToString(), no_registrasi);
            BPOM_Vaksin data = new BPOM_Vaksin
            {
                create_at = DateTime.Now,
                id_produsen = curP.id,
                no_registrasi = no_registrasi,
                kemasan = detail.kemasan,
                jumlah = detail.jumlah,
                status = "Invalid"
            };
            newProdusenVaksin = new Produsen_Vaksin
            {
                create_at = DateTime.Now,
                id_produsen = curP.id,
                no_registrasi = no_registrasi,
                jumlah = detail.jumlah,
                kemasan = detail.kemasan,
                status = "Invalid"
            };
            if (obj.Up(data) != null)
            {
                obj.UpdateProdusenVaksin(newProdusenVaksin);
                return RedirectToAction("Index", "HomeProdusen");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Distributed()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication",new { controller="User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            BPOMServiceReference.Produsen curP = obj.GetProdusen(Session["username"].ToString());
            ViewBag.TotalProduksiVaksin = obj.GetProdusenVaksin(curP.id.ToString()).Count();
            IEnumerable<RumahSakit> listRS = obj.GetListRumahSakit().ToList();
            IEnumerable<Produsen_Vaksin> listPVaksin = obj.GetProdusenVaksin(curP.id.ToString()).ToList();
            Models.ListViewModel viewModel = new Models.ListViewModel
            {
                rumahSakits = listRS,
                produsen_Vaksins = listPVaksin
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Distributed(string kode_ref, int id_rumahSakit, string[] no_registrasi)
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication",new { controller="User" });
            try
            {
                EBPOMServiceClient obj = new EBPOMServiceClient();
                List<RS_Vaksin> listRS = new List<RS_Vaksin>();
                BPOMServiceReference.Produsen curP = obj.GetProdusen(Session["username"].ToString());
                foreach (string noreg in no_registrasi) {
                    Produsen_Vaksin PDetail = obj.GetProdusenVaksinDetails(curP.id.ToString(), noreg);
                    RS_Vaksin data = new RS_Vaksin
                    {
                        create_at = DateTime.Now,
                        id_produsen = curP.id,
                        id_rumahSakit = id_rumahSakit,
                        kode_ref = kode_ref,
                        kemasan = PDetail.kemasan,
                        jumlah = PDetail.jumlah,
                        no_registrasi = noreg,
                        isReported = 0
                    };
                    Produsen_Vaksin data2 = new Produsen_Vaksin {
                        create_at = DateTime.Now,
                        id_produsen = curP.id,
                        no_registrasi = noreg,
                        jumlah = 0,
                        kemasan = PDetail.kemasan,
                        status = "Was Distributed"
                    };
                    listRS.Add(data);
                    obj.UpdateProdusenVaksin(data2);
                }
                obj.TambahListRSVaksin(listRS);
                //delete after send to RS
                return RedirectToRoute("Produsen",new { controller="HomeProdusen",action="Index" });

            }
            catch
            {
                throw new Exception();
            }
        }
    }
}