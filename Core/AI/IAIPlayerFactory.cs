namespace Core.AI;

public interface IAIPlayerFactory
{
    IAIPlayer<TMove> Create<TMove>(string difficulty);
}