import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

/**
 * Enkel guard som endast släpper in användare med rollen "Admin".
 * Kräver att AuthProvider omsluter hela appen.
 */
export default function RequireAdmin({ children }) {
  const { user, ready } = useAuth();

  // Vänta tills AuthContext laddat klart
  if (!ready) return null; // eller en spinner/loader om du vill

  // Ingen användare = inte inloggad → till /login
  if (!user) return <Navigate to="/login" replace />;

  // Kontrollera om användaren har rollen "Admin"
  const roles = Array.isArray(user.roles)
    ? user.roles
    : user.role
    ? [user.role]
    : [];

  const isAdmin = roles.includes("Admin");

  // Inte admin → tillbaka till startsidan
  if (!isAdmin) return <Navigate to="/" replace />;

  // Inloggad admin → rendera barnen (AdminPage etc.)
  return children;
}
