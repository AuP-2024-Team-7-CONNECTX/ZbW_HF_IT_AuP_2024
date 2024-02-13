using Microsoft.EntityFrameworkCore;

public interface IGenericRepository
{
    Task<List<T>> GetAllAsync<T>() where T : class;

    Task<T> CreateOrUpdateAsync<T>(T entity) where T : class;

    Task<T> GetByIdAsync<T>(string id) where T : class;
}

public class GenericRepository : IGenericRepository
{
    private readonly GameDbContext _context;
    private readonly ILogger<GenericRepository> _logger;

    public GenericRepository(GameDbContext context, ILogger<GenericRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<T>> GetAllAsync<T>() where T : class
    {
        try
        {
            return await _context.Set<T>().ToListAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Error on Method" + nameof(GetAllAsync));
            return new List<T>();
        }
    }

    public async Task<T> CreateOrUpdateAsync<T>(T entity) where T : class
    {
        try
        {
            // Annahme: Entität hat eine 'Id'-Eigenschaft vom Typ string
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException("Entity must have an Id property");
            }
            
            var entityId = (string)idProperty.GetValue(entity);
            var dbEntity = await _context.Set<T>().FindAsync(entityId);

            if (dbEntity == null)
            {
                _context.Set<T>().Add(entity);
            }
            else
            {
                _context.Entry(dbEntity).CurrentValues.SetValues(entity);
            }

            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in " + nameof(CreateOrUpdateAsync));
            throw; // Es ist oft besser, die Ausnahme weiterzuleiten, um dem Aufrufer die Behandlung zu ermöglichen.
        }
    }

    public async Task<T> GetByIdAsync<T>(string id) where T : class
    {
        try
        {
            return await _context.Set<T>().FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in " + nameof(GetByIdAsync));
            throw;
        }
    }
}