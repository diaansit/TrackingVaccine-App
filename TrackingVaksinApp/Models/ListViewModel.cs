using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrackingVaksinApp.BPOMServiceReference;

namespace TrackingVaksinApp.Models
{
    public class ListViewModel
    {
        public IEnumerable<BPOMServiceReference.Produsen_Vaksin> produsen_Vaksins { get; set; }

        public IEnumerable<BPOMServiceReference.Produsen> produsens { get; set; }
        public IEnumerable<BPOMServiceReference.RumahSakit> rumahSakits { get; set; }

        public IEnumerable<BPOMServiceReference.BPOM_Vaksin> bpom_vaksins { get; set; }

        public BPOMServiceReference.BPOM_Vaksin singlevaksin { get; set; }

        public IEnumerable<BPOMServiceReference.BPOM_Log_Kedatangan_Vaksin> LogsArrivals { get; set; }

        public IEnumerable<BPOMServiceReference.RS_Vaksin> rsVaksins { get; set; }

        public IEnumerable<BPOMServiceReference.Pasien> pasiens { get; set; }

        public BPOMServiceReference.Masyarakat singlemasyarakat { get; set; }

        public IEnumerable<BPOMServiceReference.BPOM_Log_Penggunaan_Vaksin> logUses { get; set; }

        public IEnumerable<Masyarakat> masyarakts { get; set; }


    }
}