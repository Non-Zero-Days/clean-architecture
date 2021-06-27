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