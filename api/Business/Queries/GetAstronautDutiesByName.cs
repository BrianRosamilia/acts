using MediatR;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Business.Dtos;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Queries
{
    public class GetAstronautDutiesByName : IRequest<GetAstronautDutiesByNameResult>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GetAstronautDutiesByNameHandler : IRequestHandler<GetAstronautDutiesByName, GetAstronautDutiesByNameResult>
    {
        private readonly StargateContext _context;
        private readonly IMediator _mediator;

        public GetAstronautDutiesByNameHandler(StargateContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<GetAstronautDutiesByNameResult> Handle(GetAstronautDutiesByName request, CancellationToken cancellationToken)
        {
            var result = new GetAstronautDutiesByNameResult();

            var person = await _context.People
                .Include(p => p.AstronautDetail)
                .FirstOrDefaultAsync(p => p.Name == request.Name, cancellationToken);

            if (person == null)
            {
                throw new BadHttpRequestException("This person does not exist.");
            }

            if (person.AstronautDetail == null)
            {
                throw new BadHttpRequestException("This person is not an astronaut.");
            }

            var personDto = new PersonAstronaut
            {
                PersonId = person.Id,
                Name = person.Name,
                CurrentRank = person.AstronautDetail.CurrentRank,
                CurrentDutyTitle = person.AstronautDetail.CurrentDutyTitle,
                CareerStartDate = person.AstronautDetail.CareerStartDate,
                CareerEndDate = person.AstronautDetail.CareerEndDate
            };

            result.Person = personDto;

            var duties = await _context.AstronautDuties
                .Where(ad => ad.PersonId == person.Id)
                .OrderByDescending(ad => ad.DutyStartDate)
                .ToListAsync(cancellationToken);

            result.AstronautDuties = duties;
            result.Success = true;

            return result;
        }
    }

    public class GetAstronautDutiesByNameResult : BaseResponse
    {
        public PersonAstronaut Person { get; set; }
        public List<AstronautDuty> AstronautDuties { get; set; } = new List<AstronautDuty>();
    }
}
