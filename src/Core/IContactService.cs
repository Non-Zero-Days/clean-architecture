namespace src.Core
{
    public interface IContactService
    {
        Contact Retrieve(string name);
        void Create(Contact entity);
    }
}
