using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace ContactManager.Controllers
{
    
    [Route("Countries")]
    public class CountriesController : Controller
    {
        private readonly ICountriesService _countriesService;

        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        [Route("UploadFromExcel")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }


        [HttpPost]
        [Route("UploadFromExcel")]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if(excelFile == null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please select a valid Excel(.xlsx) file.";

                return View();
            }

            if(!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Only Excel(.xlsx) files are allowed.";

                return View();
            }

            int count = await _countriesService.UploadCountriesFromExcelFile(excelFile);

            ViewBag.Message = $"{count} countries uploaded.";

            return View();
        }
    }
}
