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
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countryService;

        public PersonsController(IPersonsService personsService, ICountriesService countryService)
        {
            _personsService = personsService;
            _countryService = countryService;
        }



        [Route("/")]
        [Route("index")]
        //[Route("[action]")]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            // Search
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.Country), "Country" },
                { nameof(PersonResponse.Address), "Address" },
            };

            List<PersonResponse> allPersons = await _personsService.GetFilteredPersons(searchBy, searchString);

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            // Sort
            allPersons = await _personsService.GetSortedPersons(allPersons, sortBy, sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(allPersons);
        }

        [HttpGet]
        [Route("create")]
        //[Route("[action]")]
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
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countryService.GetAllCountries();

                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();


                return View();
            }

            PersonResponse response = await _personsService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("edit/{personId}")]
        //[Route("[action]/{personId}")]
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
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? response = await _personsService.GetPersonByPersonId(personUpdateRequest.PersonId);

            if(response == null)
            {
                return RedirectToAction("Index", "Persons");
            }

            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countryService.GetAllCountries();

                ViewBag.Countries = countries.Select(
                    country => new SelectListItem
                    {
                        Text = country.CountryName,
                        Value = country.CountryId.ToString()
                    });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return View(personUpdateRequest);
            }

            await _personsService.UpdatePerson(personUpdateRequest);

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
