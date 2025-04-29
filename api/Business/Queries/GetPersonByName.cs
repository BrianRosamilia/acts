using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;
using System.Net;

namespace StargateAPI.Business.Queries
{
    public class GetPersonByName : IRequest<GetPersonByNameResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class GetPersonByNameHandler : IRequestHandler<GetPersonByName, GetPersonByNameResult>
    {
        private readonly StargateContext _context;

        public GetPersonByNameHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<GetPersonByNameResult> Handle(GetPersonByName request, CancellationToken cancellationToken)
        {
            var result = new GetPersonByNameResult();

            var person = await _context.People
                .Include(p => p.AstronautDetail)
                .SingleAsync(p => p.Name == request.Name, cancellationToken);

            var personDto = new PersonAstronaut
            {
                PersonId = person.Id,
                Name = person.Name,
                CurrentRank = person.AstronautDetail?.CurrentRank,
                CurrentDutyTitle = person.AstronautDetail?.CurrentDutyTitle,
                CareerStartDate = person.AstronautDetail?.CareerStartDate,
                CareerEndDate = person.AstronautDetail?.CareerEndDate
            };

            result.Person = personDto;
            result.Success = true;

            return result;

        }
    }

    public class GetPersonByNameResult : BaseResponse
    {
        public PersonAstronaut? Person { get; set; }
    }
}
