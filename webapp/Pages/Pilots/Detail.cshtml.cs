/* CALEB L WHITE
CIDM 3312 - J BABB
WTAMU FALL 2020 */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VatsimLibrary.VatsimClientV1;
using VatsimLibrary.VatsimDb;
using System;

namespace VATSIMData.WebApp.Pages {
    public class PilotDetailModel : PageModel {
        private VatsimDbContext db;

        public VatsimClientPilotV1 Pilot { get; set; }
        public List <VatsimClientPilotSnapshotV1> Position { get; set; }
        public List <VatsimClientPlannedFlightV1> Flight { get; set; }
        public int fastestSpeed { get; set; }
        public int highestAlt { get; set; }
        public decimal lat { get; set; }
        public decimal lon { get; set; }
        public string depAirport { get; set; }
        public int depAirportCt { get; set; }
        public PilotDetailModel(VatsimDbContext db) {
            this.db = db;
        }

        public async Task<IActionResult> OnGetAsync(string cid, string callsign, string timelogon) {
            Pilot = await db.Pilots.FindAsync(cid, callsign, timelogon);
            if(Pilot == null) {
                return RedirectToPage("NotFound");
            }     

            Position = db.Positions.Where(p=>p.Cid==cid && p.Callsign==callsign && p.TimeLogon==timelogon).ToList();
            Flight = db.Flights.Where(p=>p.Cid==cid && p.Callsign==callsign && p.TimeLogon==timelogon).ToList();

            var fastestSpeed1 = Position.OrderByDescending(p=>Convert.ToInt32(p.Groundspeed)).ToList();
            fastestSpeed = Convert.ToInt32(fastestSpeed1[0].Groundspeed);

            var highestAlt1 = Position.OrderByDescending(p=>Convert.ToInt32(p.Altitude)).ToList();
            highestAlt = Convert.ToInt32(highestAlt1[0].Altitude);

            var lon1 = Position.OrderByDescending(p=>Convert.ToDecimal(p.Longitude)).ToList();
            lon = Convert.ToDecimal(lon1[0].Longitude);

            var lat1 = Position.OrderBy(p=>Convert.ToDecimal(p.Latitude)).ToList();
            lat = Convert.ToDecimal(lat1[0].Latitude);

            var depAirport3 = Flight.Where(p=>p.Callsign==callsign).ToList();

            var depAirport1 = depAirport3.GroupBy(p => p.PlannedDepairport)
                          .Select(lg => 
                                new { 
                                    Airport = lg.Key, 
                                    Count = lg.Count(),
                                }).ToList();

            var depAirport2 = depAirport1.OrderByDescending(p=>p.Count).ToList();

            depAirport = depAirport2[0].Airport;
            depAirportCt = depAirport2[0].Count;

            return Page();
        }


    }
}

/* CALEB L WHITE
CIDM 3312 - J BABB
WTAMU FALL 2020 */