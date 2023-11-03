using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RegionController : ControllerBase
    {
        private readonly NZWalksDbContext _dbContext;
        private readonly IRegionRepository _regionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RegionController> _logger;

        public RegionController(NZWalksDbContext dbContext,
            IRegionRepository regionRepository, IMapper mapper, ILogger<RegionController> logger)
        {
            _dbContext = dbContext;
            _regionRepository = regionRepository;
            _mapper = mapper;
            _logger = logger;
        }


        // GET: api/Region
        [HttpGet]
        //[Authorize(Roles = "Reader")]

        public async Task<IActionResult> GetAll()
        {
            try
            { 

                // Get Data form Database - Domain Models
                var regionsDomain = await _regionRepository.GetAllAsync();

                //Map Domain Models to DTOs
                var regionsDto = _mapper.Map<List<RegionDto>>(regionsDomain);


                // Log the data
                _logger.LogInformation($"Finished GetAll Region request with data: {JsonSerializer.Serialize(regionsDomain)}");

                //Return DTOs to Client
                return Ok(regionsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        // GET: api/Region/{id}
        [HttpGet("{id:Guid}", Name = "GetRegion")]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //var region = _dbContext.Regions.Find(id);
            //var region = _dbContext.Regions.SingleOrDefault(r => r.Id == id);

            // Get Region Domain Model From Database
            var regionDomain = await _regionRepository.GetByIdAsync(id);

            if (regionDomain == null)
            {
                return NotFound();
            }

            // Map/Convert Region Domain Model to Region DTO Model
            var regionDto = _mapper.Map<RegionDto>(regionDomain);

            //Return DTO to Client
            return Ok(regionDto);

        }


        // POST Single Region 
        [HttpPost]
        //[Authorize(Roles = "Writer")]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {


            // Map or Convert the Dto to Domain model 
            var regionDomainModel = _mapper.Map<Region>(addRegionRequestDto);

            // Use Domain Model to create Region
            regionDomainModel = await _regionRepository.CreateAsync(regionDomainModel);


            //Map Domain model back to DTO 
            var regionDto = _mapper.Map<RegionDto>(regionDomainModel);

            return CreatedAtAction(nameof(GetById),
                new { id = regionDomainModel.Id },
                regionDomainModel);

        }

        // Update Region
        // PUT Reguest https://localhost:portNumber/api/region/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {

            // Map Dto to Domain Model
            var regionDomainModel = _mapper.Map<Region>(updateRegionRequestDto);


            regionDomainModel = await _regionRepository.UpdateAsync(id, regionDomainModel);

            //Checks if Region exists
            if (regionDomainModel == null)
            {
                return NotFound();
            }

            await _dbContext.SaveChangesAsync();

            // Convert DomainModel to Dto
            var regionDto = _mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);

        }

        // DELETE Region
        // DELETE Reguest https://localhost:portNumber/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var regionDomainModel = await _regionRepository.DeleteByIdAsync(id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            var regionDto = _mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);
        }

    }
}
