import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import ActivityCard from "../Components/ActivityCard";
import api from "../lib/api";
import "../CSS/Activities.css";
import "../CSS/Occurrences.css";

export default function ActivitiesPage() {
  const [activities, setActivities] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    let cancelled = false;

    (async () => {
      setLoading(true);
      setError("");
      try {
        const res = await api.get("/api/Activity", {
          params: { includeInactive: false },
        });
        const payload = res?.data;
        if (!cancelled) {
          setActivities(Array.isArray(payload) ? payload : payload?.items ?? []);
        }
      } catch (e) {
        if (!cancelled) {
          console.warn("Failed to load activities:", e);
          setError("Kunde inte hämta aktiviteter just nu.");
          setActivities([]);
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, []);

  const handleSelect = (id) => {
    if (!id) return;
    navigate(`/activities/${id}`);
  };

  return (
    <div className="activities-page">
      <h1 className="mario-page-title mario-title">
        <img
          src="/IMG/icons8-pixel-star-48.png"
          alt=""
          width={22}
          height={22}
          className="m-icon"
        />
        Aktiviteter
      </h1>

      {error && (
        <div className="activities-alert">
          <img
            src="/IMG/Mario-Mushroom-Step-10.webp"
            alt=""
            width={18}
            height={18}
            className="mario-icon"
          />
          <span>{error}</span>
        </div>
      )}

      {loading && <div className="activities-status">Laddar aktiviteter…</div>}

      {!loading && !error && activities.length > 0 && (
        <div className="activities-meta">
          <img
            src="/IMG/Mario-Mushroom-Step-10.webp"
            alt=""
            width={16}
            height={16}
            className="mario-icon"
          />
          <span>Antal:</span>
          <strong>{activities.length}</strong>
        </div>
      )}

      {!loading && !error && activities.length === 0 && (
        <div className="activities-empty">
          Inga aktiviteter hittades just nu.
        </div>
      )}

      <div className="activities-grid">
        {activities.map((activity) => (
          <ActivityCard
            key={activity.id}
            activity={activity}
            onSelect={handleSelect}
          />
        ))}
      </div>
    </div>
  );
}
