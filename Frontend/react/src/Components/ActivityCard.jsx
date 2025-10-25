import "../CSS/Activities.css";
import "../CSS/Occurrences.css";

const formatPrice = (value) => {
  if (typeof value !== "number") return "–";
  try {
    return new Intl.NumberFormat("sv-SE", {
      style: "currency",
      currency: "SEK",
      minimumFractionDigits: 0,
    }).format(value);
  } catch {
    return `${value} kr`;
  }
};

export default function ActivityCard({
  activity,
  onSelect,
  showAction = true,
  actionLabel = "Visa tillfällen",
}) {
  if (!activity) return null;

  const isOutdoor =
    activity.environment === 1 ||
    activity.environment === "Outdoor" ||
    activity.environment === "OUTDOOR";

  const icon = isOutdoor
    ? "/IMG/icons8-pixel-star-48.png"
    : "/IMG/Mario-Mushroom-Step-10.webp";

  const envLabel = isOutdoor ? "UTOMHUS" : "INOMHUS";
  const category =
    activity.categoryName ??
    activity.category ??
    activity.categoryDisplayName ??
    "Okänd kategori";
  const duration =
    typeof activity.defaultDurationMinutes === "number"
      ? activity.defaultDurationMinutes
      : null;
  const price = formatPrice(activity.price);
  const description = (activity.description ?? "").trim();

  return (
    <article className="occurrence-card brick-frame mario-card">
      <header className="occurrence-card-header mario-card__header">
        <div className="mario-card__titlewrap">
          <h3 className="occurrence-card-title mario-title">
            <img
              src={icon}
              alt=""
              width={18}
              height={18}
              className="mario-icon"
            />
            {activity.name}
          </h3>
          {category && (
            <div className="mario-card__meta" >
              <span className="mario-chip mario-chip--category">
                Kategori: {category}
              </span>
            </div>
          )}
        </div>

        <span
          className={`mario-badge ${
            isOutdoor ? "mario-badge--out" : "mario-badge--in"
          }`}
        >
          {envLabel}
        </span>
      </header>

      {description && (
        <p className="activity-card__desc" title={description}>
          {description}
        </p>
      )}

      <div className="mario-row">
        <span className="mario-label">Kategori:</span>
        <span className="mario-value">{category}</span>
      </div>

      <div className="mario-row">
        <span className="mario-label">Miljö:</span>
        <span className="mario-value">{isOutdoor ? "Utomhus" : "Inomhus"}</span>
      </div>

      <div className="mario-row">
        <span className="mario-label">Standardtid:</span>
        <span className="mario-value">
          {typeof duration === "number" && duration > 0 ? `${duration} min` : "–"}
        </span>
      </div>

      <div className="mario-row">
        <span className="mario-label">Pris:</span>
        <span className="mario-value">{price}</span>
      </div>

      {showAction && (
        <button
          type="button"
          className="m-btn m-btn--primary mario-book"
          onClick={() => onSelect?.(activity.id)}
        >
          {actionLabel}
        </button>
      )}
    </article>
  );
}
