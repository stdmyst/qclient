namespace qclient.tests.Models;

public class PackageWithPagination
{
    public IList<User> Users { get; set; }
    public string? PaginationToken { get; set; }
}