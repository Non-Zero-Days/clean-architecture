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
