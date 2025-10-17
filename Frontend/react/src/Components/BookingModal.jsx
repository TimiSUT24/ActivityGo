import { useState } from "react";

export default function BookingModal({ open, onClose, onConfirm }) {
  const [people, setPeople] = useState(1);
  const [phase, setPhase] = useState("form"); // form | saving | success | error
  const [result, setResult] = useState(null);
  const [err, setErr] = useState("");

  if (!open) return null;

  const submit = async () => {
    setPhase("saving");
    setErr("");
    const r = await onConfirm(people);
    if (r?.ok) {
      setResult(r.data ?? null);
      setPhase("success");
    } else {
      setResult({ errors: r?.errors || [] });
      setErr(r?.error || "Kunde inte genomföra bokningen");
      setPhase("error");
    }
  };

  const closeAll = () => {
    setPhase("form");
    setResult(null);
    setErr("");
    onClose();
  };

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
            {phase === "success" ? "Bokning bekräftad" : "Boka aktivitet"}
          </h3>
          <button
            type="button"
            className="occurrence-modal-close mario-modal__close"
            aria-label="Stäng"
            onClick={closeAll}
          >
            ×
          </button>
        </div>

        {/* FORM */}
        {phase === "form" && (
          <>
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
                onClick={closeAll}
              >
                Avbryt
              </button>
              <button
                type="button"
                className="m-btn m-btn--primary"
                onClick={submit}
              >
                Bekräfta
              </button>
            </div>
          </>
        )}

        {/* SAVING */}
        {phase === "saving" && (
          <div className="occurrence-modal-body">
            <div className="mario-alert">
              <span>Lagrar bokning…</span>
            </div>
          </div>
        )}

        {/* SUCCESS */}
        {phase === "success" && (
          <>
            <div className="occurrence-modal-body">
              <div className="mario-alert mario-alert--success">
                <img
                  src="/IMG/icons8-pixel-star-48.png"
                  alt=""
                  width={18}
                  height={18}
                  className="mario-icon"
                />
                <strong style={{ marginLeft: 8 }}>
                  Bokningen är bekräftad
                </strong>
              </div>
              {result && (
                <div className="mario-meta" style={{ marginTop: 12 }}>
                  {result.activityName && (
                    <span>
                      Aktivitet: <strong>{result.activityName}</strong>
                    </span>
                  )}
                  <br />
                  {result.startUtc && (
                    <span>
                      Tid:{" "}
                      <strong>
                        {new Date(result.startUtc).toLocaleString()}
                      </strong>
                    </span>
                  )}
                </div>
              )}
            </div>
            <div className="occurrence-modal-footer">
              <button
                type="button"
                className="m-btn m-btn--primary"
                onClick={closeAll}
              >
                Klar
              </button>
            </div>
          </>
        )}

        {/* ERROR */}
        {phase === "error" && (
          <>
            <div className="occurrence-modal-body">
              <div className="mario-alert mario-alert--error">
                <span>{err}</span>
              </div>
              {Array.isArray(result?.errors) && result.errors.length > 0 && (
                <ul className="mario-list" style={{ marginTop: 8 }}>
                  {result.errors.map((x, i) => (
                    <li key={i}>{x}</li>
                  ))}
                </ul>
              )}
            </div>
            <div className="occurrence-modal-footer">
              <button
                type="button"
                className="m-btn m-btn--ghost"
                onClick={closeAll}
              >
                Stäng
              </button>
              <button
                type="button"
                className="m-btn m-btn--primary"
                onClick={() => setPhase("form")}
              >
                Försök igen
              </button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
