using MediatR;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class CreateLog : IRequest<CreateLogResult>
    {
        public string LogLevel { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string? AdditionalData { get; set; }
    }

    public class CreateLogHandler : IRequestHandler<CreateLog, CreateLogResult>
    {
        private readonly StargateContext _context;

        public CreateLogHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<CreateLogResult> Handle(CreateLog request, CancellationToken cancellationToken)
        {
            var log = new Log
            {
                LogLevel = request.LogLevel,
                Message = request.Message,
                Exception = request.Exception,
                AdditionalData = request.AdditionalData,
                Timestamp = DateTime.UtcNow
            };

            await _context.Logs.AddAsync(log, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreateLogResult
            {
                Id = log.Id,
                Success = true
            };
        }
    }

    public class CreateLogResult : BaseResponse
    {
        public int Id { get; set; }
    }
}