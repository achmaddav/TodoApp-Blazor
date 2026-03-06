using AutoMapper;
using MediatR;
using TodoApp.Application.Common.Models;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Interfaces;

namespace TodoApp.Application.Features.Categories.Queries.GetAllCategories
{
    public record GetAllCategoriesQuery : IRequest<Result<List<TodoCategoryDto>>>;
}
