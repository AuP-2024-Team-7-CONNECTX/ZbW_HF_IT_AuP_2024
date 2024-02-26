namespace ConnectFour.Interfaces
{
    public abstract class Entity
    {
        public string Id { get; set; }
        DateTime? InsertedOn { get; set; }
        DateTime? DeletedOn { get; set; }
    }
}
