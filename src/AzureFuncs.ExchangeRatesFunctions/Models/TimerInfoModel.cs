namespace AzureFuncs.ExchangeRatesFunctions.Models;
public record class ScheduleStatus(DateTime Last, DateTime Next, DateTime LastUpdated);
public record class TimerInfoModel(ScheduleStatus ScheduleStatus, bool IsPastDue);