using AutoMapper;
using BLL.Models.ProductDtos;
using BLL.Services.ProductServices;
using DAL.Models;
using DAL.Presistance.Repositories.Generic;
using DAL.Presistance.Repositories.Products;
using DAL.Presistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllWithCategoryAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetLatestProductsAsync(int count = 3)
        {
            var products = await _unitOfWork.Products.GetAllWithCategoryAsync();
            var latestProducts = products.OrderByDescending(p => p.CreatedAt).Take(count);
            return _mapper.Map<IEnumerable<ProductDto>>(latestProducts);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            _unitOfWork.Products.Add(product);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return false;

            _mapper.Map(dto, product);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null) return false;

                _unitOfWork.Products.Delete(product);
                var result = await _unitOfWork.CompleteAsync();
                return result > 0;
            }
            catch (Exception)
            {
                // في حالة Optimistic Concurrency Exception أو أي خطأ آخر
                return false;
            }
        }
    }
}
