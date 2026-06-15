using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using market_mvc.Infrastructure.Data;
using market_mvc.Extensions;
using market_mvc.Features.Products.Commands.CreateProduct;
using market_mvc.Features.Products.Commands.DeleteProduct;
using market_mvc.Features.Products.Commands.UpdateProduct;
using market_mvc.Features.Products.Queries.GetProductById;
using market_mvc.Features.Products.Queries.GetProducts;
using market_mvc.Domain;
using market_mvc.Domain.Enums;
using market_mvc.Domain.ViewModels.Product;
using market_mvc.Features.Products.Services;

namespace market_mvc.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly ILogger<ProductsController> _logger;
        
        public ProductsController(
            IMediator mediator,
            IMapper mapper,
            IImageService imageService,
            ILogger<ProductsController> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _imageService = imageService;
            _logger = logger;
        }

        // GET: Products
        public async Task<IActionResult> Index(ProductSearchRequest request)
        {
            // Set up filter options for the view
            ViewBag.FilterOptions = new Dictionary<string, List<SelectListItem>>
            {
                ["PriceRange"] = new List<SelectListItem>
                {
                    new SelectListItem { Text = "$0 - $50", Value = "0-50" },
                    new SelectListItem { Text = "$51 - $100", Value = "51-100" },
                    new SelectListItem { Text = "$101 - $500", Value = "101-500" },
                    new SelectListItem { Text = "$500+", Value = "500+" }
                }
            };

            ViewBag.SearchPlaceholder = "Search products by name...";

            var query = new GetProductsQuery
            {
                SearchTerm = request.Search,
                PriceRange = request.PriceRange,
                SortBy = request.SortBy,
                SortOrder = request.SortDirection,
                Page = request.Page,
                PageSize = request.PageSize
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // AJAX request - return partial view with data
                var result = await _mediator.Send(query);
                return Json(new
                {
                    html = await this.RenderViewAsync("_ProductList", result.Items),
                    totalItems = result.TotalItems,
                    currentPage = result.CurrentPage,
                    totalPages = result.TotalPages
                });
            }

            // Regular request - return full view
            var products = await _mediator.Send(query);
            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            await SetupCreateEditViewBag();
            return View(new ProductCreateVM());
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM viewModel)
        {
            if (ModelState.IsValid)
            {
                var command = _mapper.Map<CreateProductCommand>(viewModel);

                // Handle image upload
                if (viewModel.ImageFile != null && _imageService.IsValidImageFile(viewModel.ImageFile))
                {
                    var imageUrl = await _imageService.UploadImageAsync(viewModel.ImageFile, "products");
                    if (imageUrl != null)
                    {
                        command.ImageUrl = imageUrl;
                    }
                    else
                    {
                        ModelState.AddModelError("ImageFile", "Failed to upload image. Please try again.");
                        await SetupCreateEditViewBag();
                        return View(viewModel);
                    }
                }
                else if (!string.IsNullOrEmpty(viewModel.ImageUrl))
                {
                    command.ImageUrl = viewModel.ImageUrl;
                }

                var product = await _mediator.Send(command);
                TempData["SuccessMessage"] = $"Product '{product.Name}' created successfully.";
                return RedirectToAction(nameof(Index));
            }
            
            await SetupCreateEditViewBag();
            return View(viewModel);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<ProductEditVM>(product);
            await SetupCreateEditViewBag();
            return View(viewModel);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductEditVM viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var command = _mapper.Map<UpdateProductCommand>(viewModel);

                // Store old image URL for potential deletion
                var oldImageUrl = viewModel.CurrentImageUrl;

                // Handle image upload
                if (viewModel.ImageFile != null && _imageService.IsValidImageFile(viewModel.ImageFile))
                {
                    var imageUrl = await _imageService.UploadImageAsync(viewModel.ImageFile, "products");
                    if (imageUrl != null)
                    {
                        command.ImageUrl = imageUrl;
                        
                        // Delete old image if it was uploaded (not external URL)
                        if (!string.IsNullOrEmpty(oldImageUrl) && !oldImageUrl.StartsWith("http"))
                        {
                            await _imageService.DeleteImageAsync(oldImageUrl);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ImageFile", "Failed to upload image. Please try again.");
                        await SetupCreateEditViewBag();
                        return View(viewModel);
                    }
                }
                else if (!string.IsNullOrEmpty(viewModel.ImageUrl) && viewModel.ImageUrl != oldImageUrl)
                {
                    // URL changed, update it
                    command.ImageUrl = viewModel.ImageUrl;
                    
                    // Delete old uploaded image if it exists
                    if (!string.IsNullOrEmpty(oldImageUrl) && !oldImageUrl.StartsWith("http"))
                    {
                        await _imageService.DeleteImageAsync(oldImageUrl);
                    }
                }

                var product = await _mediator.Send(command);
                TempData["SuccessMessage"] = $"Product '{product.Name}' updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            
            await SetupCreateEditViewBag();
            return View(viewModel);
        }

        private async Task SetupCreateEditViewBag()
        {
            // Get categories for dropdown
            using var scope = HttpContext.RequestServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var categories = await context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
            
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.ProductStatuses = new SelectList(Enum.GetValues<ProductStatus>().Select(s => new { 
                Value = (int)s, 
                Text = s.ToString() 
            }), "Value", "Text");
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            if (product != null)
            {
                var success = await _mediator.Send(new DeleteProductCommand(id));
                if (success)
                {
                    TempData["SuccessMessage"] = $"Product '{product.Name}' deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete product.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Product not found.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

