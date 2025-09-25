using qclient.QClient.Interfaces;

namespace qclient.tests.Models;

public class PackageWithPagin : IPagin
{
    public IList<User> Users { get; set; }
    public string? PaginationToken { get; set; }
    public bool CanBeRequested { get; set; }
}