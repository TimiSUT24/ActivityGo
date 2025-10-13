import { useState } from "react";

export default function BookingModal({ open, onClose, onConfirm }) {
  const [people, setPeople] = useState(1);
  if (!open) return null;

  return (
    <div className="occurrence-modal-overlay" role="dialog" aria-modal="true">
      <div className="occurrence-modal brick-frame">
        <div className="occurrence-modal-header mario-modal__header">
          <h3 className="mario-modal__title">
            <img
              src="/IMG/Mario-Mushroom-Step-10.webp"
              alt=""
              width={18}
              height={18}
              className="mario-icon"
            />
            Boka aktivitet
          </h3>
          <button
            type="button"
            className="occurrence-modal-close mario-modal__close"
            aria-label="Stäng"
            onClick={onClose}
          >
            ×
          </button>
        </div>

        <div className="occurrence-modal-body">
          <label className="occurrence-modal-field">
            <span className="mario-label">Antal personer</span>
            <input
              className="m-input"
              type="number"
              min={1}
              value={people}
              onChange={(e) =>
                setPeople(Math.max(1, parseInt(e.target.value || "1", 10)))
              }
            />
          </label>
        </div>

        <div className="occurrence-modal-footer">
          <button
            type="button"
            className="m-btn m-btn--ghost"
            onClick={onClose}
          >
            Avbryt
          </button>
          <button
            type="button"
            className="m-btn m-btn--primary"
            onClick={() => onConfirm(people)}
          >
            Bekräfta
          </button>
        </div>
      </div>
    </div>
  );
}
