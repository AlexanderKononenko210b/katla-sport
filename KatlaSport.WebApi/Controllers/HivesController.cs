using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using KatlaSport.Services.HiveManagement;
using KatlaSport.WebApi.CustomFilters;
using KatlaSport.WebApi.Properties;
using Microsoft.Web.Http;
using Swashbuckle.Swagger.Annotations;

namespace KatlaSport.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/hives")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [CustomExceptionFilter]
    [SwaggerResponseRemoveDefaults]
    public class HivesController : ApiController
    {
        private readonly IHiveService _hiveService;
        private readonly IHiveSectionService _hiveSectionService;

        public HivesController(IHiveService hiveService, IHiveSectionService hiveSectionService)
        {
            _hiveService = hiveService ?? throw new ArgumentNullException(nameof(hiveService));
            _hiveSectionService = hiveSectionService ?? throw new ArgumentNullException(nameof(hiveSectionService));
        }

        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a list of hives.", Type = typeof(HiveListItem[]))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHivesAsync()
        {
            var hives = await _hiveService.GetHivesAsync();
            return Ok(hives);
        }

        [HttpGet]
        [Route("{hiveId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a hive.", Type = typeof(Hive))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHiveAsync(int? hiveId)
        {
            if (hiveId == null)
                return BadRequest(Resources.BadCreateRequest);

            var hive = await _hiveService.GetHiveAsync(hiveId.Value);

            return Ok(hive);
        }

        [HttpGet]
        [Route("{hiveId:int:min(1)}/sections")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a list of hive sections for specified hive.",
            Type = typeof(HiveSectionListItem))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHiveSectionsAsync(int? hiveId)
        {
            if (hiveId == null)
                return BadRequest(Resources.BadCreateRequest);

            var hive = await _hiveSectionService.GetHiveSectionsAsync(hiveId.Value);

            return Ok(hive);
        }

        [HttpPut]
        [Route("{hiveId:int:min(1)}/status/{deletedStatus:bool}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Sets deleted status for an existed hive.")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> SetStatusAsync([FromUri] int? hiveId, [FromUri] bool deletedStatus)
        {
            if (hiveId == null)
                return BadRequest(Resources.BadCreateRequest);

            await _hiveService.SetStatusAsync(hiveId.Value, deletedStatus);

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }

        /// <summary>
        /// Add new Hive
        /// </summary>
        /// <param name="updateHiveRequest">hives model</param>
        /// <returns>Task{IHttpActionResult}</returns>
        [HttpPost]
        [Route("add")]
        [SwaggerResponse(HttpStatusCode.Created, "Successful create hive", typeof(Hive))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Data for create is not valid")]
        [SwaggerResponse(HttpStatusCode.Conflict, "Conflict on the server")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Other errors")]
        public async Task<IHttpActionResult> AddHive([FromBody] UpdateHiveRequest updateHiveRequest)
        {
            if (updateHiveRequest == null)
                return BadRequest();

            var hive = await _hiveService.CreateHiveAsync(updateHiveRequest);

            var location = $"/api/hives/{hive.Id}";

            return Created<Hive>(location, hive);
        }

        /// <summary>
        /// Update exist hive
        /// </summary>
        /// <param name="hiveId">hives id</param>
        /// <param name="updateHiveRequest">hives model for update</param>
        /// <returns>Task{IHttpActionResult}</returns>
        [HttpPut]
        [Route("{hiveId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.NoContent, "Update is succesfully")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Data for update is not valid")]
        [SwaggerResponse(HttpStatusCode.Conflict, "Conflict on the server")]
        [SwaggerResponse(HttpStatusCode.NotFound, "Hive for update not found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Other errors")]
        public async Task<IHttpActionResult> UpdateHive([FromUri] int? hiveId, [FromBody] UpdateHiveRequest updateHiveRequest)
        {
            if (hiveId == null || updateHiveRequest == null || !ModelState.IsValid)
                return BadRequest();

            await _hiveService.UpdateHiveAsync(hiveId.Value, updateHiveRequest);

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }

        /// <summary>
        /// Delete hive
        /// </summary>
        /// <param name="hiveId">hives id</param>
        /// <returns>Task{IHttpActionResult}</returns>
        [HttpDelete]
        [Route("{hiveId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.NoContent, "Delete is succesfully")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Data for delete is not valid")]
        [SwaggerResponse(HttpStatusCode.Conflict, "Conflict on the server")]
        [SwaggerResponse(HttpStatusCode.NotFound, "Hive for delete not found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Other errors")]
        public async Task<IHttpActionResult> DeleteHive([FromUri] int? hiveId)
        {
            if (hiveId == null)
                return BadRequest();

            await _hiveService.DeleteHiveAsync(hiveId.Value);

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
        }
    }
}
