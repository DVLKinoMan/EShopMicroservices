﻿using Ordering.Application.Extensions;

namespace Ordering.Application.Orders.Queries
{
    public class GetOrdersByNameQueryHandler
        (IApplicationDbContext dbContext)
        : IQueryHandler<GetOrdersByNameQuery, GetOrdersByNameResult>
    {
        public async Task<GetOrdersByNameResult> Handle(GetOrdersByNameQuery query, CancellationToken cancellationToken)
        {
            var orders = await dbContext.Orders
                         .Include(o => o.OrderItems)
                         .AsNoTracking()
                         .Where(o => o.OrderName.Value.Contains(query.Name))
                         .OrderBy(o => o.OrderName)
                         .ToListAsync(cancellationToken);

            var orderDtos = orders.ToOrderDtos();

            return new GetOrdersByNameResult(orderDtos);
        }
    }
}