using qclient.QClient.Interfaces;

namespace qclient.tests.Models;

public class PaginControllerAlwaysContinue : IPaginController<PackageWithPagin>
{
    public bool ShouldContinue(PackageWithPagin response)
        => true;

    public IMessageCreator OnNext(IMessageCreator mc, PackageWithPagin response)
    {
        var token = response.PaginationToken ?? throw new NullReferenceException("Next page token is null.");
        return mc.SetOrUpdateQueryParameter("token", token);
    }
}