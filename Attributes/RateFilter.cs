﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Core.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using AzureNamingTool.Models;
using AzureNamingTool.Services;

namespace AzureNamingTool.Attributes
{
    public class RateFilter : Attribute, IResourceFilter
    {
        private const string EndPoint = "YourEndPoint";

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            try
            {
                if (!context.HttpContext.Request.Path.Value.Contains(EndPoint))
                {
                    throw new Exception($"This filter is intended to be used only on a specific end point '{EndPoint}' while it's being called from '{context.HttpContext.Request.Path.Value}'");
                }

                var minRequestRateFeature = context.HttpContext.Features.Get<IHttpMinRequestBodyDataRateFeature>();
                var minResponseRateFeature = context.HttpContext.Features.Get<IHttpMinResponseDataRateFeature>();
                //Default Bytes/s = 240, Default TimeOut = 5s

                if (minRequestRateFeature != null)
                {
                    minRequestRateFeature.MinDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                }

                if (minResponseRateFeature != null)
                {
                    minResponseRateFeature.MinDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                }
            }
            catch (Exception ex)
            {
                AdminLogService.PostItem(new AdminLogMessage() { Title = "ERROR", Message = ex.Message });
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
