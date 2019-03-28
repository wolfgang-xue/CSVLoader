using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;
using CSV_Data_Loader.Models;
using CSV_Data_Loader.Interfaces;
using CsvHelper;

namespace CSV_Data_Loader.Controllers
{
    public class HomeController : Controller
    {
        private ICSVLoadService _CSVLoadService;

        public HomeController(ICSVLoadService csvLoadService)
        {
            _CSVLoadService = csvLoadService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoadCSV()
        {
            ViewData["Message"] = "Select CSV and load its data page.";

            return View();
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                        stream.Position = 0;
                        //stream.Seek(0, SeekOrigin.Begin);
                        // process uploaded files
                        // Don't rely on or trust the FileName property without validation.
                        using (var reader = new StreamReader(stream))
                        {
                            var type = _CSVLoadService.MatchEntity(reader);
                            if (type == null)
                                continue;

                            _CSVLoadService.FindMappingPosition(type);

                            try
                            {
                                var csv = new CsvReader(reader);
                                while (csv.Read())
                                {
                                    _CSVLoadService.ProcessData(csv, type);
                                }
                            }
                            catch (Exception ex)
                            {
                                //isSuccessful = false;
                                //_Log.Error($"Exception occurred during data process with the message='{ex.Message}'");
                            }
                        }

                    }

                }
            }

            return Ok(new { count = files.Count, size, filePath });
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
