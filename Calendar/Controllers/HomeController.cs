﻿using Calendar.Models;
using Calendar.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using Task = Calendar.Models.Tasks;

namespace Calendar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly CalendarDbContext _context;

        public HomeController(ILogger<HomeController> logger, CalendarDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //return _context.TaskGenerated != null && _context.TaskStatus != null ?
            //              View(await _context.TaskGenerated.Join(
            //                  _context.Tasks, 
            //                  tg => tg.OriginalTaskId,
            //                  ts => ts.Id,
            //                  (tg, ts) => new TaskWithStatus { Name = ts.Name, TaskDescription = ts.TaskDescription, StartDate = tg.StartDate, EndDate = tg.EndDate})
            //              .ToListAsync()) :
            //              Problem("Entity set 'CalendarDbContext.TaskGenerated'  is null.");

            //var query = from taskGenerated in _context.TaskGenerated
            //            join tasks in _context.Tasks on taskGenerated.OriginalTaskId equals tasks.Id
            //            join taskStatus in _context.TaskStatus on taskGenerated.Id equals taskStatus.Id
            //            select new TaskWithStatus { Name = tasks.Name, TaskDescription = tasks.TaskDescription, StartDate = taskGenerated.StartDate, EndDate = taskGenerated.EndDate, TaskStatus = taskStatus.Status };
            //return View(query.ToList());

            //var leftJoin = from taskGenerated in _context.TaskGenerated
            //               join tasks in _context.Tasks on taskGenerated.OriginalTaskId equals tasks.Id
            //               into leftjoin
            //               from leftjoinresult in leftjoin.DefaultIfEmpty()
            //               select new TaskWithStatus
            //               {
            //                   Name = leftjoinresult.Name,
            //                   TaskDescription = leftjoinresult.TaskDescription,
            //                   StartDate = taskGenerated.StartDate,
            //                   EndDate = taskGenerated.EndDate,
            //                   OriginalTaskId = leftjoinresult.Id,
            //                   GeneratedTaskId = taskGenerated.Id,
            //                   TaskStatus = TaskStatusEnum.None,
            //               };

            //var rightJoin = from tasks in _context.Tasks
            //                join taskStatus in _context.TaskStatus on tasks.Id equals taskStatus.OriginalTaskId
            //                into rightjoin
            //                from rightjoinresult in rightjoin.DefaultIfEmpty()
            //                select new TaskWithStatus
            //                {
            //                    Name = tasks.Name,
            //                    TaskDescription = tasks.TaskDescription,
            //                    StartDate = DateTime.Now,
            //                    EndDate = DateTime.Now,
            //                    OriginalTaskId = rightjoinresult == null ? 0 : rightjoinresult.OriginalTaskId,
            //                    GeneratedTaskId = rightjoinresult == null ? 0 : rightjoinresult.GeneratedTaskId,
            //                    TaskStatus = rightjoinresult == null ? TaskStatusEnum.None : rightjoinresult.Status,
            //                };
            //var fullJoin = leftJoin.Union(rightJoin);

            var taskGeneratedWithTaskStatus = from taskGenerated in _context.TaskGenerated
                                              join taskStatus in _context.TaskStatus on taskGenerated.Id equals taskStatus.GeneratedTaskId
                                              into newjoin
                                              from newjoinresult in newjoin.DefaultIfEmpty()
                                              select new TaskWithStatus
                                              {
                                                  Name = string.Empty,
                                                  TaskDescription = string.Empty,
                                                  StartDate = taskGenerated.StartDate,
                                                  EndDate = taskGenerated.EndDate,
                                                  OriginalTaskId = taskGenerated.OriginalTaskId,
                                                  GeneratedTaskId = taskGenerated.Id,
                                                  TaskStatus = newjoinresult == null ? TaskStatusEnum.None : newjoinresult.Status,
                                              };

            var taskWithAllData = from result in taskGeneratedWithTaskStatus
                                  join tasks in _context.Tasks on result.OriginalTaskId equals tasks.Id
                                  into resultjoin
                                  from resultjoinresult in resultjoin.DefaultIfEmpty()
                                  select new TaskWithStatus
                                  {
                                      Name = resultjoinresult.Name,
                                      TaskDescription = resultjoinresult.TaskDescription,
                                      StartDate = result.StartDate,
                                      EndDate = result.EndDate,
                                      OriginalTaskId = result.OriginalTaskId,
                                      GeneratedTaskId = result.GeneratedTaskId,
                                      TaskStatus = result.TaskStatus,
                                  };

            var finalData = taskWithAllData
                    .Where(x =>
                            (x.StartDate.Date == DateTime.Today.Date || (x.StartDate.Date < DateTime.Today.Date && x.EndDate.Date >= DateTime.Today.Date))
                            && x.TaskStatus == TaskStatusEnum.None);

            return View(finalData.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}