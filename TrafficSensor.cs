using System.Globalization;

namespace ReadersWriters;

public struct TrafficSensor(string sensorOwner, double data)
{
    public string SensorOwner { get; } = sensorOwner;
    public double CurData { get; private set; } = data;

    public List<string> SensorHistory { get; } = [];

    private void IncomingData(double amount, TrafficSensor from)
    {
        CurData += amount;
        SensorHistory.Add($"Data transfer from {from.SensorOwner}, {amount.ToString(CultureInfo.CreateSpecificCulture("de-AT"))}");
    }

    public void OutgoingData(double amount, TrafficSensor to)
    {
        CurData -= amount;
        SensorHistory.Add($"Data transfer to {to.SensorOwner}, {amount}");
        to.IncomingData(amount, this);
    }
}
