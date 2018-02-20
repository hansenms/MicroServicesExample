/*
 * IoT Historian API
 *
 * Sample API for keeping a history of IoT devices.
 *
 * OpenAPI spec version: 1.0.0
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using historian.Models;
using MicroLib;
using Microsoft.Extensions.Logging;

namespace historian.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    public class DefaultApiController : Controller
    { 

        private readonly IStore store;
        private readonly ILogger logger;

        public DefaultApiController(IStore store, ILogger<DefaultApiController> logger)
        {
            this.store = store;
            this.logger = logger;
        }
        
        /// <summary>
        /// Add data to the device history
        /// </summary>
        /// <remarks>Adds a data point from an IoT device. Once saved, calculates the running average of the existing data, saves it idempotentently and returns it.</remarks>
        /// <param name="deviceId">Device Id</param>
        /// <param name="datapointId">Each data point needs to have a unique ID</param>
        /// <param name="timestamp">Timestamp when received from the device.</param>
        /// <param name="value">Value registered by the device.</param>
        /// <response code="201">Data added successfully.</response>
        /// <response code="400">Invalid input parameter.</response>
        /// <response code="500">An unexpected error occurred.</response>
        [HttpPost]
        [Route("/v1/deviceData/{deviceId}")]
        [SwaggerOperation("AddDeviceData")]
        [SwaggerResponse(200, type: typeof(float?))]
        public virtual IActionResult AddDeviceData([FromRoute]string deviceId, [FromQuery]string datapointId, [FromQuery]DateTime? timestamp, [FromQuery]float? value)
        { 
            string exampleJson = null;
            
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<float?>(exampleJson)
            : default(float?);
            return new ObjectResult(example);
        }
    }
}
