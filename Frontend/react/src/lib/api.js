import axios from "axios";

// Skapa en Axios-instans för alla API-anrop
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  withCredentials: true, // krävs om du använder HttpOnly cookies för refresh
});

// Access token hålls i minnet (inte i localStorage av säkerhetsskäl)
let accessToken = null;

export const setAccessToken = (token) => {
  accessToken = token;
};

// ✅ Lägg på Authorization-header automatiskt
api.interceptors.request.use((config) => {
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

// 🧩 Hantera 401 (Unauthorized) och försök hämta nytt access token via refresh-endpoint
let isRefreshing = false;
let queue = [];

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const original = error.config;

    if (error.response?.status === 401 && !original._retry) {
      original._retry = true;

      // Vänta om en refresh redan pågår
      if (isRefreshing) {
        await new Promise((resolve) => queue.push(resolve));
        return api(original);
      }

      isRefreshing = true;
      try {
        // 🔁 Försök hämta nytt access token från backend (via cookie)
        const res = await api.post("/api/auth/refresh");
        const newToken = res.data?.accessToken;
        setAccessToken(newToken || null);

        // Kör alla väntande requests
        queue.forEach((fn) => fn());
        queue = [];

        return api(original);
      } catch (err) {
        // Refresh misslyckades → logga ut
        setAccessToken(null);
        queue.forEach((fn) => fn());
        queue = [];
        throw err;
      } finally {
        isRefreshing = false;
      }
    }

    throw error;
  }
);

export default api;