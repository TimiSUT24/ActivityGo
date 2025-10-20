import { createContext, useContext, useEffect, useState } from "react";
import api, { setAccessToken } from "../lib/api"; // din axios-instans

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [ready, setReady] = useState(false);
  const TOKEN_KEY = "ag_access_token";

  // Hämta /auth/me och uppdatera context användaren
  async function refreshMe() {
    const { data } = await api.get("/api/auth/me");
    // Om backend skickar nytt accessToken (t.ex. vid e-postbyte)
    if (data?.accessToken) {
      localStorage.setItem(TOKEN_KEY, data.accessToken);
      setAccessToken(data.accessToken);
    }
    setUser(data);
    return data;
 }

  // Ladda ev. token vid uppstart och hämta /auth/me
  useEffect(() => {
    const t = localStorage.getItem(TOKEN_KEY);
    if (!t) { setReady(true); return; }
    setAccessToken(t);
    (async () => {
      try {
        const { data } = await api.get("/api/auth/me");
        setUser(data);
      } catch {
        localStorage.removeItem(TOKEN_KEY);
        setAccessToken(null);
        setUser(null);
      } finally {
        setReady(true);
      }
    })();
  }, []);

  // Viktigt: login sätter både token OCH user direkt
  async function login(email, password) {
    const { data } = await api.post("/api/auth/login", { email, password });
    const token = data?.accessToken || data?.token;
    if (!token) throw new Error("No access token returned");
    localStorage.setItem(TOKEN_KEY, token);
    setAccessToken(token);

    // sätt user direkt från svaret om det finns…
    if (data?.user) {
      setUser(data.user);
    } else {
      // …annars hämta /me direkt efter
      const me = await api.get("/api/auth/me").then(r => r.data).catch(() => null);
      setUser(me);
    }
  }

  async function logout() {
    await api.post("/api/auth/logout");
    localStorage.removeItem(TOKEN_KEY);
    setAccessToken(null);
    setUser(null);
  }

  return (
    <AuthContext.Provider value={{ user, ready, login, logout, refreshMe, setUser }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}