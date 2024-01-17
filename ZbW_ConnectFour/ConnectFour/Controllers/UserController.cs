namespace ConnectFour.Controllers
{
    using ConnectFour.Api.User;
    using ConnectFour.Repositories;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/Users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;

        public UserController(IUserRepository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);

            _repository = repository;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<UserResponse> Get()
        {
            var Users = _repository.GetAll();
            return Users.Select(x => new UserResponse(x.Id,x.Name,x.Email,x.Password,x.Authenticated));
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public UserResponse Get(Guid id)
        {
            return null;
        }

        // POST api/<UserController>
        [HttpPost]
        public UserResponse Post([FromBody] UserRequest value)
        {
            return null;
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public UserResponse Put(Guid id, [FromBody] UserRequest value)
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
