namespace SedDollarBot
{
    public interface IDelayedSubstitutions
    {
        void DelaySubstitute(Substitution substitution);

        int Clear(long chatId);

        Substitution[] ListSubstitutes(long chat);
        
        bool RemoveAt(int deletionIndex, long chat);
    }
}