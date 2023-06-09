namespace rxdev.Accounting.Persistence;

public class UnitOfWork
{
    private readonly AccountingDbContext _context;

    public UnitOfWork(AccountingDbContext context)
    {
        _context = context;
    }

    public void Reset()
        => _context.ChangeTracker.Clear();

    public int Save()
        => _context.SaveChanges();
}