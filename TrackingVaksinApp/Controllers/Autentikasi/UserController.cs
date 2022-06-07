using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrackingVaksinApp.BPOMServiceReference;

namespace TrackingVaksinApp.Controllers.Autentikasi
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View("Login");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string Nama,string Alamat,string No_Ijin,string username,string password)
        {
            EBPOMServiceClient obj = new EBPOMServiceClient();
            if(obj.Register(new Akun
            {
                username = username,
                password = password,
                role = "Produsen",
                create_at = DateTime.Now
            }) != null)
            {
                if(obj.TambahProdusen(new BPOMServiceReference.Produsen { 
                    Nama = Nama,
                    Alamat = Alamat,
                    No_Ijin = No_Ijin,
                    username = username
                }) != null)
                {
                    Session["SuccessRegis"] = "True";
                    return RedirectToAction("Login");
                }
            }
            ViewBag.usernameErr = "Username sudah Terdaftar";
            return View();
        }

        public ActionResult RSRegister()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RSRegister(string Nama, string Alamat, string No_Ijin, string username, string password)
        {
            EBPOMServiceClient obj = new EBPOMServiceClient();
            if (obj.Register(new Akun
            {
                username = username,
                password = password,
                role = "RS",
                create_at = DateTime.Now
            }) != null)
            {
                if (obj.TambahRS(new BPOMServiceReference.RumahSakit
                {
                    Nama = Nama,
                    Alamat = Alamat,
                    No_Ijin = No_Ijin,
                    username = username
                }) != null)
                {
                    Session["SuccessRegis"] = "True";
                    return RedirectToAction("Login");
                }
            }
            ViewBag.usernameErr = "Username sudah Terdaftar";
            return View();
        }


        public ActionResult Login()
        {
            if (Session["username"] != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            EBPOMServiceClient obj = new EBPOMServiceClient();
            if (obj.Login(username, password))
            {
                Akun getAkun = obj.GetAkun(username);
                if (getAkun.role.Equals("Produsen"))
                {
                    BPOMServiceReference.Produsen produsen = obj.GetProdusen(username);
                    Session["username"] = username;
                    Session["name"] = produsen.Nama;
                    Session["Role"] = "Produsen";
                    return RedirectToRoute("Produsen",new { action="Index",controller="HomeProdusen" });
                }
                else if (getAkun.role.Equals("BPOM"))
                {
                    BPOMServiceReference.BPOM bpom = obj.GetBPOM(username);
                    Session["username"] = username;
                    Session["name"] = bpom.Nama;
                    Session["Role"] = "BPOM";
                    return RedirectToRoute("Default", new { action = "Index", controller = "BPOMVaksin" });
                }
                else if (getAkun.role.Equals("RS")){
                    BPOMServiceReference.RumahSakit rs = obj.GetRS(username);
                    Session["username"] = username;
                    Session["name"] = rs.Nama;
                    Session["Role"] = "RS";
                    return RedirectToRoute("RS", new { action = "Index", controller = "HomeRS" });
                }
            }
            Session["Error"] = "Username atau Password salah";
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            Session.RemoveAll();
            Session["isLogout"] = "True";
            return RedirectToAction("Login");
        }
    }
}