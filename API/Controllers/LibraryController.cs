﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Authorize]
    public class LibraryController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IDirectoryService _directoryService;
        private readonly ILibraryRepository _libraryRepository;
        private readonly ILogger<LibraryController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public LibraryController(DataContext context, IDirectoryService directoryService, 
            ILibraryRepository libraryRepository, ILogger<LibraryController> logger, IUserRepository userRepository,
            IMapper mapper)
        {
            _context = context;
            _directoryService = directoryService;
            _libraryRepository = libraryRepository;
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns a list of directories for a given path. If path is empty, returns root drives.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("list")]
        public ActionResult<IEnumerable<string>> GetDirectories(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return Ok(Directory.GetLogicalDrives());
            }

            if (!Directory.Exists(path)) return BadRequest("This is not a valid path");

            return Ok(_directoryService.ListDirectory(path));
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LibraryDto>>> GetLibraries()
        {
            return Ok(await _libraryRepository.GetLibrariesAsync());
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("update-for")]
        public async Task<ActionResult<MemberDto>> UpdateLibrary(UpdateLibraryDto updateLibraryDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(updateLibraryDto.Username);

            if (user == null) return BadRequest("Could not validate user");

            user.Libraries = new List<Library>();

            foreach (var selectedLibrary in updateLibraryDto.SelectedLibraries)
            {
                user.Libraries.Add(_mapper.Map<Library>(selectedLibrary));
            }
            
            if (await _userRepository.SaveAllAsync())
            {
                return Ok(user);
            }

            return BadRequest("Not Implemented");
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("scan")]
        public async Task<ActionResult> ScanLibrary(int libraryId)
        {
            var library = await _libraryRepository.GetLibraryForIdAsync(libraryId);

            _directoryService.ScanLibrary(library);

            return Ok();
        }
    }
}