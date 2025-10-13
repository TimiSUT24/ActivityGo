export default function OccurrenceCard({ item, onBook }) {
  const start = new Date(item.startUtc);
  const end = new Date(item.endUtc);
  const cap = item.effectiveCapacity ?? item.capacity ?? 0;
  const isOutdoor = item.environment === 1;

  const title = item.activityName ?? item.activity ?? item.name ?? "Activity";
  const place = item.placeName ?? item.place ?? "Unknown";

  return (
    <article className="occurrence-card">
      <header className="occurrence-card-header">
        <div>
          <h3 className="occurrence-card-title">{title}</h3>
          <p className="occurrence-card-place">{place}</p>
        </div>
        <span
          className={`occurrence-badge ${
            isOutdoor ? "occurrence-badge-outdoor" : "occurrence-badge-indoor"
          }`}
        >
          {isOutdoor ? "Outdoor" : "Indoor"}
        </span>
      </header>

      <div className="occurrence-row">
        <span>Time:</span>
        <span>
          {start.toLocaleString()} - {end.toLocaleString()}
        </span>
      </div>

      <div className="occurrence-row">
        <span>Capacity:</span>
        <span>{cap}</span>
      </div>

      {isOutdoor && item.weatherForecast && (
        <div className="occurrence-weather">
          🌡 {item.weatherForecast.temperatureC}°C · 💨{" "}
          {item.weatherForecast.windSpeedMs} m/s · ☔{" "}
          {item.weatherForecast.rainVolumeMm} mm
        </div>
      )}

      <button
        className="occurrence-button"
        onClick={() => onBook?.(item.id)}
        disabled={cap <= 0}
      >
        Book
      </button>
    </article>
  );
}
