import { useState } from "react";
import "../CSS/Occurrences.css";

export default function ActivityOccurrencePage() {
  // State för att lagra aktivitetens filter
  const today = new Date().toISOString().slice(0, 10);
  const in7 = new Date(Date.now() + 7 * 864000000).toISOString().slice(0, 10); // 7 dagar framåt

  const [dateFrom, setDateFrom] = useState(today);
  const [dateTo, setDateTo] = useState(in7);
  const [environment, setEnvironment] = useState(""); // Tomt som standard

  const onApply = (e) => {
    e.preventDefault();
    // logik för filtrering kommer här
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

        <button type="submit" className="apply-button">
          Apply Filters
        </button>
      </form>

      <div className="occurrence-meta">Shows 0 results</div>
      <div className="occurence-grid">
        {/* Här kommer de filtrerade aktiviteterna att visas */}
        <div className="occurence-empty">No occurrences found</div>
      </div>
    </div>
  );
}
