import { useEffect, useState } from "react";
import occurrenceService from "../Services/occurrenceService.js";
import OccurrenceCard from "../Components/OccurrenceCard.jsx";
import "../CSS/Occurrences.css";

export default function ActivityOccurrencePage() {
  const today = new Date().toISOString().slice(0, 10);
  const in7 = new Date(Date.now() + 7 * 86400000).toISOString().slice(0, 10);

  const [dateFrom, setDateFrom] = useState(today);
  const [dateTo, setDateTo] = useState(in7);
  const [environment, setEnvironment] = useState("");

  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(false);
  const [err, setErr] = useState("");

  const fetchOccurrences = async () => {
    setLoading(true);
    setErr("");
    try {
      const data = await occurrenceService.list({
        dateFrom,
        dateTo,
        environment: environment === "" ? undefined : Number(environment),
      });
      console.log(
        "[occurrences] rows",
        Array.isArray(data) ? data.length : data
      );
      setItems(Array.isArray(data) ? data : []);
    } catch (e) {
      console.error("[occurrence] fetch error", e);
      setErr(
        e?.response?.data?.message ||
          e?.message ||
          "Failed to fetch occurrences"
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOccurrences();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const onApply = (e) => {
    e.preventDefault();
    fetchOccurrences();
  };

  // placeholder tills bokningsflödet kopplas
  const handleBook = (id) => {
    // TODO: koppla POST /api/bookings
    alert(`Book occurrence: ${id}`);
  };

  return (
    <div className="occurrence-page">
      <h1 className="occurrence-title">Activity Occurrences</h1>

      <form className="occurence-filters" onSubmit={onApply}>
        <label className="occurence-field">
          <span>From Date:</span>
          <input
            type="date"
            value={dateFrom}
            onChange={(e) => setDateFrom(e.target.value)}
          />
        </label>

        <label className="occurence-field">
          <span>To Date:</span>
          <input
            type="date"
            value={dateTo}
            onChange={(e) => setDateTo(e.target.value)}
          />
        </label>

        <label className="occurence-field">
          <span>Environment:</span>
          <select
            value={environment}
            onChange={(e) => setEnvironment(e.target.value)}
          >
            <option value="">All</option>
            <option value="0">Indoor</option>
            <option value="1">Outdoor</option>
          </select>
        </label>

        <button type="submit" className="occurrence-button">
          Let's a go!
        </button>
      </form>

      {loading && <div className="occurrence-status">Loading...</div>}
      {err && <div className="occurrence-status error">{err}</div>}

      <div className="occurrence-meta">Shows {items.length} results</div>

      <div className="occurence-grid">
        {items.length === 0 && !loading && !err && (
          <div className="occurence-empty">No occurrences found.</div>
        )}

        {items.map((x) => (
          <OccurrenceCard key={x.id} item={x} onBook={handleBook} />
        ))}
      </div>
    </div>
  );
}
