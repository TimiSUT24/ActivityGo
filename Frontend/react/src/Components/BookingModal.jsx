import { useState } from "react";

export default function BookingModal({ open, onClose, onConfirm }) {
  const [people, setPeople] = useState(1);
  if (!open) return null;

  return (
    <div className="occurrence-modal-overlay">
      <div className="occurrence-modal">
        <div className="occurrence-modal-header">
          <h3>Book Activity</h3>
          <button
            type="button"
            className="occurrence-modal-close"
            onClick={onClose}
            aria-label="Close"
          >
            x
          </button>
        </div>

        <div className="occurrence-modal-body">
          <label className="occurrence-modal-field">
            <span>Number of People:</span>
            <input
              type="number"
              min={1}
              value={people}
              onChange={(e) => setPeople(parseInt(e.target.value || "1", 10))}
            />
          </label>
        </div>

        <div className="occurrence-modal-footer">
          <button
            type="button"
            className="occurrence-button-ghost"
            onClick={onClose}
          >
            Cancel
          </button>
          <button
            type="button"
            className="occurrence-button"
            onClick={() => onConfirm(people)}
          >
            Confirm
          </button>
        </div>
      </div>
    </div>
  );
}
