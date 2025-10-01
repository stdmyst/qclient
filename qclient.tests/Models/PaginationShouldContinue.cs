using qclient.QClient.Interfaces;

namespace qclient.tests.Models;

public class PaginationShouldContinue : IPaginationController<PackageWithPagination>
{
    public bool ShouldContinue(PackageWithPagination response)
        => response.PaginationToken != null;

    public IMessageCreator OnNext(IMessageCreator mc, PackageWithPagination response)
    {
        var token = response.PaginationToken ?? throw new NullReferenceException("Next page token is null.");
        return mc.SetOrUpdateQueryParameter("token", token);
    }
}