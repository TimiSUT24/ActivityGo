import {Routes, Route} from "react-router-dom";
import LoginForm from "./Components/LoginForm";
import RegisterPage from "./Pages/RegisterPage";
import HomePage from "./Pages/HomePage";
import NavBar from "./Components/NavBar";
import UserPage from "./Pages/UserPage";
import RequireAuth from "./Components/RequireAuth";



export default function App() {
  return (
    <>   
      <NavBar></NavBar>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginForm />} />
        <Route path="/register" element={<RegisterPage />}></Route>
        <Route
          path="/user"
          element={
            <RequireAuth>
              <UserPage />
            </RequireAuth>
          }
        ></Route>
      </Routes>
    </>
  );
}
