using System;
using System.Collections.Generic;
using TXS.Shared;

namespace TXS.bugetalibro.Domain.ValueObjects
{
    public class Kategorie : ValueObject<Kategorie>
    {
        public Kategorie(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            this.Name = name;
        }

        public string Name { get; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.Name;
        }
    }
}
