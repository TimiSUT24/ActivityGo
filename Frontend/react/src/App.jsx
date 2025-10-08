import {Routes, Route} from "react-router-dom";
import LoginForm from "./Components/LoginForm";
import RegisterPage from "./Pages/RegisterPage";
import HomePage from "./Pages/HomePage";
import NavBar from "./Components/NavBar";

export default function App() {

  return (
    <>   
    <NavBar></NavBar>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginForm />} />
        <Route path="/register" element={<RegisterPage />}></Route>
      </Routes>     
    </>
  );
}
