﻿using Business.Services.Abstract;
using Business.ViewModels.Comment;
using Business.ViewModels.News;
using Core.Entities;
using Data.Repositories.Abstract;
using Data.UnitOfWork;
using IdentityProject.Utilities.File;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.Concrete;

public class NewsService : INewsService
{
    private readonly INewsRepository _newsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileService _fileService;
	private readonly IActionContextAccessor _actionContextAccessor;
	private readonly UserManager<User> _userManager;
	private readonly ICommentRepository _commentRepository;
	private readonly ModelStateDictionary _modelState;

    public NewsService(INewsRepository newsRepository,
                       IUnitOfWork unitOfWork,
                       IFileService fileService,
                       IActionContextAccessor actionContextAccessor,
                       UserManager<User> userManager,
                       ICommentRepository commentRepository)
    {
        _newsRepository = newsRepository;
        _unitOfWork = unitOfWork;
        _fileService = fileService;
		_actionContextAccessor = actionContextAccessor;
		_userManager = userManager;
		_commentRepository = commentRepository;
		_modelState = actionContextAccessor.ActionContext.ModelState;
    }

    public async Task<NewsIndexVM> GetAllAsync()
    {
        return new NewsIndexVM {
            News = await _newsRepository.GetAllAsync()
        };
    }

    public async Task<NewsDetailsVM> GetAsync(int id)
    {
        var news = await _newsRepository.GetAsync(id);
        if (news is not null) return new NewsDetailsVM
        {
            NewsId = id,
            Title = news.Title,
            Body = news.Body,
            Photo = news.Photo,
            CreatedAt = news.CreatedAt,
            Comments = await _commentRepository.GetCommentsByNewsId(id)
        };

        return null;
    }

    public async Task<bool> CreateAsync(NewsCreateVM model)
    {
        if (!_modelState.IsValid) return false;

        var news = await _newsRepository.GetNewsByTitle(model.Title);
        if (news is not null)
        {
            _modelState.AddModelError("News", "This news is available");
            return false;
        }

        if (!_fileService.IsImage(model.Photo.ContentType))
        {
            _modelState.AddModelError("Photo", "The image is not in the correct format");
            return false;
        }

        if (!_fileService.IsTrueSize(model.Photo.Length))
        {
            _modelState.AddModelError("Photo", "Length must be less than 500 kb");
            return false;
        }

        var photoName = _fileService.Upload(model.Photo, "assets/img");

        news = new News
        {
            Title = model.Title,
            Body = model.Body,
            Photo = photoName,
            CreatedAt = DateTime.Now,
        };

        await _newsRepository.CreateAsync(news);
        await _unitOfWork.CommitAsync();

        return true;    
    }

    public async Task<NewsUpdateVM> UpdateAsync(int id)
    {
        var news = await _newsRepository.GetAsync(id);
        if (news is null) return null;

        var model = new NewsUpdateVM
        {
            Title = news.Title,
            Body = news.Body,
            PhotoName = news.Photo
        };

        return model;
    }

    public async Task<bool> UpdateAsync(int id, NewsUpdateVM model)
    {
        if (!_modelState.IsValid) return false;
        
        var news = await _newsRepository.GetAsync(id);
        if (news is null)
        {
            _modelState.AddModelError("News", "This news is not available");
            return false;
        }

        var existNews = await _newsRepository.GetNewsByTitle(model.Title);
        if (existNews is not null && existNews.Id != id)
        {
            _modelState.AddModelError("Title", "This news has already existed");
            return false;
        }

        news.Title = model.Title;
        news.Body = model.Body;
        news.ModifiedAt = DateTime.Now;

        if (model.Photo is not null)
        {
            if (!_fileService.IsImage(model.Photo.ContentType))
            {
                _modelState.AddModelError("Photo", "The image is not in the correct format");
                return false;
            }

            if (!_fileService.IsTrueSize(model.Photo.Length))
            {
                _modelState.AddModelError("Photo", "Length must be less than 500 kb");
                return false;
            }

            _fileService.Delete("assets/img", news.Photo);
            news.Photo = _fileService.Upload(model.Photo, "assets/img");
        }

        _newsRepository.Update(news);
        await _unitOfWork.CommitAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var news = await _newsRepository.GetAsync(id);
        if (news is null) return false;

        _newsRepository.Delete(news);
        await _unitOfWork.CommitAsync();

        return true;
    }

    public async Task<bool> CreateCommentAsync(CommentCreateVM model)
    {
        if (!_modelState.IsValid) return false;

		var checkUser = await _userManager.GetUserAsync(_actionContextAccessor.ActionContext.HttpContext.User);
		if (checkUser is null) return false;

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != checkUser) return false;

        var result = new Comment
        {
            UserId = user.Id,
            NewsId = model.NewsId,
            Name = model.Name,
            Email = model.Email,
            Message = model.Message,
            CreatedAt = DateTime.Now,
        };

        await _commentRepository.CreateAsync(result);
        await _unitOfWork.CommitAsync();

        return true;
	}

}
