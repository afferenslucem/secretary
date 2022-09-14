namespace Secretary.Documents.Creators.MessageCreators;

public abstract class MessageCreator<TData>
{
    public abstract string Create(TData data);
}