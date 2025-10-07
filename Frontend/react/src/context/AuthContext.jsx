// src/context/AuthContext.jsx
import { createContext, useContext, useEffect, useState } from "react";
import api, { setAccessToken } from "../lib/api";

const TOKEN_KEY = "ag_access_token";
const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [ready, setReady] = useState(false);

  useEffect(() => {
    const t = localStorage.getItem(TOKEN_KEY);
    if (t) setAccessToken(t);
    (async () => {
      try {
        const { data } = await api.get("/api/auth/me");
        setUser(data);
      } catch {
        setAccessToken(null);
        localStorage.removeItem(TOKEN_KEY);
        setUser(null);
      } finally {
        setReady(true);
      }
    })();
  }, []);

  const login = async (email, password) => {
    const { data } = await api.post("/api/auth/login", { email, password });
    const token = data?.accessToken || data?.token;
    if (!token) throw new Error("No access token");
    setAccessToken(token);
    localStorage.setItem(TOKEN_KEY, token);
    setUser(data.user ?? null);
  };

  const logout = async () => {
    try { await api.post("/api/auth/logout"); } catch {}
    setAccessToken(null);
    localStorage.removeItem(TOKEN_KEY);
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, ready, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}