import { useState } from "react";
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

  async function handleSubmit(e) {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
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
    }
  }

  return (
    <form
      onSubmit={handleSubmit}
      style={{
        maxWidth: 360,
        margin: "40px auto",
        padding: 24,
        backgroundColor: "#1f1f1f",
        borderRadius: 8,
        color: "#fff",
        boxShadow: "0 2px 10px rgba(0,0,0,0.3)",
      }}
    >
      <h2 style={{ textAlign: "center", marginBottom: 20 }}>Logga in</h2>

      {error && (
        <p style={{ color: "#ff6b6b", textAlign: "center", marginBottom: 12 }}>
          {error}
        </p>
      )}

      <div style={{ marginBottom: 12 }}>
        <label htmlFor="email" style={{ display: "block", marginBottom: 4, fontSize: 14 }}>
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
          style={{
            width: "100%",
            padding: 10,
            borderRadius: 4,
            border: "1px solid #333",
            backgroundColor: "#2a2a2a",
            color: "#fff",
          }}
        />
      </div>

      <div style={{ marginBottom: 16 }}>
        <label htmlFor="password" style={{ display: "block", marginBottom: 4, fontSize: 14 }}>
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
          style={{
            width: "100%",
            padding: 10,
            borderRadius: 4,
            border: "1px solid #333",
            backgroundColor: "#2a2a2a",
            color: "#fff",
          }}
        />
      </div>

      <button
        type="submit"
        disabled={loading}
        style={{
          width: "100%",
          padding: 12,
          backgroundColor: loading ? "#019986" : "#00bfa5",
          color: "#fff",
          border: "none",
          borderRadius: 4,
          cursor: loading ? "not-allowed" : "pointer",
          fontWeight: "bold",
          transition: "background 0.2s ease-in-out",
        }}
        onMouseOver={(e) => !loading && (e.target.style.backgroundColor = "#00d1b2")}
        onMouseOut={(e) => !loading && (e.target.style.backgroundColor = "#00bfa5")}
      >
        {loading ? "Loggar in…" : "Logga in"}
      </button>
    </form>
  );
}