﻿using CloudSync.Modules.EmployeeManagement.Models;

namespace CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
    Task<Employee> GetByIdAsync(int id);
    Task<Employee> CreateAsync(Employee employee);
    Task UpdateAsync(int id, Employee employee);
    Task DeleteAsync(int id);
}