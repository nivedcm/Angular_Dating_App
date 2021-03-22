using API.Data;
using API.Data.Entities;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public PhotoRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
            var photos = new List<PhotoForApprovalDto>();
            try
            {
                 photos = await _context.Photos
                    .Where(p => p.IsApproved == false)
                    .IgnoreQueryFilters()
                    .Select(u => new PhotoForApprovalDto
                    {
                        Id = u.Id,
                        UserName = u.AppUser.UserName,
                        Url = u.Url,
                        IsApproved = u.IsApproved
                    }).ToListAsync();
            }
            catch(Exception ex)
            {
                throw;
            }


            return photos;
        }
        public async Task<Photo> GetPhotoById(int id)
        {
            return await _context.Photos
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id);
        }
        public void RemovePhoto(Photo photo)
        {
            _context.Photos.Remove(photo);
        }
    }
}
