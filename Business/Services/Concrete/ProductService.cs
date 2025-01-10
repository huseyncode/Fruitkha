    using Business.Services.Abstract;
using Business.ViewModels.Product;
using Core.Entities;
using Data.Repositories.Abstract;
using Data.UnitOfWork;
using IdentityProject.Utilities.File;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concrete;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
    private readonly ICategoryService _categoryService;
    private readonly ModelStateDictionary _modelState;

    public ProductService(IProductRepository productRepository,
                          IUnitOfWork unitOfWork,
                          IActionContextAccessor actionContextAccessor,
                          IFileService fileService,
                          ICategoryService categoryService)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
        _categoryService = categoryService;
        _modelState = actionContextAccessor.ActionContext.ModelState;
    }

    public async Task<ProductIndexVM> GetAllAsync()
    {
        var products = await _productRepository.GetAllProductsWithCategoriesAsync();
        return new ProductIndexVM { Products = products };
    }

    public IQueryable<Product> GetAllByFilter(ProductIndexVM model)
    {
        var products = _productRepository.FilterByName(model.Name);
        products = _productRepository.FilterByCount(products, model.MinCount, model.MaxCount);
        products = _productRepository.FilterByPrice(products, model.MinPrice, model.MaxPrice);
        return products;
    }

    public async Task<ProductCreateVM> CreateAsync()
    {
        var categories = await _categoryService.GetAllAsync();
        var model = new ProductCreateVM
        {
            Categories = categories.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList()
        };
        return model;
    }

    public async Task<bool> CreateAsync(ProductCreateVM model)
    {
        if (!_modelState.IsValid) return false;

        if (await _productRepository.GetProductByTitleAsync(model.Title) != null)
        {
            _modelState.AddModelError("Product", "This product already exists");
            return false;
        }

        if (!await ValidateCategoriesAsync(model.CategoryIds)) return false;

        if (!ValidatePhoto(model.Photo)) return false;

        var photoName = _fileService.Upload(model.Photo, "assets/img");
        var product = new Product
        {
            Title = model.Title,
            Description = model.Description,
            Price = model.Price,
            StockCount = model.StockCount,
            Photo = photoName,
            CreatedAt = DateTime.Now,
            ProductCategories = model.CategoryIds.Select(id => new ProductCategories { CategoryId = id }).ToList()
        };

        await _productRepository.CreateAsync(product);
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<ProductUpdateVM> UpdateAsync(int id)
    {
        var product = await _productRepository.GetProductWithCategoriesAsync(id);
        if (product == null) return null;

        var categories = await _categoryService.GetAllAsync();
        var categoryIds = product.ProductCategories.Select(pc => pc.CategoryId).ToList();

        var model = new ProductUpdateVM
        {
            Title = product.Title,
            Description = product.Description,
            Price = product.Price,
            StockCount = product.StockCount,
            PhotoName = product.Photo,
            Categories = categories.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList(),
            CategoryIds = categoryIds
        };

        return model;
    }

    public async Task<bool> UpdateAsync(int id, ProductUpdateVM model)
    {
        if (!_modelState.IsValid) return false;

        var product = await _productRepository.GetProductWithCategoriesAsync(id);
        if (product == null)
        {
            _modelState.AddModelError(string.Empty, "This product is not available");
            return false;
        }

        if (await _productRepository.GetProductByTitleAsync(model.Title) is { Id: not id })
        {
            _modelState.AddModelError("Name", "This product already exists");
            return false;
        }

        product.Title = model.Title;
        product.Description = model.Description;
        product.Price = model.Price;
        product.StockCount = model.StockCount;
        product.ModifiedAt = DateTime.Now;
        product.ProductCategories = model.CategoryIds.Select(cid => new ProductCategories { CategoryId = cid }).ToList();

        if (model.Photo != null)
        {
            if (!ValidatePhoto(model.Photo)) return false;

            _fileService.Delete("assets/img", product.Photo);
            product.Photo = _fileService.Upload(model.Photo, "assets/img");
        }

        _productRepository.Update(product);
        await _unitOfWork.CommitAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetAsync(id);
        if (product == null) return false;

        _productRepository.Delete(product);
        await _unitOfWork.CommitAsync();
        return true;
    }

    private async Task<bool> ValidateCategoriesAsync(List<int> categoryIds)
    {
        foreach (var categoryId in categoryIds)
        {
            if (await _categoryService.GetAsync(categoryId) == null)
            {
                _modelState.AddModelError("CategoryIds", "Category isn't available");
                return false;
            }
        }
        return true;
    }

    private bool ValidatePhoto(IFormFile photo)
    {
        if (!_fileService.IsImage(photo.ContentType))
        {
            _modelState.AddModelError("Photo", "The image is not in the correct format");
            return false;
        }

        if (!_fileService.IsTrueSize(photo.Length))
        {
            _modelState.AddModelError("Photo", "Length must be less than 500 kb");
            return false;
        }

        return true;
    }
}
