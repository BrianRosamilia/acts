using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Queries
{
    public class GetPeople : IRequest<GetPeopleResult>
    {
    }

    public class GetPeopleHandler : IRequestHandler<GetPeople, GetPeopleResult>
    {
        private readonly StargateContext _context;
        private readonly IMediator _mediator;
        
        public GetPeopleHandler(StargateContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        
        public async Task<GetPeopleResult> Handle(GetPeople request, CancellationToken cancellationToken)
        {
            var result = new GetPeopleResult();

            try
            {
                var peopleEntities = await _context.People
                    .Include(p => p.AstronautDetail)
                    .ToListAsync(cancellationToken);

                var peopleDto = peopleEntities.Select(person => new PersonAstronaut
                {
                    PersonId = person.Id,
                    Name = person.Name,
                    CurrentRank = person.AstronautDetail?.CurrentRank,
                    CurrentDutyTitle = person.AstronautDetail?.CurrentDutyTitle,
                    CareerStartDate = person.AstronautDetail?.CareerStartDate,
                    CareerEndDate = person.AstronautDetail?.CareerEndDate
                }).ToList();

                result.People = peopleDto;
                result.Success = true;
                
                int astronautCount = result.People.Count(p => p.CurrentDutyTitle != null);
                
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "An error occurred while retrieving people";
                result.ResponseCode = (int)HttpStatusCode.InternalServerError;
                
                await _mediator.Send(new CreateLog
                {
                    LogLevel = "Error",
                    Message = "Error retrieving all people",
                    Exception = ex.ToString()
                }, cancellationToken);
                
                return result;
            }
        }
    }

    public class GetPeopleResult : BaseResponse
    {
        public List<PersonAstronaut> People { get; set; } = new List<PersonAstronaut>();
    }
}
