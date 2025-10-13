import { Routes, Route } from "react-router-dom";
import LoginForm from "./Components/LoginForm";
import RegisterPage from "./Pages/RegisterPage";
import HomePage from "./Pages/HomePage";
import NavBar from "./Components/NavBar";
import UserPage from "./Pages/UserPage";
import RequireAuth from "./Components/RequireAuth";
import MyBookings from "./Pages/MyBookings"; // fixat till stor bokstav i "Pages"

export default function App() {
  return (
    <>
      <NavBar />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginForm />} />
        <Route path="/register" element={<RegisterPage />} />

        {/* Endast inloggade användare får se dessa */}
        <Route
          path="/user"
          element={
            <RequireAuth>
              <UserPage />
            </RequireAuth>
          }
        />
        <Route
          path="/me/bookings"
          element={
            <RequireAuth>
              <MyBookings />
            </RequireAuth>
          }
        />
      </Routes>
    </>
  );
}
