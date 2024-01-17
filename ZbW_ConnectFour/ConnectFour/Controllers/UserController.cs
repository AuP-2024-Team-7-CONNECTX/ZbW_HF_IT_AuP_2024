namespace ConnectFour.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("api/customers")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICustomerRepository _repository;

        public UserController(ICustomerRepository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);

            _repository = repository;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<CustomerResponse> Get()
        {
            var customers = _repository.GetAll();
            return customers.Select(x => new CustomerResponse(x.Id, x.CustomerNr, x.Name, null));
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public CustomerResponse Get(Guid id)
        {
            return null;
        }

        // POST api/<UserController>
        [HttpPost]
        public CustomerResponse Post([FromBody] CustomerRequest value)
        {
            return null;
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public CustomerResponse Put(Guid id, [FromBody] CustomerRequest value)
        {
            return null;
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
        }
    }
}
