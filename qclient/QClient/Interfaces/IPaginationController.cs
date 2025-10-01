namespace qclient.QClient.Interfaces;

public interface IPaginationController<in T>
{
    bool ShouldContinue(T response);
    IMessageCreator OnNext(IMessageCreator mc, T response);
}