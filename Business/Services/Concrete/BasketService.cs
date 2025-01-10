using Business.Services.Abstract;
using Business.ViewModels.Basket;
using Core.Entities;
using Data.Repositories.Abstract;
using Data.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concrete;

public class BasketService : IBasketService
{
    private readonly IBasketRepository _basketRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly IProductRepository _productRepo;
    private readonly IBasketProductRepository _basketProductRepo;

    public BasketService(IBasketRepository basketRepo,
                         IUnitOfWork unitOfWork,
                         UserManager<User> userManager,
                         IActionContextAccessor actionContextAccessor,
                         IProductRepository productRepo,
                         IBasketProductRepository basketProductRepo)
    {
        _basketRepo = basketRepo;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _actionContextAccessor = actionContextAccessor;
        _productRepo = productRepo;
        _basketProductRepo = basketProductRepo;
    }

    public async Task<BasketIndexVM> GetAllAsync()
    {
        var currentUser = await _userManager.GetUserAsync(_actionContextAccessor.ActionContext.HttpContext.User);
        if (currentUser == null) return null;

        var user = await _userManager.Users.Include(u => u.Basket).FirstOrDefaultAsync(u => u.Id == currentUser.Id);
        if (user?.Basket == null) return new BasketIndexVM();

        return new BasketIndexVM
        {
            BasketProducts = await _basketProductRepo.GetBasketProductsWithProducts(user.Basket.Id)
        };
    }

    public async Task<(int statusCode, string description)> AddProductAsync(int productId)
    {
        var user = await _userManager.GetUserAsync(_actionContextAccessor.ActionContext.HttpContext.User);
        if (user == null) return (401, "Product couldn't be added");

        var product = await _productRepo.GetAsync(productId);
        if (product == null) return (404, "Product couldn't be added");

        if (product.StockCount == 0) return (400, "Out of stock");

        var basket = await _basketRepo.GetBasketByUserId(user.Id) ?? new Basket
        {
            UserId = user.Id,
            CreatedAt = DateTime.Now
        };

        if (basket.Id == 0)
        {
            await _basketRepo.CreateAsync(basket);
        }

        var basketProduct = await _basketProductRepo.GetByProductIdAndUserId(product.Id, user.Id);
        if (basketProduct == null)
        {
            basketProduct = new BasketProduct
            {
                Basket = basket,
                ProductId = productId,
                Count = 1,
                CreatedAt = DateTime.Now
            };
            await _basketProductRepo.CreateAsync(basketProduct);
        }
        else
        {
            if (basketProduct.Count == product.StockCount)
                return (400, "Out of stock");

            basketProduct.Count++;
            _basketProductRepo.Update(basketProduct);
        }

        await _unitOfWork.CommitAsync();
        return (200, "The product was successfully added to the basket");
    }

    public async Task<(int statusCode, string description)> UpdateCartAsync(List<BasketUpdateVM> updatedProducts)
    {
        var user = await _userManager.GetUserAsync(_actionContextAccessor.ActionContext.HttpContext.User);
        if (user == null) return (401, "Product couldn't be added");

        var basket = await _basketRepo.GetBasketByUserId(user.Id);
        if (basket == null) return (404, "User's basket not found");

        foreach (var product in updatedProducts)
        {
            var basketProduct = await _basketProductRepo.GetByProductIdAndUserId(product.ProductId, user.Id);
            if (basketProduct == null) continue;

            var stockProduct = await _productRepo.GetAsync(product.ProductId);
            if (stockProduct == null) return (404, "Product not found");

            if (product.Count > stockProduct.StockCount) return (400, "Out of stock");

            if (product.Count <= 0) return (400, "There must be at least 1 product in basket");

            basketProduct.Count = product.Count;
            _basketProductRepo.Update(basketProduct);
        }

        await _unitOfWork.CommitAsync();
        return (200, "Basket updated");
    }

    public async Task<(int statusCode, string description)> DeleteAsync(int id)
    {
        var user = await _userManager.GetUserAsync(_actionContextAccessor.ActionContext.HttpContext.User);
        if (user == null) return (401, "Product couldn't be added");

        var basketProduct = await _basketProductRepo.GetByProductIdAndUserId(id, user.Id);
        if (basketProduct == null) return (404, "Product not found");

        if (basketProduct.Basket.UserId != user.Id)
            return (400, "The product could not be deleted");

        _basketProductRepo.Delete(basketProduct);
        await _unitOfWork.CommitAsync();

        return (200, "Product removed");
    }
}
