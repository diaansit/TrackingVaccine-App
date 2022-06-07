using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrackingVaksinApp.BPOMServiceReference;

namespace TrackingVaksinApp.Controllers.RS
{
    public class HomeRSController : Controller
    {
        // GET: HomeRS
        public ActionResult Index()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            BPOMServiceReference.RumahSakit curR = obj.GetRS(Session["username"].ToString());
            return View(obj.GetVaksinRS().Where(X=>X.id_rumahSakit==curR.id));
        }

        public ActionResult CekVaksin()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            if (this.ControllerContext.RouteData.Values["id"] != null)
            {
                string no_reg = this.ControllerContext.RouteData.Values["id"].ToString();
                Models.ListViewModel viewmodel = new Models.ListViewModel
                {
                    singlevaksin = obj.GetVaksinDetails(no_reg),
                    produsens = obj.GetListProdusen()
                };
                return View(viewmodel);
            }
            Models.ListViewModel viewmodel2 = new Models.ListViewModel
            {
                singlevaksin = null,
                produsens = null
            };
            return View(viewmodel2);
        }

        [HttpPost]
        public ActionResult CekVaksin(string no_registrasi)
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            Models.ListViewModel viewmodel = new Models.ListViewModel
            {
                singlevaksin = obj.GetVaksinDetails(no_registrasi),
                produsens = obj.GetListProdusen()
            };
            return View(viewmodel);
        }

        public ActionResult CekReceiveVaksin()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            BPOMServiceReference.RumahSakit curR = obj.GetRS(Session["username"].ToString());
            return View(obj.GetVaksinRS().Where(X=>X.id_rumahSakit==curR.id));
        }

        public ActionResult ReportArrivalVaccine()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            string noreg = this.ControllerContext.RouteData.Values["id"].ToString();
            BPOMServiceReference.RumahSakit curR = obj.GetRS(Session["username"].ToString());
            RS_Vaksin newData = obj.GetVaksinRSDetails(noreg);
            BPOM_Log_Kedatangan_Vaksin dataAdd = new BPOM_Log_Kedatangan_Vaksin
            {
                id_produsen = newData.id_produsen,
                id_RumahSakit = curR.id,
                create_at = newData.create_at,
                no_registrasi = newData.no_registrasi
            };

            newData.isReported = 1; 
            if (obj.ReportArrivalVaccine(dataAdd).statusCode.Equals(HttpStatusCode.OK.ToString()))
            {
                if (obj.UpdateRSVaksin(newData) != null)
                {
                    ViewBag.Err = obj.ReportArrivalVaccine(dataAdd).Message.ToString();
                    return RedirectToRoute("RS", new { controller = "HomeRS", action = "CekReceiveVaksin"});
                }
            }
            return RedirectToRoute("RS", new { controller = "HomeRS", action = "CekReceiveVaksin" , Err = obj.ReportArrivalVaccine(dataAdd).Description.ToString() });
        }

        public ActionResult ReportAllArrivalVaccine()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            BPOMServiceReference.RumahSakit curR = obj.GetRS(Session["username"].ToString());
            if (obj.ReportListArrivalVaccine(obj.GetLogArrivalVaccine()))
            {
                IEnumerable<RS_Vaksin> listAll = obj.GetVaksinRS().Where(X => X.id_rumahSakit==curR.id && X.isReported == 0).ToList();
                List<RS_Vaksin> newList = new List<RS_Vaksin>();
                List<BPOM_Log_Kedatangan_Vaksin> LogBaru = new List<BPOM_Log_Kedatangan_Vaksin>();
                foreach (var data in listAll)
                {
                    BPOM_Log_Kedatangan_Vaksin singleData = new BPOM_Log_Kedatangan_Vaksin
                    {
                        id_produsen = data.id_produsen,
                        id_RumahSakit = curR.id,
                        create_at = data.create_at,
                        no_registrasi = data.no_registrasi
                    };
                    LogBaru.Add(singleData);
                }
                if (obj.ReportListArrivalVaccine(LogBaru))
                {
                    foreach (var data in listAll)
                    {
                        data.isReported = 1;
                        newList.Add(data);
                    }
                    if (obj.UpdateListRSVaksin(newList))
                        return RedirectToRoute("RS", new { controller = "HomeRS", action = "CekReceiveVaksin" });
                }
            }
            return RedirectToAction("CekReceiveVaksin");
        }

        public ActionResult ReportByKodeReff(string kode_ref)
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            BPOMServiceReference.RumahSakit curR = obj.GetRS(Session["username"].ToString());
            IEnumerable<RS_Vaksin> listVaksinbyRef = obj.GetVaksinRS().Where(X => X.kode_ref.Equals(kode_ref));
            List<BPOM_Log_Kedatangan_Vaksin> LogBaru = new List<BPOM_Log_Kedatangan_Vaksin>();
            foreach(var data in listVaksinbyRef)
            {
                BPOM_Log_Kedatangan_Vaksin singleData = new BPOM_Log_Kedatangan_Vaksin
                {
                    id_produsen = data.id_produsen,
                    id_RumahSakit = curR.id,
                    create_at = data.create_at,
                    no_registrasi = data.no_registrasi
                };
                LogBaru.Add(singleData);
            }

            if (obj.ReportListArrivalVaccine(LogBaru))
            {
                List<RS_Vaksin> newList = new List<RS_Vaksin>();
                foreach (var data in listVaksinbyRef)
                {
                    data.isReported = 1;
                    newList.Add(data);
                }
                if (obj.UpdateListRSVaksin(newList))
                    return RedirectToAction("CekReceiveVaksin");
            }
            return RedirectToAction("CekReceiveVaksin");

        }

        public ActionResult AlokasiPenggunaanVaksin()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            BPOMServiceReference.RumahSakit curR = obj.GetRS(Session["username"].ToString());
            Models.ListViewModel viewmodel= new Models.ListViewModel
            {
                pasiens = obj.GetListPasien().Where(X=>X.id_RumahSakit==curR.id),
                rsVaksins = obj.GetVaksinRS().Where(X=>X.id_rumahSakit==curR.id),
                rumahSakits = obj.GetListRumahSakit()
            };
            return View(viewmodel);
        }

        public ActionResult CheckNik()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            if (this.ControllerContext.RouteData.Values["id"] != null)
            {
                string nik = this.ControllerContext.RouteData.Values["id"].ToString();
                Models.ListViewModel viewmodel = new Models.ListViewModel
                {
                    singlemasyarakat = obj.GetMasyarakatByNik(nik)
                };
                return View(viewmodel);
            }
            Models.ListViewModel viewmodel2 = new Models.ListViewModel
            {
                singlemasyarakat = null
            };
            return View(viewmodel2);
        }

        [HttpPost]
        public ActionResult CheckNik(string NIK)
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            Models.ListViewModel viewmodel = new Models.ListViewModel
            {
                singlemasyarakat = obj.GetMasyarakatByNik(NIK)
            };
            ViewBag.tempNik = NIK;
            return View(viewmodel);
        }


        public ActionResult TambahAlokasiVaksin()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            BPOMServiceReference.RumahSakit curR = obj.GetRS(Session["username"].ToString());
            var query = (from M in obj.GetListMasyarakat()
                         join P in obj.GetListPasien() on M.NIK equals P.NIK  into subset
                         from sc in subset.DefaultIfEmpty()
                         where sc == null
                         select M).Distinct().ToList();
            Models.ListViewModel viewmodel = new Models.ListViewModel
            {
                rsVaksins = obj.GetVaksinRS().Where(X=>X.id_rumahSakit==curR.id),
                masyarakts = query
            };
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult TambahAlokasiVaksin(string No_RekMedis, string NIK, string no_registrasi)
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            if (No_RekMedis == null)
            {
                ViewBag.EmptyNoReg = "No Rek is Empty";
            }
            if (NIK == null)
            {
                ViewBag.NIK = "NIK is Empty";
            }
            if (no_registrasi == null)
            {
                ViewBag.idRS = "Select any RS";
            }
            RS_Vaksin vaksinData = obj.GetVaksinRSDetails(no_registrasi);
            Pasien data = new Pasien
            {
                No_RekMedis = No_RekMedis,
                NIK = NIK,
                no_registrasi = no_registrasi,
                id_RumahSakit = vaksinData.id_rumahSakit,
                isReported = 0,
                create_at = DateTime.Now
            };
            if (obj.TambahPasien(data) != null)
            {
                vaksinData.jumlah -= 1;
                obj.UpdateRSVaksin(vaksinData);
                return RedirectToAction("AlokasiPenggunaanVaksin");
            }
            return View();
        }

        public ActionResult LaporPenggunaanVaksin()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            string no_rekMedis = this.ControllerContext.RouteData.Values["id"].ToString();
            Pasien pasienByNoRek = obj.GetPasienByNoRek(no_rekMedis);
            BPOM_Log_Penggunaan_Vaksin data = new BPOM_Log_Penggunaan_Vaksin
            {
                No_RekMedis = pasienByNoRek.No_RekMedis,
                no_registrasi = pasienByNoRek.no_registrasi,
                create_at = DateTime.Now,
                id_RumahSakit = pasienByNoRek.id_RumahSakit,
                NIK = pasienByNoRek.NIK,
            };
            if(pasienByNoRek != null)
            {
                if (obj.ReportVaccineUse(data).statusCode.Equals(HttpStatusCode.Accepted.ToString()))
                {
                    pasienByNoRek.isReported = 1;
                    if (obj.EditPasien(pasienByNoRek))
                    {
                        return RedirectToAction("AlokasiPenggunaanVaksin");
                    }
                }
            }
                Session["InValidNik"] = "Masyarakat dengan Nik " + pasienByNoRek.NIK + " Tidak ditemukan";
                //return RedirectToRoute("RS", new { controller = "HomeRS", action = "CheckNik", id = pasienByNoRek.NIK });
                return RedirectToRoute("RS", new { controller = "HomeRS", action = "AlokasiPenggunaanVaksin" });
            
        }

        public ActionResult ReportVaccineUseByKodeReff(string kode_ref)
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            IEnumerable<BPOM_Log_Penggunaan_Vaksin> listVaksinbyRef = obj.GetLogVaccineUse().Where(X => X.no_registrasi.Equals(kode_ref));
            List<BPOM_Log_Penggunaan_Vaksin> LogBaru = new List<BPOM_Log_Penggunaan_Vaksin>();
            List<Pasien> newList = new List<Pasien>();
            foreach (var data in listVaksinbyRef)
            {
                BPOM_Log_Penggunaan_Vaksin singleData = new BPOM_Log_Penggunaan_Vaksin
                {
                    No_RekMedis = data.No_RekMedis,
                    no_registrasi = data.no_registrasi,
                    create_at = DateTime.Now,
                    id_RumahSakit = data.id_RumahSakit,
                    NIK = data.NIK,
                };
                LogBaru.Add(singleData);
            }

            if (obj.ReportListVaccineUse(LogBaru))
            {
                foreach (var data in listVaksinbyRef)
                {
                    Pasien temp = new Pasien();
                    temp.isReported = 1;
                    newList.Add(temp);
                }
                if (obj.EditListPasien(newList))
                    return RedirectToAction("AlokasiPenggunaanVaksin");
            }
            return RedirectToAction("AlokasiPenggunaanVaksin");
        }

        public ActionResult ReportAllVaccineUse()
        {
            if (Session["username"] == null)
                return RedirectToRoute("Autentication", new { controller = "User" });
            EBPOMServiceClient obj = new EBPOMServiceClient();
            BPOMServiceReference.RumahSakit curR = obj.GetRS(Session["username"].ToString());
            if (obj.ReportListArrivalVaccine(obj.GetLogArrivalVaccine()))
            {
                IEnumerable<Pasien> listAll = obj.GetListPasien().Where(X =>X.id_RumahSakit==curR.id && X.isReported == 0).ToList();
                List<Pasien> newList = new List<Pasien>();
                List<BPOM_Log_Penggunaan_Vaksin> LogBaru = new List<BPOM_Log_Penggunaan_Vaksin>();
                foreach (var data in listAll)
                {
                    BPOM_Log_Penggunaan_Vaksin singleData = new BPOM_Log_Penggunaan_Vaksin
                    {
                        No_RekMedis = data.No_RekMedis,
                        no_registrasi = data.no_registrasi,
                        create_at = DateTime.Now,
                        id_RumahSakit = data.id_RumahSakit,
                        NIK = data.NIK,
                    };
                    LogBaru.Add(singleData);
                }
                if (obj.ReportListVaccineUse(LogBaru))
                {
                    foreach (var data in listAll)
                    {
                        data.isReported = 1;
                        newList.Add(data);
                    }
                    if (obj.EditListPasien(newList))
                        return RedirectToRoute("RS", new { controller = "HomeRS", action = "AlokasiPenggunaanVaksin" });
                }
            }
            return RedirectToRoute("RS", new { controller = "HomeRS", action = "AlokasiPenggunaanVaksin" });
        }
    }
}