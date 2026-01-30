using ContactManager.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;

namespace ContactManager.Filters.ActionFilters
{
    public class PersonCreateAndEditPostFilter : IAsyncActionFilter
    {
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonCreateAndEditPostFilter> _logger;

        public PersonCreateAndEditPostFilter(ICountriesService countriesService, ILogger<PersonCreateAndEditPostFilter> logger)
        {
            _countriesService = countriesService;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is PersonsController personsController) {
                // Before logic
                if (!personsController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countriesService.GetAllCountries();

                    personsController.ViewBag.Countries = countries.Select(country => new SelectListItem
                    {
                        Text = country.CountryName,
                        Value = country.CountryId.ToString()
                    });

                    personsController.ViewBag.Errors = personsController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                    var personRequest = context.ActionArguments["personRequest"];

                    context.Result = personsController.View(personRequest); // Short-circuit the action execution if there are validation errors
                }
                else
                {
                    // ModelState is valid, proceed as normal
                    // Next call in the pipeline
                    await next();
                }
            }
            else
            {
                // If the controller is not PersonsController, proceed as normal
                await next();
            }

            // After logic
            _logger.LogInformation("In after logic of PersonCreateAndEditPostFilter action filter.");
        }
    }
}
