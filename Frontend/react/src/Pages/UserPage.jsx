import React, { useEffect, useState } from "react";
import api, { setAccessToken } from "../lib/api";
import { useAuth } from "../context/AuthContext";

import MarioStyles from "../Components/mario/MarioStyles";
import Cloud from "../Components/mario/Cloud";
import Ground from "../Components/mario/Ground";

import "../CSS/UserPage.css";

function Card({ title, children, style }) {
  return (
    <div className="up-card" style={style}>
      {title && <h2 className="up-card__title">{title}</h2>}
      {children}
    </div>
  );
}

function PrimaryButton({ children, ...props }) {
  return (
    <button {...props} className="up-btn up-btn--primary">
      {children}
    </button>
  );
}
function GhostButton({ children, ...props }) {
  return (
    <button {...props} className="up-btn up-btn--ghost">
      {children}
    </button>
  );
}

function Field({ label, type = "text", value, onChange, placeholder }) {
  return (
    <label className="up-field">
      <div className="up-field__label">{label}</div>
      <input
        className="up-field__input"
        type={type}
        value={value ?? ""}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
      />
    </label>
  );
}

export default function UserPage() {
  const { user, logout, setUser } = useAuth();

  const firstName =
    user?.firstname || user?.firstName || user?.name || user?.email || "Användare";

  const [bookings, setBookings] = useState([]);
  const [loadingBookings, setLoadingBookings] = useState(true);

  const [showProfile, setShowProfile] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  // Form state (prefillas *när modalen öppnas*)
  const [pfFirst, setPfFirst] = useState("");
  const [pfLast, setPfLast] = useState("");
  const [pfEmail, setPfEmail] = useState("");
  const [savingProfile, setSavingProfile] = useState(false);

  const [curPwd, setCurPwd] = useState("");
  const [newPwd, setNewPwd] = useState("");
  const [savingPwd, setSavingPwd] = useState(false);

  const [toast, setToast] = useState(null);
  const showToast = (msg, tone = "ok") => {
    setToast({ msg, tone });
    setTimeout(() => setToast(null), 3500);
  };

  // Hämta 3 kommande bokningar
  useEffect(() => {
    let isMounted = true;
    (async () => {
      try {
        const res = await api.get("/api/Booking/me", { params: { scope: "upcoming" } });
        let list = Array.isArray(res.data) ? res.data : [];
        list = list
          .map((b) => ({
            ...b,
            start:
              b.startAt ||
              b.startUtc ||
              b.start ||
              b.activityOccurrence?.startUtc ||
              b.activityOccurrence?.startAt,
          }))
          .filter((b) => !!b.start)
          .sort((a, b) => new Date(a.start) - new Date(b.start))
          .slice(0, 3);
        if (isMounted) setBookings(list);
      } catch {
        try {
          const res2 = await api.get("/api/Booking/me");
          let list = Array.isArray(res2.data) ? res2.data : [];
          list = list
            .map((b) => ({
              ...b,
              start:
                b.startAt ||
                b.startUtc ||
                b.start ||
                b.activityOccurrence?.startUtc ||
                b.activityOccurrence?.startAt,
            }))
            .filter((b) => !!b.start && new Date(b.start) > new Date())
            .sort((a, b) => new Date(a.start) - new Date(b.start))
            .slice(0, 3);
          if (isMounted) setBookings(list);
        } catch {
          if (isMounted) setBookings([]);
        }
      } finally {
        if (isMounted) setLoadingBookings(false);
      }
    })();
    return () => {
      isMounted = false;
    };
  }, []);

  // Öppna modaler
  const openProfile = () => {
    setPfFirst(user?.firstname ?? user?.firstName ?? "");
    setPfLast(user?.lastname ?? user?.lastName ?? "");
    setPfEmail(user?.email ?? "");
    setShowProfile(true);
  };
  const openPassword = () => setShowPassword(true);

  // Spara profil
  const saveProfile = async () => {
    setSavingProfile(true);
    try {
      const body = { firstname: pfFirst, lastname: pfLast, email: pfEmail };
      const res = await api.put("/api/Auth/me/profile", body);

      if (res?.data?.accessToken) setAccessToken(res.data.accessToken);

      try {
        const me = await api.get("/api/Auth/me");
        if (typeof setUser === "function") {
          setUser({
            ...user,
            email: me.data?.email ?? pfEmail,
            firstname: pfFirst,
            lastname: pfLast,
            roles: me.data?.roles ?? user?.roles,
          });
        }
      } catch {}

      showToast("Uppgifterna är uppdaterade ✅", "ok");
      setShowProfile(false);
    } catch (err) {
      const msg = err?.response?.data?.message || "Kunde inte spara uppgifter.";
      showToast(msg, "err");
    } finally {
      setSavingProfile(false);
    }
  };

  // Byt lösenord
  const savePassword = async () => {
    if (!curPwd || !newPwd) {
      showToast("Fyll i båda fälten.", "err");
      return;
    }
    setSavingPwd(true);
    try {
      const res = await api.put("/api/Auth/me/password", {
        currentPassword: curPwd,
        newPassword: newPwd,
      });
      if (res?.data?.accessToken) setAccessToken(res.data.accessToken);
      setCurPwd("");
      setNewPwd("");
      setShowPassword(false);
      showToast("Lösenordet är uppdaterat 🔐", "ok");
    } catch (err) {
      const msg =
        err?.response?.data?.message ||
        "Kunde inte uppdatera lösenord. Kontrollera nuvarande lösenord.";
      showToast(msg, "err");
    } finally {
      setSavingPwd(false);
    }
  };

  const renderBookingLine = (b) => {
    const start = new Date(b.start);
    const time = start.toLocaleString(undefined, { dateStyle: "medium", timeStyle: "short" });
    const activity =
      b.activityName || b.activity?.name || b.activityTitle || b.title || "Aktivitet";
    const place =
      b.placeName || b.place?.name || b.location?.name || b.locationName || null;

    return (
      <div className="up-booking" key={b.id || `${activity}-${b.start}`}>
        <div className="up-booking__meta">
          <div className="up-booking__title">{activity}</div>
          <div className="up-booking__sub">
            {time}
            {place ? ` • ${place}` : ""}
          </div>
        </div>
        <div className="up-badge">Kommande</div>
      </div>
    );
  };

  return (
    <div className="up-root">
      <MarioStyles />

      <Cloud left="-10vw" duration="30s" />
      <Cloud left="-35vw" top="12vh" width={160} duration="38s" />
      <Cloud left="-55vw" top="22vh" width={110} height={55} duration="26s" />

      <div className="up-inner">
        <div className="up-header">
          <h1 className="up-title">🍄 Välkommen, {firstName}!</h1>
          <div className="up-actions">
            <GhostButton onClick={openProfile}>Ändra uppgifter</GhostButton>
            <GhostButton onClick={openPassword}>Byt lösenord</GhostButton>
            <PrimaryButton onClick={logout}>Logga ut</PrimaryButton>
          </div>
        </div>

        <Card title="Dina 3 närmsta bokningar">
          {loadingBookings ? (
            <div className="up-dim">Hämtar bokningar…</div>
          ) : bookings.length === 0 ? (
            <div className="up-dim">Inga kommande bokningar hittades.</div>
          ) : (
            bookings.map(renderBookingLine)
          )}
        </Card>

        {showProfile && (
          <div className="up-modal" role="dialog" aria-modal="true" onClick={() => setShowProfile(false)}>
            <div className="up-modal__box" onClick={(e) => e.stopPropagation()}>
              <h3 className="up-modal__title">Ändra dina uppgifter</h3>
              <Field label="Förnamn" value={pfFirst} onChange={setPfFirst} />
              <Field label="Efternamn" value={pfLast} onChange={setPfLast} />
              <Field label="E-post" type="email" value={pfEmail} onChange={setPfEmail} />
              <div className="up-modal__actions">
                <GhostButton onClick={() => setShowProfile(false)}>Avbryt</GhostButton>
                <PrimaryButton onClick={saveProfile} disabled={savingProfile}>
                  {savingProfile ? "Sparar…" : "Spara"}
                </PrimaryButton>
              </div>
            </div>
          </div>
        )}

        {showPassword && (
          <div className="up-modal" role="dialog" aria-modal="true" onClick={() => setShowPassword(false)}>
            <div className="up-modal__box" onClick={(e) => e.stopPropagation()}>
              <h3 className="up-modal__title">Byt lösenord</h3>
              <Field label="Nuvarande lösenord" type="password" value={curPwd} onChange={setCurPwd} />
              <Field label="Nytt lösenord" type="password" value={newPwd} onChange={setNewPwd} />
              <div className="up-modal__actions">
                <GhostButton onClick={() => setShowPassword(false)}>Avbryt</GhostButton>
                <PrimaryButton onClick={savePassword} disabled={savingPwd}>
                  {savingPwd ? "Sparar…" : "Spara"}
                </PrimaryButton>
              </div>
            </div>
          </div>
        )}
      </div>

      {toast && (
        <div className={`up-toast ${toast.tone === "err" ? "up-toast--err" : "up-toast--ok"}`}>
          {toast.msg}
        </div>
      )}

      <Ground />
    </div>
  );
}
