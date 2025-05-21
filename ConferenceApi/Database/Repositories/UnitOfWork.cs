using ConferenceApi.Models.Database;
using Microsoft.EntityFrameworkCore.Storage;

namespace ConferenceApi.Database.Repositories;

public class UnitOfWork : IDisposable
{
    #region  Fields
    
    private ApplicationContext _context;
    private IDbContextTransaction _transaction;
    
    #endregion
    
    #region Repositories
    
    private GenericRepository<Account> _accountRepository;
    private GenericRepository<History> _historyRepository;
    private GenericRepository<Transfer> _transferRepository;
    
    #endregion

    #region Constructor

    public UnitOfWork(ApplicationContext context)
    {
        _context = context;
    }

    #endregion

    #region Properties
    
    
    public GenericRepository<Account> AccountRepository
    {
        get
        {
            if (this._accountRepository == null)
            {
                this._accountRepository = new GenericRepository<Account>(_context);
            }
            return _accountRepository;
        }
    }
   
    public GenericRepository<History> HistoryRepository
    {
        get
        {
            if (this._historyRepository == null)
            {
                this._historyRepository = new GenericRepository<History>(_context);
            }
            return _historyRepository;
        }
    }
    public GenericRepository<Transfer> TransferRepository
    {
        get
        {
            if (this._transferRepository == null)
            {
                this._transferRepository = new GenericRepository<Transfer>(_context);
            }
            return _transferRepository;
        }
    }
    
    #endregion

    #region Methods

    public bool Save()
    {
        return _context.SaveChanges() > 0;
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        this.disposed = true;
    }
    public async Task BeginTransactionAsync()
    {
        if (_transaction is not null)
            throw new InvalidOperationException("A transaction has already been started.");
        _transaction = await _context.Database.BeginTransactionAsync();
    }
    public async Task CommitAsync()
    {
        if (_transaction is null)
            throw new InvalidOperationException("A transaction has not been started.");
    
        try
        {
            await _transaction.CommitAsync();
            _transaction.Dispose();
            _transaction = null;
        }
        catch (Exception)
        {
            if (_transaction is not null)
                await _transaction.RollbackAsync();
            throw;
        }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
