using ContactManager.Filters;
using ContactManager.Filters.ActionFilters;
using ContactManager.Filters.AuthorizationFilter;
using ContactManager.Filters.ExceptionFilters;
using ContactManager.Filters.ResourceFilters;
using ContactManager.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ContactManager.Controllers
{
    [Route("persons")]
    //[Route("[controller]")]
    [ResponseHeaderFilterFactory("X-Controller-Key", "Controller-Value", 3)]
    [TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonsAlwaysRunResultFilter))]
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countryService;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(IPersonsService personsService, ICountriesService countryService, ILogger<PersonsController> logger)
        {
            _personsService = personsService;
            _countryService = countryService;
            _logger = logger;
        }


        [Route("/")]
        [Route("index")]
        //[Route("[action]")]
        [ServiceFilter(typeof(PersonsListActionFilter), Order = 4)]
        //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] {"X-Custom-Key", "Custom-Value", 1}, Order = 1)]
        [ResponseHeaderFilterFactory("X-Custom-Key", "Custom-Value", 1)]
        [SkipFilter]

        //[TypeFilter(typeof(PersonsListResultFilter))]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index action method of PersonsController");

            List<PersonResponse> allPersons = await _personsService.GetFilteredPersons(searchBy, searchString);

            // Sort
            allPersons = await _personsService.GetSortedPersons(allPersons, sortBy, sortOrder);

            return View(allPersons);
        }

        [HttpGet]
        [Route("create")]
        //[Route("[action]")]
        //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "X-Create-Key", "Create-Value", 4 })]
        [ResponseHeaderFilterFactory("X-Create-Key", "Create-Value", 4)]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countryService.GetAllCountries();
            ViewBag.Countries = countries.Select(
                country => new SelectListItem
                {
                    Text = country.CountryName,
                    Value = country.CountryId.ToString()
                });

            return View();
        }

        [HttpPost]
        [Route("create")]
        //[Route("[action]")]
        [TypeFilter(typeof(PersonCreateAndEditPostFilter))]
        //[TypeFilter(typeof(FeatureDisabledResourceFilter))]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
           PersonResponse response = await _personsService.AddPerson(personRequest);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("edit/{personId}")]
        //[Route("[action]/{personId}")]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personId)
        {
            PersonResponse? person = await _personsService.GetPersonByPersonId(personId);

            if (person == null)
            {
                return RedirectToAction("Index", "Persons");
            }


            PersonUpdateRequest personUpdateRequest = person.ToPersonUpdateRequest();

            List<CountryResponse> countries = await _countryService.GetAllCountries();
            ViewBag.Countries = countries.Select(
                country => new SelectListItem
                {
                    Text = country.CountryName,
                    Value = country.CountryId.ToString()
                });

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("edit/{personId}")]
        //[Route("[action]/{personId}")]
        [TypeFilter(typeof(PersonCreateAndEditPostFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        //[TypeFilter(typeof(PersonsAlwaysRunResultFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? response = await _personsService.GetPersonByPersonId(personRequest.PersonId);

            if(response == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            await _personsService.UpdatePerson(personRequest);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("delete/{personId}")]
        //[Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(Guid personId)
        {
            PersonResponse? person = await _personsService.GetPersonByPersonId(personId);

            if (person == null)
            {
                return RedirectToAction("Index", "Persons");
            }


              return View(person);
        }

        [HttpPost]
        [Route("delete/{personId}")]
        //[Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? response = await _personsService.GetPersonByPersonId(personUpdateRequest.PersonId);

            if (response == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            await _personsService.DeletePerson(personUpdateRequest.PersonId);

            return RedirectToAction("Index", "Persons");
        }


        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            List<PersonResponse> allPersons = await _personsService.GetAllPersons();

            return new ViewAsPdf("PersonsPDF", allPersons, ViewData)
            {
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Bottom = 20, Left = 20, Right = 20 }
            };
        }


        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream memoryStream = await _personsService.GetPersonsCSV();

            return File(memoryStream, "application/octet-stream", "persons.csv");
        }

        [Route("PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personsService.GetPersonsExcel();

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}
