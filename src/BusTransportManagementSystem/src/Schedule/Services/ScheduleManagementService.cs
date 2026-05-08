using BusTransportManagementSystem.Domain.Bus.Repositories;
using BusTransportManagementSystem.Domain.Driver.Repositories;
using BusTransportManagementSystem.Domain.Route.Repositories;
using BusTransportManagementSystem.Domain.Schedule.Repositories;
using BusTransportManagementSystem.Domain.Shared.Common;
using BusTransportManagementSystem.Domain.Shared.Services;
using BusTransportManagementSystem.Domain.Shared.ValueObjects;

namespace BusTransportManagementSystem.Domain.Schedule.Services;

public class ScheduleManagementService : DomainServiceBase
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IBusRepository _busRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IDriverRepository _driverRepository;

    public ScheduleManagementService(
        IScheduleRepository scheduleRepository,
        IBusRepository busRepository,
        IRouteRepository routeRepository,
        IDriverRepository driverRepository)
    {
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _busRepository = busRepository ?? throw new ArgumentNullException(nameof(busRepository));
        _routeRepository = routeRepository ?? throw new ArgumentNullException(nameof(routeRepository));
        _driverRepository = driverRepository ?? throw new ArgumentNullException(nameof(driverRepository));
    }

    public async Task AssignBusToRouteAsync(Guid busId, Guid routeId, ScheduledDate date, CancellationToken cancellationToken = default)
    {
        var bus = await _busRepository.GetByIdAsync(busId, cancellationToken)
            ?? throw new DomainException($"Bus with ID {busId} not found");

        var route = await _routeRepository.GetByIdAsync(routeId, cancellationToken)
            ?? throw new DomainException($"Route with ID {routeId} not found");

        var schedule = await GetOrCreateScheduleAsync(cancellationToken);

        schedule.AssignBusToRoute(busId, routeId, date, bus, route);

        await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
    }

    public async Task AssignDriverToShiftAsync(
        Guid driverId,
        Guid busId,
        Guid routeId,
        ShiftPeriod shiftPeriod,
        ScheduledDate date,
        CancellationToken cancellationToken = default)
    {
        var driver = await _driverRepository.GetByIdAsync(driverId, cancellationToken)
            ?? throw new DomainException($"Driver with ID {driverId} not found");

        var bus = await _busRepository.GetByIdAsync(busId, cancellationToken)
            ?? throw new DomainException($"Bus with ID {busId} not found");

        var route = await _routeRepository.GetByIdAsync(routeId, cancellationToken)
            ?? throw new DomainException($"Route with ID {routeId} not found");

        var schedule = await GetOrCreateScheduleAsync(cancellationToken);

        schedule.AssignDriverToShift(driverId, busId, routeId, shiftPeriod, date, driver, bus, route);

        await _scheduleRepository.UpdateAsync(schedule, cancellationToken);
    }

    private async Task<ScheduleAggregate> GetOrCreateScheduleAsync(CancellationToken cancellationToken)
    {
        // For simplicity, get the first schedule or create a new one
        var schedules = await _scheduleRepository.GetAllAsync(cancellationToken);
        var schedule = schedules.FirstOrDefault();

        if (schedule == null)
        {
            schedule = new ScheduleAggregate();
            await _scheduleRepository.AddAsync(schedule, cancellationToken);
        }

        return schedule;
    }
}
