## C# Clean Architecture

### Prerequisites:

[.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
[Visual Studio Code](https://code.visualstudio.com/)

### Loose Agenda:
Learn how to structure C# code per the Clean Architecture

### Step by Step

Create a directory for today's exercise and navigate to it in a terminal instance.

#### Single project

Create a new webapi project via `dotnet new webapi -o src`

Open the directory in Visual Studio Code.

In the src directory, create a directory named Core. We will use this directory to store POCOs, Services, and interfaces.

In the Core directory, create a file named `Contact.cs` and add the following contents. This will be our main domain POCO entity.

``` Contact.cs
namespace src.Core
{
    public class Contact
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
    }
}
```

In the Core directory, let's create a file named `IContactRepository.cs` and add the following contents. This will be our data access repository interface. The implementation of our repository will be in another folder.

``` IContactRepository.cs
namespace src.Core
{
    public interface IContactRepository
    {
        Contact Retrieve(string name);
        void Create(Contact entity);
    }
}
```

In the Core directory, let's create a new file named `IContactService.cs`. This will be our business logic service interface.

``` IContactService.cs
namespace src.Core
{
    public interface IContactService
    {
        Contact Retrieve(string name);
        void Create(Contact entity);
    }
}
```

In the Core directory, let's create a new file called `ContactService.cs`. We will use this for business logic.

``` ContactService.cs
using System;

namespace src.Core
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _repository;

        public ContactService(IContactRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Contact Retrieve(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                return null;
            }

            return _repository.Retrieve(name);
        }

        public void Create(Contact entity)
        {
            if(entity == default)
            {
                throw new InvalidOperationException("Unable to create contact.");
            }

            if(entity.Type == "Business" && string.IsNullOrEmpty(entity.Number))
            {
                throw new InvalidOperationException("Business contacts must have a number.");
            }

            _repository.Create(entity);
        }
    }
}
```

The `Core` directory now contains all of our core concerns (the POCO, service interface and repository/infrastructure interface.) Next, we'll want to implement our infrastructure contract. Let's do so in a new directory named `Infrastructure`.

Create a new file in `Infrastructure` named `ContactRepository.cs` with the following code.

```ContactRepository.cs
using System.Collections.Concurrent;
using src.Core;

namespace src.Infrastructure
{
    public class ContactRepository : IContactRepository
    {
        private readonly ConcurrentDictionary<string, Contact> _contacts;

        public ContactRepository()
        {
            _contacts = new ConcurrentDictionary<string,Contact>();
        }

        public void Create(Contact entity)
        {
            _contacts.TryAdd(entity.Name, entity);
        }

        public Contact Retrieve(string name)
        {
            _contacts.TryGetValue(name, out var contact);
            return contact;
        }
    }
}
```

With both Infrastructure and Core code defined, let's navigate to Startup.cs and implement the dependency injection for our code.

Adjust the `ConfigureServices` method to look as follows:
```
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IContactService, ContactService>();
            services.AddSingleton<IContactRepository, ContactRepository>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "src", Version = "v1" });
            });
        }
```

In Controllers, let's create and implement a `ContactController.cs` as

```ContactController.cs
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

```

In a terminal instance from the root directory of today's exercise run `dotnet run --project src/src.csproj`

Navigate to [localhost:5001/swagger](https://localhost:5001/swagger).

Try the Contact GET endpoint with the name `nonzero`

Now try the POST endpoint with the body 
```
{
  "name": "Non Zero Inc.",
  "number": "",
  "type": "Business"
}
```

Note that this errors as we expected.

Try the POST endpoint with the body 
```
{
  "name": "nonzero",
  "number": "",
  "type": "person"
}
```

Now try the GET endpoint with `nonzero` as note the result succeeds.


#### Project Per Layer

If you instead wanted to implement this solution with projects as a boundary you could split Core and Infrastructure into their own csproj files. In the end you would have `src`, `src.Core`, and `src.Infrastructure` projects. The `src` project would reference `src.Core` and `src.Infrastructure`. The `src.Infrastructure` project would reference `src.Core`. This might be the prefered approach for larger solutions.

Congratulations on a non-zero day!


### Additional Documentation

[Clean Coding](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[Clean Architecture](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#clean-architecture)

