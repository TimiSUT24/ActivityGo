import api, { setAccessToken } from "../lib/api";

const TOKEN_KEY = "ag_access_token"; // valfritt: persistens (localStorage)

/**
 * Loggar in användaren via API:t och sparar accessToken.
 * @param {{ email: string, password: string }} dto
 * @returns {Promise<object>} user
 */
export async function login(dto) {
  const { data } = await api.post("/api/auth/login", dto);
  const token = data?.accessToken || data?.token;
  if (!token) throw new Error("No access token returned from server");

  setAccessToken(token);
  localStorage.setItem(TOKEN_KEY, token);
  return data.user;
}

/**
 * Läser in ev. sparad token vid uppstart.
 */
export function loadPersistedToken() {
  const token = localStorage.getItem(TOKEN_KEY);
  if (token) setAccessToken(token);
}

/**
 * Loggar ut användaren (rensar token och försöker meddela backend).
 */
export async function logout() {
  try {
    await api.post("/api/auth/logout");
  } catch {
    // ok om endpoint saknas
  }
  setAccessToken(null);
  localStorage.removeItem(TOKEN_KEY);
}

/**
 * Hämtar information om inloggad användare.
 * @returns {Promise<object>} user
 */
export async function me() {
  const { data } = await api.get("/api/auth/me");
  return data;
}