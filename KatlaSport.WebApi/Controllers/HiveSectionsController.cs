using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using KatlaSport.Services;
using KatlaSport.Services.HiveManagement;
using KatlaSport.WebApi.CustomFilters;
using KatlaSport.WebApi.Properties;
using Microsoft.Web.Http;
using Swashbuckle.Swagger.Annotations;

namespace KatlaSport.WebApi.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/sections")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [CustomExceptionFilter]
    [SwaggerResponseRemoveDefaults]
    public class HiveSectionsController : ApiController
    {
        private readonly IHiveSectionService _hiveSectionService;

        public HiveSectionsController(IHiveSectionService hiveSectionService)
        {
            _hiveSectionService = hiveSectionService ?? throw new ArgumentNullException(nameof(hiveSectionService));
        }

        /// <summary>
        /// Get hive sections async
        /// </summary>
        /// <returns>Task{IHttpActionResult}</returns>
        [HttpGet]
        [Route("")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a list of hive sections.", Type = typeof(HiveSectionListItem[]))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHiveSectionsAsync()
        {
            var hives = await _hiveSectionService.GetHiveSectionsAsync();
            return Ok(hives);
        }
        /// <summary>
        /// Get hive section async
        /// </summary>
        /// <param name="hiveSectionId">id hive section</param>
        /// <returns>Task{IHttpActionResult}</returns>
        [HttpGet]
        [Route("{hiveSectionId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.OK, Description = "Returns a hive section.", Type = typeof(HiveSection))]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> GetHiveSectionAsync(int hiveSectionId)
        {
            var hive = await _hiveSectionService.GetHiveSectionAsync(hiveSectionId);
            return Ok(hive);
        }

        /// <summary>
        /// Set status hive section async
        /// </summary>
        /// <param name="hiveSectionId">id hive section</param>
        /// <param name="deletedStatus">bool status isDelete</param>
        /// <returns>Task{IHttpActionResult}</returns>
        [HttpPut]
        [Route("{hiveSectionId:int:min(1)}/status/{deletedStatus:bool}")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Sets deleted status for an existed hive section.")]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async Task<IHttpActionResult> SetStatusAsync([FromUri] int? hiveSectionId, [FromUri] bool deletedStatus)
        {
            if (hiveSectionId == null)
                return BadRequest(Resources.BadCreateRequest);

            try
            {
                await _hiveSectionService.SetStatusAsync(hiveSectionId.Value, deletedStatus);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
            }
            catch (RequestedResourceNotFoundException e)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Add new hive section
        /// </summary>
        /// <param name="hiveId">hives id</param>
        /// <param name="updateRequest">hives section model</param>
        /// <returns>Task{IHttpActionResult}</returns>
        [HttpPost]
        [Route("add/{hiveId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.Created, "Successful create hive section", typeof(HiveSection))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Data for create is not valid")]
        [SwaggerResponse(HttpStatusCode.Conflict, "Conflict on the server")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Other errors")]
        public async Task<IHttpActionResult> AddSectionAsync([FromUri] int? hiveId, [FromBody] UpdateHiveSectionRequest updateRequest)
        {
            if (hiveId == null || updateRequest == null)
                return BadRequest();

            try
            {
                var hiveSection = await _hiveSectionService.CreateHiveSectionAsync(hiveId.Value, updateRequest);

                var location = $"/api/sections/{hiveSection.Id}";

                return Created<HiveSection>(location, hiveSection);
            }
            catch (RequestedResourceHasConflictException e)
            {
                return Conflict();
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Update exist hive section
        /// </summary>
        /// <param name="hiveSectionId">hives section id</param>
        /// <param name="updateRequest">hives section model for update</param>
        /// <returns>Task{IHttpActionResult}</returns>
        [HttpPut]
        [Route("update/{hiveSectionId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.NoContent, "Update is succesfully")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Data for update is not valid")]
        [SwaggerResponse(HttpStatusCode.Conflict, "Conflict on the server")]
        [SwaggerResponse(HttpStatusCode.NotFound, "Hive section for update not found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Other errors")]
        public async Task<IHttpActionResult> UpdateSectionAsync([FromUri] int? hiveSectionId, [FromBody] UpdateHiveSectionRequest updateRequest)
        {
            if (hiveSectionId == null || updateRequest == null || !ModelState.IsValid)
                return BadRequest();

            try
            {
                await _hiveSectionService.UpdateHiveSectionAsync(hiveSectionId.Value, updateRequest);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
            }
            catch (RequestedResourceHasConflictException e)
            {
                return Conflict();
            }
            catch (RequestedResourceNotFoundException e)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// Delete hive section
        /// </summary>
        /// <param name="hiveSectionId">hives section id</param>
        /// <returns>Task{IHttpActionResult}</returns>
        [HttpDelete]
        [Route("{hiveSectionId:int:min(1)}")]
        [SwaggerResponse(HttpStatusCode.NoContent, "Delete is succesfully")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Data for delete is not valid")]
        [SwaggerResponse(HttpStatusCode.Conflict, "Conflict on the server")]
        [SwaggerResponse(HttpStatusCode.NotFound, "Hive section for delete not found")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Other errors")]
        public async Task<IHttpActionResult> DeleteSectionAsync([FromUri] int? hiveSectionId)
        {
            if (hiveSectionId == null)
                return BadRequest();

            try
            {
                await _hiveSectionService.DeleteHiveSectionAsync(hiveSectionId.Value);

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NoContent));
            }
            catch (RequestedResourceNotFoundException e)
            {
                return NotFound();
            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
    }
}
