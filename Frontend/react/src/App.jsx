import { Routes, Route, NavLink, useLocation } from "react-router-dom";
import LoginForm from "./Components/LoginForm";
import RegisterPage from "./Pages/RegisterPage";
import HomePage from "./Pages/HomePage";


export default function App() {
  const location = useLocation();

  const pageClass = 
        location.pathname === "/" ? "home-navbar" :
        location.pathname === "/login" ? "login-navbar" : 
        location.pathname === "/register" ? "register-navbar" : "default-navbar";

  return (
    <>
      <nav className ={`nav-bar ${pageClass}`}style={{ display: "flex", gap: 12, padding: 12 }}>
        <NavLink to="/" className ="nav-link">Hem</NavLink>
        <NavLink to="/login" className ="nav-link">Logga in</NavLink>
        <NavLink to="/register" className ="nav-link">Register</NavLink>
      </nav>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginForm />} />
        <Route path="/register" element={<RegisterPage />}></Route>
      </Routes>
    </>
  );
}
