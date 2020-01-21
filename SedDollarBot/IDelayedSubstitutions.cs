namespace SedDollarBot
{
    public interface IDelayedSubstitutions
    {
        void DelaySubstitute(Substitution substitution);

        int Clear();

        Substitution[] ListSubstitutes();
    }
}