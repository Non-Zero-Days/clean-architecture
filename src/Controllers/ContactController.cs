using System;
using Microsoft.AspNetCore.Mvc;
using src.Core;

namespace src.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _service;

        public ContactController(IContactService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public Contact Get(string name)
        {
            return _service.Retrieve(name);
        }

        [HttpPost]
        public void Create(Contact input)
        {
            _service.Create(input);
        }
    }
}
