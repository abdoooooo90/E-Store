using AutoMapper;
using BLL.Models.ProductImageDtos;
using DAL.Models;
using DAL.Presistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.ProductImageServices
{
    public class ProductImageService : IProductImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductImageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductImageDto>> GetAllAsync()
        {
            var images = await _unitOfWork.ProductImages.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductImageDto>>(images);
        }

        public async Task<ProductImageDetailsDto?> GetByIdAsync(int id)
        {
            var image = await _unitOfWork.ProductImages.GetByIdAsync(id);
            return image == null ? null : _mapper.Map<ProductImageDetailsDto>(image);
        }

        public async Task<ProductImageDto> CreateAsync(ProductImageCreateDto dto)
        {
            var image = _mapper.Map<ProductImage>(dto);
            _unitOfWork.ProductImages.Add(image);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ProductImageDto>(image);
        }

        public async Task<bool> UpdateAsync(int id, ProductImageUpdateDto dto)
        {
            var image = await _unitOfWork.ProductImages.GetByIdAsync(id);
            if (image == null) return false;

            _mapper.Map(dto, image);
            _unitOfWork.ProductImages.Update(image);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var image = await _unitOfWork.ProductImages.GetByIdAsync(id);
            if (image == null) return false;

            _unitOfWork.ProductImages.Delete(image);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
