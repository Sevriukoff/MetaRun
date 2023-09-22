namespace Sevriukoff.MetaRun.Domain.Base;

public interface ISummationAbleWith<in T>
{
    public string GetSummationKey();
    public void Add(T other);
}