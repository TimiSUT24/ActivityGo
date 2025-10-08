import { Routes, Route, Link } from "react-router-dom";
import LoginForm from "./Components/LoginForm";
import RegisterPage from "./Pages/RegisterPage";
import HomePage from "./Pages/HomePage";


export default function App() {
  return (
    <>
      <nav className ="nav-bar"style={{ display: "flex", gap: 12, padding: 12 }}>
        <Link to="/">Hem</Link>
        <Link to="/login">Logga in</Link>
        <Link to="/register">Register</Link>
      </nav>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginForm />} />
        <Route path="/register" element={<RegisterPage />}></Route>
      </Routes>
    </>
  );
}
