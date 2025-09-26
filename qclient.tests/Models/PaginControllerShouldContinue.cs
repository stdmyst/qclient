using qclient.QClient.Interfaces;

namespace qclient.tests.Models;

public class PaginControllerShouldContinue : IPaginController<PackageWithPagin>
{
    public bool ShouldContinue(PackageWithPagin response)
        => response.PaginationToken != null;

    public IMessageCreator OnNext(IMessageCreator mc, PackageWithPagin response)
    {
        var token = response.PaginationToken ?? throw new NullReferenceException("Next page token is null.");
        return mc.SetOrUpdateQueryParameter("token", token);
    }
}