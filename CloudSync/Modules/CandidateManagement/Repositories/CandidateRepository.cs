using CloudSync.Infrastructure;
using CloudSync.Modules.CandidateManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudSync.Modules.CandidateManagement.Repositories;

public interface ICandidateRepository
{
    Task<IEnumerable<Candidate>> GetAllAsync();
    Task<Candidate?> GetByIdAsync(int id);
    Task<Candidate> AddAsync(Candidate candidate);
    Task UpdateAsync(Candidate candidate);
}

public class CandidateRepository(DatabaseContext context) : ICandidateRepository
{
    public async Task<IEnumerable<Candidate>> GetAllAsync()
    {
        return await context.Set<Candidate>().ToListAsync();
    }

    public async Task<Candidate?> GetByIdAsync(int id)
    {
        return await context.Set<Candidate>().FindAsync(id);
    }

    public async Task<Candidate> AddAsync(Candidate candidate)
    {
        await context.Set<Candidate>().AddAsync(candidate);
        await context.SaveChangesAsync();
        return candidate;
    }

    public async Task UpdateAsync(Candidate candidate)
    {
        context.Set<Candidate>().Update(candidate);
        await context.SaveChangesAsync();
    }
}