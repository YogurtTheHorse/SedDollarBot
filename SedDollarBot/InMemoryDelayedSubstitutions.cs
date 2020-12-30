using System.Collections.Generic;
using System.Linq;

namespace SedDollarBot
{
    public class InMemoryDelayedSubstitutions : IDelayedSubstitutions
    {
        private readonly List<Substitution> _substitutions = new List<Substitution>();
        
        public void DelaySubstitute(Substitution substitution) => _substitutions.Add(substitution);

        public int Clear(long chatId)
        {
            Substitution[] toRemove = ListSubstitutes(chatId);
            return _substitutions.RemoveAll(toRemove.Contains);
        }

        public bool RemoveAt(int index, long chatId)
        {
            Substitution[] substitutions = ListSubstitutes(chatId);

            if (index < 0 || index >= substitutions.Length)
            {
                return false;
            }

            _substitutions.Remove(substitutions[index]);

            return true;
        }

        public Substitution[] ListSubstitutes(long chat) => 
            _substitutions.Where(s => s.ChatId == chat).ToArray();
    }
}