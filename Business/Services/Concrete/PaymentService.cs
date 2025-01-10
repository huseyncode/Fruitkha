using Business.Services.Abstract;
using Business.Utilities.Stripe;
using Core.Constants;
using Core.Entities;
using Data.Repositories.Abstract;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concrete;

public class PaymentService : IPaymentService
{
    private readonly StripeSettings _stripeSettings;
    private readonly UserManager<User> _userManager;
    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderProductRepository _orderProductRepository;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProductRepository _productRepository;
    private readonly IBasketRepository _basketRepository;

    public PaymentService(IOptions<StripeSettings> stripeSettings,
                          UserManager<User> userManager,
                          IActionContextAccessor actionContextAccessor,
                          IUnitOfWork unitOfWork,
                          IOrderRepository orderRepository,
                          IOrderProductRepository orderProductRepository,
                          IUrlHelperFactory urlHelperFactory,
                          IHttpContextAccessor httpContextAccessor,
                          IProductRepository productRepository,
                          IBasketRepository basketRepository)
    {
        _stripeSettings = stripeSettings.Value;
        _userManager = userManager;
        _actionContextAccessor = actionContextAccessor;
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _orderProductRepository = orderProductRepository;
        _urlHelperFactory = urlHelperFactory;
        _httpContextAccessor = httpContextAccessor;
        _productRepository = productRepository;
        _basketRepository = basketRepository;
    }

    public async Task<(int statusCode, string? description, string? id)> PayAsync()
    {
        var user = await GetUserAsync();
        if (user == null) return (404, "User not found or not authorized", null);

        var order = await CreateOrderAsync(user);
        var items = await CreateSessionLineItemsAsync(user, order);

        await _unitOfWork.CommitAsync();

        var options = CreateSessionOptions(order, items);

        return await CreateStripeSessionAsync(options);
    }

    public async Task<bool> PaySuccess(Guid token)
    {
        var user = await GetUserAsync();
        if (user == null) return false;

        var order = await _orderRepository.GetOrderWithOrderProductsAsync(token, user.Id);
        if (order == null) return false;

        await UpdateOrderAndStockAsync(order);
        await ClearUserBasketAsync(user);

        return true;
    }

    public async Task<bool> PayCancel(Guid token)
    {
        var user = await GetUserAsync();
        if (user == null) return false;

        var order = await _orderRepository.GetOrderWithOrderProductsAsync(token, user.Id);
        if (order == null) return false;

        order.Status = OrderStatus.Failed;
        _orderRepository.Update(order);
        await _unitOfWork.CommitAsync();

        return true;
    }

    private async Task<User?> GetUserAsync()
    {
        return await _userManager.GetUserAsync(_actionContextAccessor.ActionContext.HttpContext.User);
    }

    private async Task<Order> CreateOrderAsync(User user)
    {
        var order = new Order
        {
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.Now,
            UserId = user.Id,
            PaymentToken = Guid.NewGuid()
        };

        await _orderRepository.CreateAsync(order);
        return order;
    }

    private async Task<List<SessionLineItemOptions>> CreateSessionLineItemsAsync(User user, Order order)
    {
        var items = new List<SessionLineItemOptions>();

        foreach (var basketProduct in user.Basket.BasketProducts)
        {
            var orderProduct = new OrderProduct
            {
                Order = order,
                Price = basketProduct.Product.Price,
                Count = basketProduct.Count,
                ProductId = basketProduct.ProductId,
            };

            await _orderProductRepository.CreateAsync(orderProduct);

            var item = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = ((orderProduct.Price * 0.09m + orderProduct.Price) * 100),
                    Currency = "USD",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = orderProduct.Product.Title
                    }
                },
                Quantity = orderProduct.Count
            };

            items.Add(item);
        }

        return items;
    }

    private SessionCreateOptions CreateSessionOptions(Order order, List<SessionLineItemOptions> items)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext(httpContext, httpContext.GetRouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()));
        var scheme = _actionContextAccessor.ActionContext.HttpContext.Request.Scheme;

        return new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "payment",
            LineItems = items,
            SuccessUrl = urlHelper.Action("Success", "Payment", new { token = order.PaymentToken }, scheme),
            CancelUrl = urlHelper.Action("Cancel", "Payment", new { token = order.PaymentToken }, scheme),
        };
    }

    private async Task<(int statusCode, string? description, string? id)> CreateStripeSessionAsync(SessionCreateOptions options)
    {
        try
        {
            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return (200, null, session.Id);
        }
        catch (StripeException e)
        {
            return (400, e.Message, null);
        }
    }

    private async Task UpdateOrderAndStockAsync(Order order)
    {
        order.Status = OrderStatus.Success;
        foreach (var orderProduct in order.OrderProducts)
        {
            var product = await _productRepository.GetAsync(orderProduct.ProductId);
            if (product != null)
            {
                product.StockCount -= orderProduct.Count;
                _productRepository.Update(product);
            }
        }
    }

    private async Task ClearUserBasketAsync(User user)
    {
        _basketRepository.Delete(user.Basket);
        await _unitOfWork.CommitAsync();
    }
}
