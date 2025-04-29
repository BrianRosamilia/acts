using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AstronautDutyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AstronautDutyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetAstronautDutiesByName(string name)
        {
            try
            {
                var result = await _mediator.Send(new GetAstronautDutiesByName()
                {
                    Name = name
                });

                return this.GetResponse(result);
            }
            catch (BadHttpRequestException ex)
            {
                await _mediator.Send(new CreateLog
                {
                    LogLevel = "Warning",
                    Message = $"Bad request when retrieving astronaut information for {name}",
                    Exception = ex.ToString()
                });

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = ex.StatusCode
                });
            }
            catch (Exception ex)
            {
                await _mediator.Send(new CreateLog
                {
                    LogLevel = "Error",
                    Message = $"Error retrieving astronaut duties for {name}",
                    Exception = ex.ToString()
                });

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateAstronautDuty([FromBody] CreateAstronautDuty request)
        {
            try
            {
                var result = await _mediator.Send(request);

                await _mediator.Send(new CreateLog
                {
                    LogLevel = "Information",
                    Message = $"Successfully created astronaut duty for {request.Name}, Duty: {request.DutyTitle}, Rank: {request.Rank}"
                });

                return this.GetResponse(result);
            }
            catch (BadHttpRequestException ex)
            {
                await _mediator.Send(new CreateLog
                {
                    LogLevel = "Warning",
                    Message = $"Bad request when creating astronaut duty for {request.Name}",
                    Exception = ex.ToString()
                });

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = ex.StatusCode
                });
            }
            catch (Exception ex)
            {
                await _mediator.Send(new CreateLog
                {
                    LogLevel = "Error",
                    Message = $"Error creating astronaut duty for {request.Name}",
                    Exception = ex.ToString(),
                    AdditionalData = System.Text.Json.JsonSerializer.Serialize(request)
                });

                return this.GetResponse(new BaseResponse()
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }
    }
}