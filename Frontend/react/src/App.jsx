import { Routes, Route, Link } from "react-router-dom";
import LoginForm from "./Components/LoginForm";
import RegisterPage from "./Pages/RegisterPage";
import ActivityOccurrencePage from "./Pages/ActivityOccurrencePage";

function Home() {
  return <h1 style={{ padding: 16 }}>Välkommen</h1>;
}

export default function App() {
  return (
    <>
      <nav style={{ display: "flex", gap: 12, padding: 12 }}>
        <Link to="/">Hem</Link>
        <Link to="/login">Logga in</Link>
        <Link to="/register">Register</Link>
        <Link to="/occurrences">Activities</Link>
      </nav>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<LoginForm />} />
        <Route path="/register" element={<RegisterPage />}></Route>
        <Route path="/occurrences" element={<ActivityOccurrencePage />} />
      </Routes>
    </>
  );
}
