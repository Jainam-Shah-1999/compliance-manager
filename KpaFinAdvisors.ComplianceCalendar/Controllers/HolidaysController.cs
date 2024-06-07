using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using KpaFinAdvisors.ComplianceCalendar;
using KpaFinAdvisors.Common.Models;

namespace KpaFinAdvisors.ComplianceCalendar.Controllers
{
    [Authorize]
    public class HolidaysController : Controller
    {
        private readonly CalendarDbContext _context;

        public HolidaysController(CalendarDbContext context)
        {
            _context = context;
        }

        // GET: Holidays
        public async Task<IActionResult> Index()
        {
            return _context.Holidays != null ?
                        View(await _context.Holidays.ToListAsync()) :
                        Problem("Entity set 'CalendarDbContext.Holidays'  is null.");
        }

        // GET: Holidays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Holidays == null)
            {
                return NotFound();
            }

            var holidays = await _context.Holidays
                .FirstOrDefaultAsync(m => m.HolidayId == id);
            if (holidays == null)
            {
                return NotFound();
            }

            return View(holidays);
        }

        // GET: Holidays/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Holidays/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("HolidayId,HolidayDate,HolidayName")] Holidays holidays)
        {
            if (ModelState.IsValid)
            {
                _context.Add(holidays);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(holidays);
        }

        // GET: Holidays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Holidays == null)
            {
                return NotFound();
            }

            var holidays = await _context.Holidays.FindAsync(id);
            if (holidays == null)
            {
                return NotFound();
            }
            return View(holidays);
        }

        // POST: Holidays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("HolidayId,HolidayDate,HolidayName")] Holidays holidays)
        {
            if (id != holidays.HolidayId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(holidays);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HolidaysExists(holidays.HolidayId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(holidays);
        }

        // GET: Holidays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Holidays == null)
            {
                return NotFound();
            }

            var holidays = await _context.Holidays
                .FirstOrDefaultAsync(m => m.HolidayId == id);
            if (holidays == null)
            {
                return NotFound();
            }

            return View(holidays);
        }

        // POST: Holidays/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Holidays == null)
            {
                return Problem("Entity set 'CalendarDbContext.Holidays'  is null.");
            }
            var holidays = await _context.Holidays.FindAsync(id);
            if (holidays != null)
            {
                _context.Holidays.Remove(holidays);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HolidaysExists(int id)
        {
            return (_context.Holidays?.Any(e => e.HolidayId == id)).GetValueOrDefault();
        }
    }
}
