import { useEffect, useRef, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { useLocation, useNavigate } from "react-router-dom";

export default function LoginForm() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from?.pathname || "/user";

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [coins, setCoins] = useState([]);
  const [marioJump, setMarioJump] = useState(false);
  const marioTimerRef = useRef(null);

  const styles = {
    form: {
      maxWidth: 360,
      margin: "40px auto",
      padding: 28,
      background:
        "linear-gradient(145deg, rgba(255,94,87,0.96), rgba(210,33,18,0.94))",
      borderRadius: 12,
      color: "#fff",
      boxShadow:
        "0 0 0 4px #ffd166, 0 0 0 10px rgba(10,10,10,0.85), 0 22px 30px rgba(0,0,0,0.45)",
      border: "4px solid #1f3fff",
      fontFamily: '"Press Start 2P", "VT323", "Courier New", monospace',
      letterSpacing: 0.5,
      position: "relative",
      overflow: "hidden",
    },
    badge: {
      position: "absolute",
      top: -22,
      left: "50%",
      transform: "translateX(-50%)",
      backgroundColor: "#ffd166",
      color: "#b3001b",
      padding: "8px 14px",
      borderRadius: 999,
      border: "3px solid #1f3fff",
      boxShadow: "0 6px 0 #1f3fff, 0 9px 16px rgba(0,0,0,0.35)",
      fontSize: 12,
      textTransform: "uppercase",
    },
    title: {
      textAlign: "center",
      marginBottom: 24,
      fontSize: 20,
      textShadow: "4px 4px 0 rgba(0,0,0,0.35)",
    },
    error: {
      color: "#ffef9f",
      backgroundColor: "rgba(0,0,0,0.35)",
      padding: "10px 14px",
      borderRadius: 8,
      textAlign: "center",
      marginBottom: 16,
      border: "2px dashed rgba(255,239,159,0.65)",
      fontSize: 12,
    },
    field: {
      marginBottom: 18,
    },
    label: {
      display: "block",
      marginBottom: 6,
      fontSize: 12,
      textTransform: "uppercase",
      color: "#ffef9f",
      textShadow: "2px 2px 0 rgba(0,0,0,0.3)",
    },
    input: {
      width: "100%",
      padding: "12px 14px",
      borderRadius: 8,
      border: "3px solid #ffd166",
      backgroundColor: "#14213d",
      color: "#fff",
      fontSize: 13,
      boxShadow: "inset 0 4px 0 rgba(0,0,0,0.3)",
      transition: "transform 0.15s ease, box-shadow 0.15s ease",
    },
    helper: {
      fontSize: 10,
      color: "rgba(122,229,143,0.9)",
      marginTop: 6,
      display: "block",
      textTransform: "uppercase",
      letterSpacing: 1,
    },
    button: {
      width: "100%",
      padding: "14px 12px",
      backgroundColor: "#7fe18a",
      color: "#064b2d",
      border: "4px solid #0b5c33",
      borderRadius: 10,
      cursor: "pointer",
      fontWeight: "bold",
      textTransform: "uppercase",
      letterSpacing: 1,
      transition:
        "background 0.2s ease-in-out, transform 0.2s ease, box-shadow 0.2s ease",
      boxShadow:
        "0 6px 0 #0b5c33, 0 12px 18px rgba(0,0,0,0.45), inset 0 -4px 0 rgba(0,0,0,0.2)",
    },
    coin: {
      position: "absolute",
      width: 28,
      height: 28,
      borderRadius: "50%",
      background:
        "radial-gradient(circle at 30% 30%, #fff9c4 0%, #ffe082 45%, #ffca28 60%, #f9a825 100%)",
      border: "2px solid rgba(255,197,54,0.85)",
      boxShadow:
        "0 0 0 3px rgba(31,63,255,0.4), 0 6px 12px rgba(0,0,0,0.35), inset -3px -3px 6px rgba(0,0,0,0.3)",
      pointerEvents: "none",
      display: "flex",
      alignItems: "center",
      justifyContent: "center",
      color: "#644117",
      fontSize: 12,
      fontWeight: "bold",
      textShadow: "1px 1px 0 rgba(0,0,0,0.25)",
    },
    marioWrap: {
      position: "absolute",
      bottom: -90,
      left: "50%",
      marginLeft: -40,
      width: 80,
      height: 120,
      pointerEvents: "none",
      filter: "drop-shadow(0 12px 16px rgba(0,0,0,0.45))",
      zIndex: 5,
    },
    marioHat: {
      position: "absolute",
      top: 0,
      left: "50%",
      transform: "translateX(-50%)",
      width: 78,
      height: 34,
      backgroundColor: "#d62828",
      borderRadius: "22px 22px 10px 10px",
      border: "4px solid #8b1c1c",
      boxShadow: "0 6px 0 rgba(0,0,0,0.25)",
    },
    marioEmblem: {
      position: "absolute",
      top: 8,
      left: "50%",
      transform: "translateX(-50%)",
      width: 24,
      height: 24,
      borderRadius: "50%",
      backgroundColor: "#fff",
      color: "#d62828",
      display: "flex",
      alignItems: "center",
      justifyContent: "center",
      fontWeight: "bold",
      fontSize: 12,
      boxShadow: "0 2px 0 rgba(0,0,0,0.2)",
    },
    marioFace: {
      position: "absolute",
      top: 34,
      left: "50%",
      transform: "translateX(-50%)",
      width: 72,
      height: 52,
      backgroundColor: "#f8d9b6",
      borderRadius: 28,
      border: "4px solid #d09b73",
      boxShadow: "0 4px 0 rgba(0,0,0,0.15)",
    },
    marioEye: {
      position: "absolute",
      top: 18,
      width: 10,
      height: 14,
      backgroundColor: "#12274a",
      borderRadius: "50%",
      boxShadow: "0 2px 0 rgba(0,0,0,0.25)",
    },
    marioNose: {
      position: "absolute",
      top: 20,
      left: "50%",
      transform: "translateX(-50%)",
      width: 30,
      height: 22,
      backgroundColor: "#f4c7a4",
      borderRadius: "50%",
      border: "3px solid #d09b73",
      boxShadow: "0 2px 0 rgba(0,0,0,0.15)",
      zIndex: 2,
    },
    marioMoustache: {
      position: "absolute",
      bottom: 14,
      left: "50%",
      transform: "translateX(-50%)",
      width: 50,
      height: 18,
      backgroundColor: "#4a1c1c",
      borderRadius: 18,
      boxShadow: "0 4px 0 rgba(0,0,0,0.2)",
      zIndex: 1,
    },
    marioBody: {
      position: "absolute",
      bottom: 0,
      left: "50%",
      transform: "translateX(-50%)",
      width: 84,
      height: 64,
      backgroundColor: "#0d3b66",
      borderRadius: "24px 24px 12px 12px",
      border: "4px solid #06233d",
      boxShadow: "0 8px 0 rgba(0,0,0,0.25)",
      overflow: "visible",
    },
    marioStrap: {
      position: "absolute",
      top: -16,
      width: 22,
      height: 54,
      backgroundColor: "#154c79",
      borderRadius: 12,
      border: "3px solid #06233d",
      boxShadow: "0 4px 0 rgba(0,0,0,0.2)",
    },
    marioButtons: {
      position: "absolute",
      bottom: 16,
      left: "50%",
      transform: "translateX(-50%)",
      display: "flex",
      gap: 24,
    },
    marioButton: {
      width: 16,
      height: 16,
      borderRadius: "50%",
      backgroundColor: "#ffd166",
      border: "3px solid #c17b00",
      boxShadow: "0 2px 0 rgba(0,0,0,0.2)",
    },
  };


  async function handleSubmit(e) {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      if (marioTimerRef.current) {
        window.clearTimeout(marioTimerRef.current);
        marioTimerRef.current = null;
      }

      await login(email, password);

      navigate(from, { replace: true }); // -> /user (eller vart man försökte gå)
    } catch (err) {
      // Försök läsa meddelande från backend, fall back till generiskt
      const msg =
        err?.response?.data?.message ||
        err?.message ||
        "Felaktig e-post eller lösenord";
      setError(msg);
    } finally {
      setLoading(false);

      setMarioJump(true);
      marioTimerRef.current = window.setTimeout(() => {
        setMarioJump(false);
        marioTimerRef.current = null;
      }, 1100);
      // Optional redirect goes here.
    } try{}catch (err) {
      console.error("Login failed:", err);
      setError("Felaktig e-post eller lösenord");
      setMarioJump(false);

    }
  }

  const handleCoinSpawn = (e) => {
    if (e.target.closest("button")) {
      return;
    }

    const rect = e.currentTarget.getBoundingClientRect();
    const id = Date.now() + Math.random();
    const coinX = e.clientX - rect.left;
    const coinY = e.clientY - rect.top;

    setCoins((prev) => [...prev, { id, x: coinX, y: coinY }]);

    window.setTimeout(() => {
      setCoins((prev) => prev.filter((coin) => coin.id !== id));
    }, 600);
  };

  useEffect(() => {
    if (typeof document === "undefined") {
      return undefined;
    }

    const styleId = "login-form-coin-pop";
    let styleEl = document.getElementById(styleId);

    if (!styleEl) {
      styleEl = document.createElement("style");
      styleEl.id = styleId;
      styleEl.textContent = `
        @keyframes coinPop {
          0% {
            transform: translateY(0) scale(0.6);
            opacity: 0.9;
          }
          55% {
            transform: translateY(-30px) scale(1.05);
            opacity: 1;
          }
          100% {
            transform: translateY(-60px) scale(0.8);
            opacity: 0;
          }
        }
        @keyframes marioJump {
          0% {
            transform: translateY(40px) scale(0.8);
            opacity: 0;
          }
          20% {
            opacity: 1;
          }
          50% {
            transform: translateY(-140px) scale(1);
            opacity: 1;
          }
          100% {
            transform: translateY(40px) scale(0.85);
            opacity: 0;
          }
        }
      `;
      document.head.appendChild(styleEl);

      return () => {
        const existing = document.getElementById(styleId);
        if (existing) {
          existing.remove();
        }
      };
    }

    return undefined;
  }, []);

  useEffect(
    () => () => {
      if (marioTimerRef.current) {
        window.clearTimeout(marioTimerRef.current);
      }
    },
    []
  );

  return (
  <form onSubmit={handleSubmit} style={styles.form} onClick={handleCoinSpawn}>
    {/* Header */}
    <h2 style={{ textAlign: "center", marginBottom: 20 }}>Logga in</h2>

    {error && <p style={styles.error}>{error}</p>}

    {/* Mario Animation */}
    {marioJump && (
      <div
        style={{
          ...styles.marioWrap,
          animation: "marioJump 1.1s ease-out forwards",
        }}
      >
        <div style={styles.marioHat}>
          <div style={styles.marioEmblem}>M</div>
        </div>
        <div style={styles.marioFace}>
          <div style={{ ...styles.marioEye, left: 18 }} />
          <div style={{ ...styles.marioEye, right: 18 }} />
          <div style={styles.marioMoustache} />
          <div style={styles.marioNose} />
        </div>
        <div style={styles.marioBody}>
          <div style={{ ...styles.marioStrap, left: 8 }} />
          <div style={{ ...styles.marioStrap, right: 8 }} />
          <div style={styles.marioButtons}>
            <span style={styles.marioButton} />
            <span style={styles.marioButton} />
          </div>
        </div>
      </div>
    )}

    {/* Coins */}
    {coins.map((coin) => (
      <span
        key={coin.id}
        style={{
          ...styles.coin,
          left: coin.x - 14,
          top: coin.y - 14,
          animation: "coinPop 0.6s ease-out forwards",
        }}
      >
        C
      </span>
    ))}

    <span style={styles.badge}>1-Up Login</span>

    {/* Email Field */}
    <div style={styles.field}>
      <label htmlFor="email" style={styles.label}>
        E-post
      </label>
      <input
        id="email"
        type="email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        autoComplete="email"
        required
        disabled={loading}
        placeholder="mario@nintendo.se"
        style={styles.input}
        onFocus={(e) => {
          e.target.style.transform = "translateY(-2px)";
          e.target.style.boxShadow =
            "inset 0 4px 0 rgba(0,0,0,0.3), 0 8px 12px rgba(0,0,0,0.35)";
        }}
        onBlur={(e) => {
          e.target.style.transform = "translateY(0)";
          e.target.style.boxShadow = "inset 0 4px 0 rgba(0,0,0,0.3)";
        }}
      />
      <span style={styles.helper}>
        Toad tips: glöm inte att Princess Peach gillar ordentliga e-postadresser!
      </span>
    </div>

    {/* Password Field */}
    <div style={styles.field}>
      <label htmlFor="password" style={styles.label}>
        Lösenord
      </label>
      <input
        id="password"
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        autoComplete="current-password"
        required
        disabled={loading}
        placeholder="*****"
        style={styles.input}
        onFocus={(e) => {
          e.target.style.transform = "translateY(-2px)";
          e.target.style.boxShadow =
            "inset 0 4px 0 rgba(0,0,0,0.3), 0 8px 12px rgba(0,0,0,0.35)";
        }}
        onBlur={(e) => {
          e.target.style.transform = "translateY(0)";
          e.target.style.boxShadow = "inset 0 4px 0 rgba(0,0,0,0.3)";
        }}
      />
      <span style={styles.helper}>Luigi säger: minst 8 tecken</span>
    </div>

    {/* Submit Button */}
    <button
      type="submit"
      disabled={loading}
      style={styles.button}
      onMouseOver={(e) => {
        e.target.style.backgroundColor = "#9af0a3";
        e.target.style.transform = "translateY(-3px)";
        e.target.style.boxShadow =
          "0 9px 0 #0b5c33, 0 16px 22px rgba(0,0,0,0.5), inset 0 -4px 0 rgba(0,0,0,0.2)";
      }}
      onMouseOut={(e) => {
        e.target.style.backgroundColor = "#7fe18a";
        e.target.style.transform = "translateY(0)";
        e.target.style.boxShadow =
          "0 6px 0 #0b5c33, 0 12px 18px rgba(0,0,0,0.45), inset 0 -4px 0 rgba(0,0,0,0.2)";
      }}
    >
      {loading ? "Loggar in…" : "Logga in"}
    </button>
  </form>
);
}