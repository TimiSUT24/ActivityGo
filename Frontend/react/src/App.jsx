import { Routes, Route } from "react-router-dom";
import LoginForm from "./Components/LoginForm";
import RegisterPage from "./Pages/RegisterPage";
import HomePage from "./Pages/HomePage";
import NavBar from "./Components/NavBar";
import UserPage from "./Pages/UserPage";
import RequireAuth from "./Components/RequireAuth";
import ActivityOccurrencePage from "./Pages/ActivityOccurrencePage";
import MyBookings from "./Pages/MyBookings";
import RequireAdmin from "./Components/RequireAdmin";
import AdminPage from "./Pages/AdminPage";
import ActivitiesPage from "./Pages/ActivitiesPage";
import ActivityDetailsPage from "./Pages/ActivityDetailsPage";

export default function App() {
  return (
    <>
      <NavBar />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route
          path="/activity-occurrences"
          element={<ActivityOccurrencePage />}
        ></Route>
        <Route path="/activities" element={<ActivitiesPage />} />
        <Route
          path="/activities/:activityId"
          element={<ActivityDetailsPage />}
        />
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
        <Route
          path="/admin"
          element={
            <RequireAdmin>
              <AdminPage />
            </RequireAdmin>
          }
        />
      </Routes>
    </>
  );
}
