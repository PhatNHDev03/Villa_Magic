using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Controllers
{
    //b1: them attribute o day de notify day la controller api
    //b2: phai define route 
    // controller --> cung duoc ma no bi overhead do no bao gom mvc luon --> dung controllerbase de no chi dung api thui
    [Route("/api/v{version:apiVersion}/VillaAPI")] //: name api thủ công
    //[Route("api/[controller]")] : ko nen nếu thay đổi name contrller thì tất cả những route tới cái api nãy cung tahy đổi theo rrất lỏ
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class VillaAPIController : ControllerBase
    {
        protected APIResponse _aPIResponse;
        private readonly ILogger<VillaAPIController> _logger;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;
        public VillaAPIController(ILogger<VillaAPIController> logger, IVillaRepository villaRepository,
            IMapper mapper)
        {
            _logger = logger;
            _villaRepository = villaRepository;
            _mapper = mapper;
            this._aPIResponse = new ();
        }

        [HttpGet]

        [ResponseCache(CacheProfileName = "Default30")]
        // [ResponseCache(Duration =30)] //cache trong 30 sencond
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name ="filter occupancy")] int? occupancy,
            [FromQuery] string? search = null, int pageSize = 3, int pageNumber = 1
            )
        {
            _logger.LogInformation("GEllVilla");
            try
            {
                IEnumerable<Villa> villaList;
                if (occupancy > 0)
                {
                    villaList = await _villaRepository.GetAll(u => u.Occupancy == occupancy,pageSize:pageSize,pageNumber:pageNumber);
                }
                else { 
                    villaList = await _villaRepository.GetAll(pageSize: pageSize, pageNumber: pageNumber);

                }
                var check1 = villaList.ToList();
                if (!String.IsNullOrEmpty(search)) {
                    var check = villaList
                        .Where(x => x.Name.ToLower().Contains(search.ToLower()))
                        .ToList();
                    villaList = villaList.Where(x => x.Name.ToLower().Contains(search.ToLower()));
                }
                Pagination pagination = new Pagination() { 
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
                _aPIResponse.result = _mapper.Map<IEnumerable<VillaDto>>(villaList);
                _aPIResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_aPIResponse);
            }
            catch (Exception ex) {
                _aPIResponse.IsSucess = false;
                _aPIResponse.ErrorMessages = new List<string> { ex.Message };
            }
            return _aPIResponse;  
        }

    
        // get ma co pamater thi phai define no o route http nay
        [HttpGet("{id:int}", Name = "GetVilla")] // cai name la de name cua cai route do de api khac call toi cai name
        // define document ve status response 
        /*[ProducesResponseType(200, Type = typeof(VillaDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]*/
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // co the  ActionResult GetVilla(int id) thay ve tra ve 1 obj cu the thì chỗ 202 sucess sẽ ko hiện ra model 
        // từ đó có thể define type trả về ở define document   [ProducesResponseType(200, Type = typeof(VillaDto))]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try {
                if (id == 0)
                {
                    _logger.LogError($"get id+: {id}");
                    _aPIResponse.StatusCode=HttpStatusCode.BadRequest;
                    return BadRequest(_aPIResponse);
                }
                var item = await _villaRepository.Get(u => u.Id == id);
                if (item == null)
                {
                    _aPIResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_aPIResponse);
                }
                _aPIResponse.result = _mapper.Map<VillaDto>(item);
                _aPIResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_aPIResponse);
            }
            catch (Exception ex)
            {
                _aPIResponse.IsSucess = false;
                _aPIResponse.ErrorMessages = new List<string> { ex.Message };
            }
            return _aPIResponse;
        }
      

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDto CreateDto)
        {
            try {
                if (CreateDto == null)
                {
                    return BadRequest();
                }
                // custome validation
                if (await _villaRepository.Get(u => u.Name == CreateDto.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa name is not unique");
                    return BadRequest(ModelState);
                }
                Villa model = _mapper.Map<Villa>(CreateDto);


                await _villaRepository.Create(model);
                _aPIResponse.result = _mapper.Map<VillaCreateDto>(CreateDto);
                _aPIResponse.StatusCode = HttpStatusCode.Created;
                return Ok(_aPIResponse);

                // return CreatedAtRoute("GetVilla", new {id = villa.Id}, villaDto); // mac dinh tra ve status 201
            }

            catch (Exception ex)
            {
                _aPIResponse.IsSucess = false;
                _aPIResponse.ErrorMessages = new List<string> { ex.Message };
            }
            return _aPIResponse;
        }
        // IAction result thi ko can phai define type return
        [HttpDelete("{id:int}", Name = "DeleteVilla")]

        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            var item = await _villaRepository.Get(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            await  _villaRepository.Remove(item);
            _aPIResponse.StatusCode = HttpStatusCode.NoContent;
            _aPIResponse.IsSucess = true;
            return Ok(_aPIResponse);
        }
        [HttpPut("{id:int}", Name = "UpdateVillaHere")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> Update(int id, [FromBody] VillaUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }
            var item = _villaRepository.Get(x => x.Id == id,tracker:false);
            if (item == null)
            {
                return NotFound();
            }
            Villa model = _mapper.Map<Villa>(updateDto);
            await _villaRepository.Update(model);
            _aPIResponse.StatusCode = HttpStatusCode.NoContent;
            _aPIResponse.IsSucess = true;
            return Ok(_aPIResponse);
        }

        [HttpPatch("{id:int}", Name = "UpdateVillaHere")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patch)
        {
            if (patch == null || id == 0) return BadRequest();
            var villa = _villaRepository.Get(x => x.Id == id, tracker: false);
            if (villa == null) return NotFound();

            // luu vao modelstate de cehck thu no co valid ko
            var villaDto = _mapper.Map<VillaUpdateDto>(villa);
          
            patch.ApplyTo(villaDto, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Villa model = _mapper.Map<Villa>(villaDto);
            await _villaRepository.Update(model);
   
            return StatusCode(StatusCodes.Status202Accepted);
        }


    }
}
