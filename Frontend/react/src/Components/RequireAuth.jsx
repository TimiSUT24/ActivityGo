import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function RequireAuth({ children }) {
  const { user, ready } = useAuth();
  const location = useLocation();

  if (!ready) return null; // eller spinner
  if (!user) return <Navigate to="/login" state={{ from: location }} replace />;
  return children;
}