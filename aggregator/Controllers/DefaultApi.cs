/*
 * IoT Aggregator API
 *
 * Sample API for aggregating data from multiple IoT devices and returning stored running averages.
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
using aggregator.Models;
using MicroLib;
using Microsoft.Extensions.Logging;
using ServiceClients;
using Microsoft.Rest;
using Polly;

namespace aggregator.Controllers
{


    /// <summary>
    /// 
    /// </summary>
    public class DefaultApiController : Controller
    {
        private readonly IStore store;
        private readonly ILogger logger;
        private readonly IHistorian historian;


        public DefaultApiController(IStore store, ILogger<DefaultApiController> logger, IHistorian historian)
        {
            this.store = store;
            this.logger = logger;
            this.historian = historian;
        }

        /// <summary>
        /// Add data generated from a device to the aggregator
        /// </summary>
        /// <remarks>Adds a data point from an IoT device. The aggregator selects the historian service, posts data to it, and receives the running average. Then updates its store for the history of running averages by device id and type.</remarks>
        /// <param name="deviceType">Device type</param>
        /// <param name="deviceId">Device ID</param>
        /// <param name="dataPointId">Each data point needs to have a unique ID</param>
        /// <param name="value">Value registered by the device.</param>
        /// <response code="201">Data added successfully.</response>
        /// <response code="401">Invalid input parameter.</response>
        /// <response code="500">An unexpected error occurred.</response>
        [HttpPost]
        [Route("/v1/deviceData/{deviceType}/{deviceId}")]
        [SwaggerOperation("AddDeviceData")]
        public virtual IActionResult AddDeviceData([FromRoute]string deviceType, [FromRoute]string deviceId, [FromQuery]string dataPointId, [FromQuery]float? value)
        {
            if (!deviceType.Equals("TEMP"))
         {
             this.logger.LogError($"Device type {deviceType} is not supported.");
             return BadRequest($"Unsupported device type {deviceType}");
         }
         float? averageValue = default(float?);

         var retryPolicy = Policy
             .Handle<HttpOperationException>()
             .WaitAndRetry(5, retryAttempt =>
                 TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
             );

         averageValue = retryPolicy.Execute(() =>
            (float?)this.historian.AddDeviceData(deviceId, dataPointId, 
                                                 DateTimeOffset.UtcNow.DateTime, value));

         if (!averageValue.HasValue)
         {
             var message = $"Cannot calculate the average.";
             this.logger.LogError(message);
             return BadRequest(message);
         }

         var key = $"{deviceType};{deviceId}";
         if (this.store.Exists(key))
         {
             this.logger.LogInformation($"Updating {key} with {averageValue.Value}");
             this.store.Update(key, averageValue.Value);
         }
         else
         {
             this.logger.LogInformation($"Added {key} with {averageValue.Value}");
             this.store.Add(key, averageValue.Value);
         }

         return Ok(averageValue.Value);
        }


        /// <summary>
        /// Get the running averages of a device type given a date range.
        /// </summary>
        /// <remarks>Returns the running average of a device type given a date range, averaged by the minute.</remarks>
        /// <param name="deviceType">Device type</param>
        /// <param name="fromTime">Start of the date range.</param>
        /// <param name="toTime">End of the date range.</param>
        /// <response code="200">Running averages per minute</response>
        /// <response code="400">Invalid input parameter.</response>
        /// <response code="500">An unexpected error occurred.</response>
        [HttpGet]
        [Route("/v1/averageByDeviceType/{deviceType}")]
        [SwaggerOperation("AverageByDeviceTypeDeviceTypeGet")]
        [SwaggerResponse(200, type: typeof(DeviceDataPoints))]
        public virtual IActionResult AverageByDeviceTypeDeviceTypeGet([FromRoute]string deviceType, [FromQuery]DateTime? fromTime, [FromQuery]DateTime? toTime)
        {
            string exampleJson = null;

            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<DeviceDataPoints>(exampleJson)
            : default(DeviceDataPoints);
            return new ObjectResult(example);
        }
    }
}
