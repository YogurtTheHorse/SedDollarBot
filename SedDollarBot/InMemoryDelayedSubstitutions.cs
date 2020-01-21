using System.Collections.Generic;

namespace SedDollarBot
{
    public class InMemoryDelayedSubstitutions : IDelayedSubstitutions
    {
        private readonly List<Substitution> _substitutions = new List<Substitution>();
        
        public void DelaySubstitute(Substitution substitution) => _substitutions.Add(substitution);

        public int Clear()
        {
            int deleted = _substitutions.Count;
            _substitutions.Clear();
            return deleted;
        }

        public Substitution[] ListSubstitutes() => _substitutions.ToArray();
    }
}