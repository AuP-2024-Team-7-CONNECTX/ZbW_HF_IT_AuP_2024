﻿namespace ConnectFour.Interfaces
{
    public abstract class Entity : IDeletable
    {
        public string Id { get; set; }
        public DateTime? InsertedOn { get; set; }
        public DateTime? DeletedOn { get; set; }

    }

    public interface IDeletable
    {
        public DateTime? DeletedOn { get; set; }
    }
}
