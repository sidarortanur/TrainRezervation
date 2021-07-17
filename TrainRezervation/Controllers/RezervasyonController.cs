using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TrainRezervation.Models;
using TrainRezervation.Services;

namespace TrainRezervation.API.Controllers
{
    [Route("api/[Controller]")]
    public class RezervasyonController : ControllerBase
    {
        private readonly IRezervasyonService service;

        public RezervasyonController(IRezervasyonService service)
        {
            this.service = service;
        }

        [HttpPost("rezervasyondurumu")]
        public async Task<RezervasyonResponse> GetRezervasyon([FromBody] RezervasyonRequest request)
        {
            return await service.GetRezervasyon(request);
        }
    }
}
