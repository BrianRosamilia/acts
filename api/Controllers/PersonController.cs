using MediatR;
using Microsoft.AspNetCore.Mvc;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Queries;
using System.Net;

namespace StargateAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PersonController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetPeople()
        {
            try
            {
                var result = await _mediator.Send(new GetPeople());

                await _mediator.Send(new CreateLog
                {
                    LogLevel = "Information",
                    Message = "Successfully retrieved all people"
                });

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                await _mediator.Send(new CreateLog
                {
                    LogLevel = "Error",
                    Message = "Error retrieving all people",
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

        [HttpGet("{name}")]
        public async Task<IActionResult> GetPersonByName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    await _mediator.Send(new CreateLog
                    {
                        LogLevel = "Warning",
                        Message = "Attempted to get person with empty name"
                    });

                    return this.GetResponse(new BaseResponse()
                    {
                        Message = "Name cannot be empty",
                        Success = false,
                        ResponseCode = (int)HttpStatusCode.BadRequest
                    });
                }

                var result = await _mediator.Send(new GetPersonByName()
                {
                    Name = name
                });

                await _mediator.Send(new CreateLog
                {
                    LogLevel = "Information",
                    Message = $"Successfully retrieved person: {name}"
                });

                return this.GetResponse(result);
            }
            catch (Exception ex)
            {
                await _mediator.Send(new CreateLog
                {
                    LogLevel = "Error",
                    Message = $"Error retrieving person: {name}",
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
        public async Task<IActionResult> CreatePerson([FromBody] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    await _mediator.Send(new CreateLog
                    {
                        LogLevel = "Warning",
                        Message = "Attempted to create person with empty name"
                    });

                    return this.GetResponse(new BaseResponse()
                    {
                        Message = "Name cannot be empty",
                        Success = false,
                        ResponseCode = (int)HttpStatusCode.BadRequest
                    });
                }

                var result = await _mediator.Send(new CreatePerson()
                {
                    Name = name
                });

                await _mediator.Send(new CreateLog
                {
                    LogLevel = "Information",
                    Message = $"Successfully created person: {name}"
                });

                return this.GetResponse(result);
            }
            catch (BadHttpRequestException ex)
            {
                await _mediator.Send(new CreateLog
                {
                    LogLevel = "Warning",
                    Message = $"Bad request when creating person: {name}",
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
                    Message = $"Error creating person: {name}",
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
    }
}