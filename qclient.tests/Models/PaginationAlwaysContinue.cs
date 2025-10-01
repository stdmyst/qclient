using qclient.QClient.Interfaces;

namespace qclient.tests.Models;

public class PaginationAlwaysContinue : IPaginationController<PackageWithPagination>
{
    public bool ShouldContinue(PackageWithPagination response)
        => true;

    public IMessageCreator OnNext(IMessageCreator mc, PackageWithPagination response)
    {
        var token = response.PaginationToken ?? throw new NullReferenceException("Next page token is null.");
        return mc.SetOrUpdateQueryParameter("token", token);
    }
}