using System;
using System.Collections.Generic;
using System.Text;

namespace TrainRezervation.Models
{

    public class RezervasyonResponse
    {
        public bool RezervasyonYapilabilir { get; set; }
        public List<YerlesimAyrinti> YerlesimAyrinti { get; set; }
    }

    public class YerlesimAyrinti
    {
        public string VagonAdi { get; set; }
        public int KisiSayisi { get; set; }
    }

}
