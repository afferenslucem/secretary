namespace Secretary.Documents.Creators.DocumentCreators;

public abstract class DocumentCreator<TData>
{
    public abstract string Create(TData data);
}