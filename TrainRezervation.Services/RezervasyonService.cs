using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainRezervation.Models;

namespace TrainRezervation.Services
{
    public interface IRezervasyonService
    {
        Task<RezervasyonResponse> GetRezervasyon(RezervasyonRequest request);
    }

    public class RezervasyonService : IRezervasyonService
    {
        public Task<RezervasyonResponse> GetRezervasyon(RezervasyonRequest request)
        {
            RezervasyonResponse response = new RezervasyonResponse();
            response.YerlesimAyrinti = new List<YerlesimAyrinti>();

            var rezervasyonYapilabilirVagonlar = request.Tren.Vagonlar.Where(x => x.Kapasite * 7 / 10 > x.DoluKoltukAdet).ToList();
            if (rezervasyonYapilabilirVagonlar != null)
            {
                Vagon kisilerinYerlesebileceğiTekVagon = rezervasyonYapilabilirVagonlar.Where(x => x.Kapasite * 7 / 10 - x.DoluKoltukAdet > request.RezervasyonYapilacakKisiSayisi).FirstOrDefault();

                if (!request.KisilerFarkliVagonlaraYerlestirilebilir && kisilerinYerlesebileceğiTekVagon == null)
                    return Task.FromResult(response);

                if (kisilerinYerlesebileceğiTekVagon != null)
                {
                    response.RezervasyonYapilabilir = true;
                    response.YerlesimAyrinti.Add(new YerlesimAyrinti
                    {
                        KisiSayisi = request.RezervasyonYapilacakKisiSayisi,
                        VagonAdi = kisilerinYerlesebileceğiTekVagon.Ad
                    });
                    return Task.FromResult(response);
                }

                if (request.KisilerFarkliVagonlaraYerlestirilebilir && kisilerinYerlesebileceğiTekVagon == null)
                {
                    return Task.FromResult(VagonKontrol(rezervasyonYapilabilirVagonlar, request.RezervasyonYapilacakKisiSayisi, response));
                }

                return Task.FromResult(response);
            }
            else
            {
                response.RezervasyonYapilabilir = false;
                response.YerlesimAyrinti = new List<YerlesimAyrinti>();
                return Task.FromResult(response);
            }
        }

        private RezervasyonResponse VagonKontrol(List<Vagon> vagonlar, int kisiSayisi, RezervasyonResponse response)
        {
            int yerlesenKisi = 0;
            foreach (var vagon in vagonlar)
            {
                int vagondakiYer = (vagon.Kapasite * 7 / 10) - vagon.DoluKoltukAdet;
                if (kisiSayisi < vagondakiYer)
                {
                    response.RezervasyonYapilabilir = true;
                    response.YerlesimAyrinti.Add(new YerlesimAyrinti
                    {
                        KisiSayisi = kisiSayisi,
                        VagonAdi = vagon.Ad
                    });
                    return response;
                }
                else
                {
                    for (int i = 1; i <= kisiSayisi; i++)
                    {
                        int vagonaEklenmis = vagon.DoluKoltukAdet++;
                        if (vagonaEklenmis < vagon.Kapasite * 7 / 10)
                        {
                            yerlesenKisi++;
                            if (i < kisiSayisi)
                                continue;
                        }
                        if (i == kisiSayisi)
                        {
                            response.YerlesimAyrinti.Add(new YerlesimAyrinti
                            {
                                KisiSayisi = yerlesenKisi,
                                VagonAdi = vagon.Ad
                            });
                            break;
                        }
                    }

                    kisiSayisi -= yerlesenKisi;
                    yerlesenKisi = 0;
                }
            }

            if (kisiSayisi <= 0)
            {
                response.RezervasyonYapilabilir = true;
                return response;
            }

            else
            {
                response.YerlesimAyrinti = new List<YerlesimAyrinti>();
                return response;
            }


        }
    }
}
