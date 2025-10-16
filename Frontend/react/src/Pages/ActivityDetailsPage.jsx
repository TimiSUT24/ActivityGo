import { useCallback, useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import BookingModal from "../Components/BookingModal";
import OccurrenceCard from "../Components/OccurrenceCard";
import api from "../lib/api";
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

const formatDate = (iso) => {
  if (!iso) return "";
  try {
    return new Intl.DateTimeFormat("sv-SE", { dateStyle: "medium" }).format(
      new Date(iso)
    );
  } catch {
    return "";
  }
};

export default function ActivityDetailsPage() {
  const { activityId } = useParams();
  const navigate = useNavigate();

  const [activity, setActivity] = useState(null);
  const [loadingActivity, setLoadingActivity] = useState(true);
  const [activityError, setActivityError] = useState("");

  const [occurrences, setOccurrences] = useState([]);
  const [loadingOccurrences, setLoadingOccurrences] = useState(true);
  const [occurrencesError, setOccurrencesError] = useState("");
  const [daysAhead, setDaysAhead] = useState(30);

  const [modalOpen, setModalOpen] = useState(false);
  const [selectedOccurrenceId, setSelectedOccurrenceId] = useState(null);
  const [bookingMessage, setBookingMessage] = useState("");

  const loadActivity = useCallback(async () => {
    if (!activityId) return;
    setLoadingActivity(true);
    setActivityError("");
    try {
      const res = await api.get(`/api/Activity/${activityId}`);
      setActivity(res?.data ?? null);
    } catch (e) {
      console.warn("Failed to load activity:", e);
      setActivityError("Kunde inte hämta aktiviteten.");
      setActivity(null);
    } finally {
      setLoadingActivity(false);
    }
  }, [activityId]);

  const loadOccurrences = useCallback(async () => {
    if (!activityId) return;
    setLoadingOccurrences(true);
    setOccurrencesError("");
    try {
      const start = new Date();
      start.setHours(0, 0, 0, 0);
      const end = new Date(start);
      end.setDate(end.getDate() + daysAhead);

      const params = new URLSearchParams();
      params.set("fromDate", start.toISOString());
      params.set("toDate", end.toISOString());
      params.set("activityId", activityId);
      const res = await api.get(
        `/api/ActivityOccurrence/with-weather?${params.toString()}`
      );
      const payload = res?.data;
      const items = Array.isArray(payload) ? payload : payload?.items ?? [];
      setOccurrences(items);
    } catch (e) {
      console.warn("Failed to load occurrences:", e);
      setOccurrencesError("Kunde inte hämta kommande tillfällen.");
      setOccurrences([]);
    } finally {
      setLoadingOccurrences(false);
    }
  }, [activityId, daysAhead]);

  useEffect(() => {
    loadActivity();
  }, [loadActivity]);

  useEffect(() => {
    loadOccurrences();
  }, [loadOccurrences]);

  const handleBack = () => navigate("/activities");

  const handleSelectOccurrence = (id) => {
    setSelectedOccurrenceId(id);
    setModalOpen(true);
    setBookingMessage("");
  };

  const handleCloseModal = () => {
    setModalOpen(false);
    setSelectedOccurrenceId(null);
  };

  const handleConfirmBooking = async (people) => {
    if (!selectedOccurrenceId) return;
    try {
      await api.post("/api/Booking", {
        ActivityOccurrenceId: selectedOccurrenceId,
        numberOfPeople: people,
      });
      setBookingMessage("Bokningen är registrerad!");
      handleCloseModal();
      loadOccurrences();
    } catch (e) {
      console.warn("Booking failed:", e);
      setBookingMessage("Det gick inte att boka aktiviteten.");
      handleCloseModal();
    }
  };

  const dayFilters = useMemo(() => [{ key: 30, label: "30 dagar" }], []);

  const isOutdoor =
    activity?.environment === 1 ||
    activity?.environment === "Outdoor" ||
    activity?.environment === "OUTDOOR";

  const categoryName = activity?.categoryName ?? "Okänd";
  const durationLabel =
    typeof activity?.defaultDurationMinutes === "number" &&
    activity.defaultDurationMinutes > 0
      ? `${activity.defaultDurationMinutes} min`
      : "–";
  const createdLabel = activity?.createdAtUtc
    ? formatDate(activity.createdAtUtc)
    : "–";

  return (
    <div className="activity-detail">
      <button type="button" className="activities-back" onClick={handleBack}>
        ← Tillbaka till alla aktiviteter
      </button>

      {loadingActivity && (
        <div className="activities-status">Laddar aktiviteten…</div>
      )}

      {activityError && (
        <div className="activities-alert">
          <img
            src="/IMG/Mario-Mushroom-Step-10.webp"
            alt=""
            width={18}
            height={18}
            className="mario-icon"
          />
          <span>{activityError}</span>
        </div>
      )}

      {activity && (
        <section className="activity-detail__summary brick-frame">
          <div className="activity-detail__summaryhead">
            <h1 className="occurrence-title mario-page-title">
              <img
                src={
                  isOutdoor
                    ? "/IMG/icons8-pixel-star-48.png"
                    : "/IMG/Mario-Mushroom-Step-10.webp"
                }
                alt=""
                width={22}
                height={22}
                className="mario-icon"
              />
              {activity.name}
            </h1>
            <div className="activity-detail__badges">
              <span
                className={`mario-badge ${
                  isOutdoor ? "mario-badge--out" : "mario-badge--in"
                }`}
              >
                {isOutdoor ? "Utomhus" : "Inomhus"}
              </span>
              {categoryName && (
                <span className="mario-chip mario-chip--category">
                  Kategori: {categoryName}
                </span>
              )}
            </div>
          </div>

          {activity.description && (
            <p className="activity-detail__desc">{activity.description}</p>
          )}

          <div className="activity-detail__meta">
            <div className="activity-detail__row">
              <span className="activity-detail__label">Standardtid:</span>
              <span className="activity-detail__value">{durationLabel}</span>
            </div>
            <div className="activity-detail__row">
              <span className="activity-detail__label">Pris:</span>
              <span className="activity-detail__value">
                {formatPrice(activity.price)}
              </span>
            </div>
            <div className="activity-detail__row">
              <span className="activity-detail__label">Skapad:</span>
              <span className="activity-detail__value">{createdLabel}</span>
            </div>
          </div>
        </section>
      )}

      <section className="activity-detail__section">
        <h2 className="occurrence-title">Kommande tillfällen</h2>

        <div className="activity-detail__filters">
          <span className="activity-detail__filterslabel">Period:</span>
          <div className="activity-detail__chips">
            <span className="mario-chip mario-chip--filter mario-chip--active">
              30 dagar
            </span>
            <button
              type="button"
              className="mario-chip mario-chip--refresh"
              onClick={() => loadOccurrences()}
              title="Uppdatera lista"
            >
              ↻ Uppdatera
            </button>
          </div>
        </div>

        {occurrencesError && (
          <div className="activities-alert">
            <img
              src="/IMG/Mario-Mushroom-Step-10.webp"
              alt=""
              width={18}
              height={18}
              className="mario-icon"
            />
            <span>{occurrencesError}</span>
          </div>
        )}

        {loadingOccurrences && (
          <div className="activity-detail__status">Laddar tillfällen…</div>
        )}

        {!loadingOccurrences && !occurrencesError && occurrences.length === 0 && (
          <div className="activities-empty">
            Inga planerade tillfällen för denna aktivitet just nu.
          </div>
        )}

        <div className="activity-detail__list">
          {occurrences.map((occurrence) => (
            <OccurrenceCard
              key={occurrence.id}
              item={occurrence}
              onBook={handleSelectOccurrence}
            />
          ))}
        </div>
      </section>

      <BookingModal
        open={modalOpen}
        onClose={handleCloseModal}
        onConfirm={handleConfirmBooking}
      />
    </div>
  );
}
