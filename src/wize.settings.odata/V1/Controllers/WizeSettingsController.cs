using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using wize.common.tenancy.Interfaces;
using wize.settings.data;
using wize.settings.data.V1;
using wize.settings.data.V1.Models;

namespace wize.settings.odata.V1.Controllers
{
    [ApiVersion("1.0")]
    //[ApiVersion("1.0-beta")]
    [ODataRoutePrefix("WizeSettings")]
    public partial class WizeSettingsController : ODataController
    {
        private readonly WizeContext _context;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<WizeSettingsController> _logger;
        public WizeSettingsController(ILogger<WizeSettingsController> logger, IActionDescriptorCollectionProvider actionProvider, WizeContext context, ITenantProvider tenantProvider)
        {
            _logger = logger;
            _context = context;
            _tenantProvider = tenantProvider;
        }

        /// <summary>
        /// OData based GET operation.
        /// This method will return the requested Dataset.
        /// </summary>
        /// <returns>IQueryable of requested type.</returns>
        [HttpGet]
        [ODataRoute]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
        public virtual ActionResult<IQueryable<WizeSetting>> Get()
        {
            try
            {
                Guid? tenantId = _tenantProvider.GetTenantId();
                return Ok(_context.Set<WizeSetting>().Where(a => EF.Property<Guid>(a, "TenantId") == tenantId.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: Get():{0}", typeof(WizeSetting).Name);
                return new StatusCodeResult(500);
            }
        }


        /// <summary>
        /// OData based GET(id) operation.
        /// This method receives a key value and will return the respective record if it exists.
        /// </summary>
        /// <param name="name">Key value</param>
        /// <param name="type">Key value</param>
        /// <returns>Data model</returns>
        [ODataRoute("({name}, {type})")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual IActionResult Get([FromODataUri]string name, [FromODataUri]string type)
        {

            _logger.LogDebug("WizeSettingsController: Get(name, type) - Start");
            //_context.Set<WizeSetting>().Single(m => m.)
            var model = _context.Find<WizeSetting>(name, type);//, _tenantProvider.GetTenantId());
            //if (EF.Property<Guid>(model, "TenantId") == _tenantProvider.GetTenantId())
            //    return BadRequest();

            if (model == null)
            {
                _logger.LogDebug("WizeSettingsController: Get(name, type) - Model not found.");
                return NotFound();
            }
            _logger.LogDebug("WizeSettingsController: Get(name, type) - With Results");
            return Ok(model);
        }

        /// <summary>
        /// OData based POST operation.
        /// This method receives a model and attempts to insert that record into the appropriate datastore.
        /// </summary>
        /// <param name="model">Data model</param>
        /// <returns>Data model</returns>
        //[Authorize("add:setting")]
        //[ODataRoute]
        //[Produces("application/json")]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public virtual IActionResult Post([FromBody] WizeSetting model)
        //{
        //    _logger.LogDebug("WizeSettingsController: Post(model) - Start");
        //    if (!ModelState.IsValid)
        //    {
        //        _logger.LogDebug("WizeSettingsController: Post(model) - Model invalid.");
        //        return BadRequest(ModelState);
        //    }

        //    _context.Set<WizeSetting>().Add(model);
        //    _context.SaveChanges();

        //    _logger.LogDebug("WizeSettingsController: Post(model) - Saved");
        //    return Created(model);
        //}

        /// <summary>
        /// OData based PATCH operation.
        /// This method receives a PatchDocument and attempts to apply the specified changes.
        /// </summary>
        /// <param name="name">Key value</param>
        /// <param name="type">Key value</param>
        /// <param name="delta">Delta changeset</param>
        /// <returns>Data model</returns>
        [Authorize("update:setting")]
        [ODataRoute("({name}, {type})")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual IActionResult Patch([FromODataUri]string name, [FromODataUri]string type, Delta<WizeSetting> delta)
        {
            _logger.LogDebug("WizeSettingsController: Patch(name, type, delta) - Start");
            if (!ModelState.IsValid)
            {
                _logger.LogDebug("WizeSettingsController: Patch(name, type, delta) - Model invalid.");
                return BadRequest(ModelState);
            }

            var model = _context.Find<WizeSetting>(name, type);//, _tenantProvider.GetTenantId());

            if (model == null)
            {
                _logger.LogDebug("WizeSettingsController: Patch(name, type, delta) - Model not found.");
                return NotFound();
            }

            delta.Patch(model);
            _context.SaveChanges();

            _logger.LogDebug("WizeSettingsController: Patch(name, type, delta) - Saved");
            return Updated(model);
        }

        /// <summary>
        /// OData based PUT operation.
        /// This method receives a key value and a data model and attempts to apply the updated model to the existing record.
        /// </summary>
        /// <param name="name">Key value</param>
        /// <param name="type">Key Value</param>
        /// <param name="model">Data model</param>
        /// <returns>Data model</returns>
        [Authorize("update:setting")]
        [ODataRoute("({name}, {type})")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual IActionResult Put([FromODataUri] string name, [FromODataUri] string type, [FromBody] WizeSetting model)
        {

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var origModel = _context.Find<WizeSetting>(name, type);

                if (origModel == default)
                {
                    return NotFound();
                }
                origModel.Value = model.Value;
                //_context.Attach(model);
                _context.Update<WizeSetting>(origModel);
                //_context.Entry(model).State = EntityState.Modified;
                _context.SaveChanges();

                return Updated(origModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// OData based PUT operation.
        /// This method receives a key value and a data model and attempts to apply the updated model to the existing record.
        /// </summary>
        /// <param name="name">Key value</param>
        /// <param name="type">Key Value</param>
        /// <param name="model">Data model</param>
        /// <returns>Data model</returns>
        [Authorize("update:setting")]
        [ODataRoute]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual IActionResult Update([FromBody] List<WizeSetting> model)
        {

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                foreach (WizeSetting settings in model)
                {
                    var origModel = _context.Find<WizeSetting>(settings.Name, settings.Type);

                    if (origModel == default)
                    {
                        return NotFound();
                    }
                    origModel.Value = settings.Value;
                    //_context.Attach(model);
                    _context.Update<WizeSetting>(origModel);
                    //_context.Entry(model).State = EntityState.Modified;
                }
                _context.SaveChanges();

                return Updated(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// OData based DELETE operation.
        /// This method receives a key value and attempts to delete the appropriate record from the datastore.
        /// </summary>
        /// <param name="name">Key value</param>
        /// <param name="type">Key value</param>
        /// <returns></returns>
        [Authorize("delete:setting")]
        [ODataRoute("({name}, {type})")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual IActionResult Delete([FromODataUri] string name, [FromODataUri]string type)
        {
            var model = _context.Find<WizeSetting>(name, type);//, _tenantProvider.GetTenantId());

            if (model == null)
                return NotFound();

            _context.Remove<WizeSetting>(model);
            _context.SaveChanges();

            return NoContent();
        }
    }
}