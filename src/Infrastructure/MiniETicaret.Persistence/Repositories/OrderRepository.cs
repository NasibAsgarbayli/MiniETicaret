﻿using MiniETicaret.Application.Abstracts.Repositories;
using MiniETicaret.Domain.Entities;
using MiniETicaret.Persistence.Contexts;

namespace MiniETicaret.Persistence.Repositories;

public class OrderRepository:Repository<Order>, IOrderRepository
{
    public OrderRepository(MiniETicaretDbContext context):base(context)
    {
        
    }
}
