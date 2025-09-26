namespace qclient.QClient.Interfaces;

public interface IPaginController<in T>
{
    bool ShouldContinue(T response);
    IMessageCreator OnNext(IMessageCreator mc, T response);
}