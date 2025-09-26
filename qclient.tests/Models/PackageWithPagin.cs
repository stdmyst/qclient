namespace qclient.tests.Models;

public class PackageWithPagin
{
    public IList<User> Users { get; set; }
    public string? PaginationToken { get; set; }
}