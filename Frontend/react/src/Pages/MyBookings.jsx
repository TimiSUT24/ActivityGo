// src/pages/MyBookings.jsx
import React, { useEffect, useMemo, useState } from "react";
import { useAuth } from "../context/AuthContext";
import api from "../lib/api";

// --- Konstanter ---
const CANCEL_CUTOFF_MIN = 120;
const STATUS = { BOOKED: 0, CANCELLED: 1, COMPLETED: 2 };

// --- Styles ---
const baseStyles = {
  form: { maxWidth: 960, margin: "40px auto", padding: 28, background: "linear-gradient(145deg, rgba(255,94,87,0.96), rgba(210,33,18,0.94))", borderRadius: 12, color: "#fff", boxShadow: "0 0 0 4px #ffd166, 0 0 0 10px rgba(10,10,10,0.85), 0 22px 30px rgba(0,0,0,0.45)", border: "4px solid #1f3fff", fontFamily: '"Press Start 2P","VT323","Courier New",monospace', letterSpacing: 0.5, position: "relative", overflow: "hidden" },
  badge: { position: "absolute", top: -22, left: "50%", transform: "translateX(-50%)", backgroundColor: "#ffd166", color: "#b3001b", padding: "8px 14px", borderRadius: 999, border: "3px solid #1f3fff", boxShadow: "0 6px 0 #1f3fff, 0 9px 16px rgba(0,0,0,0.35)", fontSize: 12, textTransform: "uppercase" },
  title: { textAlign: "center", marginBottom: 24, fontSize: 20, textShadow: "4px 4px 0 rgba(0,0,0,0.35)" },
  error: { color: "#ffef9f", backgroundColor: "rgba(0,0,0,0.35)", padding: "10px 14px", borderRadius: 8, textAlign: "center", marginBottom: 16, border: "2px dashed rgba(255,239,159,0.65)", fontSize: 12 },
  button: { padding: "12px 14px", backgroundColor: "#7fe18a", color: "#064b2d", border: "4px solid #0b5c33", borderRadius: 10, cursor: "pointer", fontWeight: "bold", textTransform: "uppercase", letterSpacing: 1, transition: "background .2s, transform .2s, box-shadow .2s", boxShadow: "0 6px 0 #0b5c33, 0 12px 18px rgba(0,0,0,0.45), inset 0 -4px 0 rgba(0,0,0,0.2)" },
};

const styles = {
  ...baseStyles,
  tabs: { display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 8, marginBottom: 18 },
  tab: (active) => ({ ...baseStyles.button, width: "100%", padding: "10px 8px", backgroundColor: active ? "#ffd166" : "#14213d", color: active ? "#5b2b00" : "#fff", border: "3px solid #ffd166", boxShadow: active ? "0 6px 0 #c49d2d" : "0 6px 0 #0b5c33" }),
  grid: { display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))", gap: 18 },
  card: { background: "#14213d", border: "3px solid #ffd166", borderRadius: 12, padding: 16, color: "#fff", boxShadow: "0 10px 18px rgba(0,0,0,0.45)", position: "relative" },
  cardTitle: { fontSize: 16, marginBottom: 10 },
  row: { display: "flex", justifyContent: "space-between", gap: 12, fontSize: 12, margin: "6px 0" },
  subtle: { color: "#ffef9f" },
  badgeStatus: (bg, border, txt) => ({ position: "absolute", top: 12, right: 12, backgroundColor: bg, color: txt, border: `3px solid ${border}`, borderRadius: 999, padding: "6px 10px", fontSize: 11, textTransform: "uppercase" }),
  dangerBtn: { ...baseStyles.button, backgroundColor: "#ff9aa2", color: "#5a0a12", border: "4px solid #7a101b", boxShadow: "0 6px 0 #7a101b, 0 12px 18px rgba(0,0,0,0.45), inset 0 -4px 0 rgba(0,0,0,0.2)" },
  ghostBtn: { ...baseStyles.button, backgroundColor: "transparent", color: "#ffef9f", border: "3px dashed #ffd166", boxShadow: "none" },
  footerBtns: { display: "flex", gap: 10, marginTop: 12 },
  loader: { textAlign: "center", padding: 18, fontSize: 12, color: "#ffef9f" },
  empty: { textAlign: "center", padding: 22, fontSize: 12, border: "2px dashed rgba(255,239,159,0.65)", borderRadius: 12, color: "#ffef9f" },
};

// --- Helpers ---
const fmt = (iso) =>
  iso ? new Intl.DateTimeFormat("sv-SE", { dateStyle: "medium", timeStyle: "short" }).format(new Date(iso)) : "—";

const minutesUntil = (isoUtc) => {
  if (!isoUtc) return null;
  const start = new Date(isoUtc).getTime();
  return Math.round((start - Date.now()) / 60000);
};

const toNumStatus = (s) => {
  if (typeof s === "number") return s;
  if (s === null || s === undefined) return -1;
  // hantera "0","1","2" säkert
  const n = Number(s);
  if (!Number.isNaN(n)) return n;
  // fallback om API någon gång skickar strängar
  const t = String(s).toLowerCase();
  if (t === "booked") return STATUS.BOOKED;
  if (t === "cancelled" || t === "canceled") return STATUS.CANCELLED;
  if (t === "completed" || t === "klar") return STATUS.COMPLETED;
  return -1;
};

const getStart = (b) => b.startUtc ?? b.StartUtc;
const getEnd   = (b) => b.endUtc   ?? b.EndUtc;

// Gör "visuellt completed" om endUtc passerat, utan att lita på backend
const coerceCompleted = (b) => {
  const end = getEnd(b);
  const s   = toNumStatus(b.status ?? b.Status);
  if (s === STATUS.BOOKED && end && new Date(end).getTime() <= Date.now()) {
    return { ...b, status: STATUS.COMPLETED, __coerced: true };
  }
  return b;
};

// stöd både camelCase (JSON default) och PascalCase (om nåt råkar läcka ut)
const g = (obj, ...keys) => keys.find((k) => obj?.[k] !== undefined && obj?.[k] !== null) ? obj[keys.find((k) => obj?.[k] !== undefined && obj?.[k] !== null)] : undefined;
const statusInt = (s) => (typeof s === "number" ? s : parseInt(s ?? -1, 10));

function StatusPill({ status }) {
  const s = statusInt(status);
  if (s === STATUS.BOOKED) return <span style={styles.badgeStatus("#7fe18a", "#0b5c33", "#064b2d")}>Bokad</span>;
  if (s === STATUS.CANCELLED) return <span style={styles.badgeStatus("#ff9aa2", "#7a101b", "#5a0a12")}>Avbokad</span>;
  if (s === STATUS.COMPLETED) return <span style={styles.badgeStatus("#9ec1ff", "#1f3fff", "#06233d")}>Klar</span>;
  return <span style={styles.badgeStatus("#ffe39e", "#c49d2d", "#5b2b00")}>Okänd</span>;
}

function BookingCard({ b, onCancel, cancelling }) {
  const start = g(b, "startUtc", "StartUtc");
  const end = g(b, "endUtc", "EndUtc");
  const actName = g(b, "activityName", "ActivityName") ?? "Aktivitet";
  const placeName = g(b, "placeName", "PlaceName") ?? "Plats";
  const bookedAt = g(b, "bookedAtUtc", "BookedAtUtc");
  const cancelledAt = g(b, "cancelledAtUtc", "CancelledAtUtc");
  const s = statusInt(g(b, "status", "Status"));

  const minsLeft = minutesUntil(start);
  const canCancel = s === STATUS.BOOKED && minsLeft !== null && minsLeft > CANCEL_CUTOFF_MIN;

  // 🧠 Lokal state för att visa förklaring bara när man klickar
  const [showReason, setShowReason] = useState(false);
  let reason = "";

  if (!canCancel) {
    if (minsLeft !== null && minsLeft <= CANCEL_CUTOFF_MIN) {
      reason = `För sent för avbokning (mindre än ${CANCEL_CUTOFF_MIN} min kvar)`;
    } else if (minsLeft === null) {
      reason = "Tiden för aktiviteten kunde inte fastställas";
    }
  }

  // 🧩 Klicklogik
  const handleClick = () => {
    if (canCancel) {
      onCancel(b);
    } else {
      setShowReason(true);
      setTimeout(() => setShowReason(false), 4000); // dölj igen efter 4 sek
    }
  };

  return (
    <div style={styles.card}>
      <StatusPill status={s} />
      <div style={styles.cardTitle}>{actName}</div>

      <div style={styles.row}><span style={styles.subtle}>Plats</span><span>{placeName}</span></div>
      <div style={styles.row}><span style={styles.subtle}>Start</span><span>{fmt(start)}</span></div>
      <div style={styles.row}><span style={styles.subtle}>Slut</span><span>{fmt(end)}</span></div>
      <div style={styles.row}><span style={styles.subtle}>Bokad</span><span>{fmt(bookedAt)}</span></div>
      {cancelledAt && (
        <div style={styles.row}>
          <span style={styles.subtle}>Avbokad</span><span>{fmt(cancelledAt)}</span>
        </div>
      )}

      {/* ✅ Visa endast om status = BOOKED */}
      {s === STATUS.BOOKED && (
        <div style={styles.footerBtns}>
          <button
            style={styles.dangerBtn}
            onClick={handleClick}
            disabled={cancelling}
          >
            {cancelling ? "Avbokar…" : "Avboka"}
          </button>
        </div>
      )}

      {/* 💬 Visa orsaken endast efter klick */}
      {showReason && reason && (
        <div
          style={{
            marginTop: 8,
            fontSize: 10,
            color: "#ffef9f",
            textAlign: "center",
            opacity: 0.9,
            transition: "opacity 0.3s ease",
          }}
        >
          {reason}
        </div>
      )}
    </div>
  );
}




export default function MyBookings() {
  const { user, ready, logout } = useAuth();

  const [scope, setScope] = useState("upcoming");
  const [bookings, setBookings] = useState([]);
  const [loading, setLoading] = useState(false);
  const [cancellingId, setCancellingId] = useState(null);
  const [error, setError] = useState("");

  const tabs = useMemo(() => [
    { key: "upcoming", label: "Kommande" },
    { key: "past", label: "Historik" },
    { key: "cancelled", label: "Avbokade" },
    { key: "all", label: "Alla" },
  ], []);

async function load() {
  setError(""); setLoading(true);
  try {
    // 1) Hämta alltid allt (ingen scope-param)
    const { data } = await api.get("/api/booking/me");

    // 2) Normalisera + “coerca” completed
    let list = (Array.isArray(data) ? data : []).map(coerceCompleted);

    // 3) Filtrera per flik
    const now = Date.now();
    if (scope === "upcoming") {
      list = list.filter((b) =>
        toNumStatus(b.status ?? b.Status) === STATUS.BOOKED &&
        getStart(b) && new Date(getStart(b)).getTime() > now
      );
    } else if (scope === "past") {
      // Historik = completed (antingen från DB eller coerce)
      list = list.filter((b) => toNumStatus(b.status ?? b.Status) === STATUS.COMPLETED);
    } else if (scope === "cancelled") {
      list = list.filter((b) => toNumStatus(b.status ?? b.Status) === STATUS.CANCELLED);
    } // scope === 'all' -> ingen extra filtrering

    // 4) Sortering: historik nyast först, annars tidigast först
    list.sort((a, b) => {
      const aTime = new Date(getStart(a) ?? 0).getTime();
      const bTime = new Date(getStart(b) ?? 0).getTime();
      return scope === "past" ? bTime - aTime : aTime - bTime;
    });

    setBookings(list);
  } catch (e) {
    const status = e?.response?.status;
    const msg = e?.response?.data ?? e?.message ?? "Något gick fel.";
    if (status === 401) {
      setError("Du är inte inloggad eller din session har gått ut.");
      try { logout?.(); } catch {}
    } else {
      setError(typeof msg === "string" ? msg : "Fel vid hämtning.");
    }
  } finally {
    setLoading(false);
  }
}


  useEffect(() => { if (ready && user) load(); }, [ready, user, scope]);

  async function handleCancel(b) {
    const actName = b.activityName ?? b.ActivityName ?? "aktiviteten";
    if (!confirm(`Avboka "${actName}"?`)) return;

    setError(""); setCancellingId(b.id ?? b.Id);
    try {
      const id = b.id ?? b.Id;
      const res = await api.delete(`/api/booking/${id}`);
      if (res.status === 204) {
        setBookings((prev) => prev.filter((x) => (x.id ?? x.Id) !== id));
      } else {
        throw new Error(`Kunde inte avboka (${res.status})`);
      }
    } catch (e) {
      const status = e?.response?.status;
      const msg = e?.response?.data ?? e?.message ?? "Kunde inte avboka.";
      if (status === 401) {
        setError("Sessionen har gått ut. Logga in igen.");
        try { logout?.(); } catch {}
      } else {
        setError(typeof msg === "string" ? msg : "Kunde inte avboka.");
      }
    } finally {
      setCancellingId(null);
    }
  }

  if (!ready) return <div style={styles.loader}>Initierar…</div>;

  if (!user) {
    return (
      <div>
        <div style={styles.form}>
          <div style={styles.badge}>Mina bokningar</div>
          <h2 style={styles.title}>Bokningar</h2>
          <div style={styles.error}>Du måste vara inloggad för att se dina bokningar.</div>
        </div>
      </div>
    );
  }

  return (
    <div>
      <style>
      {`
        @media (max-width: 435px) {
          .tabs {
            grid-template-columns: repeat(2, 1fr) !important;
          }
        }
      `}
    </style>
      <div style={styles.form}>
        <div style={styles.badge}>Mina bokningar</div>
        <h2 style={styles.title}>Bokningar</h2>

        <div className="tabs" style={styles.tabs}>
          {tabs.map((t) => (
            <button key={t.key} style={styles.tab(scope === t.key)} onClick={() => setScope(t.key)}>
              {t.label}
            </button>
          ))}
        </div>

        {error && <div style={styles.error}>{error}</div>}
        {loading && <div style={styles.loader}>Laddar bokningar…</div>}

        {!loading && bookings.length === 0 && (
          <div style={styles.empty}>
            {scope === "upcoming" && "Du har inga kommande bokningar just nu."}
            {scope === "past" && "Inga avslutade bokningar hittades."}
            {scope === "cancelled" && "Du har inga avbokade bokningar."}
            {scope === "all" && "Inga bokningar hittades."}
          </div>
        )}

        <div style={styles.grid}>
          {bookings.map((b) => (
            <BookingCard
              key={b.id ?? b.Id}
              b={b}
              onCancel={handleCancel}
              cancelling={(b.id ?? b.Id) === cancellingId}
            />
          ))}
        </div>
      </div>
    </div>
  );
}
