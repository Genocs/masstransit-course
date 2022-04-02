using Genocs.MassTransit.Customers.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Genocs.MassTransit.Customers.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ILogger<CustomersController> logger)
        {
            _logger = logger;
        }

        //[HttpGet(Name = "GetCustomers")]
        //public async Task<IActionResult> GetCustomers()
        //{
        //    return await Task.Run(() =>
        //    {
        //        return Ok(Enumerable.Range(1, 5).Select(index => new Customer
        //        {
        //            Date = DateTime.Now.AddDays(index),
        //            CustomerNumber = Random.Shared.Next(0, 10000).ToString("f4"),
        //            Active = Random.Shared.Next(0, 10000) > 5000
        //        })
        //        .ToArray());
        //    });
        //}

        [HttpGet(Name = "GetCustomer")]
        public async Task<IActionResult> GetCustomer([FromQuery] string customerNumber)
        {
            return await Task.Run(() =>
            {
                return Ok(new Customer
                {
                    Date = DateTime.Now,
                    CustomerNumber = customerNumber,
                    Active = true
                });
            });
        }
    }
}