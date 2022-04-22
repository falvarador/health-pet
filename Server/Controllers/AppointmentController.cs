using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

[ApiController]
[Route("api/appointments")]
public class AppointmentController : ControllerBase
{
    private readonly IDbConnection _connection;
    private readonly IEmailService _emailService;
    private readonly ILogger<AppointmentController> _logger;

    public AppointmentController(IDbConnection connection,
        IEmailService emailService, ILogger<AppointmentController> logger)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        _logger.LogInformation("Cancelling appointment");

        var appointment = await _connection.QueryFirstOrDefaultAsync<Appointment>(@"
            select AppointmentId, OwnerId, PetId, CategoryId, Hour, Date, State
            from dbo.Appointments
            where AppointmentId = @AppointmentId", new { AppointmentId = id });

        if (appointment is null)
        {
            _logger.LogInformation("Appointment not found");
            return NotFound();
        }

        await _connection.ExecuteAsync(@"
            update dbo.Appointments
            set OwnerId = @OwnerId, PetId = @PetId, CategoryId = @CategoryId, Hour = @Hour, Date = @Date, State = @State
            where AppointmentId = @AppointmentId",
                  new
                  {
                      AppointmentId = id,
                      OwnerId = appointment.OwnerId,
                      PetId = appointment.PetId,
                      CategoryId = appointment.CategoryId,
                      Hour = appointment.Hour,
                      Date = appointment.Date,
                      State = AppointmentState.Cancelled
                  });

        _logger.LogInformation("Appointment canceled");

        return NoContent();
    }

    [HttpGet("schedules/{date}")]
    public async Task<IEnumerable<ScheduleDto>> GetSchedules(DateTime date)
    {
        _logger.LogInformation("Getting schedules");

        var schedules = await _connection.QueryAsync<ScheduleDto>(@"
            select sch.ScheduleId, sch.Schedule 
            from dbo.Schedules sch
            where sch.Schedule not in (
                select app.[Hour] from dbo.Appointments app
                where app.[Date] = @Date
                and app.[State] not in ('Cancelled', 'Missing')
            )", new { Date = date });

        _logger.LogInformation("Schedules retrieved");

        return schedules;
    }

    [HttpGet("{id:int}")]
    public async Task<Appointment> GetAppointment(int id)
    {
        _logger.LogInformation("Getting appointment");

        var appointment = await _connection.QueryFirstOrDefaultAsync<Appointment>(@"
            select AppointmentId, OwnerId, PetId, CategoryId, Hour, Date, State
            from dbo.Appointments
            where AppointmentId = @AppointmentId", new { AppointmentId = id });

        _logger.LogInformation("Appointment retrieved");

        return appointment;
    }

    [HttpPost]
    public async Task<IActionResult> AddAppointment([FromBody] AppointmentDto appointmentDto)
    {
        _logger.LogInformation("Adding appointment");

        var ownerId = await _connection.QuerySingleOrDefaultAsync<int>(@"
            select OwnerId from dbo.Owners
            where ID = @IDOwner", new { IDOwner = appointmentDto.IDOwner });

        var appointmentId = await _connection.ExecuteScalarAsync<int>(@"
            insert into dbo.Appointments (OwnerId, PetId, CategoryId, Hour, Date, State)
            values (@OwnerId, @PetId, @CategoryId, @Hour, @Date, @State)
            select cast(scope_identity() as int)",
                new AppointmentDto()
                {
                    OwnerId = ownerId,
                    CategoryId = appointmentDto.CategoryId,
                    PetId = appointmentDto.PetId,
                    Hour = appointmentDto.Hour,
                    Date = appointmentDto.Date,
                    State = AppointmentState.Confirmed
                });

        _logger.LogInformation("Appointment added");

        var email = await _connection.QuerySingleOrDefaultAsync<string>(@"
            select Email from dbo.Owners
            where ID = @IDOwner", new { IDOwner = appointmentDto.IDOwner });

        _emailService.SendEmail(email,
            "Cita agendada satisfactoriamente",
            $@"
                <p>Estimado cliente,</p>
                <p>Le informamos que su cita ha sido agendada con éxito.</p>
                <strong>Fecha: {appointmentDto.Date.ToString("dd/MM/yyyy")}</strong></br>
                <strong>Hora: {appointmentDto.Hour.ToFormatDisplaySchedule()}</strong>
                <p>Atentamente,</p>
                <strong><i>El equipo de Health Pet 🐶 🐱</i></strong>");

        return Created($"api/appointments/{appointmentId}",
            new Appointment
            {
                AppointmentId = appointmentId,
                OwnerId = ownerId,
                CategoryId = appointmentDto.CategoryId,
                PetId = appointmentDto.PetId,
                Hour = appointmentDto.Hour,
                Date = appointmentDto.Date,
                State = AppointmentState.Confirmed
            });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDto appointmentDto)
    {
        _logger.LogInformation("Updating appointment");

        var appointment = await _connection.QueryFirstOrDefaultAsync<Appointment>(@"
            select AppointmentId, OwnerId, PetId, CategoryId, Hour, Date, State
            from dbo.Appointments
            where AppointmentId = @AppointmentId", new { AppointmentId = id });

        if (appointment is null)
            return await AddAppointment(appointmentDto);

        await _connection.ExecuteAsync(@"
            update dbo.Appointments
            set OwnerId = @OwnerId, PetId = @PetId, CategoryId = @CategoryId, Hour = @Hour, Date = @Date, State = @State
            where AppointmentId = @AppointmentId",
                new Appointment
                {
                    AppointmentId = id,
                    OwnerId = appointment.OwnerId,
                    CategoryId = appointmentDto.CategoryId,
                    PetId = appointmentDto.PetId,
                    Hour = appointmentDto.Hour,
                    Date = appointmentDto.Date,
                    State = AppointmentState.Confirmed
                });

        _logger.LogInformation("Appointment updated");

        var email = await _connection.QuerySingleOrDefaultAsync<string>(@"
            select Email from dbo.Owners
            where ID = @IDOwner", new { IDOwner = appointmentDto.IDOwner });

        _emailService.SendEmail(email,
            "Cita agendada satisfactoriamente",
            $@"
                <p>Estimado cliente,</p>
                <p>Le informamos que su cita ha sido agendada con éxito.</p>
                <strong>Fecha: {appointmentDto.Date.ToString("dd/MM/yyyy")}</strong></br>
                <strong>Hora: {appointmentDto.Hour.ToFormatDisplaySchedule()}</strong>
                <p>Atentamente,</p>
                <strong><i>El equipo de Health Pet 🐶 🐱</i></strong>");

        return Ok(new Appointment
        {
            AppointmentId = id,
            OwnerId = appointmentDto.OwnerId,
            CategoryId = appointmentDto.CategoryId,
            PetId = appointmentDto.PetId,
            Hour = appointmentDto.Hour,
            Date = appointmentDto.Date,
            State = AppointmentState.Confirmed
        });
    }
}
