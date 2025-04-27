using FluentResults;
using Microsoft.AspNetCore.Mvc;
using StationeryShop.Models;
using StationeryShop.Services;

namespace StationeryShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "��������� ���������� ������");
            }
        }

        //�������� ����� �� id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //�������� ������ �� ��������
        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string name)
        {
            try
            {
                var products = await _productService.SearchProductsAsync(name);

                if (!products.Any())
                {
                    return NotFound("������ �� ������ ������� �� �������");
                }

                return Ok(products);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}