using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
   
    [Route("/api/VillaNumberAPI")] //: name api thủ công

    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _aPIResponse;
        private readonly ILogger<VillaAPIController> _logger;
        private readonly IVillaNumberRepository _villaNumberRepository;
        private readonly IMapper _mapper;
        public VillaNumberAPIController(ILogger<VillaAPIController> logger, IVillaNumberRepository villaNumberRepository,
            IMapper mapper)
        {
            _logger = logger;
            _villaNumberRepository = villaNumberRepository;
            _mapper = mapper;
            this._aPIResponse = new ();
        }
        [HttpGet]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillasNumber()
        {
            _logger.LogInformation("GEllVilla");
            try
            {
                IEnumerable<VillaNumber> villaList = await _villaNumberRepository.GetAll();
                _aPIResponse.result = _mapper.Map<IEnumerable<VillaNumberDTO>>(villaList);
                _aPIResponse.StatusCode = HttpStatusCode.OK;
                _aPIResponse.IsSucess = true;
                return Ok(_aPIResponse);
            }
            catch (Exception ex) {
                _aPIResponse.IsSucess = false;
                _aPIResponse.ErrorMessages = new List<string> { ex.Message };
            }
            return _aPIResponse;  
        }

        // get ma co pamater thi phai define no o route http nay
        [HttpGet("{id:int}", Name = "GetVillaNumber")] // cai name la de name cua cai route do de api khac call toi cai name
        // define document ve status response 
        /*[ProducesResponseType(200, Type = typeof(VillaDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]*/

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // co the  ActionResult GetVilla(int id) thay ve tra ve 1 obj cu the thì chỗ 202 sucess sẽ ko hiện ra model 
        // từ đó có thể define type trả về ở define document   [ProducesResponseType(200, Type = typeof(VillaDto))]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try {
                if (id == 0)
                {
                    _logger.LogError($"get id+: {id}");
                    _aPIResponse.StatusCode=HttpStatusCode.BadRequest;
                    return BadRequest(_aPIResponse);
                }
                var item = await _villaNumberRepository.Get(u => u.VillaNo == id);
                if (item == null)
                {
                    _aPIResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_aPIResponse);
                }
                _aPIResponse.result = _mapper.Map<VillaNumberDTO>(item);
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


        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO CreateDto)
        {
            try {
                if (CreateDto == null)
                {
                    return BadRequest();
                }
             
                VillaNumber model = _mapper.Map<VillaNumber>(CreateDto);


                await _villaNumberRepository.Create(model);
                _aPIResponse.result = _mapper.Map<VillaNumberCreateDTO>(CreateDto);
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
        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVillaNumber(int id)
        {
            if (id == 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            var item = await _villaNumberRepository.Get(x => x.VillaNo == id);
            if (item == null)
            {
                return NotFound();
            }
            await  _villaNumberRepository.Remove(item);
            _aPIResponse.StatusCode = HttpStatusCode.NoContent;
            _aPIResponse.IsSucess = true;
            return Ok(_aPIResponse);
        }
        [HttpPut("{id:int}", Name = "UpdateVillaNumberHere")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateNumber(int id, [FromBody] VillaNumberUpdateDTO updateDto)
        {
            if (updateDto == null || id != updateDto.VillaNo)
            {
                return BadRequest();
            }
            var item = await _villaNumberRepository.Get(x => x.VillaNo == id,tracker:false);
            if (item == null)
            {
                return NotFound();
            }
            VillaNumber model = _mapper.Map<VillaNumber>(updateDto);
            await _villaNumberRepository.Update(model);
            _aPIResponse.StatusCode = HttpStatusCode.NoContent;
            _aPIResponse.IsSucess = true;
            return Ok(_aPIResponse);
        }

        [HttpPatch("{id:int}", Name = "UpdateVillaNumberHere")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVillaNumber(int id, JsonPatchDocument<VillaNumberUpdateDTO> patch)
        {
            if (patch == null || id == 0) return BadRequest();
            VillaNumber villa = await _villaNumberRepository.Get(x => x.VillaNo == id, tracker: false);
            if (villa == null) return NotFound();

            // luu vao modelstate de cehck thu no co valid ko
            VillaNumberUpdateDTO villaNumberDto = _mapper.Map<VillaNumberUpdateDTO>(villa);
          
            patch.ApplyTo(villaNumberDto, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            VillaNumber model = _mapper.Map<VillaNumber>(villaNumberDto);
            await _villaNumberRepository.Update(model);
   
            return StatusCode(StatusCodes.Status202Accepted);
        }


    }
}
