import { useAuth } from "../context/AuthContext";
import { Navigate, useLocation } from "react-router-dom";

export default function RequireAuth({ children }) {
  const { user } = useAuth();
  const location = useLocation();

  if (!user) {
    // Skicka användaren till login och spara nuvarande path
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return children;
}