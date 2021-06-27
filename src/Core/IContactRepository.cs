namespace src.Core
{
    public interface IContactRepository
    {
        Contact Retrieve(string name);
        void Create(Contact entity);
    }
}
