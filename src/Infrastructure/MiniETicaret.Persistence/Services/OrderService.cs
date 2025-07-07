using Microsoft.AspNetCore.Identity;
using MiniETicaret.Application.Abstracts.Services;
using MiniETicaret.Application.DTOs.OrderDtos;
using MiniETicaret.Application.DTOs.OrderProductDtos;
using MiniETicaret.Application.Shared;
using MiniETicaret.Domain.Entities;
using System.Net;
using System;
using MiniETicaret.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace MiniETicaret.Persistence.Services;

public class OrderService : IOrderService
{
    private readonly MiniETicaretDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<OrderService> _logger;

    public OrderService(MiniETicaretDbContext context, UserManager<AppUser> userManager, IEmailService emailService, ILogger<OrderService> logger)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;   
        _logger = logger;
    }
    public async Task<BaseResponse<Guid>> CreateAsync(OrderCreateDto dto, string buyerId)
    {
        var buyer = await _userManager.FindByIdAsync(buyerId);
        if (buyer == null)
            return new BaseResponse<Guid>("Buyer not found", Guid.Empty, HttpStatusCode.NotFound);

        var orderProducts = new List<OrderProduct>();
        decimal total = 0;

        foreach (var p in dto.Products)
        {
            var product = await _context.Products
                .Include(x => x.AppUser) // Seller-ə çıxmaq üçün
                .FirstOrDefaultAsync(x => x.Id == p.ProductId);

            if (product == null || product.Stock < p.ProductCount)
                return new BaseResponse<Guid>("Product not available or stock is insufficient", Guid.Empty, HttpStatusCode.BadRequest);

            var productTotal = product.Price * p.ProductCount;
            total += productTotal;

            orderProducts.Add(new OrderProduct
            {
                ProductId = p.ProductId,
                ProductCount = p.ProductCount,
                ProductPrice = product.Price
            });

            product.Stock -= p.ProductCount;
        }

        var order = new Order
        {
            TotalPrice = total,
            Note = dto.Note,
            DeliveryAddress = dto.DeliveryAddress,
            PaymentMethod = dto.PaymentMethod,
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            OrderProducts = orderProducts,
            AppUsers = new List<AppUser> { buyer }
        };

        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();

        try
        {
            // 1. Buyer-ə email
            string subject = "Sifarişiniz qəbul olundu!";
            string body = $"Sifariş nömrəsi: {order.Id}<br>Ümumi məbləğ: {order.TotalPrice} AZN";
            await _emailService.SendEmailAsync(
                new[] { buyer.Email }, subject, body
            );

            // 2. Seller-lərə email (hər seller üçün öz məhsullarının siyahısı ilə)
            // SellerId-ləri uniq topla
            var sellerIds = orderProducts
                .Select(op =>
                    _context.Products
                        .Where(prod => prod.Id == op.ProductId)
                        .Select(prod => prod.UserId)
                        .FirstOrDefault())
                .Distinct()
                .ToList();

            foreach (var sellerId in sellerIds)
            {
                var seller = await _userManager.FindByIdAsync(sellerId);
                if (seller != null && !string.IsNullOrEmpty(seller.Email))
                {
                    // Sellerə aid bu orderdəki məhsulları tap
                    var sellerProducts = orderProducts
                        .Where(op =>
                            _context.Products
                                .Where(prod => prod.Id == op.ProductId)
                                .Select(prod => prod.UserId)
                                .FirstOrDefault() == sellerId)
                        .ToList();

                    var sb = new StringBuilder();
                    sb.AppendLine($"Salam {seller.Fullname}, məhsulunuza yeni sifariş gəlib!<br>");
                    sb.AppendLine($"Sifariş nömrəsi: {order.Id}<br><br>");
                    sb.AppendLine("Satılan məhsullar:<br>");
                    decimal sellerTotal = 0;
                    foreach (var op in sellerProducts)
                    {
                        var product = await _context.Products.FindAsync(op.ProductId);
                        decimal totalPrice = op.ProductCount * op.ProductPrice;
                        sellerTotal += totalPrice;

                        sb.AppendLine(
                            $"- {product?.Title} | Say: {op.ProductCount} | Qiymət: {op.ProductPrice} AZN | Cəmi: {totalPrice} AZN<br>");
                    }
                    sb.AppendLine($"<br>Ümumi məbləğ: {sellerTotal} AZN");

                    await _emailService.SendEmailAsync(
                        new[] { seller.Email },
                        "Məhsulunuza sifariş gəldi!",
                        sb.ToString()
                    );
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Order email notification failed. OrderId: {OrderId}, BuyerEmail: {BuyerEmail}", order.Id, buyer.Email);
        }

        return new BaseResponse<Guid>("Order created", order.Id, HttpStatusCode.Created);
    }


    public async Task<BaseResponse<List<OrderGetDto>>> GetMyOrdersAsync(string buyerId)
    {
        var orders = await _context.Orders
            .Include(o => o.OrderProducts).ThenInclude(op => op.Product)
            .Where(o => o.AppUsers.Any(u => u.Id == buyerId))
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var result = orders.Select(MapOrderToDto).ToList();
        return new BaseResponse<List<OrderGetDto>>("Orders retrieved successfully", result, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<OrderGetDto>>> GetMySalesAsync(string sellerId)
    {
        var orders = await _context.Orders
            .Include(o => o.OrderProducts).ThenInclude(op => op.Product)
            .Where(o => o.OrderProducts.Any(op => op.Product.UserId == sellerId))
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var result = orders.Select(MapOrderToDto).ToList();
        return new BaseResponse<List<OrderGetDto>>("Sales retrieved successfully", result, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<OrderGetDto>> GetByIdAsync(Guid id, string userId, string role)
    {
        var order = await _context.Orders
            .Include(o => o.OrderProducts).ThenInclude(op => op.Product)
            .Include(o => o.AppUsers)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return new BaseResponse<OrderGetDto>("Order not found", null, HttpStatusCode.NotFound);

        bool isOwner = order.AppUsers.Any(u => u.Id == userId);
        bool isSeller = order.OrderProducts.Any(op => op.Product.UserId == userId);

        if (!(isOwner || isSeller || role == "Admin"))
            return new BaseResponse<OrderGetDto>("You do not have access to this order", null, HttpStatusCode.Forbidden);

        var dto = MapOrderToDto(order);
        return new BaseResponse<OrderGetDto>("Order retrieved successfully", dto, HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> UpdateStatusAsync(Guid id, string status, string userId, string role)
    {
        var order = await _context.Orders
            .Include(o => o.AppUsers)
            .Include(o => o.OrderProducts).ThenInclude(op => op.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return new BaseResponse<string>("Order not found", HttpStatusCode.NotFound);

        bool isOwner = order.AppUsers.Any(u => u.Id == userId);
        bool isSeller = order.OrderProducts.Any(op => op.Product.UserId == userId);

        if (!(isOwner || isSeller || role == "Admin"))
            return new BaseResponse<string>("You do not have permission to update this order", HttpStatusCode.Forbidden);

        order.Status = status;
        await _context.SaveChangesAsync();

        return new BaseResponse<string>("Order status updated", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<string>> DeleteAsync(Guid id, string userId, string role)
    {
        var order = await _context.Orders
            .Include(o => o.AppUsers)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
            return new BaseResponse<string>("Order not found", HttpStatusCode.NotFound);

        bool isOwner = order.AppUsers.Any(u => u.Id == userId);

        if (!(isOwner || role == "Admin"))
            return new BaseResponse<string>("You do not have permission to delete this order", HttpStatusCode.Forbidden);

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return new BaseResponse<string>("Order deleted", HttpStatusCode.OK);
    }

    public async Task<BaseResponse<List<OrderGetDto>>> GetAllAsync(string role)
    {
        if (role != "Admin")
            return new BaseResponse<List<OrderGetDto>>("You do not have permission to view all orders", null, HttpStatusCode.Forbidden);

        var orders = await _context.Orders
            .Include(o => o.OrderProducts).ThenInclude(op => op.Product)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        var result = orders.Select(MapOrderToDto).ToList();
        return new BaseResponse<List<OrderGetDto>>("All orders retrieved successfully", result, HttpStatusCode.OK);
    }

    // Mapping
    private OrderGetDto MapOrderToDto(Order order)
    {
        return new OrderGetDto
        {
            Id = order.Id,
            TotalPrice = order.TotalPrice,
            Note = order.Note,
            DeliveryAddress = order.DeliveryAddress,
            PaymentMethod = order.PaymentMethod,
            OrderDate = order.OrderDate,
            Status = order.Status,
            Products = order.OrderProducts?.Select(op => new OrderProductGetDto
            {
                ProductId = op.ProductId,
                ProductName = op.Product?.Name,
                ProductCount = op.ProductCount,
                ProductPrice = op.ProductPrice
                
            }).ToList()
        };
    }



}

