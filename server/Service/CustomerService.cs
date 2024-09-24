﻿using DataAccess.Interfaces;
using DataAccess.Models;
using Service.Interfaces;

namespace Service;

public class CustomerService(ICustomerRepository repository) : ICustomerService
{
    public List<Customer> All()
    {
        return repository.All();
    }

    public Customer ById(int id)
    {
        return repository.ById(id);
    }
}