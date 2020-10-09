using capgemini_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace capgemini_api.Controllers
{

    [Route("api/importacao")]
    [ApiController]
    public class ImportacaoController : ControllerBase
    {


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetImportacao([FromServices] ImportacaoService importacaoService, int id)
        {
            try
            {
                var result = await importacaoService.FindById(id);
                if(result == null) {
                    return NotFound("Não foi possível encontrar a importação com Id: " + id);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllImportacoes([FromServices] ImportacaoService importacaoService)
        {
            try
            {
                var result = await importacaoService.ReturnAllImportacoes();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Insert(
            [FromServices] ImportacaoService importacaoService,
            IFormFile file
        )
        {
            try
            {
                var errors = await importacaoService.ImportarExcel(file);
                if (errors.Length > 0)
                {
                    return BadRequest(errors);
                }

                return Ok("Importação realizada com sucesso");
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }
    }
}