﻿using System.Web.Http;
using NuClear.ValidationRules.Hosting.Common;
using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.SingleCheck;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Controllers
{
    [RoutePrefix("api/Single")]
    public class SingleController : ApiController
    {
        private readonly ValidationResultFactory _factory;
        private readonly PipelineFactory _pipelineFactory;
        private readonly VersionHelper _versionHelper;

        public SingleController(ValidationResultFactory factory, PipelineFactory pipelineFactory, VersionHelper versionHelper)
        {
            _factory = factory;
            _pipelineFactory = pipelineFactory;
            _versionHelper = versionHelper;
        }

        [Route(""), HttpPost]
        public IHttpActionResult Post([FromBody] ApiRequest request)
        {
            var pipeline = _pipelineFactory.Create(_versionHelper.Version);
            var validationResults = pipeline.Execute(CheckMode.Single, request.OrderId);
            var result = _factory.GetValidationResult(CheckMode.Single, validationResults);
            return Ok(result);
        }

        public class ApiRequest
        {
            public long OrderId { get; set; }
        }
    }
}
