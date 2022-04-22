using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/vouchers")]
public class VoucherController : ControllerBase
{
    private readonly IDbConnection _connection;
    private readonly IVoucherService _voucherService;
    private readonly ILogger<VoucherController> _logger;

    public VoucherController(IDbConnection connection,
        IVoucherService voucherService, ILogger<VoucherController> logger)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _voucherService = voucherService ?? throw new ArgumentNullException(nameof(voucherService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetVoucher(int id)
    {
        _logger.LogInformation("Getting appointment");

        var appointment = await _connection.QueryFirstOrDefaultAsync<Appointment>(@"
            select AppointmentId, OwnerId, PetId, CategoryId, Hour, Date, State
            from dbo.Appointments
            where AppointmentId = @AppointmentId", new { AppointmentId = id });

        _logger.LogInformation("Appointment retrieved");

        _logger.LogInformation("Create pdf voucher");

        var pdfFile = _voucherService.GenerateVoucher("<h1>Cita agendada satisfactoriamente.</h1>", $@"
            <p>Estimado cliente,</p>
            <p>Le informamos que su cita ha sido agendada con éxito.</p>
            <strong>Fecha: {appointment.Date.ToString("dd/MM/yyyy")}</strong></br>
            <strong>Hora: {appointment.Hour.ToFormatDisplaySchedule()}</strong>
            <p>Atentamente,</p>
            <strong><i>El equipo de Health Pet :)</i></strong>
        ");

        _logger.LogInformation("Voucher created");

        return File(pdfFile, "application/octet-stream", "health-pet.pdf");
    }
}
