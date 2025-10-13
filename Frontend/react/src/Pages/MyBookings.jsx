// src/pages/MyBookings.jsx
import React, { useEffect, useMemo, useState } from "react";
import { useAuth } from "../context/AuthContext";
import api from "../lib/api"; // ⬅️ använd din axios-instans

const CANCEL_CUTOFF_MIN = 120;

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

const fmt = (iso) =>
  iso ? new Intl.DateTimeFormat("sv-SE", { dateStyle: "medium", timeStyle: "short" }).format(new Date(iso)) : "";

const minutesUntil = (isoUtc) => {
  if (!isoUtc) return -99999;
  const start = new Date(isoUtc).getTime();
  return Math.round((start - Date.now()) / 60000);
};

function StatusPill({ status }) {
  const s = (status || "").toLowerCase();
  if (s === "booked") return <span style={styles.badgeStatus("#7fe18a", "#0b5c33", "#064b2d")}>Bokad</span>;
  if (s === "cancelled") return <span style={styles.badgeStatus("#ff9aa2", "#7a101b", "#5a0a12")}>Avbokad</span>;
  return <span style={styles.badgeStatus("#9ec1ff", "#1f3fff", "#06233d")}>Klar</span>;
}

function BookingCard({ b, onCancel, cancelling }) {
  const start = b?.activityOccurrence?.startUtc;
  const end = b?.activityOccurrence?.endUtc;
  const actName = b?.activityOccurrence?.activity?.name ?? "Aktivitet";
  const placeName = b?.activityOccurrence?.place?.name ?? "Plats";
  const status = b?.status;
  const minsLeft = minutesUntil(start);
  const canCancel = (status || "").toLowerCase() === "booked" && minsLeft > CANCEL_CUTOFF_MIN;

  return (
    <div style={styles.card}>
      <StatusPill status={status} />
      <div style={styles.cardTitle}>{actName}</div>
      <div style={styles.row}><span style={styles.subtle}>Plats</span><span>{placeName}</span></div>
      <div style={styles.row}><span style={styles.subtle}>Start</span><span>{fmt(start)}</span></div>
      <div style={styles.row}><span style={styles.subtle}>Slut</span><span>{fmt(end)}</span></div>
      <div style={styles.row}><span style={styles.subtle}>Bokad</span><span>{fmt(b?.bookedAtUtc)}</span></div>
      {b?.cancelledAtUtc && <div style={styles.row}><span style={styles.subtle}>Avbokad</span><span>{fmt(b.cancelledAtUtc)}</span></div>}
      <div style={styles.footerBtns}>
        <button
          style={canCancel ? styles.dangerBtn : styles.ghostBtn}
          disabled={!canCancel || cancelling}
          onClick={() => onCancel(b)}
          title={canCancel ? "Avboka" : "Avbokning spärrad (cutoff 120 min före start eller ej bokad)"}
        >
          {cancelling ? "Avbokar…" : "Avboka"}
        </button>
      </div>
    </div>
  );
}

export default function MyBookings() {
  const { user, ready, logout } = useAuth(); // ⬅️ från din AuthContext

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

  // Hämta bokningar via axios-instansen (api har redan Bearer-token via setAccessToken)
  async function load() {
    setError(""); setLoading(true);
    try {
      const { data } = await api.get("/api/booking/me", {
        params: scope === "all" ? undefined : { scope },
      });
      setBookings(Array.isArray(data) ? data : []);
    } catch (e) {
      const status = e?.response?.status;
      const msg = e?.response?.data ?? e?.message ?? "Något gick fel.";
      if (status === 401) {
        setError("Du är inte inloggad eller din session har gått ut.");
        // valfritt: auto-logout
        try { logout?.(); } catch {}
      } else {
        setError(typeof msg === "string" ? msg : "Fel vid hämtning.");
      }
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => { if (ready && user) load(); }, [ready, user, scope]); // vänta på ready + user

  async function handleCancel(b) {
    if (!confirm(`Avboka "${b?.activityOccurrence?.activity?.name}"?`)) return;
    setError(""); setCancellingId(b.id);
    try {
      const res = await api.delete(`/api/booking/${b.id}`);
      if (res.status === 204) {
        setBookings((prev) => prev.filter((x) => x.id !== b.id));
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

  // Enkel guarding mot crash:
  if (!ready) {
    return <div style={styles.loader}>Initierar…</div>;
  }
  if (!user) {
    return (
      <div style={styles.form}>
        <div style={styles.badge}>Mina bokningar</div>
        <h2 style={styles.title}>Användarsida · Bokningar</h2>
        <div style={styles.error}>Du måste vara inloggad för att se dina bokningar.</div>
      </div>
    );
  }

  return (
    <div style={styles.form}>
      <div style={styles.badge}>Mina bokningar</div>
      <h2 style={styles.title}>Användarsida · Bokningar</h2>

      <div style={styles.tabs}>
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
          <BookingCard key={b.id} b={b} onCancel={handleCancel} cancelling={cancellingId === b.id} />
        ))}
      </div>
    </div>
  );
}
