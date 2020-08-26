using System;

namespace TXS.bugetalibro.Domain.Entities
{
    public class Kategorie 
    {
        protected Kategorie() {}
        public Kategorie(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            
            this.Id = Guid.NewGuid();
            this.Name = name;
        }

        public Guid Id { get; }

        public string Name { get; }
    }
}
