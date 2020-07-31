using System;
using System.Collections.Generic;
using TXS.Shared;

namespace TXS.buĝetalibro.Domain.ValueObjects
{
    public class Kategorie : ValueObject<Kategorie>
    {
        public string Name { get; private set; }

        public Kategorie(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            this.Name = name;
        }
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return this.Name;
        }
     }
}