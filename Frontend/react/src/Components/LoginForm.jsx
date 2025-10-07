import { useState } from "react";
import { useAuth } from "../context/AuthContext";

export default function LoginForm() {
  const { login } = useAuth();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    try {
      await login(email, password);
      // ✅ Redirect efter lyckad inloggning (valfritt)
      // window.location.href = "/admin";
    } catch (err) {
      console.error("Login failed:", err);
      setError("Felaktig e-post eller lösenord");
    }
  };

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
        <label
          htmlFor="email"
          style={{ display: "block", marginBottom: 4, fontSize: 14 }}
        >
          E-post
        </label>
        <input
          id="email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          autoComplete="email"
          required
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
        <label
          htmlFor="password"
          style={{ display: "block", marginBottom: 4, fontSize: 14 }}
        >
          Lösenord
        </label>
        <input
          id="password"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          autoComplete="current-password"
          required
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
        style={{
          width: "100%",
          padding: 12,
          backgroundColor: "#00bfa5",
          color: "#fff",
          border: "none",
          borderRadius: 4,
          cursor: "pointer",
          fontWeight: "bold",
          transition: "background 0.2s ease-in-out",
        }}
        onMouseOver={(e) => (e.target.style.backgroundColor = "#00d1b2")}
        onMouseOut={(e) => (e.target.style.backgroundColor = "#00bfa5")}
      >
        Logga in
      </button>
    </form>
  );
}